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
    public string _actualType;
    public bool _needPaintingPlacer1;
    public bool _needPaintingPlacer2;
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
    private string _idType;
    private string _idPeriod;
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
                        PlacePaintingsPlacers();
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
                        laser2._currentTime = _period / 2;
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
                        laser2._currentTime = _period / 2;
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

            void PlacePaintingsPlacers()
            {
                Vector3 r = transform.right;

                if (_needPaintingPlacer1)
                    PlaceOnePaintingPlacer(r, "a");
                else if (_needPaintingPlacer2)
                    PlaceOnePaintingPlacer(-r, "b");

                void PlaceOnePaintingPlacer(Vector3 dir, string str)
                {
                    Vector3 p = GetHitPoint(transform.position, dir) - dir * 0.1f;
                    GameObject paintingPlacerObj = new GameObject("The Painting Placer");
                    paintingPlacerObj.transform.position = p;
                    paintingPlacerObj.transform.rotation = Quaternion.LookRotation(-dir);
                    paintingPlacerObj.transform.SetParent(S.Loader.Roots[_sceneName], true);
                    PaintingPlacer paintingPlacer = paintingPlacerObj.AddComponent<PaintingPlacer>();
                    paintingPlacer._id = S.IDM(_id, $"PP {str}");
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

            IEnumerator GetActualType()
            {
                while (S.SM == null)
                    yield return new WaitForSeconds(0.2f);

                _actualType = S.SM.LoadString(_idType) ?? "oh no";

                if (_actualType.Equals("oh no"))
                    DefineActualType();
                else
                    _period = S.SM.LoadFloat(_idPeriod) ?? 1f;
            }

            void DefineActualType()
            {
                byte number = 0;

                if (_typeFamily == "BR")
                {
                    number = Convert.ToByte(UnityEngine.Random.Range(0, 5));

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
                }
                else if (_typeFamily == "BR Single LF")
                {
                    number = Convert.ToByte(UnityEngine.Random.Range(0, 2));

                    if (number == 0)
                        _actualType = "singleVertical";
                    else
                        _actualType = "singleHorizontal";
                }
                else
                {
                    number = Convert.ToByte(UnityEngine.Random.Range(0, 5));

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
                }

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

                float prob = 100f;
                if (_sceneName.Contains("BR"))
                    prob = S.Backrooms._lasersProbabilities[_sceneName];

                if (Convert.ToByte(UnityEngine.Random.Range(0, 100)) > prob)
                    _actualType = "skipped";

                if (_typeFamily == "BR Single LF")
                {
                    if (Convert.ToByte(UnityEngine.Random.Range(0, 30)) > 10)
                        _actualType = "skipped";
                }

                S.SM.Save(_idType, _actualType);
                S.SM.Save(_idPeriod, _period);
            }

            void GetId()
            {
                _sceneName = gameObject.scene.name;
                _id = S.ID("LS", gameObject);
                _idType = S.IDM(_id, "type");
                _idPeriod = S.IDM(_id, "period");
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
            _unityEditorMeshFilter.sharedMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
        }

    }

    void OnDrawGizmos()
    {
        EnsureMeshFilter();
        transform.localScale = Vector3.one * 1f;

        Gizmos.color = Color.red;
      
        Vector3 r = transform.right;
        Vector3 r9 = r * 9f;
        Vector3 r2 = r * 2f;
        Vector3 r4 = r2 * 2f;
        Vector3 u = transform.up;
        Vector3 u6 = u * 6f;
        Vector3 f = transform.forward;

        Gizmos.DrawLine(transform.position + u6, transform.position - u6);
        Gizmos.DrawLine(transform.position + r9, transform.position - r9);
        Gizmos.DrawWireSphere(transform.position, 1f);

        if (_needPaintingPlacer1)
        {
            Gizmos.DrawLine(transform.position + r4, transform.position + r2 - f);
            Gizmos.DrawLine(transform.position + r4, transform.position + r2 + f);
        }
        if (_needPaintingPlacer2)
        {
            Gizmos.DrawLine(transform.position - r4 ,transform.position - r2 - f);
            Gizmos.DrawLine(transform.position - r4, transform.position - r2 + f);
        }
    }
    #endif
}