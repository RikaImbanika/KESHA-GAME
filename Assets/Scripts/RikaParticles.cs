// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RikaParticles : MonoBehaviour
{
    float _timer;
    float _period;
    float _h;
    float _r;
    Shader _shader;
    Texture2D _sparkleTex;
    Texture2D _longSparkleTex;
    float _currentAngle = 0f;
    float _angleStep;

    void Start()
    {
        _period = 0.16f;
        _angleStep = 0.1f;
        _h = 5f;
        _r = 2.6f;
        _shader = Shader.Find("Custom/HueShiftUnlitTransparentTwoSidesF");
        _sparkleTex = Resources.Load<Texture2D>("Textures/Sparkles/SPARKLE_2_BLUE");
        _longSparkleTex = Resources.Load<Texture2D>("Textures/Lasers/BlueLaser");
    }

    void Update()
    {
        if (S.RikaParticle == null)
            return;

        _timer += Time.deltaTime;

        if (_timer > _period)
        {
            Vector3 p1 = S.Camera.transform.position + new Vector3(0, _h, 0);

            _currentAngle += _angleStep;
            float sinBuf = Mathf.Sin(_currentAngle);
            Vector3 dir = new Vector3(sinBuf * _r, 0f, Mathf.Cos(_currentAngle) * _r);

            Vector3 p2 = p1 + dir;

            if (!S.Loader.Roots.ContainsKey(S.PS._currentSceneName))
                return;

            Transform root = S.Loader.Roots[S.PS._currentSceneName];

            GameObject particle = Instantiate(S.RikaParticle, p2, Quaternion.identity, root);

            RikaParticle rp = particle.GetComponent<RikaParticle>();

            Material matFlat = new Material(_shader);
            Material matLong = new Material(_shader);

            matFlat.mainTexture = _sparkleTex;
            matLong.mainTexture = _longSparkleTex;

            matFlat.SetFloat("_HueOffset", sinBuf);
            matLong.SetFloat("_HueOffset", sinBuf);
            matFlat.SetFloat("_Speed", 0);
            matLong.SetFloat("_Speed", 0);

            rp._flatSparkle.GetComponent<Renderer>().material = matFlat;
            rp._longSparkle.transform.GetChild(0).GetComponent<Renderer>().material = matLong;
            rp._longSparkle.transform.GetChild(1).GetComponent<Renderer>().material = matLong;
        }
    }
}
