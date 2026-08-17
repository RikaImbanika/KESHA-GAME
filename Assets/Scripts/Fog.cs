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

            _mpbs["Start"].SetColor("_FogColor", new Color(0.1f, 0f, 0.2f));
            _mpbs["Start"].SetFloat("_FogDensity", 0.005f);

            _mpbs["Income"].SetColor("_FogColor", new Color(0.4292453f, 0.6394855f, 1f));
            _mpbs["Income"].SetFloat("_FogDensity", 0.001f);

            _mpbs["Hall"].SetColor("_FogColor", new Color(0.47f, 0.27f, 0.2f));
            _mpbs["Hall"].SetFloat("_FogDensity", 0.013f);

            _mpbs["Corridor"].SetColor("_FogColor", new Color(0.6f, 0.2f, 0.15f));
            _mpbs["Corridor"].SetFloat("_FogDensity", 0.01f);

            _mpbs["Final"].SetColor("_FogColor", new Color(0.09f, 0f, 0.3f));
            _mpbs["Final"].SetFloat("_FogDensity", 0.04f);

            _mpbs["TL 0"].SetColor("_FogColor", new Color(0.12f, 0.08f, 0f));
            _mpbs["TL 0"].SetFloat("_FogDensity", 0.022f);


            _mpbs["BR 7"].SetColor("_FogColor", new Color(0f, 0f, 0f));
            _mpbs["BR 7"].SetFloat("_FogDensity", 0.08f);

            _mpbs["BR 7R"].SetColor("_FogColor", new Color(0f, 0f, 0f));
            _mpbs["BR 7R"].SetFloat("_FogDensity", 0.08f);

            _mpbs["BR 6"].SetColor("_FogColor", new Color(0f, 0f, 0f));
            _mpbs["BR 6"].SetFloat("_FogDensity", 0.08f);

            _mpbs["BR 6R"].SetColor("_FogColor", new Color(0f, 0f, 0f));
            _mpbs["BR 6R"].SetFloat("_FogDensity", 0.08f);

            _mpbs["MR 1"].SetColor("_FogColor", new Color(0.2f, 0.15f, 0f));
            _mpbs["MR 1"].SetFloat("_FogDensity", 0.03f);

            _mpbs["BR 8"].SetColor("_FogColor", new Color(0.03f, 0.01f, 0f));
            _mpbs["BR 8"].SetFloat("_FogDensity", 0.05f);

            _mpbs["BR 5"].SetColor("_FogColor", new Color(0.03f, 0.01f, 0f));
            _mpbs["BR 5"].SetFloat("_FogDensity", 0.05f);

            _mpbs["BR 1"].SetColor("_FogColor", new Color(0f, 0f, 0f));
            _mpbs["BR 1"].SetFloat("_FogDensity", 0.1f);

            _mpbs["BR 1R"].SetColor("_FogColor", new Color(0f, 0f, 0f));
            _mpbs["BR 1R"].SetFloat("_FogDensity", 0.1f);

            _mpbs["MR 4"].SetColor("_FogColor", new Color(0.3f, 0.65f, 1f));
            _mpbs["MR 4"].SetFloat("_FogDensity", 0.01f);

            _mpbs["MR 2"].SetColor("_FogColor", new Color(0.3f, 0.65f, 1f));
            _mpbs["MR 2"].SetFloat("_FogDensity", 0.01f);

            _mpbs["MR 3"].SetColor("_FogColor", new Color(0f, 0f, 0f));
            _mpbs["MR 3"].SetFloat("_FogDensity", 0.03f);

            _mpbs["BR 2"].SetColor("_FogColor", new Color(0f, 0f, 0f));
            _mpbs["BR 2"].SetFloat("_FogDensity", 0.015f);

            _mpbs["BR 2R"].SetColor("_FogColor", new Color(0f, 0f, 0f));
            _mpbs["BR 2R"].SetFloat("_FogDensity", 0.015f);

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

            string[] scenes = { "BR 3", "BR 3R", "BR 4", "BR 4R", "TL 1", "TL 2" };

            foreach (string scene in scenes)
            {
                _mpbs[scene].SetColor("_FogColor", fogPalette[Random.Range(0, fogPalette.Length)]);
                _mpbs[scene].SetFloat("_FogDensity", Random.Range(0.005f, 0.04f));
            }
            
            S.Fog = this;

            yield return new WaitForSeconds(3f);

            SetFog("Start");
        }
    }

    public void SetFog(string sceneName)
    {
        MaterialPropertyBlock mpb = _mpbs[sceneName];
        Color clr = mpb.GetColor("_FogColor");
        float density = mpb.GetFloat("_FogDensity");
        SetFog(sceneName, clr, density);
    }

    public void SetFog(string sceneName, Color clr, float density)
    {
        Transform root = S.Loader.Roots[sceneName];
        GameObject rootGo = root.gameObject;
        Scene scene = rootGo.scene;
        _mpbs[sceneName].SetColor("_FogColor", clr);
        _mpbs[sceneName].SetFloat("_FogDensity", density);

        foreach (GameObject rootObj in scene.GetRootGameObjects())
            foreach (Renderer r in rootObj.GetComponentsInChildren<Renderer>(true))
                r.SetPropertyBlock(_mpbs[sceneName]);

        //It's not saving into save file yet
    }

    public void SetFog(string sceneName, GameObject root)
    {
        MaterialPropertyBlock mpb = _standardMPB;
        if (_mpbs.ContainsKey(sceneName))
            mpb = _mpbs[sceneName];

        Scene scene = root.scene;
        foreach (GameObject rootObj in scene.GetRootGameObjects())
            foreach (Renderer r in rootObj.GetComponentsInChildren<Renderer>(true))
                r.SetPropertyBlock(mpb);
    }

    public float[] GetFog(string sceneName)
    {
        float[] result = new float[4];

        MaterialPropertyBlock mpb = _mpbs[sceneName];

        Color clr = mpb.GetColor("_FogColor");
        float density = mpb.GetFloat("_FogDensity");

        result[0] = clr.r;
        result[1] = clr.g;
        result[2] = clr.b;
        result[3] = density;

        return result;
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