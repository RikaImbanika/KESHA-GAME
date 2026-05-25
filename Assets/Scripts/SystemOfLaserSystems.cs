using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemOfLaserSystems : MonoBehaviour
{
    public int _count;
    public string _typeFamily;
    public bool _dontNeedPaintingPlacer1;
    public bool _dontNeedPaintingPlacer2;
    private int _actualCount;
    private string _id;
    private string _idActualCount;
    private string _idMask;
    private string _sceneName;
    private MeshRenderer _unityEditorMeshRenderer;
    private MeshFilter _unityEditorMeshFilter;
    private List<bool> _mask;

    void Start()
    {
        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<MeshFilter>());

        StartCoroutine(Start0());

        IEnumerator Start0()
        {
            GetId();
            StartCoroutine(TryLoad());
            PlaceLasersSystems();
            yield return null;

            IEnumerator TryLoad()
            {
                while (S.SM == null)
                    yield return new WaitForSeconds(0.201f);

                while (!S.Loader.Roots.ContainsKey(_sceneName))
                    yield return new WaitForSeconds(0.11f);

                var a = S.SM.LoadInt(_idActualCount);
                if (a != null)
                {
                    _actualCount = a.Value;
                    _mask = S.SM.LoadListBool(_idMask);
                    CheckExistenZ();
                }
                else
                {
                    Initiate();
                    CheckExistenZ();
                }
            }

            void Initiate()
            {
                if (_count < 2)
                    _count = 2;

                _actualCount = UnityEngine.Random.Range(0, _count + 1);

                S.SM.Save(_idActualCount, _actualCount);

                _mask = new List<bool>();

                for (int i = 0; i < (_actualCount + 1) / 2; i++)
                    _mask.Add(UnityEngine.Random.Range(0, 2) == 1);

                float prob = S.Backrooms._lasersProbabilities[_sceneName];

                if (Convert.ToByte(UnityEngine.Random.Range(0, 100)) > prob)
                    for (int i = 0; i < _mask.Count; i++)
                        _mask[i] = false;

                S.SM.Save(_idMask, _mask);
            }

            void CheckExistenZ()
            {
                bool exists = false;

                for (int i = 0; i < _mask.Count; i++)
                    if (_mask[i])
                    {
                        exists = true;
                        break;
                    }

                if (!exists)
                    PlacePaintingsPlacers();
            }

            void PlacePaintingsPlacers()
            {
                Vector3 r = transform.right;

                if (!_dontNeedPaintingPlacer1)
                    PlaceOnePaintingPlacer(r, "a");
                if (!_dontNeedPaintingPlacer2)
                    PlaceOnePaintingPlacer(-r, "b");

                void PlaceOnePaintingPlacer(Vector3 dir, string str)
                {
                    Vector3 p = GetHitPoint(transform.position, dir) - dir * 0.1f;
                    GameObject paintingPlacerObj = new GameObject("The Painting Placer");
                    paintingPlacerObj.transform.position = p;
                    paintingPlacerObj.transform.rotation = Quaternion.LookRotation(-dir);
                    paintingPlacerObj.transform.SetParent(S.Loader.Roots[_sceneName], true);
                    PaintingPlacer paintingPlacer = paintingPlacerObj.AddComponent<PaintingPlacer>();
                    paintingPlacer._id = S.IDM(_id, $"PP{str}");
                    paintingPlacer.Instantiate();
                    //Debug.LogError("PLACED.");
                }
            }

            Vector3 GetHitPoint(Vector3 from, Vector3 direction)
            {
                RaycastHit hit;
                if (Physics.Raycast(from, direction, out hit))
                {
                    float len = (hit.point - from).magnitude;
                    return hit.point;
                }
                else
                 return transform.position + direction;
            }

            void PlaceLasersSystems()
            {
                float length = transform.localScale.z;

                if (_actualCount == 1)
                {
                    if (_mask[0])
                        PlaceOneLaserSystem(transform.position);
                    return;
                }

                Vector3 step = transform.forward * length / (_actualCount - 1);
                Vector3 start = transform.position - transform.forward * length / 2;

                int half = (_actualCount + 1) / 2;

                transform.localScale = new Vector3(1, 1, 1);

                for (int i = 0; i < _actualCount; i++)
                {
                    int j = i;
                    if (j >= half)
                        j = half - i;

                    if (_mask[j])
                        PlaceOneLaserSystem(start + step * i);
                }

                void PlaceOneLaserSystem(Vector3 pos)
                {
                    GameObject dummy = new GameObject("LaserSystemDummy");

                    dummy.transform.SetParent(transform);
                    dummy.transform.position = pos;
                    dummy.transform.rotation = transform.rotation;
                    LasersSystem ls = dummy.AddComponent<LasersSystem>();
                    if (string.IsNullOrEmpty(_typeFamily))
                        ls._typeFamily = "BRConstant";
                    else
                        ls._typeFamily = _typeFamily;
                }
            }

            void GetId()
            {
                _sceneName = gameObject.scene.name;
                _id = S.ID("SOLS", gameObject);
                _idActualCount = S.IDM(_id, "actualCount");
                _idMask = S.IDM(_id, "mask");           
            }
        }
    }

        #if UNITY_EDITOR
    private void EnsureMeshFilter()
    {
        if (GetComponent<MeshRenderer>() == null)
        {
            _unityEditorMeshRenderer = gameObject.AddComponent<MeshRenderer>();
            _unityEditorMeshRenderer.sharedMaterial = Materials.GetInEditor("DUMMY");
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
        transform.localScale = new Vector3(1, 1, transform.localScale.z);

        Gizmos.color = Color.red;

        Vector3 r = transform.right;
        Vector3 r9 = r * 9f;
        Vector3 r2 = r * 2f;
        Vector3 r4 = r2 * 2f;
        Vector3 u = transform.up;
        Vector3 u6 = u * 6f;
        Vector3 f = transform.forward;

        if (_count < 2)
            _count = 2;
        float length = transform.localScale.z;
        Vector3 step = transform.forward * length / (_count - 1);
        Vector3 start = transform.position - transform.forward * length / 2;

        for (int i = 0; i < _count; i++)
        {
            Vector3 d = start + step * i;
            Gizmos.DrawLine(d + u6, d - u6);
            Gizmos.DrawLine(d + r9, d - r9);
            Gizmos.DrawWireSphere(transform.position, 1f);
        }

        if (!_dontNeedPaintingPlacer1)
        {
            Gizmos.DrawLine(transform.position + r4, transform.position + r2 - f);
            Gizmos.DrawLine(transform.position + r4, transform.position + r2 + f);
        }
        if (!_dontNeedPaintingPlacer2)
        {
            Gizmos.DrawLine(transform.position - r4, transform.position - r2 - f);
            Gizmos.DrawLine(transform.position - r4, transform.position - r2 + f);
        }
    }
#endif
}
