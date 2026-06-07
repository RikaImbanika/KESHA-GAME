// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleLayers : MonoBehaviour
{
    public Dictionary<string, int> _numbers;

    void Start()
    {
        _numbers = new Dictionary<string, int>();
        _numbers.Add("Default", 0);

        S.VisibleLayers = this;
    }
}
