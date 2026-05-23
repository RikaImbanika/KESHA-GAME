using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PaintingPlacer : MonoBehaviour
{
    private string _sceneName;
    public string _id;
    private string _pidid;
    private int _paintingId;
    private int _layerMask;
    private Color _tint;
    private MeshRenderer _unityEditorMeshRenderer;
    private MeshFilter _unityEditorMeshFilter;
    private float _wallHueShiftSpeed;

    void Start()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null)
            Destroy(mr);

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf != null)
            Destroy(mf);

        Instantiate();
    }

    public void Instantiate()
    {
        if (_layerMask != 0)
            return;

        _layerMask = 1 << LayerMask.NameToLayer("Static") |
                     1 << LayerMask.NameToLayer("Default");

        GetId();
        StartCoroutine(GetActualId());

        IEnumerator GetActualId()
        {
            while (S.SM == null)
                yield return new WaitForSeconds(0.2f);

            while (!S.Loader.Roots.ContainsKey(_sceneName))
                yield return new WaitForSeconds(0.2f);

            _pidid = S.ID(_id, "pid");
            _paintingId = S.SM.LoadInt(_pidid) ?? -1;

            if (_paintingId == -1)
                yield return DefineAndPlace();
            else if (_paintingId == -2)
                Destroy(gameObject);
            else
                yield return Place();
        }

        IEnumerator DefineAndPlace()
        {
            int number = S.RND.Next(3);
            if (number > 0)
            {
                S.SM.Save(_pidid, -2);
                Destroy(gameObject);
            }
            else
            {
                _paintingId = S.RND.Next(S.Paintings._names.Count());
                S.SM.Save(_pidid, _paintingId);
                yield return Place();
            }
        }
    }

    IEnumerator Place()
    {
        while (S.Loader.Roots == null ||
            !S.Loader.Roots.ContainsKey(_sceneName) ||
            S.Loader.Roots[_sceneName] == null)
            yield return new WaitForSeconds(0.25f);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.forward, out hit, 5f, _layerMask))
        {
            Vector3 point1 = hit.point + transform.forward * 0.1f;

            RaycastHit hit2;
            if (Physics.Raycast(point1, -transform.up, out hit2, 25f, _layerMask))
            {
                Material mat;
                Material frameMat;

                if (_sceneName == "BR 7" || _sceneName == "BR 7R")
                {
                    mat = new Material(Shader.Find("Custom/SelfIlluminUnlitTintSingleSideHueShift"));
                    frameMat = new Material(Shader.Find("Custom/SelfIlluminUnlitTintSingleSideHueShift"));
                    mat.SetFloat("_Speed", 0.1667f);
                    frameMat.SetFloat("_Speed", 0.1667f);
                    _wallHueShiftSpeed = 0.1667f;
                }
                else if (_sceneName == "BR 6" || _sceneName == "BR 6R")
                {
                    mat = new Material(Shader.Find("Custom/SelfIlluminUnlitTintSingleSideHueShift"));
                    frameMat = new Material(Shader.Find("Custom/SelfIlluminUnlitTintSingleSideHueShift"));
                    mat.SetFloat("_Speed", 0.08f);
                    frameMat.SetFloat("_Speed", 0.08f);
                    _wallHueShiftSpeed = 0.08f;
                }
                else
                {
                    mat = new Material(Shader.Find("Custom/SelfIlluminUnlitTintSingleSide"));
                    frameMat = new Material(Shader.Find("Custom/SelfIlluminUnlitTintSingleSide"));
                    _wallHueShiftSpeed = 0f;
                }

                yield return GetWallColor();

                if (_tint == Color.magenta)
                {
                    ////////////////////////
                    // This is bad but we just skip
                    Destroy(gameObject);
                    yield break;
                }

                mat.color = _tint;
                frameMat.color = _tint;

                Vector3 point2 = new Vector3(hit.point.x, hit2.point.y + 7f, hit.point.z);

                GameObject painting = GameObject.Instantiate(S.SquarePainting, point2, transform.rotation, S.Loader.Roots[_sceneName]);
                GameObject child = painting.transform.GetChild(0).gameObject;

                GameObject frame = GameObject.Instantiate(S.WoodenPaintingFrame, point2, transform.rotation, S.Loader.Roots[_sceneName]);

                string name = S.Paintings._names[_paintingId];
                Debug.LogError($"Texture name = {name}");
                mat.mainTexture = Resources.Load<Texture2D>($"Textures/Paintings/{name}");
                frameMat.mainTexture = Resources.Load<Texture2D>($"Textures/WoodenFrameTexture");

                child.GetComponent<MeshRenderer>().material = mat;

                GameObject frameChild = frame.transform.Find("Child/Frame").gameObject;
                frameChild.GetComponent<MeshRenderer>().material = frameMat;
            }
        }

        Destroy(gameObject);
        yield return null;
    }

    IEnumerator GetWallColor()
    {
        Vector3 u = transform.up;
        Vector3 r = transform.right;
        Vector3[] points = new Vector3[]
        {
            transform.position,
            transform.position + u + r,
            transform.position + u - r,
            transform.position - u + r,
            transform.position - u - r,
            transform.position + u,
            transform.position - u,
            transform.position + r,
            transform.position - r
        };

        Color[] colors = S.WallColorCapturer.CaptureAtPoints(points, transform.forward, _layerMask);

        Color brightest = colors[0];
        for (int i = 1; i < colors.Length; i++)
            if (colors[i].grayscale > brightest.grayscale)
                brightest = colors[i];

        if (_wallHueShiftSpeed > 0f)
        {
            //This is just for synchronisation of hue shift with wall
            float timeShift = Mathf.Repeat(Time.timeSinceLevelLoad * _wallHueShiftSpeed, 1f);
            Color.RGBToHSV(brightest, out float h, out float s, out float v);
            h = (h - timeShift + 1f) % 1f;
            brightest = Color.HSVToRGB(h, s, v);
        }

        _tint = BrightenAndDesaturateColor(brightest, 0.3f, 0.2f);

        yield return null;
    }

    public Color BrightenAndDesaturateColor(Color baseColor, float b = 0.2f, float st = 0.2f)
    {
        //Here are no any problems

        Color.RGBToHSV(baseColor, out float h, out float s, out float v);

        //v = Mathf.Max(v, b);
        //v = Mathf.Clamp01(v * b);
        //v = v + (1 - v) * b;
        v = Mathf.Pow(v, 1 - b);

        s = Mathf.Pow(s, 1 + st);

        return Color.HSVToRGB(h, s, v);
    }

    void GetId()
    {
        _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
        
        if (string.IsNullOrEmpty(_id))
            _id = S.ID(_sceneName, S.ID(gameObject));
    }

#if UNITY_EDITOR

    private void EnsureMeshFilter()
    {
        if (GetComponent<MeshRenderer>() == null)
        {
            _unityEditorMeshRenderer = gameObject.AddComponent<MeshRenderer>();
            _unityEditorMeshRenderer.sharedMaterial = Materials.GetInEditor("DUMMY_YELLOW");
        }

        if (GetComponent<MeshFilter>() == null)
        {
            _unityEditorMeshFilter = gameObject.AddComponent<MeshFilter>();
            _unityEditorMeshFilter.sharedMesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
        }
    }

    void OnDrawGizmos()
    {
        EnsureMeshFilter();

        transform.localScale = new Vector3(4, 4, 0.1f);

        Vector3 c = transform.position;
        Vector3 u = transform.up;
        Vector3 r = transform.right;

        Vector3 u2 = u * 2;
        Vector3 r2 = r * 2;

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(c + transform.forward, c + transform.forward * 6);
        Gizmos.DrawLine(c + transform.forward, c + transform.forward * 3 + transform.right);
        Gizmos.DrawLine(c + transform.forward, c + transform.forward * 3 - transform.right);

        Gizmos.DrawLine(c + u2 + r2, c - u2 - r2);
        Gizmos.DrawLine(c + u2 - r2, c - u2 + r2);

        Gizmos.DrawLine(c + u - r, c - u - r);
        Gizmos.DrawLine(c + u + r, c - u + r);
        Gizmos.DrawLine(c + u - r, c + u + r);
        Gizmos.DrawLine(c - u - r, c - u + r);

        Gizmos.DrawLine(c + u2 - r2, c - u2 - r2);
        Gizmos.DrawLine(c + u2 + r2, c - u2 + r2);
        Gizmos.DrawLine(c + u2 - r2, c + u2 + r2);
        Gizmos.DrawLine(c - u2 - r2, c - u2 + r2);
    }
#endif
}