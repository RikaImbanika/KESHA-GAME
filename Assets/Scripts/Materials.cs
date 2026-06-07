// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Materials : object
{
    public static Material Get(string name)
    {
        return Resources.Load<Material>($"Materials/{name}"); //No .mat
    }

#if UNITY_EDITOR
    public static Material GetInEditor(string path)
    {
        return AssetDatabase.LoadAssetAtPath<Material>($"Assets/Resources/Materials/{path}.mat");
    }
#endif
}
