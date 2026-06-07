// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paintings : MonoBehaviour
{
    public string[] _names;

    void Start()
    {
        _names = new string[]
        {
            "YouAreVase",
            "Remember",
            "Tokyo",
            "Paris",
            "SoManyVases",
            "Palms",
            "BeDifferent"
        };

        S.Paintings = this;
    }
}
