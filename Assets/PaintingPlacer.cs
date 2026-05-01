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
    private MeshRenderer _unityEditorMeshRenderer;
    private MeshFilter _unityEditorMeshFilter;

    void Start()
    {
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
                DefineAndPlace();
            else if (_paintingId == -2)
                Destroy(gameObject);
            else
                Place();
        }

        void DefineAndPlace()
        {
            int number = S.RND.Next(1);
            if (number > 0)
            {
                S.SM.Save(_pidid, -2);
                Destroy(gameObject);
            }
            else
            {
                _paintingId = S.RND.Next(S.Paintings._names.Count());
                S.SM.Save(_pidid, _paintingId);
                Place();
            }
        }
    }

    void Place()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.forward, out hit, 5f, _layerMask))
        {
            Vector3 point1 = hit.point + transform.forward * 0.1f;

            RaycastHit hit2;
            if (Physics.Raycast(point1, -transform.up, out hit2, 25f, _layerMask))
            {
                Vector3 point2 = new Vector3(hit.point.x, hit2.point.y + 7f, hit.point.z);

                GameObject painting = GameObject.Instantiate(S.SquarePainting, point2, transform.rotation, S.Loader.Roots[_sceneName]);
                GameObject child = painting.transform.GetChild(0).gameObject;

                Material mat = new Material(Shader.Find("Unlit/Texture"));
                mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

                string name = S.Paintings._names[_paintingId];

                Debug.LogError($"Texture name = {name}");
                mat.mainTexture = Resources.Load<Texture2D>($"Textures/Paintings/{name}");

                child.GetComponent<MeshRenderer>().material = mat;
            }
        }

        Destroy(gameObject);
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