using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Materials : object
{
    public static Material GetInEditor(string path)
    {
        return AssetDatabase.LoadAssetAtPath<Material>($"Assets/Resources/Materials/{path}.mat");
    }

    public static Material Get(string name)
    {
        return Resources.Load<Material>($"Materials/{name}");
    }
}
