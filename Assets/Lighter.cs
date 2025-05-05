using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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

    void Start()
    {
        _nma = gameObject.GetComponent<NavMeshAgent>();
        _startScale = _vis.transform.localScale;
        _startHeightOffset = _vis.transform.position.y - transform.position.y;
        _seed1 = UnityEngine.Random.Range(-500, 500);
        _seed2 = UnityEngine.Random.Range(-500, 500) - 222.222f;

        _prevPos = transform.position;
    }

    void Update()
    {
        _vis.transform.position = transform.position + new Vector3(0, _startHeightOffset + GetHeight(), 0);
        float scale = GetSize();
        _vis.transform.localScale = _startScale * scale;
        Vector3 direction = S.Camera.transform.position - _vis.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        _vis.transform.rotation = lookRotation;

        float k = Time.deltaTime * 60f * 0.3f;

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

            /*if (bingo)
                Debug.Log($"BINGO! slowDown = {slowDown}");
            else
                Debug.Log("Not bingo!");*/
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

        _newVelocity *= Time.deltaTime * (1 + _fear * 3f); ///

        _targetSpeed = _newVelocity.magnitude;

        transform.position += _newVelocity;
    }

    float GetHeight()
    {
        float sp = Math.Min((1 + (_speedUp - 1)) * 0.3f, 1.3f);
        _time1 += Time.deltaTime * sp * _heightChangeSpeed;
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
        _time2 += Time.deltaTime * (1 + (1 - _speedUp)) * 0.75f * _sizeChangeSpeed;
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