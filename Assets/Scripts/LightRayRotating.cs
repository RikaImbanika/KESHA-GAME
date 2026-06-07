// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRayRotating : MonoBehaviour
{
    public float _rotationSpeed;
    string _sceneName;
    Optimiser _opti;

    void Start()
    {
        _sceneName = gameObject.scene.name;
        _opti = new Optimiser(_sceneName);
        _opti.MaxPeriodForDistance = 1f / 24f;
        _opti.MaxPeriodForRotation = 1f / 24f;

        transform.Rotate(Random.Range(0, 360), 0f, 0f, Space.Self);
    }

    void Update()
    {
        if (_opti.Optimise(transform.position))
        {
            Do();
            _opti.Reset();
        }

        void Do()
        {
            transform.Rotate(_rotationSpeed * _opti.DeltaTime, 0f, 0f, Space.Self);
        }
    }
}
