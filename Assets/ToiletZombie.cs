using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletZombie : MonoBehaviour
{
    AllFather _allFather;
    GameObject _theBullet;
    private float _fireCooldown;
    public float _nextFireTime;
    Camera _camera;
    public bool _active;

    // Start is called before the first frame update
    void Start()
    {
        _active = true;
        _fireCooldown = 1.3f;
        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();
        _camera = Camera.main;
        _theBullet = GameObject.Find("EnemyBullet");
    }

    // Update is called once per frame
    void Update()
    {
        if (_active)
        {
            Vector3 toPlayer = _camera.transform.position - transform.position;
            Ray ray = new Ray(transform.position, toPlayer);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            _nextFireTime -= Time.deltaTime;

            if (hit.collider.gameObject.tag == "Player")
                if (_nextFireTime <= 0)
                {
                    _nextFireTime = _fireCooldown;
                    GameObject bullet = Instantiate(_theBullet);
                    bullet.transform.position = gameObject.transform.position + new Vector3(0, 0, 0);
                    bullet.transform.LookAt(_camera.transform.position);
                    EnemyBullet eb = bullet.GetComponent<EnemyBullet>();
                    eb._active = true;
                    eb._speed = 30;
                    Destroy(bullet, 15);

                    AudioSource shot = Instantiate(_allFather._shot);
                    shot.transform.position = transform.position;
                    shot.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                    float distance = (transform.position - _allFather._camera.transform.position).magnitude;
                    shot.volume = MathF.Min(0.5f, 60 / (distance * distance));
                    shot.Play();
                    Destroy(shot, 5);
                }
        }
    }
}
