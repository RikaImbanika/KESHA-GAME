// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using UnityEngine;

public class Dust : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _count = 20;
    [SerializeField] private float _r = 10f; // fade distance & side length of the cube
    [SerializeField] private float _minSpeed = 0.5f;
    [SerializeField] private float _maxSpeed = 2f;
    [SerializeField] private float _speedSmooth = 2f;
    [SerializeField] private float _noiseScale = 0.3f;

    private GameObject[] _dust;
    private Transform[] _transforms;
    private Vector3[] _speeds;
    private float _uniformScale;
    private GameObject _dustPrefab;

    void Start()
    {
        _dust = new GameObject[_count];
        _transforms = new Transform[_count];
        _speeds = new Vector3[_count];
        _dustPrefab = Prefabs.Get("Dust");

        if (S.Camera != null)
        {
            InitializeDust();
        }
        else
        {
            StartCoroutine(WaitForCameraAndInitialize());
        }
    }

    private IEnumerator WaitForCameraAndInitialize()
    {
        while (S.Camera == null)
            yield return new WaitForSeconds(0.25f);
        InitializeDust();
    }

    private void InitializeDust()
    {
        Transform cam = S.Camera.transform;
        Vector3 cameraPos = cam.position;
        Vector3 forward = cam.forward;
        // Cube center is offset by r/2 in the camera's forward direction
        Vector3 cubeCenter = cameraPos + forward * (_r * 0.5f);
        float half = _r * 0.5f; // half of the cube side (side = r)

        for (int i = 0; i < _count; i++)
        {
            _dust[i] = Instantiate(_dustPrefab);
            _transforms[i] = _dust[i].transform;

            // Random position inside the cube
            _transforms[i].position = cubeCenter + new Vector3(
                Random.Range(-half, half),
                Random.Range(-half, half),
                Random.Range(-half, half)
            );

            _speeds[i] = Random.insideUnitSphere * Random.Range(_minSpeed, _maxSpeed);
        }

        _uniformScale = _transforms[0].localScale.x;
    }

    void Update()
    {
        if (S.PS == null)
            return;
            
        if (S.PS._currentSceneName.Contains("BR") ||
            S.PS._currentSceneName == "MR 1" ||
            S.PS._currentSceneName == "TL 0" ||
            S.PS._currentSceneName == "Corridor")
        {
            Transform cam = S.Camera.transform;
            Vector3 c = cam.position;
            Vector3 forward = cam.forward;

            // Wrapping cube center: camera position + r/2 forward
            Vector3 cubeCenter = c + forward * (_r * 0.5f);
            float half = _r * 0.5f;            // half side length (cube side = r)
            float doubleHalf = _r;             // 2 * half = full side
            float invR = 1f / _r;              // for fade only
            float time = Time.time;
            float timeScaled = time * _noiseScale;
            float smoothDelta = _speedSmooth * Time.deltaTime;

            for (int i = 0; i < _count; i++)
            {
                // Smooth turbulent velocity (Perlin noise)
                Vector3 noiseDir = new Vector3(
                    Mathf.PerlinNoise(i * 1.7f + timeScaled, 0.0f) - 0.5f,
                    Mathf.PerlinNoise(0.0f, i * 2.3f + timeScaled) - 0.5f,
                    Mathf.PerlinNoise(i * 3.1f, timeScaled) - 0.5f
                ).normalized;

                float mag = Mathf.Lerp(_minSpeed, _maxSpeed,
                    Mathf.PerlinNoise(timeScaled * 0.7f, i * 5.7f));

                Vector3 targetSpeed = noiseDir * mag;
                _speeds[i] = Vector3.Lerp(_speeds[i], targetSpeed, smoothDelta);

                // Clamp speed
                float speedMag = _speeds[i].magnitude;
                if (speedMag < _minSpeed * 0.5f)
                    _speeds[i] = _speeds[i].normalized * _minSpeed;
                else if (speedMag > _maxSpeed)
                    _speeds[i] = _speeds[i].normalized * _maxSpeed;

                // Move
                _transforms[i].position += _speeds[i] * Time.deltaTime;

                // Wrap inside cube (side = r, center = cubeCenter)
                Vector3 pos = _transforms[i].position;
                Vector3 delta = pos - cubeCenter;

                // Optimized wrapping using Mathf.Repeat (no branches)
                delta.x = Mathf.Repeat(delta.x + half, doubleHalf) - half;
                delta.y = Mathf.Repeat(delta.y + half, doubleHalf) - half;
                delta.z = Mathf.Repeat(delta.z + half, doubleHalf) - half;

                _transforms[i].position = cubeCenter + delta;

                // Face camera
                Vector3 toPlayer = _transforms[i].position - c;
                _transforms[i].rotation = Quaternion.LookRotation(-toPlayer);

                // Size fade
                float dist = toPlayer.magnitude;
                float t = 1f - Mathf.SmoothStep(0f, 1f, dist * invR);
                _transforms[i].localScale = new Vector3(_uniformScale * t, _uniformScale * t, _uniformScale * t);
            }
        }
    }
}