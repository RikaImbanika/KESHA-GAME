using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Granny : MonoBehaviour
{
    public Vector3 _startPosition; //set from editor
    public Quaternion _startRotation; //set from editor
    public Vector2[] _corners2d;
    public float _fleeingSpeed;
    public float _calmingSpeed;
    public float _fleeingAcceleration;
    public float _calmingAcceleration;
    public long _lastGreetingsTime;

    private string _sceneName;
    private string _id;
    private string _idPos;
    private string _idRot;
    private string _idPhase;

    private NavMeshAgent _agent;
    public string _phase;
    private Optimiser _opti;
    private float _areaDivideTotalBuffered;
    private Vector3 _aim;
    private float _distanceThreshold;
    private int _calmingPointsLeft;
    private Trader _trader;
    private string _previousPlayerScene;
    private bool _saidFirstAim;

    void Start()
    {
        //LETS FUN!!!

        StartCoroutine(Start0());

        IEnumerator Start0()
        {
            _distanceThreshold = 3f;

            _phase = "Stay on market";

            _opti = new Optimiser(gameObject.scene.name);

            FixCorners();
            PreCalculateAreas(_corners2d);

            _agent = GetComponent<NavMeshAgent>();
            _agent.acceleration = _calmingAcceleration;

            _trader = GetComponent<Trader>();

            _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
            _id = "HallGranny";
            _idPos = S.IDM(_id, "pos");
            _idRot = S.IDM(_id, "rot");
            _idPhase = S.IDM(_id, "phase");

            _lastGreetingsTime = S.SM.LoadLong("GrannyLastGreetingsTime") ?? 0;
            _saidFirstAim = S.SM.LoadBool("GrannySaidFirstAim") ?? false;

            _phase = S.SM.LoadString(_idPhase) ?? "Stay on market";

            if (!_phase.Equals("Stay on market"))
                Load();

            S.Granny = this;

            yield return null;
        }
    }

    void Load()
    {
        Debug.LogError($"Loaded Granny Phase: {_phase}");

        transform.position = S.SM.LoadVector3(_idPos) ?? _startPosition;
        transform.rotation = S.SM.LoadQuaternion(_idRot) ?? _startRotation;

        if (_phase.Equals("Fleeing"))
            StartFleeing();
        else if (_phase.Equals("Calming"))
            Calming();
        else if (_phase.Equals("Returning to market"))
            ReturnToMarket();
    }

    public void StartFleeing()
    {
        _phase = "Fleeing";
        _agent.speed = _fleeingSpeed;
        _agent.acceleration = _fleeingAcceleration;
        _trader.CloseMarket();
        S.Console.AddMessage("Granny: Aaaaaaaaaahhhhhhhhhh!", Color.cyan);
        GoToNewPoint();
    }

    public void Calming()
    {
        _phase = "Calming";
        _calmingPointsLeft = 3;
        _agent.speed = _calmingSpeed;
        _agent.acceleration = _calmingAcceleration;
        GoToNewPoint();
    }

    public void ReturnToMarket()
    {
        _phase = "Returning to market";
        _aim = _startPosition;
        _agent.destination = _aim;
    }

    public void ReturnToMarketFinal()
    {
        _phase = "Stay on market";
        transform.position = _startPosition;
        transform.rotation = _startRotation;
        Save();
    }

    void Update()
    {
        if (_opti.Optimise(transform.position))
        {
            Do();
            _opti.Reset();
        }

        void Do()
        {
            if (!_phase.Equals("Stay on market"))
            {
                Vector3 dif = transform.position - _aim;
                float dist = dif.magnitude;

                if (dist < _distanceThreshold)
                {
                    if (_phase.Equals("Fleeing"))
                    {
                        GoToNewPoint();
                    }
                    else if (_phase.Equals("Calming"))
                    {
                        _calmingPointsLeft--;

                        if (_calmingPointsLeft > 0)
                            GoToNewPoint();
                        else
                            ReturnToMarket();
                    }
                    else if (_phase.Equals("Returning to market"))
                    {
                        ReturnToMarketFinal();
                    }
                }

                Save();
            }

            Talking();
        }
    }

    void Talking()
    {
        if (_phase.Equals("Stay on market") || _phase.Equals("Calming"))
        {
            if (S.Ps._currentSceneName == _sceneName && _previousPlayerScene != _sceneName)
            {
                _previousPlayerScene = _sceneName;

                long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                long delta = now - _lastGreetingsTime;

                if (delta > 120)
                {
                    S.Console.AddMessage($"Granny: Hello, dear!)", Color.cyan);
                    _lastGreetingsTime = now;
                    S.SM.Save("GrannyLastGreetingsTime", _lastGreetingsTime);
                    
                    if (!_saidFirstAim)
                    {
                        _saidFirstAim = true;
                        StartCoroutine(SayAimLater());
                        S.SM.Save("GrannySaidFirstAim", _saidFirstAim);
                    }

                    IEnumerator SayAimLater()
                    {
                        yield return new WaitForSeconds(2f);
                        S.Console.AddMessage($"Granny: Those bakas came and turned my house into chaos!", Color.cyan);
                        yield return new WaitForSeconds(2f);
                        S.Console.AddMessage($"Rika: Oh no!", Color.magenta);
                    }
                }
            }
        }
    }

    void Save()
    {
        S.SM.Save(_idPos, transform.position);
        S.SM.Save(_idRot, transform.rotation);
        S.SM.Save(_idPhase, _phase);
    }
    
    void GoToNewPoint()
    {
        _aim = GetNewPoint();
        _agent.destination = _aim;
    }

    Vector3 GetNewPoint()
    {
        Vector3 point;

        for (int i = 0; i < 10; i++)
        {
            point = transform.position;
            Vector2 flatPoint = new Vector2(point.x, point.z);

            while ((point - transform.position).magnitude < 3f)
            {
                //Maybe dangerous in little scenes, but they should not exist
                flatPoint = GetRandomPointInQuad(_corners2d);
                point = new Vector3(flatPoint.x, point.y, flatPoint.y);
            }
            
            bool reachable;
            Vector3 newDirection = GetDirectionAndWait(point, Vector3.zero, out reachable);

            if (reachable)
            {
                Debug.LogError("REACHABLE");
                return point;
            }
        }

        Debug.LogError("Sorry, granny stuck.");
        return _startPosition;
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

    //Math

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

    void FixCorners()
    {
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
}
