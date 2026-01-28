using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoint : MonoBehaviour
{
    private float _timeLeft;
    private Vector3 _scale;

    void Start()
    {
        _timeLeft = 0.3f;
        _scale = transform.localScale;
    }

    void Update()
    {
        _timeLeft -= Time.deltaTime;

        if (_timeLeft < 0)
            Destroy(gameObject);

        transform.rotation = S.RandRot.Get();

        float c = MathF.Pow(_timeLeft / 0.3f, 4);
        transform.localScale = new Vector3(_scale.x * c, _scale.y * c, _scale.z * c);
    }
}
