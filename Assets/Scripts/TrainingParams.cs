// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingParams : MonoBehaviour
{
    private int _mutagen1;
    private float _mutagen2;

    void Start()
    {
        S.TRP = this;
    }

    public int Mutagen1
    {
        get
        {
            return _mutagen1;
        }
        set
        {
            _mutagen1 = value;
        }
    }

    public float Mutagen2
    {
        get
        {
            return _mutagen2;
        }
        set
        {
            _mutagen2 = value;
        }
    }
}
