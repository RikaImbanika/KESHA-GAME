using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Zombie : MonoBehaviour
{
    public string _type;
    public string _visibleName;
    public float _fireCooldown;
    public float _health;
    public float _maxHealth;
    public float _speed;
    public float _nextFireTime;
    public bool _followPlayer;
    public float _realSpeed;
    public bool _active; //Deactivator
    public bool _dead;
    public Animator _ani; //public for FirstZombie.cs

    private float _heigh;
    private float _stopSpeed;
    private float _animationSpeed;
    private string _sceneName;
    private Vector3 _startPosition;
    private string _id;
    private string _idPos;
    private string _idHealth;
    private string _idRot;
    private NavMeshAgent _agent;
    private Vector3 _pos;
    private bool _run;
    private bool _screamerStarted;
    private Collider _collider;
    private EnemyParams _ep;    
    private Optimiser _opti;

    void Start()
    {
        _opti = new Optimiser(gameObject.scene.name);
        
        StartCoroutine(Start0());

        IEnumerator Start0()
        {
            while (S.AllFather == null)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log("Zombie waiting for S.AllFather");
            }

            if (_health == 0)
                _health = 100;
            if (_fireCooldown == 0)
                _fireCooldown = 1.3f;
            if (_speed == 0)
                _speed = 19f;
                
            _heigh = 4f;
            _stopSpeed = 350;
            _animationSpeed = 0.005f;

            _ep = S.AllFather.GetEnemyParams(_type);

            _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
            _id = S.ID(_sceneName, S.ID(gameObject));
            _idPos = S.ID(_id, "pos");
            _idRot = S.ID(_id, "rot");
            _idHealth = S.ID(_id, "health");

            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = _speed;

            _ani = GetComponent<Animator>();

            _collider = gameObject.GetComponent<Collider>();

            _followPlayer = false;

            _startPosition = transform.position;

            while (S.SM == null)
                yield return new WaitForSeconds(0.1f);

            var loadPos = S.SM.LoadVector3(_idPos);

            if (loadPos.HasValue)
            {
                transform.position = loadPos ?? transform.position;
                transform.rotation = S.SM.LoadQuaternion(_idRot) ?? transform.rotation;
                _health = S.SM.LoadFloat(_idHealth) ?? _health;

                if (_health <= 0)
                    Die();
                else
                    InvokeRepeating("SavingMethod", 0f, 5f);
            }
            else
                InvokeRepeating("SavingMethod", 0f, 5f);
        }
    }

    void SavingMethod()
    {
        S.SM.Save(_idPos, transform.position);
        S.SM.Save(_idRot, transform.rotation);
        S.SM.Save(_idHealth, _health);
    }

    public void Damage(float amount)
    {
        if (!_dead)
        {
            _followPlayer = true;
            _health -= amount;

            if (_health <= 0)
            {
                _dead = true;
                Die();
                S.AudioManager.Play("Kill", 1.1f);

                StartCoroutine(Loott());

                IEnumerator Loott()
                {
                    yield return new WaitForSeconds(0.5f);
                    GameObject loot = Instantiate(S.Loot);
                    loot.transform.position = transform.position;
                }
            }
            else
            {
                S.AudioManager.Play("Kill", 0.9f);
            }
        }
    }

    public void Die()
    {
        _ani.SetTrigger("TrDie");
        _agent.speed = 0f;
        _followPlayer = false;
        _health = 0;
        _dead = true;
        Destroy(_collider);
        Destroy(_agent);
    }

    void Update()
    {
        if (_health <= 0)
            _dead = true;

        if (!_dead && _active)
        {
            if (!_screamerStarted)
            {
                if (_opti.Optimise(transform.position))
                {
                    Do();
                    _opti.Reset();
                }

                void Do()
                {
                    if (S.PS._currentSceneName == _sceneName)
                    {
                        Vector3 from = transform.position + new Vector3(0, _heigh, 0);
                        Vector3 toPlayer = S.Camera.transform.position - from;
                        Ray ray = new Ray(from, toPlayer);
                        RaycastHit hit;
                        Physics.Raycast(ray, out hit);

                        _nextFireTime -= _opti.DeltaTime;

                        if (hit.collider.gameObject.tag == "Player")
                        {
                            float angle = Vector3.Angle(toPlayer, transform.forward);
                            if (angle > -90f && angle < 90f)
                            {
                                _followPlayer = true;
                            }

                            if (_followPlayer)
                                Fire();
                        }

                        if (_followPlayer)
                        {
                            _agent.destination = S.Ph.transform.position;
                            Screamer();
                        }
                    }
                    else
                    {
                        _followPlayer = false;
                        _agent.destination = _startPosition;
                    }

                    float k = Math.Min(0.1f / (1 / 60f) * _opti.DeltaTime, 1f);

                    Vector3 delta = transform.position - _pos;
                    _pos = transform.position;
                    _realSpeed = _realSpeed * (1 - k) + delta.magnitude * k * 60f / Time.deltaTime;

                    if (_realSpeed < _stopSpeed)
                    {
                        if (_run)
                        {
                            _ani.SetTrigger("TrIdle");
                            _run = false;
                        }

                        if (_followPlayer)
                            LookToPlayer();
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
            else
            {
                _ani.SetFloat("speed", _stopSpeed * 2 * _animationSpeed);

                transform.position = Camera.main.transform.position;
                transform.rotation = Camera.main.transform.rotation;

                transform.position += transform.right * _ep._screamerX;
                transform.position += transform.up * _ep._screamerY;
                transform.position += transform.forward * _ep._screamerZ;
                transform.Rotate(0, 180, 0);
            }
        }
    }

    void LookToPlayer()
    {
        Vector3 direction = Camera.main.transform.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void Screamer()
    {
        if (!_screamerStarted)
            if (UnityEngine.Random.Range(0, 1200) < 1)
                StartCoroutine(ScreamerC());

        IEnumerator ScreamerC()
        {
            _screamerStarted = true;
            _collider.enabled = false;
            _agent.enabled = false;

            _ani.SetTrigger("TrRun");
            _ani.SetFloat("speed", 4f * _animationSpeed);

            Vector3 normalPosition = transform.position;
            Quaternion normalRotation = transform.rotation;

            transform.position = Camera.main.transform.position;
            transform.rotation = Camera.main.transform.rotation;

            transform.position += transform.right * _ep._screamerX;
            transform.position += transform.up * _ep._screamerY;
            transform.position += transform.forward * _ep._screamerZ;
            transform.Rotate(0, 180, 0);

            int number = UnityEngine.Random.Range(0, _ep._screamerSounds.Length);
            string audio = _ep._screamerSounds[number];
            S.AudioManager.Play(audio, 1);

            yield return new WaitForSeconds(0.7f);

            _screamerStarted = false;
            _ani.SetFloat("speed", _realSpeed * _animationSpeed);

            transform.position = normalPosition;
            transform.rotation = normalRotation;

            _agent.enabled = true;
            _collider.enabled = true;
        }
    }

    void Fire()
    {
        if (_nextFireTime <= 0)
        {
            _nextFireTime = _fireCooldown;
            GameObject bullet = Instantiate(S.FireballRed, S.Loader.Roots[_sceneName]);
            bullet.transform.position = gameObject.transform.position + new Vector3(0, _heigh, 0);
            bullet.transform.LookAt(S.Camera.transform.position);
            Fireball eb = bullet.GetComponent<Fireball>();
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