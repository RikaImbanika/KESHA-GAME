// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToiletZombie : MonoBehaviour
{
    private string _sceneName;
    private float _fireCooldown;
    public float _nextFireTime;
    public bool _active;

    // Start is called before the first frame update
    void Start()
    {
        _active = true;
        _fireCooldown = 1.3f;

        _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
    }

    // Update is called once per frame
    void Update()
    {
        if (_active)
        {
            Vector3 toPlayer = S.Camera.transform.position - transform.position;
            Ray ray = new Ray(transform.position, toPlayer);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            _nextFireTime -= Time.deltaTime;

            if (hit.collider.gameObject.tag == "Player")
                if (_nextFireTime <= 0)
                {
                    _nextFireTime = _fireCooldown;
                    GameObject bullet = Instantiate(S.FireballRed, S.Loader.Roots[_sceneName]);
                    bullet.transform.position = gameObject.transform.position + new Vector3(0, 0, 0);
                    bullet.transform.LookAt(S.Camera.transform.position);
                    Fireball eb = bullet.GetComponent<Fireball>();
                    eb._active = true;
                    eb._speed = 30;
                    Destroy(bullet, 10);

                    AudioSource shot = Instantiate(S.Shot);
                    shot.transform.position = transform.position;
                    shot.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                    float distance = (transform.position - S.Camera.transform.position).magnitude;
                    shot.volume = MathF.Min(0.5f, 60 / (distance * distance));
                    shot.Play();
                    Destroy(shot, 5);
                }
        }
    }
}
