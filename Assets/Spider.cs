using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Spider : MonoBehaviour
{
    public float _speed;

    string _sceneName;

    public float _nextLaserTime;
    public float _nextFireTime;

    public bool _followPlayer;
    
    public bool _dead;

    public float _health;

    private Vector3[] _directions;
    private GameObject[] _lasers;
    private bool[] _damagePlayer;
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

    void Start()
    {
        _opti = new Optimiser(gameObject.scene.name);
        _opti.MinFps = 1 / 24f;
        
        _laserCooldown = 0.3f;
        _fireCooldown = 0.7f;
        _stopSpeed = 350;
        _animationSpeed = 0.005f; //Still need check
        
        _layerMask = 1 << LayerMask.NameToLayer("Player") |
                         1 << LayerMask.NameToLayer("Static") |
                         1 << LayerMask.NameToLayer("Enemies") |
                         1 << LayerMask.NameToLayer("Items") |
                         1 << LayerMask.NameToLayer("Default");
        
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
        _damagePlayer = new bool[4];

        _collider = gameObject.GetComponent<Collider>();

        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _speed;

        _ani = GetComponent<Animator>();
        _followPlayer = false;              

        for (int i = 0; i < 4; i++)
        {
            _directions[i] = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-0.2f, 0.2f), UnityEngine.Random.Range(-1f, 1f));
            _lasers[i] = Instantiate(S.RedLaser, transform.position);
        }

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
        }

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
        _ani.SetTrigger("TrDie");
        _agent.speed = 0f;
        _followPlayer = false;
        for (int i = 0; i < 4; i++)
            Destroy(_lasers[i]);;
        Destroy(_collider);
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
            if(!_dead)
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
                                _lasers[i].SetActive(true);
                        }
                    }
                }
    
                if (_followPlayer)
                {
                    Laser();
                    Fire();
                    _agent.destination = S.Ph.transform.position;
                }                
                else
                {
                    _followPlayer = false;
                    _agent.destination = _startPosition;
        
                    for (int i = 0; i < 4; i++)
                        _lasers[i].SetActive(false);
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

    void Laser()
    {
        _nextLaserTime -= _opti.DeltaTime;
        
        _laserRotation += _laserRotationDelta * _opti.DeltaTime * 0.001f;

        if (_nextLaserTime <= 0)
        {
            _nextLaserTime = _laserCooldown;

            _directions[Convert.ToInt32(UnityEngine.Random.Range(0, 3))] = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-0.2f, 0.2f), UnityEngine.Random.Range(-1f, 1f));
        }

        Vector3 from = transform.position + new Vector3(0, 2, 0);
        
        _laserOptimiser++;

        for (int i = 0; i < 4; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, _laserRotation, 0);

            Ray ray = new Ray(from, rotation * _directions[i]);
            RaycastHit hit;
            
            _lasers[i].transform.rotation = Quaternion.LookRotation(_directions[i]);

            if (_laserOptomiser >= 4)
            {
                _laserOptomiser = 0;
                
                if (Physics.Raycast(ray, out hit, _layerMask))
                {
                    _laserLength[i] = (hit.point - from).magnitude;
                    var scale = _lasers[i].transform.localScale;
                    float factor = 0.9f;
                    scale.z = _laserLength[i] * factor;
                    _lasers[i].transform.localScale = scale;
    
                    /*                if (UnityEngine.Random.Range(0, 4) < 1)
                                    {
                                        GameObject sparkle = Instantiate(_sparkle);
                                        sparkle.transform.position = hit.point;
                                        sparkle.transform.rotation = Quaternion.LookRotation(hit.normal);
                                        sparkle.GetComponent<IsSparkle>()._active = true;
                                    }*/
                                    
                _damagePlayer[i] = hit.collider.gameObject.CompareTag("Player");
                }
                else
                    _damagePlayer[i] = false;
            }
            
            
            if (_damagePlayer[i])
                S.PS.Damage(0.75f);
        }
    }

    void Fire()
    {
        _nextFireTime -= _opti.DeltaTime;

        if (_nextFireTime <= 0)
        {
            _nextFireTime = _fireCooldown;
            GameObject bullet = Instantiate(S.EnemyBullet);
            bullet.transform.position = gameObject.transform.position + new Vector3(0, 2, 0);
            //bullet.transform.LookAt(S.Camera.transform.position);
            bullet.transform.Rotate(0, UnityEngine.Random.Range(0f, 360f), 0);
            EnemyBullet eb = bullet.GetComponent<EnemyBullet>();
            eb._active = true;
            eb._speed = 30;
            Destroy(bullet, 15);

            AudioSource shot = Instantitiste(S.Shot);
            shot.transform.position = transform.position;
            shot.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            float distance = (transform.position - S.Camera.transform.position).magnitude;
            shot.volume = MathF.Min(0.5f, 60 / (distance * distance));
            shot.Play();
            Destroy(shot, 5);
        }
    }
}