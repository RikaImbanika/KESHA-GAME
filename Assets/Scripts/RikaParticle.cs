// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RikaParticle : MonoBehaviour
{
    public GameObject _flatSparkle;
    public GameObject _longSparkle;
    GameObject[] _flatSparkles;
    GameObject[] _longSparkles;
    MaterialPropertyBlock _mpb;
    float _timer;
    float _globalTimer;
    float _period;
    Vector3 _startPos;
    Vector3 _flatSparkleStartScale;
    bool _endedPart1;
    Vector3[] _flatSparklesDirs;
    Quaternion _rotatorBuffered;
    int _longIndex;
    float _fallSpeed;
    Vector3 _p1;
    Vector3 _p2;
    float _gravity;

    void Start()
    {
        _period = 1 / 30f;

        _gravity = 1.3f;

        _startPos = transform.position;

        _mpb = S.Fog.GetMPB(S.PS._currentSceneName);

        _flatSparkleStartScale = _flatSparkle.transform.localScale;

        _flatSparkles = new GameObject[3];
        _longSparkles = new GameObject[3];

        _flatSparkles[0] = _flatSparkle;
        _longSparkles[0] = _longSparkle; //TO DO: Only one.

        _flatSparklesDirs = new Vector3[3];

        Vector3 _toPlayer = S.Camera.transform.position - _startPos;

        _rotatorBuffered = Quaternion.Euler(90, 0, 0);

        for (int i = 1; i < 3; i++)
        {
            _flatSparkles[i] = Instantiate(_flatSparkle, _flatSparkle.transform.position, _flatSparkle.transform.rotation, transform);
            _longSparkles[i] = Instantiate(_longSparkle, transform);
        }

        for (int i = 0; i < 3; i++)
        {
            float power = Random.Range(0.05f, 1f);

            _flatSparklesDirs[i] = RandomPerpendicular(_toPlayer) * power;

            S.Fog.ApplyToGameObject(_flatSparkles[i], _mpb);
            S.Fog.ApplyToGameObject(_longSparkles[i], _mpb);

            _longSparkles[i].transform.localScale = Vector3.zero;
        }
    }

    void Update()
    {
        _timer += Time.deltaTime;
        _globalTimer += Time.deltaTime;

        if (_timer > _period)
        {
            Part1();
            Part2();

            _timer = 0;

            void Part1()
            {
                float _dur = 0.6f;

                if (_globalTimer < _dur)
                {
                    Vector3 _toPlayer = S.Camera.transform.position - _startPos;

                    float s = (_dur - _globalTimer) / _dur;

                    for (int i = 0; i < 3; i++)
                    {
                        _flatSparkles[i].transform.rotation = Quaternion.LookRotation(_toPlayer) * _rotatorBuffered;
                        _flatSparkles[i].transform.localScale = _flatSparkleStartScale * s;
                        _flatSparkles[i].transform.position += _flatSparklesDirs[i] * Time.deltaTime;
                    }
                }
                else if (!_endedPart1)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Destroy(_flatSparkles[i]);
                        Destroy(_longSparkles[i]);
                    }

                    _endedPart1 = true;

                    Destroy(gameObject); //
                }
            }

            void Part2()
            {
                _p1 = _longSparkles[_longIndex].transform.position;

                _fallSpeed += _gravity * Time.deltaTime;

                _p2 = _p1 - new Vector3(0, _fallSpeed, 0);

                _longIndex++;
                if (_longIndex >= 3)
                    _longIndex = 0;

                _longSparkles[_longIndex].transform.position = _p2;
                _longSparkles[_longIndex].transform.localScale = new Vector3(1, 1, _fallSpeed);
            }
        }
    }

    public static Vector3 RandomPerpendicular(Vector3 direction)
    {
        Vector3 arbitrary = Mathf.Abs(direction.y) < 0.99f
            ? Vector3.up
            : Vector3.forward;

        Vector3 perp = Vector3.Cross(direction, arbitrary).normalized;

        float randomAngle = Random.Range(0f, 360f);
        return Quaternion.AngleAxis(randomAngle, direction) * perp;
    }
}
