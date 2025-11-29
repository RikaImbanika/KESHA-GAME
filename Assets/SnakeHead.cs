using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;
using System;

public class SnakeHead : MonoBehaviour
{
    public int _ballsCount;
    public float _acceleration;
    public float _speed;
    public float _turn;
    public float _h;
    public float _dh;
    public float _sin;
    public float _rotationSpeed;
    public SnakeBrain _brain;

    public int _aimId;

    private float _saveInterval;
    public List<Vector3> _aims;
    private List<Vector3> _savedPositions;
    private List<GameObject> _clones;
    private List<SnakeBody> _bodies;
    private List<NavMeshObstacle> _clonesNMOs;
    private float L = 3.7f; //Wha?
    private float _timeCapacity = 20;
    private bool _tailEnabled;
    private Vector3 _dir;
    private float _walk = 0;
    public bool _seePlayer;
    private int _layerMaskForPlayer;

    private Optimiser _opti;

    void Start()
    {
        _ballsCount = 10;

        _layerMaskForPlayer = 1 << LayerMask.NameToLayer("Player") |
                 1 << LayerMask.NameToLayer("Static") |
                 1 << LayerMask.NameToLayer("Items") |
                 1 << LayerMask.NameToLayer("Default");

        _opti = new Optimiser(gameObject.scene.name);
        _opti.MinFps = 1 / 120f;
        
        _aims = new List<Vector3>();
        _savedPositions = new List<Vector3>();
        _clones = new List<GameObject>();
        _bodies = new List<SnakeBody>();
        _clonesNMOs = new List<NavMeshObstacle>();
        
        int percents = 100;
        _tailEnabled = true;
        _saveInterval = 1 / 60f;

        int randomIndex = UnityEngine.Random.Range(0, S.SnakeBallMaterials.Count);

        for (int i = 0; i < _ballsCount; i++)
        {
            GameObject clone = Instantiate(S.SnakeBody, transform.parent);
            
            _clones.Add(clone);
            
            SnakeBody body = clone.GetComponent<SnakeBody>();
            
            _bodies.Add(body);

            NavMeshObstacle obstacle = body.Obstacle;
            
            if (i < 3)
                obstacle.enabled = false;
            _clonesNMOs.Add(obstacle);

            Transform CBT = body.BallInBall.transform;

            body.R = L + UnityEngine.Random.Range(-0.0125f, 0.0125f); //Why?

            Transform CCBT = body.BallInBall.transform;
            CCBT.transform.Rotate(UnityEngine.Random.Range(0, 180), UnityEngine.Random.Range(0, 180), UnityEngine.Random.Range(0, 180));

            body.Drone._head = this;

            percents = 100;
            string type = "";

            if (GetPercent(1))
                type = "6lasers";
            else if (GetPercent(1))
                type = "4lasers";
            else if (GetPercent(1))
                type = "3lasers";
            else if (GetPercent(2))
                type = "2lasers";
            else if (GetPercent(2))
                type = "flat2lasers";
            else if (GetPercent(3))
                type = "rotated2lasers";

            percents = 100;

            string type2 = "";

            if (GetPercent(10))
                type2 = "1";
            else if (GetPercent(7))
                type2 = "2";
            else if (GetPercent(6))
                type2 = "3";
            else if (GetPercent(5))
                type2 = "sniper";

            body.Drone.Init(type, type2);

            int randomNumber = UnityEngine.Random.Range(0, 2); //Why 2?
            if (randomNumber == 0)
                randomIndex = UnityEngine.Random.Range(0, S.SnakeBallMaterials.Count);
            body.Renderer.material = S.SnakeBallMaterials[randomIndex];
        }

        bool GetPercent(int percent)
        {
            percents -= percent;
            return UnityEngine.Random.Range(0, percents + percent) < percent;
        }
    }

    void Update()
    {
        if (_aims.Count > 10000)
        {
            _aims.RemoveRange(0, 1000);
            _aimId -= 1000;
        }

        if (_savedPositions.Count > 50000)
            _savedPositions.RemoveRange(0, 5000);

        _savedPositions.Add(transform.position);

        _walk = _speed * Time.deltaTime;

        UpdateTail();

        Vector3 aim = GetAim();

        Vector3 bestDir = (aim - transform.position).normalized;

        _dir = Vector3.Slerp(_dir, bestDir, Time.deltaTime * _turn).normalized;
        _dir *= _speed;

        transform.position += _dir * Time.deltaTime;        
        transform.position = new Vector3(transform.position.x, aim.y, transform.position.z);
    }

    Vector3 GetAim()
    {
        while (true)
        {
            if (_aimId == 0)
                _aimId = 1;
            if (_aims.Count < 2)
                return _aims[0];
            else if (_aimId == _aims.Count - 1)
                return _aims[_aimId];

            //float distance0 = (_aims[_aimId - 1] - transform.position).magnitude;
            float distance1 = (_aims[_aimId] - transform.position).magnitude;
            float distance2 = (_aims[_aimId + 1] - transform.position).magnitude;

            if (distance2 <= distance1)
            {
                _aimId++;
                continue;
            }
            else
            {
                return _aims[_aimId + 1];
            }
        }
    }

    void UpdateTail()
    {
        _seePlayer = false; ////////////////////
        int p = 0;
        int c = 1;
        for (; c < _clones.Count && p < _savedPositions.Count - 1;)
        {
            Vector3 v = _savedPositions[_savedPositions.Count - 1 - p] - _clones[c - 1].transform.position;
            if (v.magnitude > L)
            {
                Vector3 B = transform.position + new Vector3(0, _h, 0);
                if (c > 1)
                    B = _bodies[c - 1].transform.position;

                Vector3 C = _savedPositions[_savedPositions.Count - 1 - p];
                float h2 = _h + _dh * Mathf.Sin((_walk - L * c) / _sin);
                C += new Vector3(0, h2, 0);

                Vector3 bestPoint = FindPointEasyWay(C, B, _bodies[c].R);                
                _clones[c].transform.position = new Vector3(bestPoint.x, transform.position.y, bestPoint.z);

                var dir = _bodies[c].transform.position - transform.position;
                if (c > 1)
                    dir = _bodies[c].transform.position - _bodies[c - 1].transform.position;
                //Is that so? ^

                _bodies[c].Ball.transform.rotation = Quaternion.LookRotation(dir);
                _bodies[c].BallInBall.transform.Rotate(-_rotationSpeed * _walk, 0, 0);

                _bodies[c].Drone.transform.position = bestPoint;
                _bodies[c].Drone.transform.rotation = Quaternion.LookRotation(dir);

                _bodies[c].Drone.Work(_walk);

                if (SeePlayer())
                    _seePlayer = true;

                c++;
                p--;
            }
            p++;
        }
    }

    public bool SeePlayer()
    {
        Vector3 toPlayer = S.Camera.transform.position - transform.position;
        Ray ray = new Ray(transform.position, toPlayer);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, _layerMaskForPlayer);

        return hit.collider.gameObject.tag == "Player";
    }

    public void SwitchTail(bool enabled)
    {
        if (!_tailEnabled && enabled)
            for (int i = 3; i < _clonesNMOs.Count; i++)
                _clonesNMOs[i].enabled = true;

        if (_tailEnabled && !enabled)
            for (int i = 3; i < _clonesNMOs.Count; i++)
                _clonesNMOs[i].enabled = false;

        _tailEnabled = enabled;
    }

    Vector3 FindPointEasyWay(Vector3 C, Vector3 B, float L)
    {
        return B + (C - B).normalized * L; //---------
    }
}
