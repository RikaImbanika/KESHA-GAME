// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDs : MonoBehaviour
{
    public long _id;


    void Start()
    {
        S.IDs = this;
    }

    public void Load()
    {
        _id = S.SM.LoadLong("maxid") ?? _id;
    }

    public string GetId
    {
        get
        {
            _id++;
            S.SM.Save("maxid", _id);
            return _id.ToString();
        }
    }
}
