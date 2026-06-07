// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

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
    public string _idPos;
    public string _idType;
    public bool _forLoader;

    public SnakeHead _head;

    private UnityEngine.AI.NavMeshAgent _agent;
    private bool _stuckAvoidance;
    private bool _tailEnabled;
    private Vector3 _velocity;
    private float _distanceThreshold = 3f;
    private bool _dead;
    private string _sceneName;
    private SceneRoot _sceneRoot;

    void Start()
    {
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _agent.acceleration = 0;
        _head._aims.Add(transform.position);
        _sceneName = gameObject.scene.name;

        GetId();

        StartCoroutine(LateStart());

        IEnumerator LateStart()
        {
            while (S.Loader == null)
                yield return new WaitForSeconds(0.33f);

            while (S.Loader.SceneRoots == null)
                yield return new WaitForSeconds(0.33f);

            while (!S.Loader.SceneRoots.ContainsKey(_sceneName))
                yield return new WaitForSeconds(0.33f);

            while (S.Loader.SceneRoots[_sceneName] == null)
                yield return new WaitForSeconds(0.33f);

            _sceneRoot = S.Loader.SceneRoots[_sceneName];

            InvokeRepeating("SavingMethod", 0f, 5f);
        }
    }

    void GetId()
    {
        _idPos = S.IDM(_id, "pos");
        _idHealth = S.IDM(_id, "hp");
        _idType = S.IDM(_id, "type");
    }

    void SavingMethod()
    {
        S.SM.Save(_idPos, transform.position);
        S.SM.Save(_idHealth, _health);
    }

    public void Damage(float amount)
    {
        if (!_dead)
        {
            _health = MathF.Max(_health - amount, 0);

            if (_health > _maxHealth)
                _health = _maxHealth;
            else if (_health <= 0)
            {
                _dead = true;
                Die();
            }
            else
            {
                S.AM.Play("Kill", 0.9f);
            }

            S.SM.Save(_idHealth, _health);
        }
    }

    void Die()
    {
        S.AM.Play("Kill", 1.1f);
                
        S.SM.Save(_idHealth, _health);

        if (_forLoader)
        {
            S.SM.RemoveFromList(S.IDM(_sceneName, "ids"), _id);
            S.SM.RemoveString(_idHealth);
            S.SM.RemoveVector3(_idPos);
            S.SM.RemoveQuaternion(_idType);
        }

        StartCoroutine(Later());

        IEnumerator Later()
        {
            int layerMaskForLoot = 1 << LayerMask.NameToLayer("Static") |
            1 << LayerMask.NameToLayer("Default");

            for (int i = 0; i < _head._ballsCount; i++)
            {
                int a = S.RND.Next(3);
                if (a == 0)
                    SetLoot(1);
                else if (a == 1)
                    SetLoot(2);

                void SetLoot(int count)
                {
                    Vector3 point1 = _head._clones[i].transform.position - Vector3.down * 2f; //it's up

                    GameObject loot = Instantiate(S.Loot, point1, Quaternion.identity, S.Loader.Roots[_head._sceneName]);
                    ItemP itemP = loot.GetComponent<ItemP>();
                    itemP._count = count;

                    RaycastHit hit;
                    if (Physics.Raycast(point1, Vector3.down, out hit, 40f, layerMaskForLoot))
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

        if (!_agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                _agent.Warp(hit.position);
            }
            else
                transform.position = Vector3.zero; //

            _stuckAvoidance = true;
            SwitchTail(!_stuckAvoidance);
            _agent.destination = GetNewPoint();
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
                point = _sceneRoot.GetRandomPoint();
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
            reachable = dist < _distanceThreshold;
        }
        else
            reachable = false;

        if (path.corners.Length >= 2)
            return (path.corners[1] - path.corners[0]).normalized;
        else
            return -direction;
    }
}