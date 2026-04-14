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
    private int _actualCount;
    private string _id;
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

                var a = S.SM.LoadInt(S.ID(_id, "actualCount"));
                if (a != null)
                {
                    _actualCount = a.Value;
                    _mask = S.SM.LoadListBool(S.ID(_id, "mask"));
                }
                else
                    Initiate();
            }

            void Initiate()
            {
                if (_count < 2)
                    _count = 2;

                _actualCount = UnityEngine.Random.Range(0, _count + 1);

                S.SM.Save(S.ID(_id, "actualCount"), _actualCount);

                _mask = new List<bool>();

                for (int i = 0; i < (_actualCount + 1) / 2; i++)
                    _mask.Add(UnityEngine.Random.Range(0, 2) == 1);

                float prob = S.Backrooms._lasersProbabilities[_sceneName];

                if (Convert.ToByte(UnityEngine.Random.Range(0, 100)) > prob)
                    for (int i = 0; i < _mask.Count; i++)
                        _mask[i] = false;

                S.SM.Save(S.ID(_id, "mask"), _mask);
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
                _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
                _id = _sceneName + transform.position.x + transform.position.y + transform.position.z + "sols";
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
        Quaternion rotation = Quaternion.LookRotation(transform.forward);
        Vector3 right = rotation * Vector3.right * 9f;
        Vector3 up = rotation * Vector3.up * 6f;

        if (_count < 2)
            _count = 2;
        float length = transform.localScale.z;
        Vector3 step = transform.forward * length / (_count - 1);
        Vector3 start = transform.position - transform.forward * length / 2;

        for (int i = 0; i < _count; i++)
        {
            Vector3 d = start + step * i;
            Gizmos.DrawLine(d + up, d - up);
            Gizmos.DrawLine(d + right, d - right);
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }
#endif
}
