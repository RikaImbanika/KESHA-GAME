// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakePlayer : MonoBehaviour
{
    Vector3 _startPos;

    void Start()
    {
        _startPos = transform.position;
    }

    void Update()
    {
        if (Time.time - S.FakePlayerLastUpdated > 5f)
        {
            S.FakePlayer.position = _startPos;
            S.FakePlayerScene = "Start";
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(transform.position, new Vector3(2, 4, 2));

        Gizmos.DrawLine(transform.position, transform.position + S.FakePlayerCamera.forward * 2f);
    }
#endif
}
