// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDuplicateCleaner : MonoBehaviour
{
    private int _cleanupCounter = 0;
    private bool _isCleanupRoutineRunning = false;
    private Coroutine _cleanupCoroutine;
    public float _delay = 7f;
    public int _retryCount = 5;

    public SceneDuplicateCleaner()
    {
        S.SDC = this;
    }

    public void RequestCleanup()
    {
        if (_isCleanupRoutineRunning)
            _cleanupCounter = _retryCount;
        else
            _cleanupCoroutine = StartCoroutine(CheckForDuplicatesRoutine());
    }

    private IEnumerator CheckForDuplicatesRoutine()
    {
        _isCleanupRoutineRunning = true;
        _cleanupCounter = _retryCount;

        while (_cleanupCounter > 0)
        {
            yield return new WaitForSeconds(_delay);
            CheckAndRemoveDuplicateScenes();
            _cleanupCounter--;
        }

        _isCleanupRoutineRunning = false;
    }

    private void CheckAndRemoveDuplicateScenes()
    {
        List<Scene> loadedScenes = new List<Scene>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            loadedScenes.Add(SceneManager.GetSceneAt(i));
        }

        var sceneGroups = loadedScenes
            .GroupBy(scene => scene.name)
            .Where(group => group.Count() > 1);

        foreach (var group in sceneGroups)
        {
            bool isFirst = true;
            foreach (Scene scene in group)
            {
                if (!isFirst && scene.isLoaded)
                {
                    StartCoroutine(UnloadSceneAsync(scene));
                }
                else isFirst = false;
            }
        }
    }

    private IEnumerator UnloadSceneAsync(Scene scene)
    {
        string name = scene.name;

        if (scene.isLoaded)
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);
            while (!asyncUnload.isDone) yield return null;
            S.Console.AddMessage($"Unloaded scene duplicate: {name}", Color.red);
        }
    }

    private bool IsSceneAlreadyLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
            if (SceneManager.GetSceneAt(i).name == sceneName)
                return true;
        return false;
    }
}