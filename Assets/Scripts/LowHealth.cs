// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowHealth : MonoBehaviour
{
    public GameObject _lowHealthObj;
    public Material _mat;

    float _updateTimer;
    float _targetAlpha;
    float _currentAlpha;

    void Start()
    {
        Renderer rend = _lowHealthObj.GetComponent<Renderer>();
        _mat = rend.sharedMaterial;
        _mat.SetFloat("_Alpha", 0);
    }

    void Update()
    {
        _updateTimer += Time.deltaTime;

        if (_updateTimer > 0.5f)
        {
            _updateTimer = 0;
            float health = S.PS._health / 100;
            _targetAlpha = (health - 0.05f) / 0.35f;
            _targetAlpha = 1 - Mathf.Pow(Mathf.SmoothStep(0, 1, _targetAlpha), 0.5f);
        }

        if (Mathf.Abs(_currentAlpha - _targetAlpha) > 0.005f)
        {
            float d = Time.deltaTime;
            float mk = Mathf.Min(2f * Time.deltaTime, 1f);
            float k = 1 - mk;
            _currentAlpha = _currentAlpha * k + _targetAlpha * mk;
            _mat.SetFloat("_Alpha", _currentAlpha);
        }
    }
}
