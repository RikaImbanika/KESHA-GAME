using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Spider : MonoBehaviour
{
    private int _lasersCount;
    public float _speed;

    string _sceneName;

    public float _nextLaserTime;
    public float _nextFireTime;

    public bool _followPlayer;

    public bool _dead;

    public float _health;

    private Vector3[] _directions;
    private GameObject[] _lasers;
    private GameObject[] _points;
    private GameObject _laserDown;
    private float _damagePlayer;
    private float[] _laserLength;

    //Privates
    private Vector3 _startPosition;
    private string _key;
    private Collider _collider;
    private NavMeshAgent _agent;
    private float _laserCooldown;
    private float _fireCooldown;
    private Vector3 _pos;
    private float _realSpeed;
    private float _stopSpeed;
    private float _animationSpeed;
    private bool _run;
    private Animator _ani;
    private float _laserRotation;
    private float _laserRotationDelta;

    private Optimiser _opti;
    private int _layerMask;
    private int _laserOptimiser;
    private Vector3 _lasersOffset;
    private float _lasersScaleFactor;


    void Start()
    {
        _lasersCount = 4;
        _lasersOffset = new Vector3(0, 6, 0);

        _opti = new Optimiser(gameObject.scene.name);
        _opti.MaxPeriodForDistance = 1 / 24f;

        _laserCooldown = 0.3f;
        _fireCooldown = 0.7f;
        _stopSpeed = 350;
        _animationSpeed = 0.005f; //Still need check

        _layerMask = 1 << LayerMask.NameToLayer("Player") |
                        1 << LayerMask.NameToLayer("Static") |
                        1 << LayerMask.NameToLayer("Enemies") |
                        1 << LayerMask.NameToLayer("Items") |
                        1 << LayerMask.NameToLayer("Default");

        _laserLength = new float[_lasersCount];

        if (Mathf.RoundToInt(UnityEngine.Random.Range(0f, 1f)) == 0)
            _laserRotationDelta = 1f;
        else
            _laserRotationDelta = -1f;

        _startPosition = transform.position;
        _health = 300f;

        _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
        _key = _sceneName + transform.position.x + transform.position.y + transform.position.z;

        _directions = new Vector3[4];
        _lasers = new GameObject[4];
        _points = new GameObject[4];

        _collider = gameObject.GetComponent<Collider>();

        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _speed;

        _ani = GetComponent<Animator>();
        _followPlayer = false;

        for (int i = 0; i < _lasersCount; i++)
        {
            _directions[i] = GetLasDir();
            _lasers[i] = Instantiate(S.RedLaser, transform.position + _lasersOffset, transform.rotation, transform); //

            _lasersScaleFactor = 1f / _lasers[i].transform.lossyScale.z;
            _lasers[i].SetActive(false);
            _points[i] = Instantiate(S.RedPoint, transform.position + _lasersOffset, transform.rotation, transform); //
            _points[i].SetActive(false);
        }

        _laserDown = Instantiate(S.RedLaser, transform.position + _lasersOffset, Quaternion.LookRotation(Vector3.down), transform); //
        _laserDown.SetActive(false);

        var loadPos = S.SM.LoadVector3(S.ID(_key, "position"));

        if (loadPos.HasValue)
        {
            transform.position = loadPos ?? transform.position;
            transform.rotation = S.SM.LoadQuaternion(S.ID(_key, "rotation")) ?? transform.rotation;
            _health = S.SM.LoadFloat(S.ID(_key, "health")) ?? _health;

            if (_health <= 0)
            {
                _ani.SetFloat("deathSpeed", 100f);
                Die();
            }
            else
                InvokeRepeating("SavingMethod", 0f, 3f);
        }
        else
            InvokeRepeating("SavingMethod", 0f, 3f);
    }

    void SavingMethod()
    {
        S.SM.Save(S.ID(_key, "position"), transform.position);
        S.SM.Save(S.ID(_key, "rotation"), transform.rotation);
        S.SM.Save(S.ID(_key, "health"), _health);
    }

    public void Damage(float amount)
    {
        _followPlayer = true;

        _health -= amount;

        if (_health <= 0)
        {
            if (!_dead)
            {
                _dead = true;

                for (int i = 0; i < 4; i++)
                    Destroy(_lasers[i]);

                Die();

                StartCoroutine(Loott());

                IEnumerator Loott()
                {
                    yield return new WaitForSeconds(0.5f);
                    GameObject loot = Instantiate(S.Loot);
                    loot.transform.position = transform.position;
                }

                S.AudioManager.Play("Kill", 1.1f);
            }
        }
        else
            S.AudioManager.Play("Kill", 0.9f);
    }

    public void Die()
    {
        _dead = true;
        _ani.SetTrigger("TrDie");
        _agent.speed = 0f;
        _followPlayer = false;
        for (int i = 0; i < 4; i++)
            Destroy(_lasers[i]);
        for (int i = 0; i < 4; i++)
            Destroy(_points[i]);
        Destroy(_laserDown);
        Destroy(_collider);
        Destroy(_agent);
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
            if (!_dead)
            {
                CheckPlayer();

                if (_followPlayer)
                {
                    Laser();
                    Fire();
                    _agent.destination = S.Ph.transform.position;
                }

                float k = Math.Min(0.1f / (1 / 60f) * _opti.DeltaTime, 1f);

                Vector3 delta = transform.position - _pos;
                _pos = transform.position;
                _realSpeed = _realSpeed * (1 - k) + delta.magnitude * k * 60f / _opti.DeltaTime;

                if (_realSpeed < _stopSpeed)
                {
                    if (_run)
                    {
                        _ani.SetTrigger("TrIdle");
                        _run = false;
                    }
                }
                else
                {
                    if (!_run)
                    {
                        _ani.SetTrigger("TrRun");
                        _run = true;
                    }
                    _ani.SetFloat("speed", _realSpeed * _animationSpeed);
                }
            }
        }

        void CheckPlayer()
        {
            if (S.PS._currentSceneName != _opti.SceneName)
            {
                if (_followPlayer)
                {
                    _followPlayer = false;
                    _agent.destination = _startPosition;

                    for (int i = 0; i < 4; i++)
                    {
                        _lasers[i].SetActive(false);
                        _points[i].SetActive(false);
                    }

                    _laserDown.SetActive(false);
                }
            }
            else
            {
                Vector3 toPlayer = S.Camera.transform.position - transform.position;
                Ray ray = new Ray(transform.position, toPlayer);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);

                if (hit.collider.gameObject.tag == "Player")
                {
                    float angle = Vector3.Angle(toPlayer, transform.forward);
                    if (angle > -90f && angle < 90f)
                    {
                        if (!_followPlayer)
                        {
                            _followPlayer = true;
                            for (int i = 0; i < 4; i++)
                            {
                                _lasers[i].SetActive(true);
                                _points[i].SetActive(true);
                            }

                            //

                            _laserDown.SetActive(true);
                            var scale = new Vector3(1.5f, 1.5f, 3f);
                            _laserDown.transform.localScale = scale;
                        }
                    }
                }
            }
        }

        void Laser()
        {
            _nextLaserTime -= _opti.DeltaTime;

            _laserRotation += _laserRotationDelta * _opti.DeltaTime * 0.001f;

            if (_nextLaserTime <= 0)
            {
                _nextLaserTime = _laserCooldown;

                _directions[Convert.ToInt32(UnityEngine.Random.Range(0, 3))] = GetLasDir();
            }

            _laserOptimiser++;

            if (_laserOptimiser >= 4)
            {
                _damagePlayer = 0;
                _laserOptimiser = 0;

                Vector3 from = transform.position + _lasersOffset;

                for (int i = 0; i < 4; i++)
                {
                    Quaternion rotation = Quaternion.Euler(0, _laserRotation, 0);
                    _directions[i] = rotation * _directions[i];

                    _lasers[i].transform.rotation = Quaternion.LookRotation(_directions[i]);

                    Ray ray = new Ray(from, _directions[i]);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, _layerMask))
                    {
                        _laserLength[i] = (hit.point - from).magnitude;
                        var scale = _lasers[i].transform.localScale;
                        scale.z = _laserLength[i] * _lasersScaleFactor;
                        _lasers[i].transform.localScale = scale;
                        _points[i].transform.position = hit.point;
                        _points[i].transform.rotation = S.RandRot.Get();

                        int speed = (int)(3 * 60f * _opti.DeltaTime);
                        if (S.RND.Next(0, speed) == 0)
                        {
                            GameObject sparkle = Instantiate(S.RedSparkle);
                            sparkle.transform.position = hit.point;
                            sparkle.transform.rotation = Quaternion.LookRotation(hit.normal);
                        } /////////////

                        if (hit.collider.gameObject.CompareTag("Player"))
                            _damagePlayer += 24f;
                    }
                }
            }

            if (_damagePlayer > 0)
                S.PS.Damage(_damagePlayer * _opti.DeltaTime);
        }



        void Fire()
        {
            _nextFireTime -= _opti.DeltaTime;

            if (_nextFireTime <= 0)
            {
                _nextFireTime = _fireCooldown;
                Quaternion rotation = Quaternion.Euler(UnityEngine.Random.Range(-30f, 30f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(-30f, 30));
                GameObject bullet = Instantiate(S.FireballRed, gameObject.transform.position + new Vector3(0, 6, 0), rotation, S.Loader.Roots[_sceneName]);

                Fireball eb = bullet.GetComponent<Fireball>();
                eb._active = true;
                eb._speed = 30;
                Destroy(bullet, 15);

                AudioSource shot = Instantiate(S.Shot);
                shot.transform.position = transform.position;
                shot.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                float distance = (transform.position - S.Camera.transform.position).magnitude;
                shot.volume = MathF.Min(0.5f, 60 / (distance * distance));
                shot.Play();
                Destroy(shot, 5);
            }
        }
    }

    Vector3 GetLasDir()
    {
        return new Vector3(UnityEngine.Random.Range(-15f, 15f), UnityEngine.Random.Range(-8, 7), UnityEngine.Random.Range(-15f, 15f));
    }
}