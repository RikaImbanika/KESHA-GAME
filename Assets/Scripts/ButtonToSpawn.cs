// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonToSpawn : MonoBehaviour
{
    string _sceneName;

    void Start()
    {
        _sceneName = gameObject.scene.name;    
    }

    public void Activate()
    {
        StartCoroutine(ActivateAsync());
        
        IEnumerator ActivateAsync()
        {
            string sceneName = "Income";

            Vector3 forward = -S.PM.orientation.forward;
            S.Loader.GoTo(sceneName, -1, forward);

            while (!S.AllFather.SceneCurrentlyLoaded("Income"))
                yield return new WaitForSeconds(0.2f);

            Vector3 v = new Vector3(0, 0, 0);
            if (S.Pm.isCrouching)
                v = new Vector3(0, -2.2f, 0);

            S.Ph.transform.position = new Vector3(6.08f, -13.18f, -852.67f) + v;
        }
    }
}
