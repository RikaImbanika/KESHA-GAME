// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fog : MonoBehaviour
{
    MaterialPropertyBlock _standardMPB;
    Dictionary<string, MaterialPropertyBlock> _mpbs;

    void Start()
    {
        _mpbs = new Dictionary<string, MaterialPropertyBlock>();

        StartCoroutine(LateStart());

        IEnumerator LateStart()
        {
            while (S.Loader == null ||
                S.Loader._rooms == null ||
                S.Loader._rooms.Count == 0)
                yield return new WaitForSeconds(0.25f);

            _standardMPB = new MaterialPropertyBlock();
            _standardMPB.SetColor("_FogColor", new Color(0.5f, 0.6f, 0.7f));
            _standardMPB.SetFloat("_FogDensity", 0.02f);

            foreach (string sceneName in S.Loader._rooms.Keys)
                _mpbs.Add(sceneName, new MaterialPropertyBlock());

            _mpbs["Hall"].SetColor("_FogColor", new Color(0.5f, 0.6f, 0.7f));
            _mpbs["Hall"].SetFloat("_FogDensity", 0.02f);

            _mpbs["Corridor"].SetColor("_FogColor", new Color(0.5f, 0.6f, 0.7f));
            _mpbs["Corridor"].SetFloat("_FogDensity", 0.02f);

            Color[] fogPalette = {
                new Color(0f, 0f, 0f),
                new Color(1f, 1f, 1f),
                new Color(1f, 0f, 0f),
                new Color(0f, 0f, 1f),
                new Color(0.5f, 0.5f, 0.5f),
                new Color(0.5f, 0.6f, 0.7f),
                new Color(0.8f, 0.7f, 0.5f),
                new Color(0.4f, 0.3f, 0.2f),
                new Color(0.3f, 0.2f, 0.5f),
                new Color(0.4f, 0.5f, 0.4f),
                new Color(0.2f, 0.2f, 0.3f),
                new Color(0.7f, 0.6f, 0.6f),
                new Color(0.2f, 0.3f, 0.4f),
                new Color(0.6f, 0.6f, 0.6f),
                new Color(0.9f, 0.9f, 0.85f),
                new Color(0.3f, 0.25f, 0.2f),
                new Color(0.1f, 0.15f, 0.2f),
                new Color(0.8f, 0.85f, 0.9f),
                new Color(0.55f, 0.45f, 0.35f),
                new Color(0.35f, 0.4f, 0.45f),
                new Color(0.9f, 0.5f, 0.2f),
                new Color(0f, 0.5f, 0.5f),
                new Color(0.7f, 0.7f, 0.5f),
                new Color(0.45f, 0.5f, 0.6f),
                new Color(0.25f, 0.3f, 0.35f),
                new Color(0.75f, 0.8f, 0.85f),
                new Color(0.6f, 0.5f, 0.6f),
                new Color(0.15f, 0.1f, 0.15f),
                new Color(0.85f, 0.75f, 0.65f),
                new Color(0.5f, 0.55f, 0.5f)
            };

            string[] scenes = { "BR 1", "BR 1R", "BR 2", "BR 2R", "BR 3", "BR 3R", "BR 4", "BR 4R", "BR 5", "BR 6", "BR 6R", "BR 7", "BR 7R", "BR 8", "TL 1", "TL 2", "TL 0", "MR 1", "MR 2", "MR 3", "MR 4" };

            foreach (string scene in scenes)
            {
                _mpbs[scene].SetColor("_FogColor", fogPalette[Random.Range(0, fogPalette.Length)]);
                _mpbs[scene].SetFloat("_FogDensity", Random.Range(0.005f, 0.04f));
            }
            
            S.Fog = this;

            yield return null;
        }
    }

    public void SetFog(string sceneName, GameObject root)
    {
        MaterialPropertyBlock mpt = _standardMPB;
        if (_mpbs.ContainsKey(sceneName))
            mpt = _mpbs[sceneName];

        Scene scene = root.scene;
        foreach (GameObject rootObj in scene.GetRootGameObjects())
            foreach (Renderer r in rootObj.GetComponentsInChildren<Renderer>(true))
                r.SetPropertyBlock(mpt);
    }

    public void ApplyToGameObject(GameObject obj, MaterialPropertyBlock mpb)
    {
        foreach (Renderer r in obj.GetComponentsInChildren<Renderer>(true))
            r.SetPropertyBlock(mpb);
    }

    public MaterialPropertyBlock GetMPB(string sceneName)
    {
        if (_mpbs.ContainsKey(sceneName))
            return _mpbs[sceneName];
        else
            return _standardMPB;
    }
}