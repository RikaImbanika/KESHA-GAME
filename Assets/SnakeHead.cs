using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;
using System;

public class SnakeHead : MonoBehaviour
{
    public float _acceleration;
    public float _speed;
    public float _turn;
    public float _h;
    public float _dh;
    public float _sin;
    public float _rotationSpeed;
    public AllFather _allFather;

    public SnakeBrain _brain;

    public GameObject _ball;
    public GameObject _theBullet;

    public int _aimId;

    float _saveInterval;
    public List<Vector3> _aims = new List<Vector3>();
    private List<Vector3> _savedPositions = new List<Vector3>();
    private List<GameObject> _clones = new List<GameObject>();
    private List<SnakeBody> _clonesSBs = new List<SnakeBody>();
    private List<NavMeshObstacle> _clonesNMOs = new List<NavMeshObstacle>();
    float L = 3.7f;
    float _timeCapacity = 20;
    bool _tailEnabled;
    Vector3 _dir;
    private float _walk = 0;
    public List<Material> _ballMaterials;
    public bool _seePlayer;
    public GameObject _player;

    void Start()
    {
        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();
        _theBullet = GameObject.Find("EnemyBullet");
        _player = GameObject.FindWithTag("Player");

        int percents = 100;
        _tailEnabled = true;
        _saveInterval = 1 / 60f;

        _clones.Add(gameObject);
        _clonesSBs.Add(null);

        int randomIndex = UnityEngine.Random.Range(0, _ballMaterials.Count);

        for (int i = 0; i < 50; i++)
        {
            GameObject clone = Instantiate(gameObject);
            Destroy(clone.GetComponent<SnakeHead>());
            Destroy(clone.GetComponent<UnityEngine.AI.NavMeshAgent>());
            clone.transform.position = new Vector3(clone.transform.position.x, clone.transform.position.y, clone.transform.position.z);

            clone.AddComponent<SnakeBody>();

            NavMeshObstacle obstacle = clone.GetComponent<NavMeshObstacle>();
            if (i >= 3)
                obstacle.enabled = true;
            _clonesNMOs.Add(obstacle);

            SnakeBody sb = clone.GetComponent<SnakeBody>();
            _clonesSBs.Add(sb);

            Transform childBall = clone.transform.Find("Ball");

            sb._R = L + UnityEngine.Random.Range(-0.0125f, 0.0125f);
            sb._ball = childBall.gameObject;

            Transform childChildBall = childBall.transform.Find("Ball");
            childChildBall.transform.Rotate(UnityEngine.Random.Range(0, 180), UnityEngine.Random.Range(0, 180), UnityEngine.Random.Range(0, 180));
            sb._ball_ball = childChildBall.gameObject;

            Transform droneTr = clone.transform.Find("Drone");
            sb._droneObj = droneTr.gameObject;
            sb._drone = sb._droneObj.GetComponent<Drone>();
            sb._drone._head = this;
            sb._drone._player = _player;
            sb._drone._theBullet = _theBullet;
            sb._drone._allFather = _allFather;
            sb._drone.enabled = true;

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

            sb._drone.Init(type, type2);

            int randomNumber = UnityEngine.Random.Range(0, 2);
            if (randomNumber == 0)
                randomIndex = UnityEngine.Random.Range(0, _ballMaterials.Count);
            sb._ball_ball.GetComponent<Renderer>().material = _ballMaterials[randomIndex];

            _clones.Add(clone);
        }

        _ball.SetActive(false);

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

        UpdateTail();

        Vector3 aim = GetAim();

        Vector3 bestDir = (aim - transform.position).normalized;

        _dir = Vector3.Slerp(_dir, bestDir, Time.deltaTime * _turn).normalized;
        _dir *= _speed;

        _walk += _speed * Time.deltaTime;
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

            float distance0 = (_aims[_aimId - 1] - transform.position).magnitude;
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
        _seePlayer = false;
        int p = 0;
        int c = 1;
        for (; c < _clones.Count && p < _savedPositions.Count - 1;)
        {
            Vector3 v = _savedPositions[_savedPositions.Count - 1 - p] - _clones[c - 1].transform.position;
            if (v.magnitude > L)
            {
                Vector3 B = transform.position + new Vector3(0, _h, 0);
                if (c > 1)
                    B = _clonesSBs[c - 1]._ball.transform.position;

                Vector3 C = _savedPositions[_savedPositions.Count - 1 - p];
                float h2 = _h + _dh * Mathf.Sin((_walk - L * c) / _sin);
                C += new Vector3(0, h2, 0);

                Vector3 bestPoint = FindPointEasyWay(C, B, _clonesSBs[c]._R);                
                _clones[c].transform.position = new Vector3(bestPoint.x, transform.position.y, bestPoint.z);
                _clonesSBs[c]._ball.transform.position = bestPoint;

                _clonesSBs[c]._ball.transform.rotation = Quaternion.identity;
                var dir = _clonesSBs[c]._ball.transform.position - _ball.transform.position;
                if (c > 1)
                    dir = _clonesSBs[c]._ball.transform.position - _clonesSBs[c - 1]._ball.transform.position;

                _clonesSBs[c]._ball.transform.rotation = Quaternion.LookRotation(dir);
                _clonesSBs[c]._ball.transform.Rotate(-_rotationSpeed * _walk, 0, 0);

                _clonesSBs[c]._droneObj.transform.position = bestPoint;
                _clonesSBs[c]._droneObj.transform.rotation = Quaternion.LookRotation(dir);

                _clonesSBs[c]._drone.Work(_walk);

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
        Vector3 toPlayer = _player.transform.position - transform.position;
        Ray ray = new Ray(transform.position, toPlayer);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);

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
