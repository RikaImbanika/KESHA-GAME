// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    private Quaternion[] _rotations;
    private Vector3[] _rotationsEuler;
    private float[] _rotationsFloat;
    private int _index1;
    private int _index2;
    private int _index3;

    void Start()
    {
        _rotations = new Quaternion[150];
        _rotationsEuler = new Vector3[150];
        _rotationsFloat = new float[150];

        StartCoroutine(Yep());

        IEnumerator Yep()
        {
            while (S.RND == null)
                yield return new WaitForSeconds(0.15f);

            for (int i = 0; i < 150; i++)
            {
                _rotations[i] = Quaternion.Euler(S.RND.Next(-180, 180), S.RND.Next(-180, 180), S.RND.Next(-180, 180));
                _rotationsEuler[i] = new Vector3(S.RND.Next(-180, 180), S.RND.Next(-180, 180), S.RND.Next(-180, 180));
                _rotationsFloat[i] = S.RND.Next(-180, 180);
            }

            S.RandRot = this;
        }
    }

    public Quaternion Get()
    {
        _index1++;
        if (_index1 >= 150)
            _index1 = 0;

        return _rotations[_index1];
    }

    public Vector3 GetEuler()
    {
        _index2++;
        if (_index2 >= 150)
            _index2 = 0;

        return _rotationsEuler[_index2];
    }

    public float GetFloat()
    {
        _index3++;
        if (_index3 >= 150)
            _index3 = 0;

        return _rotationsFloat[_index3];
    }
}
