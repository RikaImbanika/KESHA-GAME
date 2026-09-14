// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GongParticle : MonoBehaviour
{
    public float _minSpeed;
    public float _maxSpeed;
    public float _minScale;
    public float _maxDistance;
    public float _maxLifeTime;
    public float _velocityDecreaser;
    public float _scaleDecreaser;
    Vector3 _velocity;
    Vector3 _startPoint;
    Transform _child;
    float _lifeTime;

    void Start()
    {
        _startPoint = transform.position;
        _velocity = transform.right * Random.Range(_minSpeed, _maxSpeed);
        float randomAngle = Random.Range(0f, 360f);
        _child = transform.GetChild(0);
        Vector3 r = _child.localRotation.eulerAngles;
        _child.localRotation = Quaternion.Euler(r.x, r.y, randomAngle);
    }

    void Update()
    {
        float d = Time.deltaTime;
        _lifeTime += d;
        transform.position += _velocity * d;

        _velocity *= (1 - (_velocityDecreaser * d));
        transform.localScale *= (1 - (_scaleDecreaser * d));

        if ((transform.position - _startPoint).magnitude > _maxDistance)
            Destroy(gameObject);
        else if (transform.localScale.x < _minScale)
            Destroy(gameObject);
        else if (_lifeTime > _maxLifeTime)
            Destroy(gameObject);
    }
}