// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentSceneFixer : MonoBehaviour
{
    float _time;
    int _layerMask;

    void Start()
    {
        _layerMask = 1 << LayerMask.NameToLayer("Static") |
                     1 << LayerMask.NameToLayer("Default");
    }

    void Update()
    {
        _time += Time.deltaTime;

        if (_time > 0.5f)
        {
            _time = 0;

            if (S.Loader._teleporting)
                return;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 20f, _layerMask))
            {
                GameObject obj = hit.collider.gameObject;
                string sceneName = obj.scene.name;
                string savedSceneName = S.PS._currentSceneName;

                if (savedSceneName != sceneName)
                {
                    S.Loader.GoTo(sceneName, -1, Vector3.zero);
                }
            }
        }
    }
}
