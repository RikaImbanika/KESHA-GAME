using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EB2 : MonoBehaviour
{
    private string _sceneName;
    private Optimiser _opti;

    void Start()
    {
        _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
        _opti = new Optimiser(_sceneName);
    }
    
    void Update()
    {
        if (_opti.Optimise(transform.position))
        {
            gameObject.transform.LookAt(S.PlayerTarget(_sceneName));
            gameObject.transform.Rotate(0f, 0f, S.RandRot.Get().z);
            _opti.Reset();
        }
    }
}