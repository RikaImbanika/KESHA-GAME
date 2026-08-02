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
    Vector3 _point2;
    Vector3 _point4;
    string _rememberedScene;

    void Start()
    {
        _rememberedScene = "Start";
        _point2 = new Vector3(0, 0, 0);
        _point4 = new Vector3(0, 0, 0);
        _period = 0.3f;
        _angleStep = 0.051f;
        _h = 5f;
        _r = 3f;
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
            if (!S.Loader.Roots.ContainsKey(S.PS._currentSceneName))
                return;

            Transform root = S.Loader.Roots[S.PS._currentSceneName];

            //

            Vector3 point1 = S.Camera.transform.position;

            _currentAngle += _angleStep;
            float sinBuf = Mathf.Sin(_currentAngle);
            Vector3 dir = new Vector3(sinBuf * _r, 0f, Mathf.Cos(_currentAngle) * _r);

            // Smoother movement 1

            float mk = 4f * Time.deltaTime;
            mk = Mathf.Clamp(mk, 0, 1);

            if (_rememberedScene != S.PS._currentSceneName)
            {
                mk = 1f;
                _rememberedScene = S.PS._currentSceneName;
            }

            float k = 1 - mk;

            _point2 = _point2 * k + point1 * mk;

            Vector3 point3 = _point2;

            //For close walls

            Ray ray1 = new Ray(point3, dir);
            RaycastHit hit1;
            if (Physics.Raycast(ray1, out hit1, _r + 0.5f))
                point3 = hit1.point - dir.normalized * 0.5f;
            else
                point3 += dir;

            //For low ceilings

            Ray ray2 = new Ray(point3, Vector3.up);
            RaycastHit hit2;
            if (Physics.Raycast(ray2, out hit2, _h))
                point3 = hit2.point + new Vector3(0, -1, 0);
            else
                point3 += new Vector3(0, _h, 0);

            Debug.DrawRay(_point2, Vector3.up * 100f, Color.red, 0.1f);
            Debug.DrawRay(point3, Vector3.up * 100f, Color.green, 0.1f);
            Debug.DrawRay(_point4, Vector3.up * 100f, Color.green, 0.1f);

            // Smoother movement 2

            mk = 4f * Time.deltaTime;
            mk = Mathf.Clamp(mk, 0, 1);

            if (_rememberedScene != S.PS._currentSceneName)
            {
                mk = 1f;
                _rememberedScene = S.PS._currentSceneName;
            }

            k = 1 - mk;

            _point4 = _point4 * k + point3 * mk;

            //

            GameObject particle = Instantiate(S.RikaParticle, _point4, Quaternion.identity, root);

            RikaParticle rp = particle.GetComponent<RikaParticle>();

            Material matFlat = new Material(_shader);
            Material matLong = new Material(_shader);

            matFlat.mainTexture = _sparkleTex;
            matLong.mainTexture = _longSparkleTex;

            matFlat.SetFloat("_HueOffset", sinBuf);
            matLong.SetFloat("_HueOffset", sinBuf);
            matFlat.SetFloat("_Speed", 0);
            matLong.SetFloat("_Speed", 0);

            rp._flatSparkle.transform.GetChild(0).GetComponent<Renderer>().material = matFlat;
            rp._longSparkle.transform.GetChild(0).GetComponent<Renderer>().material = matLong;
            rp._longSparkle.transform.GetChild(1).GetComponent<Renderer>().material = matLong;
        }
    }
}
