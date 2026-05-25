using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;
using System;

public class SnakeBrain : MonoBehaviour
{
    public float _acceleration;
    public float _speed;
    public float _turn;
    public float _health;
    public float _maxHealth;
    public string _visibleName;
    public Color _lookNameColor;
    public Color _lifeBarNameColor;
    public Color _lifeColor;
    public string _id;
    public string _idHealth;

    public SnakeHead _head;

    public Vector3[] _points;
    private Vector2[] _corners2d;

    private UnityEngine.AI.NavMeshAgent _agent;
    private bool _stuckAvoidance;
    private bool _tailEnabled;
    private Vector3 _velocity;
    private float _distanceThreshold = 3f;
    private float _areaDivideTotalBuffered;
    private bool _dead;

    void Start()
    {
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _agent.acceleration = 0;
        _head._aims.Add(transform.position);

        MakeCorners2d();
        PreCalculateAreas(_corners2d);
    }

    public void Damage(float amount)
    {
        if (!_dead)
        {
            _health = MathF.Max(_health - amount, 0);
            S.SM.Save(_idHealth, _health);

            if (_health <= 0)
            {
                _dead = true;
                Die();
            }
            else
            {
                S.AM.Play("Kill", 0.9f);
            }
        }
    }

    void Die()
    {
        S.SM.Save(_idHealth, _health);

        S.AM.Play("Kill", 1.1f);

        StartCoroutine(Later());

        IEnumerator Later()
        {
            for (int i = 0; i < _head._ballsCount; i++)
            {
                int a = S.RND.Next(3);
                if (a == 0)
                    SetLoot(1);
                if (a == 1)
                {
                    SetLoot(2);
                }

                void SetLoot(int count)
                {
                    Vector3 point1 = _head._clones[i].transform.position;

                    GameObject loot = Instantiate(S.Loot, point1, Quaternion.identity, S.Loader.Roots[_head._sceneName]);
                    ItemP itemP = loot.GetComponent<ItemP>();
                    itemP._count = count;

                    RaycastHit hit;
                    if (Physics.Raycast(point1, Vector3.down, out hit, 10f))
                    {
                        Vector3 point2 = hit.point;
                        loot.transform.position = point2;
                    }
                    else
                        Destroy(loot);
                }
            }

            _head.Die();
            Destroy(gameObject);
            yield return null;
        }
    }

    void MakeCorners2d()
    {
        _corners2d = new Vector2[4];
        for (int i = 0; i < 4; i++)
            _corners2d[i] = new Vector2(_points[i].x, _points[i].z);

        _corners2d = OrderQuadPointsClockwise(_corners2d);

        Vector2[] OrderQuadPointsClockwise(Vector2[] points)
        {
            Vector2 center = (points[0] + points[1] + points[2] + points[3]) / 4f;

            Vector2[] ordered = (Vector2[])points.Clone();

            Array.Sort(ordered, (p1, p2) =>
            {
                float angle1 = Mathf.Atan2(p1.y - center.y, p1.x - center.x);
                float angle2 = Mathf.Atan2(p2.y - center.y, p2.x - center.x);
                return angle1.CompareTo(angle2);
            });

            return ordered;
        }
    }

    void Update()
    {
        Debug.DrawRay(transform.position - transform.up * 50f, transform.up * 100f, Color.red, 0.1f);

        if (_head._aims.Count == 0)
            _head._aims.Add(transform.position);
        else if ((_head._aims[_head._aims.Count - 1] - transform.position).magnitude > 0.5f)
            _head._aims.Add(transform.position);

        CheckP();

        if ((transform.position - _agent.destination).magnitude < _distanceThreshold)
        {
            _stuckAvoidance = false;
            SwitchTail(!_stuckAvoidance);
            _agent.destination = GetNewPoint();
            //This is doing something important
        }

        Vector2 p = new Vector2(transform.position.x, transform.position.z);
        if (!IsPointInQuad(p, _corners2d))
        {
            Vector2 center = (_corners2d[0] + _corners2d[2]) / 2;
            transform.position = new Vector3(center.x, transform.position.y, center.y);

            _stuckAvoidance = true;
            SwitchTail(!_stuckAvoidance);
            _agent.destination = GetNewPoint();
            Debug.LogError($"Brain is out of quad");
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
        _head.SwitchTail(enabled, Time.deltaTime);
    }

    Vector3 GetNewPoint()
    {
        Vector3 point = new Vector3();
        Vector2 flatPoint = new Vector2();
        for (int i = 0; i < 60; i++)
        {
            point = transform.position;
            flatPoint = new Vector2(point.x, point.z);

            while ((point - transform.position).magnitude < 8f)
            {
                //Maybe dangerous in little scenes, but they should not exist
                flatPoint = GetRandomPointInQuad(_corners2d);
                point = new Vector3(flatPoint.x, point.y, flatPoint.y);
            }
            
            bool reachable;
            Vector3 newDirection = GetDirectionAndWait(point, Vector3.zero, out reachable);

            if (reachable)
            {
                Debug.Log("REACHABLE");
                return point;
            }
        }

        Debug.Log("I'M STUCK");
        _stuckAvoidance = true;
        SwitchTail(!_stuckAvoidance);
        return point;
    }

    private Vector3 GetDirectionAndWait(Vector3 targetPosition, Vector3 direction, out bool reachable)
    {
        NavMeshPath path = new NavMeshPath();
        _agent.CalculatePath(targetPosition, path);

        for (int i = 0; i < 500000; i++)
            if (!_agent.pathPending)
                break;  //yield? //Cannot add it here easely

        if (path.corners.Length >= 2)
        {
            Vector3 final = path.corners[path.corners.Length - 1];
            Vector3 target = new Vector3(targetPosition.x, final.y, targetPosition.z);

            Debug.DrawRay(target - transform.up * 50f, transform.up * 100f, Color.blue, 0.1f);
            Debug.DrawRay(final - transform.up * 50f, transform.up * 100f, Color.green, 0.1f);

            float dist = (final - target).magnitude;
            Debug.LogError($"Distance: {dist}");
            reachable = dist < _distanceThreshold;
        }
        else
            reachable = false;

        if (path.corners.Length >= 2)
            return (path.corners[1] - path.corners[0]).normalized;
        else
            return -direction;
    }

    public Vector2 GetRandomPointInQuad(Vector2[] quad)
    {
        if ((float)S.RND.NextDouble() < _areaDivideTotalBuffered)
            return RandomPointInTriangle(quad[0], quad[1], quad[2]);
        else
            return RandomPointInTriangle(quad[0], quad[2], quad[3]);

        Vector2 RandomPointInTriangle(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float r1 = (float)S.RND.NextDouble();
            float r2 = (float)S.RND.NextDouble();
            if (r1 + r2 > 1f)
            {
                r1 = 1f - r1;
                r2 = 1f - r2;
            }
            return p1 + r1 * (p2 - p1) + r2 * (p3 - p1);
        }
    }

    void PreCalculateAreas(Vector2[] quad)
    {
        //For optimisation
        float area1 = TriangleArea(quad[0], quad[1], quad[2]);
        float area2 = TriangleArea(quad[0], quad[2], quad[3]);
        float total = area1 + area2;
        _areaDivideTotalBuffered = area1 / total;

        float TriangleArea(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return Mathf.Abs((p2.x - p1.x) * (p3.y - p1.y) - (p3.x - p1.x) * (p2.y - p1.y)) * 0.5f;
        }
    }

    bool IsPointInQuad(Vector2 point, Vector2[] quad)
    {
        return IsPointInTriangle(point, quad[0], quad[1], quad[2]) ||
            IsPointInTriangle(point, quad[0], quad[2], quad[3]);

        bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            float Cross(Vector2 o, Vector2 p1, Vector2 p2) =>
                (p1.x - o.x) * (p2.y - o.y) - (p1.y - o.y) * (p2.x - o.x);

            bool sign1 = Cross(a, b, p) >= 0f;
            bool sign2 = Cross(b, c, p) >= 0f;
            bool sign3 = Cross(c, a, p) >= 0f;

            return (sign1 == sign2) && (sign2 == sign3);
        }
    }   
}