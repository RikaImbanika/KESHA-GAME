// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fireball : MonoBehaviour
{
    public float _speed;
    public bool _active;
    private int _layerMask;

    private Optimiser _opti;
    public Transform[] _children;
    public string _color;
    private string _sceneName;
    public float _damage;
    
    void Start()
    {
        _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
        _opti = new Optimiser(_sceneName);
        _opti.MaxPeriodForDistance = 1 / 12f;

        if (_damage == 0)
            _damage = 5f;

        _layerMask = 1 << LayerMask.NameToLayer("Player") |
                 1 << LayerMask.NameToLayer("Static") |
                 1 << LayerMask.NameToLayer("Enemies") |
                 1 << LayerMask.NameToLayer("Items") |
                 1 << LayerMask.NameToLayer("Default");

        AudioSource shot = Instantiate(S.Shot);
        shot.transform.position = transform.position;
        shot.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        float distance = (transform.position - S.Camera.transform.position).magnitude;
        shot.volume = MathF.Min(0.2f, 50 / (distance * distance));
        shot.Play();
        Destroy(shot, 5);
    }

    void Update()
    {
        if (_active)
        {
            if (_opti.Optimise(transform.position))
            {
                Do();
                _opti.Reset();
            }

            void Do()
            {
                gameObject.transform.position += gameObject.transform.forward.normalized * _opti.DeltaTime * _speed;

                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 60 * _opti.DeltaTime, _layerMask))
                {
                    GameObject go = hit.collider.gameObject;
                    if (go.CompareTag("Player"))
                    {
                        PlayerStorage ps = go.transform.parent.gameObject.GetComponent<PlayerStorage>();
                        ps.Damage(_damage);
                    }

                    AudioSource caboom = Instantiate(S.Caboom);
                    caboom.transform.position = gameObject.transform.position;
                    caboom.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                    float distance = (transform.position - S.Camera.transform.position).magnitude;
                    caboom.volume = MathF.Min(0.20f, 50 / (distance * distance));
                    caboom.Play();
                    Destroy(caboom, 5);

                    NoSpots ns = go.GetComponent<NoSpots>();
                    if (ns == null)
                    {
                        GameObject spot = Instantiate(S.Spot);
                        spot.transform.position = hit.point;
                        spot.transform.rotation = Quaternion.LookRotation(hit.normal);
                        spot.transform.Rotate(0f, 0f, UnityEngine.Random.Range(-180, 180));
                        spot.transform.SetParent(hit.collider.gameObject.transform);
                        spot.transform.localScale = spot.transform.localScale * 5;

                        S.AllFather._spots.Add(spot);
                        if (S.AllFather._spots.Count > 300)
                        {
                            Destroy(S.AllFather._spots[0]);
                            S.AllFather._spots.RemoveAt(0);
                        }
                    }

                    for (int i = 0; i < S.AllFather._enemyBulletSparklesCount; i++)
                    {
                        GameObject sparkle = null;

                        if (_color == "red")
                            sparkle = Instantiate(S.RedSparkle);
                        else if (_color == "green")
                            sparkle = Instantiate(S.GreenSparkle);
                        else if (_color == "blue")
                            sparkle = Instantiate(S.BlueSparkle);
                        else if (_color == "purple")
                            sparkle = Instantiate(S.PurpleSparkle);

                        sparkle.transform.position = hit.point;
                        sparkle.transform.rotation = Quaternion.LookRotation(hit.normal);

                        int gg2 = S.RND.Next(25);

                        float decay = 1f;
                        if (gg2 == 0)
                            decay = 0.5f;
                        else if (gg2 == 1)
                            decay = 2f;
                        else if (gg2 == 2)
                            decay = 0.25f;
                        else if (gg2 == 3)
                            decay = 4f;

                        sparkle.GetComponent<Sparkle3>()._minimisingSpeedCoef = decay;
                    }

                    if (_color == "red")
                    {
                        Instantiate(S.RedHitPoint, hit.point, Quaternion.identity);
                        Instantiate(S.RedHeavySparkle);
                    }
                    else if (_color == "green")
                    {
                        Instantiate(S.GreenHitPoint, hit.point, Quaternion.identity);
                        Instantiate(S.GreenHeavySparkle);
                    }
                    else if (_color == "blue")
                    {
                        Instantiate(S.BlueHitPoint, hit.point, Quaternion.identity);
                        Instantiate(S.BlueHeavySparkle);
                    }
                    else if (_color == "purple")
                    {
                        Instantiate(S.PurpleHitPoint, hit.point, Quaternion.identity);
                        Instantiate(S.PurpleHeavySparkle);
                    }

                    int gg = S.RND.Next(10);

                    if (gg == 0)
                    {
                        GameObject sparkle = Instantiate(S.RedHeavySparkle);
                        sparkle.transform.position = hit.point;
                        sparkle.transform.rotation = Quaternion.LookRotation(hit.normal);
                    }

                    Destroy(gameObject);
                }

                foreach (Transform child in _children)
                {
                    child.LookAt(S.PlayerTarget(_sceneName));
                    child.Rotate(0f, 0f, S.RandRot.GetFloat());
                }
            }
        }
    }
}