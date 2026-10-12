// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Amoeba : MonoBehaviour
{
    public Animator _ani;
    public string _type;
    public string _id;
    private string _sceneName;
    private string _idPos;
    private string _idRot;
    private string _idType;
    private Optimiser _opti;
    private LayerMask _mask;

    void Start()
    {
        _opti = new Optimiser(gameObject.scene.name);

        StartCoroutine(Start0());

        IEnumerator Start0()
        {
            _sceneName = gameObject.scene.name;

            while (S.AllFather == null ||
                S.Loader.Roots == null ||
                !S.Loader.Roots.ContainsKey(_sceneName) ||
                S.Loader.Roots[_sceneName] == null ||
                S.Fog == null)
                yield return new WaitForSeconds(0.15f);

            GetId();
            _ani = GetComponent<Animator>();

            while (S.SM == null)
                yield return new WaitForSeconds(0.1f);

            Load();

            void Load()
            {
                MaterialPropertyBlock fogMpb = S.Fog.GetMPB(_sceneName);

                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                mpb.SetColor("_FogColor", fogMpb.GetColor("_FogColor"));
                mpb.SetFloat("_FogDensity", fogMpb.GetFloat("_FogDensity"));

                var loadPos = S.SM.LoadVector3(_idPos);

                if (loadPos.HasValue)
                {
                    transform.position = loadPos ?? transform.position;
                    transform.rotation = S.SM.LoadQuaternion(_idRot) ?? transform.rotation;
                    _type = S.SM.LoadString(_idType);
                }

                InvokeRepeating("SavingMethod", 0f, 5f);

                S.Fog.ApplyToGameObject(gameObject, mpb);
            }
        }
    }

    void GetId()
    {
        //Id must be set by spawner.
        //If not, this is amoeba in scene.

        if (string.IsNullOrWhiteSpace(_id))
            _id = S.ID("AM", gameObject);

        _idPos = S.IDM(_id, "pos");
        _idRot = S.IDM(_id, "rot");
        _idType = S.IDM(_id, "typ");
    }

    void SavingMethod()
    {
        Save();
    }

    void Save()
    {
        S.SM.Save(_idPos, transform.position);
        S.SM.Save(_idRot, transform.rotation);
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

        }
    }
}