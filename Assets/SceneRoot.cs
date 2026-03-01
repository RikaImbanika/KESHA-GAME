using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRoot : MonoBehaviour
{
    void Start()
    {
        string sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
        if (!S.Loader.SceneRoots.ContainsKey(sceneName))
            S.Loader.SceneRoots.Add(sceneName, transform);
        else
            S.Loader.SceneRoots[sceneName] = transform;
    }
}
