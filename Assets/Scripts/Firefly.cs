using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Firefly : MonoBehaviour
{
    public GameObject _vis;
    public GameObject _wingLeft;
    public GameObject _wingRight;
    NavMeshAgent _nma;
    public string _sceneName;

    private float _fearCoef = 300f;
    private float _speed = 2f;
    private float _heightChangeSpeed = 3f;
    private float _sizeChangeSpeed = 2f;
    private Vector3 _finalVelocity;
    private Vector3 _startVelocity;
    private float _lastDirectionChange;
    private float _interval;
    private float _slowActualSpeed;
    private float _slowTargetSpeed;
    private Vector3 _prevPos;
    private float _targetSpeed;
    private float _startHeightOffset;
    private Vector3 _startScale;
    private float _seed1;
    private float _seed2;
    Vector3 _newVelocity;
    private float _time1;
    private float _time2;
    private float _speedUp;
    public string _id;
    public string _idPos;
    public float _swayAmplitude;
    public float _swayFrequency; //For sway
    public float _wingAmplitude;
    public float _wingFrequency; //For wings
    public Optimiser _opti;

    void Start()
    {
        _nma = gameObject.GetComponent<NavMeshAgent>();
        _startScale = _vis.transform.localScale;
        _startHeightOffset = _vis.transform.position.y - transform.position.y;
        _seed1 = UnityEngine.Random.Range(-500, 500);
        _seed2 = UnityEngine.Random.Range(-500, 500) - 222.222f;

        _opti = new Optimiser(_sceneName);

        _prevPos = transform.position;

        InvokeRepeating("SavingMethod", 0f, 6.5f);
    }

    void SavingMethod()
    {
        S.SM.Save(_idPos, transform.position);
    }

    void GetId()
    {
        if (string.IsNullOrEmpty(_id))
        {
            _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
            _id = S.ID("FF", gameObject);
            _idPos = S.IDM(_id, "pos");
        }
    }

    void Update()
    {
        if (_opti.Optimise(_vis.transform.position))
        {
            Do();
            _opti.Reset();
        }


        void Do()
        {
            _vis.transform.position = transform.position + new Vector3(0, _startHeightOffset + GetHeight(), 0);
            float scale = GetSize();
            _vis.transform.localScale = _startScale * scale;
            Vector3 direction = S.PlayerTarget(_sceneName) - _vis.transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            if (_swayAmplitude > 0)
            {
                float angle = Mathf.Sin(Time.time * _swayFrequency * Mathf.PI * 2f) * _swayAmplitude;
                _vis.transform.rotation = lookRotation * Quaternion.Euler(0, 0, angle);

                Wings();
            }
            else
                _vis.transform.rotation = lookRotation;

            float k = _opti.DeltaTime * 60f * 0.3f;

            Vector3 actualSpeed = transform.position - _prevPos;
            _prevPos = transform.position;

            _slowActualSpeed = (1 - k) * _slowActualSpeed + k * actualSpeed.magnitude;
            _slowTargetSpeed = (1 - k) * _slowTargetSpeed + k * _targetSpeed;

            float slowDown = _slowActualSpeed / _slowTargetSpeed;

            float factor = (Time.time - _lastDirectionChange) / _interval;

            bool bingo = slowDown < 0.95f;

            if (factor > 1 || bingo)
            {
                Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;
                _startVelocity = _finalVelocity;

                if (bingo)
                {
                    _startVelocity = Vector3.zero; //Okay?
                    _slowActualSpeed = _slowTargetSpeed;
                }

                _finalVelocity = new Vector3(randomDir.x, 0, randomDir.y) * _speed;
                _lastDirectionChange = Time.time;
                _interval = UnityEngine.Random.Range(1.8f, 11f);
                factor = 0;
            }

            _newVelocity = Vector3.Lerp(
                _startVelocity,
                _finalVelocity,
                factor
            );
            string gg = "";

            if (_newVelocity.magnitude <= 0)
                gg = $"(_sv = {_startVelocity}; _fv = {_finalVelocity}; factor = {factor})";

            Vector3 _fromPlayer = _vis.transform.position - S.PlayerTarget(_sceneName);
            float fear = _fearCoef / MathF.Pow(_fromPlayer.magnitude, 2); //

            float farSpeedUp = Math.Clamp(_fromPlayer.magnitude / 350f, 0f, 1f) * 3f;

            _speedUp = 1 + fear + farSpeedUp;
            //Debug.Log($"fear {_fear} speedUp {_speedUp} nv {_newVelocity.magnitude} sd {slowDown} scale {scale} {gg}");

            _newVelocity *= _opti.DeltaTime * (1 + fear * 3f + farSpeedUp); ///

            _targetSpeed = _newVelocity.magnitude;

            transform.position += _newVelocity;
        }
    }

    void Wings()
    {
        if (_wingLeft != null)
        {
            float angle = Mathf.Sin(Time.time * _wingFrequency * Mathf.PI * 2f) * _wingAmplitude;
            _wingLeft.transform.localRotation = Quaternion.Euler(angle, 90, 0);
            _wingRight.transform.localRotation = Quaternion.Euler(-angle, 90, 0);
        }
    }

    float GetHeight()
    {
        float sp = Math.Min((1 + (_speedUp - 1)) * 0.3f, 1.3f);
        _time1 += _opti.DeltaTime * sp * _heightChangeSpeed;
        float x = _time1 + _seed1;

        return 1 + 2f * MathF.Cos(
            2.2f * MathF.Sin(
                MathF.Sin(x / 1.5f)
                + 1.49f
                + MathF.Sin(0.18f * x - 2.63f)
            )
            - MathF.Sin(0.33f * x)
        );
    }

    float GetSize()
    {
        _time2 += _opti.DeltaTime * (1 + (1 - _speedUp)) * 0.75f * _sizeChangeSpeed;
        float x = _time2 + _seed2;

        return 1f + 0.6f * MathF.Cos(
            2.2f * MathF.Sin(
                MathF.Sin(x / 1.5f)
                + 1.49f
                + MathF.Sin(0.18f * x - 2.63f)
            )
            - MathF.Sin(0.33f * x)
        );
    }
}