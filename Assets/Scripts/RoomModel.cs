// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomModel : MonoBehaviour
{
    public string _sceneName;

    public Dictionary<int, DoorModel> _doors;

    public RoomModel(string sceneName)
    {
        _doors = new Dictionary<int, DoorModel>();
        _sceneName = sceneName;
    }
}
