using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Lighter : MonoBehaviour
{
    public GameObject _vis;
    NavMeshAgent _nma;

    private float _fearCoef = 300f;
    private float _speed = 2f;
    private float _heightChangeSpeed = 3f;
    private float _sizeChangeSpeed = 1.8f;
    private Vector3 _finalVelocity;
    private Vector3 _startVelocity;
    private float _lastDirectionChange;
    private float _interval = 1f;
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
    private float _deltaTime;
    private float _updateInterval;
    private float _fpsDeltaTime;
    private bool _playerInScene;
    private bool _needUpdate;
    private int _fpsD;
    private float _minFps = 1 / 12f;
    private float _maxFps = 1 / 120f;
    public string _id;
    public string _idPos;

    void Start()
    {
        _nma = gameObject.GetComponent<NavMeshAgent>();
        _startScale = _vis.transform.localScale;
        _startHeightOffset = _vis.transform.position.y - transform.position.y;
        _seed1 = UnityEngine.Random.Range(-500, 500);
        _seed2 = UnityEngine.Random.Range(-500, 500) - 222.222f;

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
            string _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
            _id = _sceneName + transform.position.x + transform.position.y + transform.position.z;
            _idPos = S.ID(_id, "pos");
        }
    }

    void Update()
    {
        if (FPS())
        {
            Do();
            _deltaTime = 0;
        }


        void Do()
        {
            _vis.transform.position = transform.position + new Vector3(0, _startHeightOffset + GetHeight(), 0);
            float scale = GetSize();
            _vis.transform.localScale = _startScale * scale;
            Vector3 direction = S.Camera.transform.position - _vis.transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            _vis.transform.rotation = lookRotation;

            float k = _deltaTime * 60f * 0.3f;

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

            Vector3 _fromPlayer = _vis.transform.position - S.PObj.transform.position;
            float _fear = _fearCoef / MathF.Pow(_fromPlayer.magnitude, 2); //

            _speedUp = 1 + _fear;
            //Debug.Log($"fear {_fear} speedUp {_speedUp} nv {_newVelocity.magnitude} sd {slowDown} scale {scale} {gg}");

            _newVelocity *= _deltaTime * (1 + _fear * 3f); ///

            _targetSpeed = _newVelocity.magnitude;

            transform.position += _newVelocity;
        }
    }

    float GetHeight()
    {
        float sp = Math.Min((1 + (_speedUp - 1)) * 0.3f, 1.3f);
        _time1 += _deltaTime * sp * _heightChangeSpeed;
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
        _time2 += _deltaTime * (1 + (1 - _speedUp)) * 0.75f * _sizeChangeSpeed;
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

    bool FPS()
    {
        _deltaTime += Time.deltaTime;
        _fpsDeltaTime += Time.deltaTime;

        bool check = Check();

        if (_fpsDeltaTime > 0.2f || _needUpdate)
            return RecalcFPS();
        else
            return check;

        bool RecalcFPS()
        {
            _fpsDeltaTime = 0;
            _fpsD++;

            if (check || _fpsD >= 3)
            {
                _fpsD = 0;

                float lim = 20f;
                float step = 40f;
                float dist = Vector3.Distance(transform.position, S.Ph.transform.position);
                float t = (dist - lim) / step;

                float smoothed = 1;
                if (t > 0 && S.Camera.fieldOfView > 35f)
                    smoothed = Mathf.SmoothStep(0f, 1f, 1f / (t * t));

                float coef1 = Mathf.Lerp(_minFps, _maxFps, smoothed);

                float angle = Vector3.Angle(S.Camera.transform.forward, (transform.position - S.Camera.transform.position).normalized);
                //+ visible - invisible
                float k = 0.5f - (S.Camera.fieldOfView - angle) / 30f; //degrees
                float coef2 = 0.05f;
                if (dist > 30f)
                    coef2 = Mathf.Clamp(k, 0.05f, 1) * 20f;

                _updateInterval = coef1 * coef2;

                _needUpdate = false;

                return true;
            }
            else
                return false;
        }

        bool Check()
        {
            bool buf = S.PS._currentSceneName == gameObject.scene.name;

            if (!_playerInScene && buf)
                _needUpdate = true;

            _playerInScene = buf;

            float coef3 = 20f;

            if (_playerInScene)
                coef3 = 1f;

            return _deltaTime > _updateInterval * coef3;
        }
    }
}