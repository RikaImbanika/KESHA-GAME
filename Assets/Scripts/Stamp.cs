// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stamp : MonoBehaviour
{
    public Door _door;
    string _id;
    string _idDestroyed;
    string _sceneName;
    public GameObject _go;
    public GameObject _blueFlames;
    bool _alreadyUnlocked;
    float _stampAnimationTimeLeft;
    Vector3 _startScale;
    Vector3 _startPosition;
    MaterialPropertyBlock _mpb;

    [Header("Unlock Animation")]
    [Tooltip("How high the object rises during the animation.")]
    public float _riseHeight = 4f;
    public float _animationDuration = 2.3f;

    public void Start()
    {
        _go = gameObject;

        _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;

        GetId();

        _startScale = transform.localScale;
        _startPosition = transform.position;

        bool destroyed = S.SM.LoadBool(_idDestroyed) ?? false;
        _door._locked = !destroyed;

        if (destroyed)
        {
            Destroy(gameObject);

            Debug.Log($"Stamp Opened. id = {_id}");
        }
        else
        {
            Debug.Log($"Stamp Not Opened. id = {_id}");

            StartCoroutine(SetParent());

            IEnumerator SetParent()
            {
                while (S.Loader.Roots[_sceneName] == null)
                    yield return new WaitForSeconds(0.25f);

                Transform root = S.Loader.Roots[_sceneName];

                _blueFlames = Instantiate(Prefabs.Get("BlueFlames"), root);
                _mpb = S.Fog.GetMPB(_sceneName);
                S.Fog.ApplyToGameObject(_blueFlames, _mpb);
                S.Fog.ApplyToGameObject(gameObject, _mpb);

                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 20f))
                {
                    Vector3 point = hit.point;
                    _blueFlames.transform.position = point;
                    _blueFlames.transform.rotation = Quaternion.LookRotation(transform.forward);
                }
            }
        }
    }

    public void GetId()
    {
        _id = S.ID("ST", gameObject);
        _idDestroyed = S.IDM(_id, "destoyed");
    }

    public void Unlock()
    {
        if (!_alreadyUnlocked)
        {
            _alreadyUnlocked = true;
            _stampAnimationTimeLeft = _animationDuration;
            S.AM.Play("Stamp Sound", 1);
            S.SM.Save(_idDestroyed, true);
            Debug.Log($"STAMP UNLOCKED AND SAVED!!! id = {_id}");

            float count = _blueFlames.transform.childCount;

            for (int i = 0; i < count; i++)
            {
                GameObject child = _blueFlames.transform.GetChild(i).gameObject;
                child.AddComponent<BlueFlame>();
            }

            _door.Unlock();
        }
    }

    public void Update()
    {
        if (_stampAnimationTimeLeft > 0)
        {
            _stampAnimationTimeLeft -= Time.deltaTime;

            // progress: 0 at start, 1 at end
            float progress = 1f - (_stampAnimationTimeLeft / _animationDuration);
            progress = Mathf.Clamp01(progress);

            // --- Rotation: only around local Y, starts slow then accelerates ---
            // Angular speed grows quadratically, giving a smooth start and constant angular acceleration.
            float angularSpeed = 12000f * progress * progress; // degrees per second (adjust if desired)
            transform.Rotate(0f, angularSpeed * Time.deltaTime, 0f, Space.Self);

            // --- Scale: noticeable shrink before rotation becomes apparent ---
            // Power curve drops quickly at the beginning.
            float scaleFactor = Mathf.Pow(1f - progress, 2.5f);
            transform.localScale = _startScale * scaleFactor;

            // --- Vertical movement: upward with acceleration (parabolic speed) ---
            // Displacement = 0.5 * a * t^2, so y offset grows with progress^2.
            float yOffset = _riseHeight * progress * progress;
            Vector3 pos = _startPosition;
            pos.y += yOffset;
            transform.position = pos;

            if (_stampAnimationTimeLeft <= 0)
                Destroy(gameObject);
        }
    }
}