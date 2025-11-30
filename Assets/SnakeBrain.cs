using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;

public class SnakeBrain : MonoBehaviour
{
    public float _acceleration;
    public float _speed;
    public float _turn;

    public SnakeHead _head;

    public Vector3[] _points;

    private UnityEngine.AI.NavMeshAgent _agent;
    private bool _stuckAvoidance;
    private bool _tailEnabled;
    public GameObject _BLUE;
    public GameObject _GREEN;
    private Vector3 _velocity;
    private float _distanceThreshold = 3f;

    void Start()
    {
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _agent.acceleration = 0;
        _head._aims.Add(transform.position);
    }

    void Update()
    {
        if ((_head._aims[_head._aims.Count - 1] - transform.position).magnitude > 0.5f)
            _head._aims.Add(transform.position);

        CheckP();

        if ((transform.position - _agent.destination).magnitude < _distanceThreshold)
        {
            _stuckAvoidance = false;
            SwitchTail(!_stuckAvoidance);
            _agent.destination = GetNewPoint();
            //SwitchTail(!_stuckAvoidance);
            //What?
        }

        Vector3 bestDir = (_agent.steeringTarget - _agent.transform.position).normalized;
        _velocity = Vector3.Slerp(_velocity, bestDir, Time.deltaTime * _turn).normalized;
        _velocity *= _speed * 12 / (_head._aims.Count - _head._aimId);
        transform.position += _velocity * Time.deltaTime;
    }

    void CheckP()
    {
        for (int i = 1; i < 15; i++)
        {
            if (i <= _head._aims.Count - 2)
            {
                if ((_head._aims[_head._aims.Count - 1] - _head._aims[_head._aims.Count - 2 - i]).magnitude < 0.5f)
                {
                    _head._aims.RemoveRange(_head._aims.Count - 2 - i, i);
                    i = 0;
                }
            }
            else
                break;
        }
        if (_head._aimId >= _head._aims.Count)
            _head._aimId = _head._aims.Count - 1;
    }

    void SwitchTail(bool enabled)
    {
        _head.SwitchTail(enabled);
    }

    Vector3 GetNewPoint()
    {
        Vector3 point = new Vector3();
        for (int i = 0; i < 60; i++)
        {
            point = transform.position;
            while ((point - transform.position).magnitude < 8f)
                point = GetRandomPointInQuad(_points[0], _points[1], _points[2], _points[3]);
            
            bool reachable = false;
            Vector3 newDirection = GetDirectionAndWait(point, Vector3.zero, out reachable);

            if (reachable)
            {
                Debug.Log("REACHABLE");
                return point;
            }
        }

        Debug.Log("I'M STUCK");
        _stuckAvoidance = true;
        return point;
    }

    private Vector3 GetDirectionAndWait(Vector3 targetPosition, Vector3 direction, out bool reachable)
    {
        NavMeshPath path = new NavMeshPath();
        _agent.CalculatePath(targetPosition, path);

		for (int i = 0; i < 500000; i++)
			if (!_agent.pathPending)
				break;  //yield?

        if (path.corners.Length >= 2)
        {
            Vector3 final = path.corners[path.corners.Length - 1];
            Vector3 target = new Vector3(targetPosition.x, final.y, targetPosition.z);
            _BLUE.transform.position = target;
            _GREEN.transform.position = final;

            float dist = (final - target).magnitude;
            Debug.Log($"Distance: {dist}");
            reachable = dist < 3f;
        }
        else
            reachable = false;
        
        if (path.corners.Length >= 2)
            return (path.corners[1] - path.corners[0]).normalized;
        else
            return -direction;
    }

    Vector3 GetRandomPointInQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        float randomX = Random.Range(0f, 1f);
        float randomY = Random.Range(0f, 1f);

        Vector3 pointAB = Vector3.Lerp(a, b, randomX);
        Vector3 pointCD = Vector3.Lerp(c, d, randomX);
        Vector3 randomPoint = Vector3.Lerp(pointAB, pointCD, randomY);

        return randomPoint;
    }
}