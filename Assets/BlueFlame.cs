using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueFlame : MonoBehaviour
{
    private float _duration;
    private float _timeLeft;
    private Vector3 _scale;
    private Vector3 _pos;
    private float _dist;
    private float _upLimit;

    void Start()
    {
        _pos = transform.position;
        _timeLeft = 0.5f + S.RND.Next(1000) / 1000f * 3;
        _dist = _timeLeft * 3f * 3f;
        _duration = _timeLeft;
        _scale = transform.localScale;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, 20f))
        {
            _upLimit = hit.point.y - transform.position.y;
        }
    }

    void Update()
    {
        _timeLeft -= Time.deltaTime;

        if (_timeLeft < 0)
            Destroy(gameObject);

        float c0 = (_duration - _timeLeft) / _duration; //0-1
        float c1 = 1 - c0; //1-0
        float c2 = MathF.Pow(c1, 0.5f);
        float c3 = MathF.Pow(c0, 4f) * _dist;
        float c = Math.Min(c3, _upLimit - _scale.y * c2);
        transform.position = _pos + new Vector3(0, c, 0);
        transform.localScale = new Vector3(_scale.x * c2, _scale.y * c2, _scale.z * c2);
    }
}