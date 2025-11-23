using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float _speed;
    public bool _active;
    public string _sceneName;
    private int _layerMask;

    private float _updateInterval;
    private float _deltaTime;
    private float _fpsDeltaTime;
    private bool _playerInScene;
    private bool _needUpdate;
    private int _fpsD;
    private float _minFps = 1 / 24f;
    private float _maxFps = 1 / 120f;
    void Start()
    {
        _layerMask = 1 << LayerMask.NameToLayer("Player") |
                 1 << LayerMask.NameToLayer("Static") |
                 1 << LayerMask.NameToLayer("Enemies") |
                 1 << LayerMask.NameToLayer("Items") |
                 1 << LayerMask.NameToLayer("Default");
    }

    void Update()
    {
        if (_active)
        {
            if (FPS())
            {
                Do();
                _deltaTime = 0;
            }
            void Do()
            {
                gameObject.transform.position += gameObject.transform.forward.normalized * _deltaTime * _speed;

                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 60 * _deltaTime, _layerMask))
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
                        AudioSource caboom = Instantiate(S.AllFather._caboom);
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

                        S.AllFather._spots.Add(spot);
                        if (S.AllFather._spots.Count > 300)
                        {
                            Destroy(S.AllFather._spots[0]);
                            S.AllFather._spots.RemoveAt(0);
                        }
                    }

                    for (int i = 0; i < S.AllFather._enemyBulletSparklesCount; i++)
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

        bool FPS()
        {
            _deltaTime += Time.deltaTime;
            _fpsDeltaTime += Time.deltaTime;

            bool check = Check();

            if (_fpsDeltaTime > 0.2f || _needUpdate)
                return RecalcFPS();
            else
                return check;

            bool RecalcFPS()
            {
                _fpsDeltaTime = 0;
                _fpsD++;

                if (check || _fpsD >= 3)
                {
                    _fpsD = 0;

                    float lim = 20f;
                    float step = 40f;
                    float dist = Vector3.Distance(transform.position, S.Ph.transform.position);
                    float t = (dist - lim) / step;

                    float smoothed = 1;
                    if (t > 0 && S.Camera.fieldOfView > 35f)
                        smoothed = Mathf.SmoothStep(0f, 1f, 1f / (t * t));

                    float coef1 = Mathf.Lerp(_minFps, _maxFps, smoothed);

                    float angle = Vector3.Angle(S.Camera.transform.forward, (transform.position - S.Camera.transform.position).normalized);
                    //+ visible - invisible
                    float k = 0.5f - (S.Camera.fieldOfView - angle) / 30f; //degrees
                    float coef2 = 0.05f;
                    if (dist > 30f)
                        coef2 = Mathf.Clamp(k, 0.05f, 1) * 20f;

                    _updateInterval = coef1 * coef2;

                    _needUpdate = false;

                    return true;
                }
                else
                    return false;
            }

            bool Check()
            {
                bool buf = S.PS._currentSceneName == _sceneName;

                if (!_playerInScene && buf)
                    _needUpdate = true;

                _playerInScene = buf;

                float coef3 = 20f;

                if (_playerInScene)
                    coef3 = 1f;

                return _deltaTime > _updateInterval * coef3;
            }
        }
    }
}
