using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoint2 : MonoBehaviour
{
    private float _timeLeft;
    private Vector3 _scale;

    void Start()
    {
        _timeLeft = 0.1f;
        _scale = transform.localScale;
        transform.rotation = S.RandRot.Get();
    }

    void Update()
    {
        _timeLeft -= Time.deltaTime;

        if (_timeLeft < 0)
            Destroy(gameObject);

        //transform.rotation = S.RandRot.Get();

        float c = MathF.Pow(1f + (0.1f - _timeLeft) * 10f * 4f, 6f) * 0.01f;
        transform.localScale = new Vector3(_scale.x * c, _scale.y * c, _scale.z * c);
    }
}