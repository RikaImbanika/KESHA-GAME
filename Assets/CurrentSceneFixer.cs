using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentSceneFixer : MonoBehaviour
{
    float _time;
    int _layerMask;

    void Start()
    {
        _layerMask = 1 << LayerMask.NameToLayer("Static") |
                     1 << LayerMask.NameToLayer("Default");
    }

    void Update()
    {
        _time += Time.deltaTime;

        if (_time > 1f) //I will change to 0.5f later
        {
            _time = 0;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 20f, _layerMask))
            {
                GameObject obj = hit.collider.gameObject;
                string sceneName = obj.scene.name;
                string savedSceneName = S.PS._currentSceneName;

                if (savedSceneName != sceneName)
                {
                    S.Loader.GoTo(sceneName, -1, Vector3.zero);
                }
            }
        }
    }
}
