using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class SnakeHead : MonoBehaviour
{
    public int _ballsCount;
    public float _acceleration;
    public float _speed;
    public float _turn;
    public float _h;

    private float _rotationSpeed;
    public SnakeBrain _brain;
    public GameObject _headVis;

    public int _aimId;

    private float _period;
    private float _heigh;
    private float _dHeigh;
    private float _saveInterval;
    public List<Vector3> _aims;
    private List<Vector3> _savedPositions;
    public List<GameObject> _clones;
    private List<SnakeBody> _bodies;
    private List<NavMeshObstacle> _clonesNMOs;
    private List<Vector3> _prevDirs;
    private Vector3 _headPrevDir;
    private float L = 3.7f; //Wha?
    private float _timeCapacity = 20;
    private bool _tailEnabled;
    private Vector3 _dir;
    private float _walk = 0;
    private float _dWalk = 0;
    public bool _seePlayer;
    private int _layerMaskForPlayer;
    private int _layerMaskForAims;
    private int _lastAimId;
    public GameObject _BLUE;
    public string _sceneName;
    public string _type;
    public float _timeSinceTailSwitched;

    private Optimiser _opti;

    void Start()
    {
        _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;

        _ballsCount = 30;
        _period = 7f;
        _heigh = 7f;
        _dHeigh = 3.5f;
        _rotationSpeed = 18f;

        _layerMaskForPlayer = 1 << LayerMask.NameToLayer("Player") |
                 1 << LayerMask.NameToLayer("Static") |
                 1 << LayerMask.NameToLayer("Items") |
                 1 << LayerMask.NameToLayer("Default");

        _layerMaskForAims = 1 << LayerMask.NameToLayer("Static") |
            1 << LayerMask.NameToLayer("Default");

        _opti = new Optimiser(_sceneName);
        _opti.MaxPeriodForDistance = 1 / 24f;
        _opti.MaxPeriodForRotation = 1 / 60f;
        _opti.MaxPeriodForScene = 2f;

        _aims = new List<Vector3>();
        _savedPositions = new List<Vector3>() { transform.position };
        _clones = new List<GameObject>();
        _bodies = new List<SnakeBody>();
        _clonesNMOs = new List<NavMeshObstacle>();
        _prevDirs = new List<Vector3>();

        int percents = 100;
        _tailEnabled = true;
        _saveInterval = 1 / 60f;

        int randomIndex = UnityEngine.Random.Range(0, S.SnakeBallMaterials.Count);

        int ballsTypesCount = S.SnakeBalls[_type].Count;

        Debug.LogError($"Balls of \"{_type}\" count: {ballsTypesCount}");

        for (int i = 0; i < _ballsCount; i++)
        {
            GameObject clone = Instantiate(S.SnakeBody, transform.position, transform.rotation, transform.parent);

            _clones.Add(clone);

            SnakeBody body = clone.GetComponent<SnakeBody>();

            _bodies.Add(body);

            _prevDirs.Add(new Vector3(0, 1, 0));

            NavMeshObstacle obstacle = body.Obstacle;

            if (i < 3)
                obstacle.enabled = false;
            _clonesNMOs.Add(obstacle);

            body.R = L + UnityEngine.Random.Range(-0.0125f, 0.0125f); //Why?

            Transform BIBIB = body.BallInBallInBall.transform;
            BIBIB.transform.Rotate(UnityEngine.Random.Range(0, 180), UnityEngine.Random.Range(0, 180), UnityEngine.Random.Range(0, 180));

            body.Drone._head = this;
            SnakeBall bib = body.BallInBall.GetComponent<SnakeBall>();
            bib._brain = _brain;
            bib._head = this;

            percents = 100;
            string type = "";

            if (GetPercent(4))
                type = "6lasers";
            else if (GetPercent(5))
                type = "4lasers";
            else if (GetPercent(6))
                type = "3lasers";
            else if (GetPercent(6))
                type = "2lasers";
            else if (GetPercent(3))
                type = "flat2lasers";

            percents = 100;

            string type2 = "";

            if (i == _ballsCount - 1 || i == 0)
                type2 = "sniper";
            else if (GetPercent(10))
                type2 = "1";
            else if (GetPercent(5))
                type2 = "2";
            else if (GetPercent(4))
                type2 = "3";
            else if (GetPercent(15))
                type2 = "sniper";

            string color = "";

            if (_type == "Classic")
                color = "red";
            else if (_type == "Nature")
                color = "green";
            else if (_type == "Ice")
                color = "blue";
            else if (_type == "Silent")
                color = "purple";

            float damageMultiplier = 1f;
            if (_type == "Silent")
                damageMultiplier = 2f;

            body.Drone.Init(type, type2, color, damageMultiplier);

            int randomNumber = UnityEngine.Random.Range(0, 2);
            if (randomNumber == 0)
                randomIndex = UnityEngine.Random.Range(0, ballsTypesCount);

            GameObject ballInBallInBallInBall = GameObject.Instantiate(S.SnakeBalls[_type][randomIndex], body.BallInBallInBall.transform);
            ballInBallInBallInBall.transform.position = body.BallInBallInBall.transform.position;

            StartCoroutine(IEKek());

            IEnumerator IEKek()
            {
                SwitchTail(false, 4f);

                yield return new WaitForSeconds(3f);

                SwitchTail(true, 4f);
            }
        }

        bool GetPercent(int percent)
        {
            percents -= percent;
            return UnityEngine.Random.Range(0, percents + percent) < percent;
        }
    }

    void Update()
    {
        if (_opti.Optimise(transform.position, _clones.Last().transform.position, _clones[_ballsCount / 2].transform.position))
        {
            Do();
            _opti.Reset();
        }

        void Do()
        {
            if (_aims.Count > 10000)
            {
                _aims.RemoveRange(0, 1000);
                _aimId -= 1001;
                _lastAimId -= 1001;
            }

            if (_savedPositions.Count > 50000)
                _savedPositions.RemoveRange(0, 5000);

            float velocity = _speed * (500f + _aims.Count - _aimId) / 500f;

            _dWalk = velocity * _opti.DeltaTime;

            _walk += _dWalk;

            if (_savedPositions.Count > 0)
            {
                float d = (_savedPositions.Last() - transform.position).magnitude;
                if (d > 0.5f)
                    _savedPositions.Add(transform.position + new Vector3(0, _heigh + MathF.Sin(_walk / _period) * _dHeigh, 0));
            }
            else
                _savedPositions.Add(transform.position);

            UpdateTail();

            Vector3 aim = GetAim();
            //_BLUE.transform.position = aim;

            Vector3 bestDir = (aim - transform.position);

            bestDir.y = 0;

            _dir = Vector3.Slerp(_dir.normalized, bestDir.normalized, _opti.DeltaTime * _turn).normalized;

            _dir = Check(_dir).normalized;

            _dir *= velocity;

            transform.position += _dir * _opti.DeltaTime;
            transform.position = new Vector3(transform.position.x, aim.y, transform.position.z);
        }
    }

    Vector3 Check(Vector3 dir)
    {
        float radius = 8f;
        Vector3 right = Quaternion.AngleAxis(45 + UnityEngine.Random.Range(-5, 5), Vector3.up) * dir;
        Vector3 left = Quaternion.AngleAxis(45 + UnityEngine.Random.Range(-5, 5), Vector3.down) * dir;
        float dright = radius;
        float dleft = radius;
        RaycastHit hit;
        Vector3 start = transform.position + Vector3.up;
        Ray ray = new Ray(start, right);

        Debug.DrawRay(start, right * radius, Color.red, 0.1f);
        Debug.DrawRay(start, left * radius, Color.red, 0.1f);
        Debug.DrawRay(start, dir * radius, Color.red, 0.1f);

        if (Physics.Raycast(ray, out hit, radius, _layerMaskForAims))
            dright = (hit.point - start).magnitude;
        ray = new Ray(start, left);
        if (Physics.Raycast(ray, out hit, radius, _layerMaskForAims))
            dleft = (hit.point - start).magnitude;

        if (dleft < dright)
        {
            float k = (dleft / radius);
            return right * (1 - k) + dir * k;
        }
        else if (dright < dleft)
        {
            float k = (dright / radius);
            return left * (1 - k) + dir * k;
        }
        else
            return dir;
    }
    
    Vector3 GetAim()
    {
        bool end = false;

        for (int x = 0; x < 1000; x++)
        {
            if (_aims.Count == 0)
            {
                Debug.LogError($"WTF?");
                return transform.position + transform.forward;
            }
            else if (_aimId >= _aims.Count)
            {
                _aimId = _aims.Count - 1;
                return _aims[_aimId];
            }

            Vector3 dir = _aims[_aimId] - transform.position;
            dir.y = 0;
            float distance1 = dir.magnitude;

            RaycastHit hit;
            if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), dir, out hit, distance1, _layerMaskForAims))
            {
                end = true;

                if (_aimId > 0)
                    _aimId--;
                if (_aimId > 0 && _aimId > _lastAimId - 600)
                {
                    if (_aims.Count > 30)
                    {
                        _aims.RemoveRange(0, 30);
                        _aimId -= 29;
                        _lastAimId -= 30;

                        if (_aimId < 0)
                            _aimId = 0;
                        if (_lastAimId < 0)
                            _aimId = 0;
                    }
                }
                else
                    return _aims[_aimId];
            }
            else
            {
                if (!end)
                {
                    _aimId++;
                    if (_aimId > _lastAimId)
                        _lastAimId = _aimId;
                }
                else
                    return _aims[_aimId];
            }
        }

        Debug.LogError("WHAT?");
        return transform.position + transform.forward;
    }

    void UpdateTail()
    {
        _seePlayer = false; //correct
        bool skippedFirst = false;
        float headLength = L * 1.2f;
        float distance = 0;
        int p = _savedPositions.Count - 1;
        int c = 0;

        for (; c < _clones.Count && p > 0;)
        {
            Vector3 B = _savedPositions.Last();
            if (skippedFirst)
                B = _headVis.transform.position;

            if (c > 0)
                B = _bodies[c - 1].Ball.transform.position;

            Vector3 v = _savedPositions[p] - B;
            Vector3 D = Vector3.zero;
            if (p < _savedPositions.Count - 1)
            {
                D = _savedPositions[p] - _savedPositions[p - 1];
                distance += D.magnitude;
            }

            if (skippedFirst)
            {
                float L2 = L;
                if (c == 0)
                    L2 = headLength;

                if (distance > L2)
                {
                    Vector3 C = _savedPositions[p];

                    Vector3 dirdi = _savedPositions[p - 1] - C;

                    C += dirdi / dirdi.magnitude * (distance - L2);

                    Vector3 bestPoint = FindPointEasyWay(C, B, L2);

                    var dir = bestPoint - _bodies[c].Ball.transform.position;

                    float k = Math.Clamp(2 * _opti.DeltaTime, 0, 1);

                    _prevDirs[c] = _prevDirs[c] * (1 - k) + dir * k;

                    _bodies[c].transform.position = new Vector3(bestPoint.x, bestPoint.y, bestPoint.z);
                    _bodies[c].Ball.transform.position = bestPoint;

                    _bodies[c].Ball.transform.rotation = Quaternion.LookRotation(_prevDirs[c], Vector3.up);
                    Quaternion xRot = Quaternion.Euler(_rotationSpeed * _walk, 0, 0);
                    _bodies[c].BallInBall.transform.localRotation = xRot;

                    _bodies[c].Drone.Work(_walk, _dWalk);

                    if (!_seePlayer)
                        if (SeePlayer(_clones[c].transform.position))
                            _seePlayer = true;

                    c++;
                    distance -= L2;
                }
            }
            else
            {
                if (distance > headLength)
                {
                    Vector3 C = _savedPositions[p];

                    Vector3 dirdi = _savedPositions[p - 1] - C;

                    C += dirdi / dirdi.magnitude * (distance - L);

                    Vector3 bestPoint = FindPointEasyWay(C, B, L);

                    var dir = bestPoint - _headVis.transform.position;

                    float k = Math.Clamp(2 / Time.deltaTime, 0, 1);


                    _headPrevDir = _headPrevDir * k + dir * (1 - k);

                    _headVis.transform.position = bestPoint;

                    if (!_seePlayer)
                        if (SeePlayer(_headVis.transform.position))
                            _seePlayer = true;

                    //

                    skippedFirst = true;
                    distance -= headLength;
                }
            }

            p--;
        }

        Vector3 a = _savedPositions.Last();
        Vector3 b = _headVis.transform.position;
        Vector3 dir2 = (a - b);
        _headVis.transform.rotation = Quaternion.LookRotation(dir2);
    }

    public bool SeePlayer(Vector3 from)
    {
        Vector3 toPlayer = S.Camera.transform.position - transform.position;
        Ray ray = new Ray(transform.position, toPlayer);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, _layerMaskForPlayer))
        {
            return hit.collider.gameObject.tag == "Player";
        }
        else
            return false;
    }

    public void SwitchTail(bool enabled, float deltaTime)
    {
        if (_timeSinceTailSwitched > 3f && (!_tailEnabled && enabled || _tailEnabled && !enabled))
        {
            for (int i = 3; i < _clonesNMOs.Count; i++)
                _clonesNMOs[i].enabled = enabled;

            _timeSinceTailSwitched = 0f;
        }
        else
            _timeSinceTailSwitched += deltaTime;

        _tailEnabled = enabled;
    }

    Vector3 FindPointEasyWay(Vector3 C, Vector3 B, float L)
    {
        return B + (C - B).normalized * L; //---------
    }

    bool PlayerInScene()
    {
        return S.PS._currentSceneName == _sceneName;
    }
}
