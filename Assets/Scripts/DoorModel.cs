// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorModel : MonoBehaviour
{
    public Vector3 _coordinates;
    public Vector3 _right;
    public Door _door;
    public string _nextSceneName;
    public int _nextDoorId;
    public bool _locked;
    public bool _needArrow;

    public DoorModel(bool locked)
    {
        _coordinates = new Vector3(0, 0, 0);
        _right = new Vector3(0, 5, 0);
        _locked = locked;
    }

    public DoorModel(float x, float y, float z, Vector3 right, bool locked)
    {
        _coordinates = new Vector3(x, y, z);
        _right = right;
        _locked = locked;
    }
}
