// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System;

public class LocalLaser : MonoBehaviour
{
    [Header("Timings")]
    public float _forwardTime = 2f;
    public float _forwardPause = 0f;
    public float _backwardTime = 2f;
    public float _backwardPause = 0f;
    public byte _currentState = 0;
    public float _currentTime;

    [Header("Distance Settings")]
    public Vector3 _movementDirection;
    public float _forwardDistance = 5f;
    public float _backwardDistance = 5f;

    [Header("Laser Settings")]
    public Vector3 _laserDirection;
    public float _laserLimit = 50f;

    [Header("Privates")]
    private Vector3 _forwardPosition;
    private Vector3 _backwardPosition;
    private GameObject _leftLaserObj;
    private GameObject _rightLaserObj;
    private GameObject _leftPointObj;
    private GameObject _rightPointObj;
    private GameObject _leftBotObj;
    private GameObject _rightBotObj;
    private Vector3 _leftDelta;
    private Vector3 _rightDelta;
    private float _period;
    private float _frequency;
    private int _layerMask;
    private float _normalLength;
    private bool _hitSomething;
    private bool _hitSomethingBeforeRight;
    private bool _hitSomethingBeforeLeft;
    private string _sceneName;

    private Optimiser _opti;

    void Start()
    {
        StartCoroutine(Start0());

        IEnumerator Start0()
        {
            _sceneName = gameObject.scene.name;
                        
            while (S.Loader.Roots == null ||
                !S.Loader.Roots.ContainsKey(_sceneName) ||
                S.Loader.Roots[_sceneName] == null)
                yield return new WaitForSeconds(0.25f);

            _opti = new Optimiser(_sceneName);
            _opti.MaxPeriodForDistance = 1 / 24f;

            _period = (_forwardTime + _backwardTime + _forwardPause + _backwardPause);
            _frequency = 2 * MathF.PI / _period;
            _forwardPosition = transform.position + _movementDirection * _forwardDistance;
            _backwardPosition = transform.position - _movementDirection * _backwardDistance;

            _leftLaserObj = Instantiate(S.RedLaser, transform);
            _rightLaserObj = Instantiate(S.RedLaser, transform);
            _leftPointObj = Instantiate(S.RedPoint, transform);
            _rightPointObj = Instantiate(S.RedPoint, transform);
            _leftPointObj.SetActive(false);
            _rightPointObj.SetActive(false);

            _layerMask = 1 << LayerMask.NameToLayer("Player") |
                             1 << LayerMask.NameToLayer("Static") |
                             1 << LayerMask.NameToLayer("Enemies") |
                             1 << LayerMask.NameToLayer("Items") |
                             1 << LayerMask.NameToLayer("Default");

            InitLaserBots();
            UpdateLaserVisuals();
            SetFog();

            yield return null;
        }
    }

    void SetFog()
    {
        MaterialPropertyBlock mpb = S.Fog.GetMPB(_sceneName);
        S.Fog.ApplyToGameObject(gameObject, mpb);
        //Should automaticly include points, lasers, bots
    }

    void Update()
    {
        if (_opti.Optimise(transform.position))
        {
            UpdateMovement();
            UpdateLaserVisuals();

            _opti.Reset();
        }
    }

    void UpdateMovement()
    {
        _currentTime += _opti.DeltaTime;
        float smoothed = (MathF.Sin(_currentTime * _frequency) + 1) / 2f;
        transform.position = Vector3.Lerp(_forwardPosition, _backwardPosition, smoothed);
    }

    void InitLaserBots()
    {
        GameObject botPrefab = Prefabs.Get("LaserBot");

        float sc = 0.35f;

        _leftBotObj = Instantiate(botPrefab, transform.position, Quaternion.LookRotation(_laserDirection), transform);
        _leftBotObj.transform.localScale = new Vector3(sc, sc, sc);
        _rightBotObj = Instantiate(botPrefab, transform.position, Quaternion.LookRotation(-_laserDirection), transform);
        _rightBotObj.transform.localScale = new Vector3(sc, sc, sc);

        Vector3 leftHitPoint = GetLaserHitPoint(transform.position, -_laserDirection, _leftPointObj);
        Vector3 rightHitPoint = GetLaserHitPoint(transform.position, _laserDirection, _rightPointObj);
        _normalLength = (leftHitPoint - rightHitPoint).magnitude;

        _leftDelta = leftHitPoint - transform.position;
        _rightDelta = rightHitPoint - transform.position;
    }

    void UpdateLaserVisuals()
    {
        Vector3 leftHitPoint = transform.position + _leftDelta;
        Vector3 rightHitPoint = transform.position + _rightDelta;

        Vector3 leftHitPoint2 = GetLaserHitPoint(leftHitPoint, _laserDirection, false);
        Vector3 rightHitPoint2 = GetLaserHitPoint(rightHitPoint, -_laserDirection, true);

        UpdateSingleLaser(_leftLaserObj, _leftBotObj, leftHitPoint, leftHitPoint2, _leftPointObj);
        UpdateSingleLaser(_rightLaserObj, _rightBotObj, rightHitPoint, rightHitPoint2, _rightPointObj);
    }

    Vector3 GetLaserHitPoint(Vector3 from, Vector3 direction, bool right)
    {
        RaycastHit hit;
        if (Physics.Raycast(from, direction, out hit, _laserLimit, _layerMask))
        {
            float len = (hit.point - from).magnitude;

            if (len < _normalLength * 0.99f)
            {
                if (!_hitSomething)
                {
                    _hitSomething = true;
                    _leftPointObj.SetActive(true);
                    _rightPointObj.SetActive(true);
                }

                //Stable damage
                if (hit.collider.gameObject.CompareTag("Player"))
                    S.PS.Damage(14f * _opti.DeltaTime);
            }
            else if (_hitSomething)
            {
                _hitSomething = false;
                _leftPointObj.SetActive(false);
                _rightPointObj.SetActive(false);
            }

            if (_hitSomething)
            {
                if (right && _hitSomethingBeforeRight || !right && _hitSomethingBeforeLeft)
                {
                    int speed = (int)(15 * 60f * _opti.DeltaTime);
                    if (S.RND.Next(0, speed) == 0)
                        ThrowSparkle();
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                        ThrowSparkle();

                    //First touch damage
                    if (hit.collider.gameObject.CompareTag("Player"))
                        S.PS.Damage(2f); //x2 for right & left

                    if (right)
                        _hitSomethingBeforeRight = true;
                    else
                        _hitSomethingBeforeLeft = true;
                }
            }
            else
            {
                _hitSomethingBeforeRight = false;
                _hitSomethingBeforeLeft = false;
            }

            void ThrowSparkle()
            {
                GameObject sparkle = Instantiate(S.RedSparkle);
                sparkle.transform.position = hit.point;
                sparkle.transform.rotation = Quaternion.LookRotation(hit.normal);
            }

            return hit.point;
        }

        return transform.position + direction * _laserLimit;
    }

    void UpdateSingleLaser(GameObject laserObj, GameObject botObj, Vector3 startPoint, Vector3 endPoint, GameObject point)
    {
        if (laserObj == null) return;

        laserObj.transform.position = startPoint;

        // Rotate laser towards points
        Vector3 direction = endPoint - startPoint;
        if (direction == Vector3.zero)
            return;

        laserObj.transform.rotation = Quaternion.LookRotation(direction);

        // Scale laser by length
        float distance = Vector3.Distance(startPoint, endPoint);
        laserObj.transform.localScale = new Vector3(
            laserObj.transform.localScale.x,
            laserObj.transform.localScale.y,
            distance
        );

        if (_hitSomething)
        {
            point.transform.position = endPoint;
            point.transform.rotation = S.RandRot.Get();
        }

        botObj.transform.position = startPoint;
    }
}