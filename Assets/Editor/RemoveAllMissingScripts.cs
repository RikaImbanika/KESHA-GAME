// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class RemoveAllMissingScripts
{
    [MenuItem("Tools/Remove Missing Scripts (Safe)/From ALL Scenes and Prefabs")]
    public static void RemoveMissingScriptsFromAll()
    {
        if (!EditorUtility.DisplayDialog("Safe Removal of Missing Scripts",
                "All scenes and prefabs in the project will be processed.\n" +
                "It is recommended to open an empty scene beforehand to avoid background scripts.\n\n" +
                "Continue?",
                "Yes, remove all", "Cancel"))
            return;

        RemoveMissingFromAllScenes();
        RemoveMissingFromAllPrefabs();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Done", "All Missing Scripts have been safely removed.", "OK");
    }

    [MenuItem("Tools/Remove Missing Scripts (Safe)/From All Scenes Only")]
    public static void RemoveMissingFromAllScenes()
    {
        if (!EditorUtility.DisplayDialog("Safe Removal of Missing Scripts",
                "All scenes in the project will be processed.\n" +
                "It is recommended to open an empty scene beforehand to avoid background scripts.\n\n" +
                "Continue?",
                "Yes, remove all", "Cancel"))
            return;

        // Remember the active scene before starting
        Scene originalScene = EditorSceneManager.GetActiveScene();

        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        int total = sceneGuids.Length;

        for (int i = 0; i < total; i++)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);

            if (EditorUtility.DisplayCancelableProgressBar(
                    $"Processing Scenes ({i + 1}/{total})",
                    scenePath, (float)i / total))
            {
                EditorUtility.ClearProgressBar();
                // Reopen the original scene if interrupted
                EditorSceneManager.OpenScene(originalScene.path);
                return;
            }

            bool modified = false;
            Scene processedScene = default;

            try
            {
                processedScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                GameObject[] roots = processedScene.GetRootGameObjects();
                foreach (GameObject root in roots)
                {
                    modified |= SafeRemoveMissingRecursive(root);
                }

                if (modified)
                {
                    EditorSceneManager.MarkSceneDirty(processedScene);
                    EditorSceneManager.SaveScene(processedScene);
                    Debug.Log($"Scene cleaned: {scenePath}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error processing scene {scenePath}: {e.Message}");
            }
            // Do not close the scene here; it will be replaced by the next OpenScene or final restoration
        }

        EditorUtility.ClearProgressBar();

        // Restore the scene that was originally open
        if (originalScene.IsValid())
        {
            EditorSceneManager.OpenScene(originalScene.path);
        }
        else
        {
            // If the original scene is invalid for some reason, open the first one from the list
            if (sceneGuids.Length > 0)
            {
                string firstScenePath = AssetDatabase.GUIDToAssetPath(sceneGuids[0]);
                EditorSceneManager.OpenScene(firstScenePath);
            }
        }

        Debug.Log("Scene cleanup completed.");
    }

    [MenuItem("Tools/Remove Missing Scripts (Safe)/From All Prefabs Only")]
    public static void RemoveMissingFromAllPrefabs()
    {
        if (!EditorUtility.DisplayDialog("Safe Removal of Missing Scripts",
                "All prefabs in the project will be processed.\n\n" +
                "Continue?",
                "Yes, remove all", "Cancel"))
            return;

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        int total = prefabGuids.Length;

        for (int i = 0; i < total; i++)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);

            if (EditorUtility.DisplayCancelableProgressBar(
                    $"Processing Prefabs ({i + 1}/{total})",
                    prefabPath, (float)i / total))
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            try
            {
                GameObject prefabContents = PrefabUtility.LoadPrefabContents(prefabPath);
                if (prefabContents == null) continue;

                bool modified = SafeRemoveMissingRecursive(prefabContents);

                if (modified)
                {
                    PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabPath);
                    Debug.Log($"Prefab cleaned: {prefabPath}");
                }

                PrefabUtility.UnloadPrefabContents(prefabContents);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error processing prefab {prefabPath}: {e.Message}");
            }
        }

        EditorUtility.ClearProgressBar();
        Debug.Log("Prefab cleanup completed.");
    }

    /// <summary>
    /// Recursively removes Missing Scripts from the object and all its children.
    /// Uses the safe method GameObjectUtility.RemoveMonoBehavioursWithMissingScript.
    /// </summary>
    private static bool SafeRemoveMissingRecursive(GameObject go)
    {
        bool removed = false;

        // Remove all missing scripts on this specific object
        int before = go.GetComponents<MonoBehaviour>().Length; // before removal
        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
        int after = go.GetComponents<MonoBehaviour>().Length;  // after

        if (after < before)
            removed = true;

        // Recursively for children
        foreach (Transform child in go.transform)
        {
            removed |= SafeRemoveMissingRecursive(child.gameObject);
        }

        return removed;
    }
}