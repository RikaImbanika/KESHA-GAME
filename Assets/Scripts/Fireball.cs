// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fireball : MonoBehaviour
{
    public float _speed;
    public bool _active;
    private int _layerMask;

    private Optimiser _opti;
    public Transform[] _children;
    public string _color;
    private string _sceneName;
    public float _damage;
    private MaterialPropertyBlock _mpb;
    private Transform _sceneRoot;
    private Transform[] _trailParticles;
    private Vector3[] _trailVelocities;
    private Vector3[] _trailAccelerations;
    private bool[] _trailDead;
    private float[] _trailAges;
    private Vector3 _trailInitialScale;
    private float _distance;
    private const ushort _trailParticlesCount = 12;
    private const float _trailStep = 4.5f;
    private const float _trailSpeedCoef = 0.00008f;
    private const float _trailAccelerationCoef = 0.00016f;
    private int _trailSpawnedCount;
    private int _trailCreatedCount;
    private float _trailLifetime;
    private Vector3 _forwardBuf;
    private float _deltaDistance;
    private bool _dying;
    byte _trailParticleOptimiser;

    void Start()
    {
        _sceneName = gameObject.scene.name;
        _opti = new Optimiser(_sceneName);
        _opti.MaxPeriodForDistance = 1 / 12f;

        _trailParticles = new Transform[_trailParticlesCount];
        _trailVelocities = new Vector3[_trailParticlesCount];
        _trailAccelerations = new Vector3[_trailParticlesCount];
        _trailAges = new float[_trailParticlesCount];
        _trailDead = new bool[_trailParticlesCount];

        float safeSpeed = MathF.Max(0.0001f, MathF.Abs(_speed));
        _trailLifetime = _trailStep * _trailParticlesCount / safeSpeed;

        _sceneRoot = S.Loader.Roots[_sceneName];

        MaterialPropertyBlock srcMpb = S.Fog.GetMPB(_sceneName);
        _mpb = new MaterialPropertyBlock();
        _mpb.SetColor("_FogColor", srcMpb.GetColor("_FogColor"));
        _mpb.SetFloat("_FogDensity", srcMpb.GetFloat("_FogDensity") * 0.75f);

        S.Fog.ApplyToGameObject(gameObject, _mpb);

        _forwardBuf = gameObject.transform.forward.normalized;

        if (_damage == 0)
            _damage = 5f;

        _layerMask = 1 << LayerMask.NameToLayer("Player") |
                     1 << LayerMask.NameToLayer("Static") |
                     1 << LayerMask.NameToLayer("Enemies") |
                     1 << LayerMask.NameToLayer("Items") |
                     1 << LayerMask.NameToLayer("Default");

        AudioSource shot = Instantiate(S.Shot);
        shot.transform.position = transform.position;
        shot.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        float distance = (transform.position - S.Camera.transform.position).magnitude;
        shot.volume = MathF.Min(0.15f, 36 / (distance * distance));
        shot.Play();
        Destroy(shot.gameObject, 5);
    }

    void Update()
    {
        if (_opti == null)
            return;

        if (!_active && !_dying)
            return;

        if (_opti.Optimise(transform.position))
        {
            Do();
            _opti.Reset();
        }

        void Do()
        {
            if (_active)
            {
                _deltaDistance = _opti.DeltaTime * _speed;
                gameObject.transform.position += _forwardBuf * _deltaDistance;

                SpawnTrailParticles();
                UpdateTrailParticles();

                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 60 * _opti.DeltaTime, _layerMask))
                {
                    PushRigidbody(hit);

                    GameObject go = hit.collider.gameObject;
                    if (go.CompareTag("Player"))
                    {
                        PlayerStorage ps = go.transform.parent.gameObject.GetComponent<PlayerStorage>();
                        ps.Damage(_damage);

                        bool damaged = S.SM.LoadBool("DamagedByFireball") ?? false;

                        if (!damaged)
                        {
                            S.SM.Save("DamagedByFireball", true);

                            S.AllFather.StartCoroutine(Ough());

                            IEnumerator Ough()
                            {
                                yield return new WaitForSeconds(0.8f);
                                S.Console.AddMessage("Rika: Ough!", Color.magenta);
                            }
                        }
                    }

                    AudioSource caboom = Instantiate(S.Caboom);
                    caboom.transform.position = gameObject.transform.position;
                    caboom.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                    float distance = (transform.position - S.Camera.transform.position).magnitude;
                    caboom.volume = MathF.Min(0.20f, 50 / (distance * distance));
                    caboom.Play();
                    Destroy(caboom.gameObject, 5);

                    NoSpots ns = go.GetComponent<NoSpots>();
                    if (ns == null)
                    {
                        GameObject spot = Instantiate(S.Spot);
                        spot.transform.position = hit.point;
                        spot.transform.rotation = Quaternion.LookRotation(hit.normal);
                        spot.transform.Rotate(0f, 0f, UnityEngine.Random.Range(-180, 180));
                        spot.transform.SetParent(hit.collider.gameObject.transform);
                        spot.transform.localScale = spot.transform.localScale * 5;

                        S.Fog.ApplyToGameObject(spot, _mpb);
                        //This should include HitPoint

                        S.AllFather._spots.Add(spot);
                        if (S.AllFather._spots.Count > 300)
                        {
                            Destroy(S.AllFather._spots[0]);
                            S.AllFather._spots.RemoveAt(0);
                        }
                    }

                    for (int i = 0; i < S.AllFather._enemyBulletSparklesCount; i++)
                    {
                        GameObject sparkle = null;

                        if (_color == "red")
                            sparkle = Instantiate(S.RedSparkle, _sceneRoot);
                        else if (_color == "green")
                            sparkle = Instantiate(S.GreenSparkle, _sceneRoot);
                        else if (_color == "blue")
                            sparkle = Instantiate(S.BlueSparkle, _sceneRoot);
                        else if (_color == "purple")
                            sparkle = Instantiate(S.PurpleSparkle, _sceneRoot);

                        sparkle.transform.position = hit.point;
                        sparkle.transform.rotation = Quaternion.LookRotation(hit.normal);

                        S.Fog.ApplyToGameObject(sparkle, _mpb);

                        int gg2 = S.RND.Next(25);

                        float decay = 1f;
                        if (gg2 == 0)
                            decay = 0.5f;
                        else if (gg2 == 1)
                            decay = 2f;
                        else if (gg2 == 2)
                            decay = 0.25f;
                        else if (gg2 == 3)
                            decay = 4f;

                        sparkle.GetComponent<Sparkle>()._minimisingSpeedCoef = decay;
                    }

                    Vector3 p = hit.point;
                    Quaternion r = Quaternion.LookRotation(hit.normal);

                    if (_color == "red")
                        Inst(S.RedHitPoint, S.RedHeavySparkle);
                    else if (_color == "green")
                        Inst(S.GreenHitPoint, S.GreenHeavySparkle);
                    else if (_color == "blue")
                        Inst(S.BlueHitPoint, S.BlueHeavySparkle);
                    else if (_color == "purple")
                        Inst(S.PurpleHitPoint, S.PurpleHeavySparkle);

                    _active = false;
                    _dying = true;
                    DisableFireball();

                    void Inst(GameObject hitPointPrefab, GameObject heavySparklePrefab)
                    {
                        GameObject hitPoint = Instantiate(hitPointPrefab, p, r, _sceneRoot);
                        GameObject heavySparkle = Instantiate(heavySparklePrefab, p, r, _sceneRoot);

                        S.Fog.ApplyToGameObject(hitPoint, _mpb);
                        S.Fog.ApplyToGameObject(heavySparkle, _mpb);

                        Destroy(hitPoint, 5f);
                    }
                }

                foreach (Transform child in _children)
                {
                    child.LookAt(S.PlayerTarget(_sceneName));
                    child.Rotate(0f, 0f, S.OptiRandom.Float);
                }
            }
            else if (_dying)
            {
                UpdateTrailParticles();
            }

            if (_dying && !HasActiveTrailParticles())
            {
                Destroy(gameObject);
            }
        }
    }

    private void SpawnTrailParticles()
    {
        _distance += _deltaDistance;

        while (_distance > _trailStep)
        {
            _distance -= _trailStep;

            int slot = _trailSpawnedCount % _trailParticlesCount;
            _trailSpawnedCount++;

            Transform t = _trailParticles[slot];

            if (t == null)
            {
                GameObject obj = Instantiate(S.TrailParticles[_color], transform.position, Quaternion.identity, _sceneRoot);
                S.Fog.ApplyToGameObject(obj, _mpb);
                t = obj.transform;
                _trailParticles[slot] = t;
                _trailInitialScale = t.localScale;
                _trailCreatedCount++;
            }
            else
            {
                t.gameObject.SetActive(true);
                t.position = transform.position;
                t.rotation = Quaternion.identity;
                t.localScale = _trailInitialScale;
            }

            _trailAges[slot] = 0f;
            _trailVelocities[slot] = S.OptiRandom.Vector * S.OptiRandom.Float * _trailSpeedCoef;
            _trailAccelerations[slot] = S.OptiRandom.Vector * S.OptiRandom.Float * _trailAccelerationCoef;
        }
    }

    private void UpdateTrailParticles()
    {
        float dt = _opti.DeltaTime;

        _trailParticleOptimiser++;
        if (_trailParticleOptimiser >= 4)
        {
            _trailParticleOptimiser = 0;
            Slow();
        }
        else
            Fast();

        void Fast()
        {
            for (int i = 0; i < _trailCreatedCount; i++)
            {
                if (_trailDead[i])
                    continue;

                _trailAges[i] += dt;

                float life01 = _trailLifetime > 0f ? _trailAges[i] / _trailLifetime : 1f;

                if (life01 >= 1f)
                {
                    if (_dying)
                    {
                        Destroy(_trailParticles[i].gameObject);
                        _trailParticles[i] = null;
                        _trailDead[i] = true;
                    }
                    continue;
                }

                _trailParticles[i].position += _trailVelocities[i] * dt;
                _trailParticles[i].localScale = _trailInitialScale * Mathf.Pow(1f - life01, 2);
            }
        }

        void Slow()
        {
            for (int i = 0; i < _trailCreatedCount; i++)
            {
                if (_trailDead[i])
                    continue;

                _trailAges[i] += dt;

                float life01 = _trailLifetime > 0f ? _trailAges[i] / _trailLifetime : 1f;

                if (life01 >= 1f)
                {
                    if (_dying)
                    {
                        Destroy(_trailParticles[i].gameObject);
                        _trailParticles[i] = null;
                        _trailDead[i] = true;
                    }
                    continue;
                }

                _trailVelocities[i] += _trailAccelerations[i] * dt * 4; //optimised
                _trailParticles[i].position += _trailVelocities[i] * dt;

                Vector3 toCamera = _trailParticles[i].position - S.Camera.transform.position; //optimised
                _trailParticles[i].rotation = Quaternion.LookRotation(toCamera); //optimised

                _trailParticles[i].localScale = _trailInitialScale * Mathf.Pow(1f - life01, 3);
            }
        }
    }

    private bool HasActiveTrailParticles()
    {
        for (int i = 0; i < _trailParticlesCount; i++)
        {
            if (_trailParticles[i] != null)
                return true;
        }
        return false;
    }

    private void DisableFireball()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = false;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].enabled = false;
    }

    private void PushRigidbody(RaycastHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb == null)
            rb = hit.collider.GetComponentInParent<Rigidbody>();

        if (rb == null || rb.isKinematic)
            return;

        Vector3 pushDirection = -hit.normal;

        rb.AddForceAtPosition(
            pushDirection * 45f,
            hit.point,
            ForceMode.Impulse
        );
    }
}