// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBoostZone : MonoBehaviour
{
    string _sceneName;

    float _timer;

    public float _boost;
    public float _bottom;
    public float _top;
    public float _xMin;
    public float _xMax;
    public float _zMin;
    public float _zMax;

    float _newBottom;
    float _newTop;
    float _newXMin;
    float _newXMax;
    float _newZMin;
    float _newZMax;

    void Start()
    {
        _sceneName = gameObject.scene.name;
        Vector3 p = transform.position;
        _newBottom = p.y + _bottom;
        _newTop = p.y + _top;
        _newXMin = p.x + _xMin;
        _newXMax = p.x + _xMax;
        _newZMin = p.z + _zMin;
        _newZMax = p.z + _zMax;
    }

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer > 0.25f)
        {
            _timer = 0;

            if (S.PS._currentSceneName != _sceneName)
                Deactivate();
            else
            {
                Vector3 p = S.Camera.transform.position;

                if (p.x < _newXMin || p.x > _newXMax ||
                    p.y < _newBottom || p.y > _newTop ||
                    p.z < _newZMin || p.z > _newZMax)
                    Deactivate();
                else
                    Activate();
            }
        }
    }

    void Activate()
    {
        S.PM.SetJumpBoost(_boost);
    }

    void Deactivate()
    {
        S.PM.SetJumpBoost(1f);
    }

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 p2 = transform.position * 2;
        Vector3 center = new Vector3(
            (p2.x + _xMin + _xMax) * 0.5f,
            (p2.y + _bottom + _top) * 0.5f,
            (p2.z + _zMin + _zMax) * 0.5f
        );
        Vector3 size = new Vector3(
            _xMax - _xMin,
            _top - _bottom,
            _zMax - _zMin
        );
        Gizmos.DrawWireCube(center, size);
    }
#endif
}