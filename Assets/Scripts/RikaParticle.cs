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
    GameObject[] _flatSparklesDown;
    MaterialPropertyBlock _mpb;
    float _timer;
    float _globalTimer;
    float _timer3;
    float _period;
    Vector3 _startPos;
    Vector3 _flatSparkleStartScale;
    bool _endedPart1;
    Vector3[] _flatSparklesDirs;
    Vector3[] _flatSparklesDownDirs;
    Quaternion _rotatorBuffered;
    Vector3 _position;
    float _gravity;
    bool _needPart2;
    float _height;
    bool _part3Started;
    bool _endedPart3;
    bool _initialised;
    Vector3 _hitPoint;

    void Start()
    {
        StartCoroutine(LateStart());

        IEnumerator LateStart()
        {
            _period = 1 / 30f;

            _gravity = 4.5f;

            _startPos = transform.position;

            while (S.Fog == null ||
                S.PS == null)
                yield return new WaitForSeconds(0.25f);

            _mpb = S.Fog.GetMPB(S.PS._currentSceneName);

            _flatSparkleStartScale = _flatSparkle.transform.localScale;

            _flatSparkles = new GameObject[3];

            _needPart2 = S.RND.Next(20) == 0;

            if (_needPart2)
            {
                _flatSparklesDown = new GameObject[3];
                _flatSparklesDownDirs = new Vector3[3];
            }

            _flatSparkles[0] = _flatSparkle;

            _flatSparklesDirs = new Vector3[3];

            Vector3 _toPlayer = S.Camera.transform.position - _startPos;

            _rotatorBuffered = Quaternion.Euler(90, 0, 0);

            for (int i = 1; i < 3; i++)
                _flatSparkles[i] = Instantiate(_flatSparkle, _flatSparkle.transform.position, _flatSparkle.transform.rotation, transform);

            for (int i = 0; i < 3; i++)
            {
                float power = Random.Range(0.02f, 0.65f);

                _flatSparklesDirs[i] = RandomPerpendicular(_toPlayer) * power + new Vector3(0, -4f, 0);

                S.Fog.ApplyToGameObject(_flatSparkles[i], _mpb);

                if (_needPart2)
                {
                    _flatSparklesDown[i] = Instantiate(_flatSparkle, _flatSparkle.transform.position, _flatSparkle.transform.rotation, transform);

                    S.Fog.ApplyToGameObject(_flatSparklesDown[i], _mpb);

                    _flatSparklesDown[i].transform.localScale = Vector3.zero;
                }
            }

            if (!_needPart2)
                Destroy(_longSparkle);
            else
            {
                _longSparkle.transform.localScale = Vector3.zero;
                _height = 25f;

                Ray ray = new Ray(transform.position, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 25f))
                    _height = Vector3.Magnitude(hit.point - transform.position);

                for (int i = 0; i < 3; i++)
                    _flatSparklesDown[i].transform.position = hit.point;

                _hitPoint = hit.point;
            }

            _initialised = true;
        }
    }

    void Update()
    {
        if (!_initialised)
            return;

        _timer += Time.deltaTime;
        _globalTimer += Time.deltaTime;

        if (_timer > _period)
        {
            Part1();
            if (_needPart2)
            {
                if (_part3Started)
                {
                    if (!_endedPart3)
                        Part3();
                }
                else
                    Part2();
            }

            _timer = 0;

            void Part1()
            {
                float dur = 1.5f;

                if (_globalTimer < dur)
                {
                    Vector3 _toPlayer = S.Camera.transform.position - _startPos;

                    Quaternion rot = Quaternion.LookRotation(_toPlayer) * _rotatorBuffered;

                    Vector3 s = _flatSparkleStartScale * (dur - _globalTimer) / dur;

                    for (int i = 0; i < 3; i++)
                    {
                        _flatSparkles[i].transform.rotation = rot;
                        _flatSparkles[i].transform.localScale = s;
                        _flatSparkles[i].transform.position += _flatSparklesDirs[i] * _period;
                    }
                }
                else if (!_endedPart1)
                {
                    for (int i = 0; i < 3; i++)
                        Destroy(_flatSparkles[i]);

                    _endedPart1 = true;

                    if (_needPart2 && _part3Started && _endedPart3 || !_needPart2)
                        Destroy(gameObject);
                }
            }

            void Part2()
            {
                float a1 = Mathf.Pow(_globalTimer - 0.15f, 2) * _gravity;
                if (_globalTimer < 0.1f)
                    a1 = 0;
                float a2 = Mathf.Pow(_globalTimer, 2) * _gravity;

                _position = _startPos - new Vector3(0, a2, 0);

                _longSparkle.transform.position = _position;
                _longSparkle.transform.localScale = new Vector3(1, 1, a2 - a1);

                if (a2 > _height)
                {
                    Destroy(_longSparkle);

                    Vector3 _toPlayer = S.Camera.transform.position - _flatSparklesDown[0].transform.position;

                    for (int i = 0; i < 3; i++)
                    {
                        float power = Random.Range(0.05f, 3f);

                        _flatSparklesDownDirs[i] = RandomPerpendicular(_toPlayer) * power + new Vector3(0, 0.4f, 0);
                    }

                    _part3Started = true;
                }
            }

            void Part3()
            {
                _timer3 += Time.deltaTime;

                float dur = 0.2f;

                if (_timer3 < dur)
                {
                    Vector3 _toPlayer = S.Camera.transform.position - _flatSparklesDown[0].transform.position;
                    Quaternion rot = Quaternion.LookRotation(_toPlayer) * _rotatorBuffered;

                    Vector3 s = _flatSparkleStartScale * (dur - _timer3) / dur;

                    for (int i = 0; i < 3; i++)
                    {
                        _flatSparklesDown[i].transform.rotation = rot;
                        _flatSparklesDown[i].transform.localScale = s;
                        _flatSparklesDown[i].transform.position += _flatSparklesDownDirs[i] * _period;
                    }
                }
                else if (!_endedPart3)
                {
                    for (int i = 0; i < 3; i++)
                        Destroy(_flatSparklesDown[i]);

                    _endedPart3 = true;

                    if (_endedPart1)
                        Destroy(gameObject);
                }
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
