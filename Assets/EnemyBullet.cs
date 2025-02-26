using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public AllFather _allFather;
    public float _speed;
    public bool _active;

    void Start()
    {
        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();
    }

    void Update()
    {
        if (_active)
        {
            gameObject.transform.position += gameObject.transform.forward.normalized * Time.deltaTime * _speed;

            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1))
            {
                GameObject go = hit.collider.gameObject;
                if (go.CompareTag("Player"))
                {
                    PlayerStorage ps = go.transform.parent.gameObject.GetComponent<PlayerStorage>();
                    ps.Damage(5);
                }

                NoSpots ns = go.GetComponent<NoSpots>();
                if (ns == null)
                {
                    AudioSource caboom = Instantiate(_allFather._caboom);
                    caboom.transform.position = gameObject.transform.position;
                    caboom.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                    float distance = (transform.position - S.Camera.transform.position).magnitude;
                    caboom.volume = MathF.Min(0.5f, 60 / (distance * distance));
                    caboom.Play();
                    Destroy(caboom, 5);

                    GameObject spot = Instantiate(S.Spot);
                    spot.transform.position = hit.point;
                    spot.transform.rotation = Quaternion.LookRotation(hit.normal);
                    spot.transform.Rotate(0f, 0f, UnityEngine.Random.Range(-180, 180));
                    spot.transform.SetParent(hit.collider.gameObject.transform);
                    spot.transform.localScale = spot.transform.localScale * 5;

                    _allFather._spots.Add(spot);
                    if (_allFather._spots.Count > 300)
                    {
                        Destroy(_allFather._spots[0]);
                        _allFather._spots.RemoveAt(0);
                    }
                }

                for (int i = 0; i < _allFather._enemyBulletSparklesCount; i++)
                {
                    GameObject sparkle = Instantiate(S.RedSparkle);
                    sparkle.transform.position = hit.point;
                    sparkle.transform.rotation = Quaternion.LookRotation(hit.normal);
                    sparkle.GetComponent<IsSparkle>()._active = true;
                }

                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
       
    }
}
