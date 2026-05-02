using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRoot : MonoBehaviour
{
    void Start()
    {
        string sceneName = gameObject.scene.name;

        StartCoroutine(LateStart());

        IEnumerator LateStart()
        {
            while (S.Loader == null)
                yield return new WaitForSeconds(0.1f);

            while (S.Loader.SceneRoots == null)
                yield return new WaitForSeconds(0.1f);

            if (!S.Loader.SceneRoots.ContainsKey(sceneName))
                S.Loader.SceneRoots.Add(sceneName, transform);
            else
                S.Loader.SceneRoots[sceneName] = transform;
        }
    }
}
