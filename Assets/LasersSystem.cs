using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LasersSystem : MonoBehaviour
{
    public string _typeFamily;
    private Vector3 _forward;
    private string _actualType;
    private float _period;
    private Vector3 _leftDir;
    private Vector3 _downDir;
    private Vector3 _rightDir;
    private Vector3 _upDir;
    private Vector3 _left;
    private Vector3 _right;
    private Vector3 _down;
    private Vector3 _up;
    private float _heigh;
    private float _width;
    private string _sceneName;
    private string _id;
    private MeshRenderer _unityEditorMeshRenderer;
    private MeshFilter _unityEditorMeshFilter;

    void Start()
    {
        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<MeshFilter>());

        StartCoroutine(Start0());

        IEnumerator Start0()
        {
            _forward = transform.forward;

            GetId();
            StartCoroutine(GetActualType());
            PlaceLasers();
            yield return null;

            void PlaceLasers()
            {
                FindBorders();
                Check();
                Centralise();
                Place();

                void Place()
                {
                    float d = 1.25f;

                    if (_actualType == "skipped")
                    {
                        Destroy(gameObject);
                    }
                    else if (_actualType == "singleVertical")
                    {
                        LocalLaser laser = AddLaser();

                        laser._laserDirection = _rightDir;
                        laser._forwardTime = _period / 2;
                        laser._backwardTime = _period / 2;
                        laser._forwardPause = 0f;
                        laser._backwardPause = 0f;
                        laser._currentState = Convert.ToByte(UnityEngine.Random.Range(0, 2) * 2);
                        laser._forwardDistance = _heigh * 0.5f - d;
                        laser._backwardDistance = _heigh * 0.5f - d;
                        laser._movementDirection = _upDir;
                    }
                    else if (_actualType == "singleHorizontal")
                    {
                        LocalLaser laser = AddLaser();

                        laser._laserDirection = _rightDir;
                        laser._forwardTime = _period / 2;
                        laser._backwardTime = _period / 2;
                        laser._forwardPause = 0f;
                        laser._backwardPause = 0f;
                        laser._currentState = Convert.ToByte(UnityEngine.Random.Range(0, 2) * 2);
                        laser._forwardDistance = _heigh * 0.5f - d;
                        laser._backwardDistance = _heigh * 0.5f - d;
                        laser._movementDirection = _upDir;
                    }
                    else if (_actualType == "doubleVertical")
                    {
                        LocalLaser laser1 = AddLaser();

                        laser1._laserDirection = _upDir;
                        laser1._forwardTime = _period / 2;
                        laser1._backwardTime = _period / 2;
                        laser1._forwardPause = 0f;
                        laser1._backwardPause = 0f;
                        laser1._currentState = 0;
                        laser1._forwardDistance = _width * 0.5f - d;
                        laser1._backwardDistance = _width * 0.5f - d;
                        laser1._movementDirection = _rightDir;

                        LocalLaser laser2 = AddLaser();

                        laser2._laserDirection = _upDir;
                        laser2._forwardTime = _period / 2;
                        laser2._backwardTime = _period / 2;
                        laser2._forwardPause = 0f;
                        laser2._backwardPause = 0f;
                        laser2._currentState = 2;
                        laser2._forwardDistance = _width * 0.5f - d;
                        laser2._backwardDistance = _width * 0.5f - d;
                        laser2._movementDirection = _rightDir;
                    }
                    else if (_actualType == "doubleHorizontal")
                    {
                        LocalLaser laser1 = AddLaser();

                        laser1._laserDirection = _rightDir;
                        laser1._forwardTime = _period / 2;
                        laser1._backwardTime = _period / 2;
                        laser1._forwardPause = 0f;
                        laser1._backwardPause = 0f;
                        laser1._currentState = 0;
                        laser1._forwardDistance = _heigh * 0.5f - d;
                        laser1._backwardDistance = _heigh * 0.5f - d;
                        laser1._movementDirection = _upDir;

                        LocalLaser laser2 = AddLaser();

                        laser2._laserDirection = _rightDir;
                        laser2._forwardTime = _period / 2;
                        laser2._backwardTime = _period / 2;
                        laser2._forwardPause = 0f;
                        laser2._backwardPause = 0f;
                        laser2._currentState = 2;
                        laser2._forwardDistance = _heigh * 0.5f - d;
                        laser2._backwardDistance = _heigh * 0.5f - d;
                        laser2._movementDirection = _upDir;
                    }
                    else if (_actualType == "circular")
                    {
                        LocalLaser laser1 = AddLaser();

                        laser1._laserDirection = _upDir;
                        laser1._forwardTime = _period / 2;
                        laser1._backwardTime = _period / 2;
                        laser1._forwardPause = 0f;
                        laser1._backwardPause = 0f;
                        laser1._currentState = 0;
                        laser1._forwardDistance = _width * 0.5f - d;
                        laser1._backwardDistance = _width * 0.5f - d;
                        laser1._movementDirection = _rightDir;

                        LocalLaser laser2 = AddLaser();

                        laser2._laserDirection = _rightDir;
                        laser2._forwardTime = _period / 2;
                        laser2._backwardTime = _period / 2;
                        laser2._forwardPause = 0f;
                        laser2._backwardPause = 0f;
                        laser2._currentState = 2;
                        laser2._currentTime = 0.5f;
                        laser2._forwardDistance = _heigh * 0.5f - d;
                        laser2._backwardDistance = _heigh * 0.5f - d;
                        laser2._movementDirection = _upDir;
                    }

                    LocalLaser AddLaser()
                    {
                        GameObject dummy = new GameObject("LaserDummy");
                        dummy.transform.SetParent(transform);
                        dummy.transform.position = transform.position;
                        return dummy.AddComponent<LocalLaser>();
                    }
                }

                void FindBorders()
                {
                    Quaternion rotation = Quaternion.LookRotation(_forward);

                    RaycastHit hit;

                    _leftDir = (rotation * Vector3.left).normalized;
                    _downDir = (rotation * Vector3.down).normalized;
                    _rightDir = (rotation * Vector3.right).normalized;
                    _upDir = (rotation * Vector3.up).normalized;

                    _left = Physics.Raycast(transform.position, _leftDir, out hit) ? hit.point : Vector3.zero;
                    _down = Physics.Raycast(transform.position, _downDir, out hit) ? hit.point : Vector3.zero;
                    _right = Physics.Raycast(transform.position, _rightDir, out hit) ? hit.point : Vector3.zero;
                    _up = Physics.Raycast(transform.position, _upDir, out hit) ? hit.point : Vector3.zero;
                    _heigh = (_up - _down).magnitude;
                    _width = (_right - _left).magnitude;
                }

                void Centralise()
                {
                    var buf1 = transform.position - (_left + _right) / 2;
                    var buf2 = transform.position - (_up + _down) / 2;
                    transform.position = transform.position - (buf1 + buf2);
                }

                void Check()
                {
                    if (_right == Vector3.zero || _left == Vector3.zero || _down == Vector3.zero || _up == Vector3.zero)
                    {
                        Debug.LogError($"LaserSystem can't be placed! {transform.position.x} {transform.position.y} {transform.position.z} {gameObject.scene}");
                        Destroy(gameObject);
                        return;
                    }
                }
            }

            IEnumerator GetActualType()
            {
                while (S.SM == null)
                    yield return new WaitForSeconds(0.2f);

                _actualType = S.SM.LoadString(S.ID(_id, "laserSystemType")) ?? "oh no";

                if (_actualType.Equals("oh no"))
                    DefineActualType();
                else
                    _period = S.SM.LoadFloat(S.ID(_id, "laserSystemPeriod")) ?? 1f;
            }

            void DefineActualType()
            {
                int number = Convert.ToByte(UnityEngine.Random.Range(0, 5));

                if (number == 0)
                    _actualType = "singleVertical";
                else if (number == 1)
                    _actualType = "singleHorizontal";
                else if (number == 2)
                    _actualType = "doubleVertical";
                else if (number == 3)
                    _actualType = "doubleHorizontal";
                else if (number == 4)
                    _actualType = "circular";

                number = Convert.ToByte(UnityEngine.Random.Range(0, 10));
                float d = 1.25f;

                if (number == 0)
                    _period = d * 1f;
                if (number == 1)
                    _period = d * 2.5f;
                else if (number == 2)
                    _period = d * 1.25f;
                else if (number == 3)
                    _period = d * (1f + 1 / 3f);
                else if (number == 4)
                    _period = d * 1.5f;
                else if (number == 5)
                    _period = d * 1.5f;
                else if (number == 6)
                    _period = d * (1f + 2 / 3f);
                else if (number == 7)
                    _period = d * 1.75f;
                else if (number == 8)
                    _period = d * 2f;
                else if (number == 9)
                    _period = d * 2f;

                if (_typeFamily == "Backrooms")
                {
                    if (Convert.ToByte(UnityEngine.Random.Range(0, 10)) > 0)
                        _actualType = "skipped";
                    //BackroomsConstant family will not not be skipped
                }

                S.SM.Save(S.ID(_id, "laserSystemType"), _actualType);
                S.SM.Save(S.ID(_id, "laserSystemPeriod"), _period);
            }

            void GetId()
            {
                _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
                _id = _sceneName + transform.position.x + transform.position.y + transform.position.z;
            }
        }
    }

#if UNITY_EDITOR
    private void EnsureMeshFilter()
    {
        if (GetComponent<MeshRenderer>() == null)
        {
            _unityEditorMeshRenderer = gameObject.AddComponent<MeshRenderer>();
            _unityEditorMeshRenderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Materials/DUMMY.mat");
            //Debug.Log(AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Materials/DUMMY.mat"));
        }
        /*else
        {
            DestroyImmediate(GetComponent<MeshRenderer>());
            _unityEditorMeshRenderer = gameObject.AddComponent<MeshRenderer>();
            _unityEditorMeshRenderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Materials/DUMMY.mat");
        }*/

        if (GetComponent<MeshFilter>() == null)
        {
            _unityEditorMeshFilter = gameObject.AddComponent<MeshFilter>();
            _unityEditorMeshFilter.sharedMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
        }

    }

    void OnDrawGizmos()
    {
        EnsureMeshFilter();
        transform.localScale = Vector3.one * 1f;

        Gizmos.color = Color.red;
        Quaternion rotation = Quaternion.LookRotation(transform.forward);
        Vector3 right = rotation * Vector3.right * 6f;
        Vector3 up = rotation * Vector3.up * 6f;

        Gizmos.DrawLine(transform.position + up, transform.position - up);
        Gizmos.DrawLine(transform.position + right, transform.position - right);
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
#endif
}