// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Illuminati : MonoBehaviour
{
    string _sceneName;
    string _id;
    string _idSoundId;
    string _idTriangleId;
    int _soundId;
    int _triangleId;
    public List<GameObject> _triangles;

    public float _bottom;
    public float _top;
    public float _xMin;
    public float _xMax;
    public float _zMin;
    public float _zMax;

    float _lastSaidTime;
    float _timer;
    string _state;
    float _lastStateSwitchTime;
    MaterialPropertyBlock _mpb;

    void Start()
    {
        _sceneName = gameObject.scene.name;
        _id = S.ID("IL", gameObject);
        _mpb = S.Fog.GetMPB(_sceneName);

        _idTriangleId = S.IDM(_id, "tri");
        _idSoundId = S.IDM(_id, "snd");

        _triangleId = S.SM.LoadInt(_idTriangleId) ?? -1;

        _state = "deactivated";

        if (_triangleId == -1)
            Init();
        else
            Load();

        for (int i = 0; i < _triangles.Count; i++)
            _triangles[i].SetActive(false);

        void Load()
        {
            _soundId = S.SM.LoadInt(_idSoundId) ?? 0;
        }

        void Init()
        {
            _soundId = S.RND.Next(2);
            _triangleId = S.RND.Next(_triangles.Count);

            Save();
        }
    }

    void Save()
    {
        S.SM.Save(_idTriangleId, _triangleId);
        S.SM.Save(_idSoundId, _soundId);
    }

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer > 0.25f)
        {
            _timer = 0;

            if (S.Inventory._zoomed < S.Inventory._zoomTime && !S.PM.isCrouching)
            {
                Deactivate();
                return;
            }

            if (S.PS._currentSceneName != _sceneName)
            {
                Deactivate();
                return;
            }

            Vector3 p = S.Camera.transform.position;

            if (p.x < _xMin || p.x > _xMax ||
                p.y < _bottom || p.y > _top ||
                p.z < _zMin || p.z > _zMax)
            {
                Deactivate();
                return;
            }


            if (!TriangleIsVisible())
            {
                Deactivate();
                return;
            }

            Activate();
        }
    }

    void Activate()
    {
        if (_state == "deactivating")
        {
            _state = "activated";
        }
        else if (_state == "deactivated")
        {
            _lastStateSwitchTime = Time.time;
            _state = "activating";
        }
        else if (_state == "activating")
        {
            float t = Time.time - _lastStateSwitchTime;

            if (t > 0.4f)
            {
                GameObject obj = _triangles[_triangleId];
                obj.SetActive(true);
                _state = "activated";
                S.AM.Play($"Illuminati {_soundId + 1}");

                SayIt();
                
                Inst(S.GreenHitPoint, S.GreenHeavySparkle);

                void Inst(GameObject hitPointPrefab, GameObject heavySparklePrefab)
                {
                    Vector3 p = obj.transform.position;
                    Quaternion r = Quaternion.LookRotation(obj.transform.forward);

                    GameObject hitPoint = Instantiate(hitPointPrefab, p, r);
                    GameObject heavySparkle = Instantiate(heavySparklePrefab, p, r);

                    S.Fog.ApplyToGameObject(hitPoint, _mpb);
                    S.Fog.ApplyToGameObject(heavySparkle, _mpb);

                    Destroy(hitPoint, 5f);
                }
            }
        }
    }

    void SelectSound()
    {
        if (_soundId == 0)
            _soundId = 1;
        else
            _soundId = 0;
    }

    void SelectTriangle()
    {
        int remembered = _triangleId;

        while (_triangleId == remembered)
            _triangleId = S.RND.Next(_triangles.Count);
    }

    void SayIt()
    {
        if (Time.time - _lastSaidTime > 30)
        {
            _lastSaidTime = Time.time;

            StartCoroutine(SayItAsync());

            IEnumerator SayItAsync()
            {
                yield return new WaitForSeconds(3f);

                int n = S.RND.Next(10);

                if (n == 0)
                    S.Console.AddMessage("Rika: I found them!", Color.magenta);
                else if (n == 1)
                    S.Console.AddMessage("Rika: HoLy CoW iT's A TRIANGLE!!!", Color.magenta);
                else if (n == 2)
                    S.Console.AddMessage("Rika: Triangles... Triangles everywhere! Why...", Color.magenta);
                else if (n == 3)
                    S.Console.AddMessage("Rika: Here you are!", Color.magenta);
                else if (n == 4)
                    S.Console.AddMessage("Rika: Omg omg omg", Color.magenta);
                else if (n == 5)
                    S.Console.AddMessage("Rika: Wow!", Color.magenta);
                else if (n == 6)
                    S.Console.AddMessage("Rika: Oh no", Color.magenta);
                else if (n == 7)
                    S.Console.AddMessage("Rika: ThEy WaTcHiNg Me!", Color.magenta);
                else if (n == 8)
                    S.Console.AddMessage("Rika: Seriously...", Color.magenta);
                else if (n == 9)
                    S.Console.AddMessage("Rika: What?", Color.magenta);
            }
        }
    }

    bool TriangleIsVisible()
    {
        Vector3 point = _triangles[_triangleId].transform.position;
        Camera cam = S.Camera;

        Vector3 viewportPoint = cam.WorldToViewportPoint(point);
        return viewportPoint.z > 0f &&
               viewportPoint.x > 0f && viewportPoint.x < 1f &&
               viewportPoint.y > 0f && viewportPoint.y < 1f;
    }

    void Deactivate()
    {
        if (_state == "activating")
        {
            _state = "deactivated";
        }
        else if (_state == "activated")
        {
            _lastStateSwitchTime = Time.time;
            _state = "deactivating";
        }
        else if (_state == "deactivating")
        {
            float t = Time.time - _lastStateSwitchTime;

            if (t > 0.4f)
            {
                _triangles[_triangleId].SetActive(false);
                _state = "deactivated";

                SelectTriangle();
                SelectSound();
                Save();
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3(
            (_xMin + _xMax) * 0.5f,
            (_bottom + _top) * 0.5f,
            (_zMin + _zMax) * 0.5f
        );
        Vector3 size = new Vector3(
            _xMax - _xMin,
            _top - _bottom,
            _zMax - _zMin
        );
        Gizmos.DrawWireCube(center, size);
    }
#endif
}