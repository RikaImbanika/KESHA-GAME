// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomParticleSpawner : MonoBehaviour
{
    private string _sceneName;
    private MaterialPropertyBlock _mpb;
    private Optimiser _opti;
    private float _timer;
    private float _delay;
    private MeshRenderer _unityEditorMeshRenderer;
    private MeshFilter _unityEditorMeshFilter;

    void Start()
    {
        _sceneName = gameObject.scene.name;
        _opti = new Optimiser(_sceneName);
        _mpb = S.Fog.GetMPB(_sceneName);

        _opti.MaxPeriodForScene = 10f;
        _opti.MaxPeriodForDistance = 3f;

        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<MeshFilter>());
    }

    void Update()
    {
        if (_opti.Optimise(transform.position))
        {
            Do();
            _opti.Reset();
        }

        void Do()
        {
            _timer += _opti.DeltaTime;

            if (_timer > _delay)
            {
                _delay = Random.Range(0.5f, 3.5f);

                _timer = 0f;

                int count = S.RND.Next(1, 45);

                Quaternion rotation = S.RandRot.Get();

                float decay2 = Random.Range(0.05f, 0.7f);

                int segmentsCount = S.RND.Next(1, 5);

                float friction = Random.Range(0.3f, 1f);

                float pushNormal = Random.Range(0.2f, 3f);

                float pushRandom = Random.Range(0.2f, 3f);

                float period = Random.Range(0.008f, 0.03f);

                float gravity = Random.Range(-6, 6);

                float width = Random.Range(0.1f, 1.2f);

                float sizeRandom = Random.Range(3, 14);

                int a = S.RND.Next(5);

                bool isRainbow = a == 0;

                int clrn = S.RND.Next(4);

                for (int i = 0; i < count; i++)
                {
                    GameObject sparkle = null;

                    if (isRainbow)
                        clrn = S.RND.Next(4);

                    if (clrn == 0)
                        sparkle = Instantiate(S.RedSparkle, transform);
                    else if (clrn == 1)
                        sparkle = Instantiate(S.GreenSparkle, transform);
                    else if (clrn == 2)
                        sparkle = Instantiate(S.BlueSparkle, transform);
                    else if (clrn == 3)
                        sparkle = Instantiate(S.PurpleSparkle, transform);

                    sparkle.transform.position = transform.position;
                    sparkle.transform.rotation = rotation;

                    Sparkle sp = sparkle.GetComponent<Sparkle>();

                    int gg2 = S.RND.Next(25);

                    float decay1 = 1f;
                    if (gg2 == 0)
                        decay1 = 0.5f;
                    else if (gg2 == 1)
                        decay1 = 2f;
                    else if (gg2 == 2)
                        decay1 = 0.25f;
                    else if (gg2 == 3)
                        decay1 = 4f;

                    sp._minimisingSpeedCoef = decay1 * decay2;

                    sp._count = segmentsCount;

                    sp._friction = friction;

                    sp._pushNormal = pushNormal;

                    sp._pushRandom = pushRandom;

                    sp._period = period;

                    sp._gravity = gravity;

                    sp._width = width;

                    sp._sizeRandom = sizeRandom;
                }

                AudioSource caboom = Instantiate(S.Caboom);
                caboom.transform.position = gameObject.transform.position;
                caboom.pitch = UnityEngine.Random.Range(1.3f, 1.5f);
                float distance = (transform.position - S.Camera.transform.position).magnitude;
                caboom.volume = System.MathF.Min(0.05f, 25 / (distance * distance));
                caboom.Play();
                Destroy(caboom.gameObject, 5);
            }
        }
    }

#if UNITY_EDITOR
    private void EnsureMeshFilter()
    {
        if (GetComponent<MeshRenderer>() == null)
        {
            _unityEditorMeshRenderer = gameObject.AddComponent<MeshRenderer>();
            _unityEditorMeshRenderer.sharedMaterial = Materials.GetInEditor("DUMMY_GREEN");
        }

        if (GetComponent<MeshFilter>() == null)
        {
            _unityEditorMeshFilter = gameObject.AddComponent<MeshFilter>();
            _unityEditorMeshFilter.sharedMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
        }
    }

    void OnDrawGizmos()
    {
        EnsureMeshFilter();
        transform.localScale = Vector3.one;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
#endif
}
