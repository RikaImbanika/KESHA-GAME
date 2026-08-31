// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PaintingPlacer : MonoBehaviour
{
    private string _sceneName;
    public string _id;
    private string _pidid;
    private string _mirid;
    private string _prsid;
    private int _paintingId;
    private int _mirrored;
    private int _phraseNumber;
    private int _layerMask;
    private Color _tint;
    private MeshRenderer _unityEditorMeshRenderer;
    private MeshFilter _unityEditorMeshFilter;
    private float _wallHueShiftSpeed;
    private Transform _root;

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

            while (S.Loader.Roots == null ||
                !S.Loader.Roots.ContainsKey(_sceneName) ||
                S.Loader.Roots[_sceneName] == null)
                yield return new WaitForSeconds(0.25f);

            _root = S.Loader.Roots[_sceneName];

            _paintingId = S.SM.LoadInt(_pidid) ?? -1;
            _phraseNumber = S.SM.LoadInt(_prsid) ?? -1;

            if (_paintingId == -1 && _phraseNumber == -1)
                yield return DefineAndPlace();
            else if (_paintingId == -2)
                Destroy(gameObject);
            else
            {
                _mirrored = S.SM.LoadInt(_mirid) ?? 0;

                yield return Place();
            }
        }

        IEnumerator DefineAndPlace()
        {
            yield return new WaitForSeconds(Random.Range(0f, 0.5f));

            _phraseNumber = S.Paintings.TryTakePhrase(_sceneName);
            if (_phraseNumber != -1)
            {
                S.SM.Save(_prsid, _phraseNumber);
                yield return Place();
            }
            else
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
                    _mirrored = S.RND.Next(2) == 1 ? 1 : 0;

                    //for rare paintings
                    float number2 = Random.Range(0, 100);
                    if (number2 > S.Paintings._probabilities[_paintingId])
                    {
                        _paintingId = -2;
                        S.SM.Save(_pidid, -2);
                        Destroy(gameObject);
                    }

                    S.SM.Save(_pidid, _paintingId);
                    S.SM.Save(_mirid, _mirrored);
                    yield return Place();
                }
            }
        }
    }

    IEnumerator Place()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.forward, out hit, 5f, _layerMask))
        {
            Quaternion wallRotation = Quaternion.FromToRotation(transform.forward, hit.normal) * transform.rotation;

            Vector3 point1 = hit.point + hit.normal * 0.1f;

            RaycastHit hit2;
            if (Physics.Raycast(point1, -transform.up, out hit2, 25f, _layerMask))
            {
                Material mat;
                Material frameMat;

                if (_sceneName == "BR 7" || _sceneName == "BR 7R")
                {
                    mat = new Material(Shader.Find("Custom/SelfIlluminUnlitTintSingleSideHueShiftF"));
                    frameMat = new Material(Shader.Find("Custom/SelfIlluminUnlitTintSingleSideHueShiftF"));
                    mat.SetFloat("_Speed", 0.1667f);
                    frameMat.SetFloat("_Speed", 0.1667f);
                    _wallHueShiftSpeed = 0.1667f;
                }
                else if (_sceneName == "BR 6" || _sceneName == "BR 6R")
                {
                    mat = new Material(Shader.Find("Custom/SelfIlluminUnlitTintSingleSideHueShiftF"));
                    frameMat = new Material(Shader.Find("Custom/SelfIlluminUnlitTintSingleSideHueShiftF"));
                    mat.SetFloat("_Speed", 0.08f);
                    frameMat.SetFloat("_Speed", 0.08f);
                    _wallHueShiftSpeed = 0.08f;
                }
                else
                {
                    mat = new Material(Shader.Find("Custom/SelfIlluminUnlitTintSingleSideF"));
                    frameMat = new Material(Shader.Find("Custom/SelfIlluminUnlitTintSingleSideF"));
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

                GameObject painting = Instantiate(S.SquarePainting, point2, wallRotation, _root);
                GameObject child = painting.transform.GetChild(0).gameObject;

                GameObject frame = Instantiate(S.WoodenPaintingFrame, point2, wallRotation, _root);

                frameMat.mainTexture = Resources.Load<Texture2D>($"Textures/WoodenFrameTexture");

                if (_phraseNumber != -1)
                {
                    Material blackMat = new Material(mat.shader);
                    blackMat.color = Color.black;

                    if (_wallHueShiftSpeed > 0f)
                        blackMat.SetFloat("_Speed", _wallHueShiftSpeed);

                    child.GetComponent<MeshRenderer>().material = blackMat;

                    GameObject phraseObject = new GameObject("Phrase", typeof(RectTransform));
                    phraseObject.transform.SetParent(_root, false);

                    phraseObject.transform.position = point2 + hit.normal * 0.12f;

                    phraseObject.transform.rotation = wallRotation * Quaternion.Euler(0, 180, 0);

                    Vector3 rootLossy = _root.lossyScale;
                    phraseObject.transform.localScale = new Vector3(
                        1f / Mathf.Max(rootLossy.x, 0.0001f),
                        1f / Mathf.Max(rootLossy.y, 0.0001f),
                        1f / Mathf.Max(rootLossy.z, 0.0001f)
                    );

                    TextMeshPro tmp = phraseObject.AddComponent<TextMeshPro>();
                    tmp.text = S.Paintings._phrases[_phraseNumber].Item1;
                    tmp.color = Color.white;
                    tmp.alignment = TextAlignmentOptions.Center;
                    tmp.enableWordWrapping = true;

                    tmp.enableAutoSizing = false;

                    tmp.fontSize = 4f;

                    Bounds localBounds = child.GetComponent<MeshRenderer>().localBounds;
                    Vector3 localSize = localBounds.size;

                    Vector3 worldScale = child.transform.lossyScale;

                    float width = Mathf.Max(localSize.x * worldScale.x * 0.9f, 1f);
                    float height = Mathf.Max(localSize.y * worldScale.y * 0.9f, 1f);

                    tmp.rectTransform.sizeDelta = new Vector2(width, height);

                    Shader tmpFogShader = Shader.Find("Custom/TMPUnlitF");
                    if (tmpFogShader != null)
                    {
                        Texture fontAtlas = tmp.font.material.mainTexture;

                        Material tmpMaterial = new Material(tmpFogShader);
                        tmpMaterial.mainTexture = fontAtlas;
                        tmpMaterial.SetColor("_FaceColor", Color.white);

                        tmp.fontMaterial = tmpMaterial;
                    }
                }
                else
                {
                    string name = S.Paintings._names[_paintingId];
                    mat.mainTexture = Resources.Load<Texture2D>($"Textures/Paintings/{name}");

                    bool canMirror = S.Paintings._canMirror[_paintingId];

                    if (canMirror && _mirrored == 1)
                    {
                        mat.mainTextureScale = new Vector2(-1, 1);
                        mat.mainTextureOffset = new Vector2(1, 0);
                    }

                    child.GetComponent<MeshRenderer>().material = mat;
                }

                GameObject frameChild = frame.transform.Find("Child/Frame").gameObject;
                frameChild.GetComponent<MeshRenderer>().material = frameMat;

                MaterialPropertyBlock mpb = S.Fog.GetMPB(_sceneName);
                S.Fog.ApplyToGameObject(painting, mpb);
                S.Fog.ApplyToGameObject(frame, mpb);
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
        _sceneName = gameObject.scene.name;

        if (string.IsNullOrEmpty(_id))
            _id = S.ID("PA", gameObject);

        _pidid = S.IDM(_id, "pid");
        _mirid = S.IDM(_id, "mir");
        _prsid = S.IDM(_id, "prn");
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