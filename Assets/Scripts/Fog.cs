// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog : MonoBehaviour
{
    Dictionary<string, MaterialPropertyBlock> _mpts;

    void Start()
    {
        _mpts = new Dictionary<string, MaterialPropertyBlock>();

        StartCoroutine(LateStart());

        IEnumerator LateStart()
        {
            while (S.Loader == null ||
                S.Loader._rooms == null ||
                S.Loader._rooms.Count == 0)
                yield return new WaitForSeconds(0.25f);

            foreach (string sceneName in S.Loader._rooms.Keys)
                _mpts.Add(sceneName, new MaterialPropertyBlock());

            //Set them
            //_mpts["Hall"];
            //Then some randomisation

            S.Fog = this;

            yield return null;
        }
    }

    public void SetFog(string sceneName)
    {
        //TO DO: realise
    }

    public MaterialPropertyBlock GetMPT(string sceneName)
    {
        return _mpts[sceneName];
    }
}