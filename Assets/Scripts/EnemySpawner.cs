// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    private string _sceneName;
    private string _id;
    private string _idType;
    private string _type;
    private MeshRenderer _unityEditorMeshRenderer;
    private MeshFilter _unityEditorMeshFilter;
    private Transform _root;

    void Start()
    {
        StartCoroutine(Start0());

        IEnumerator Start0()
        {
            GetId();
            yield return Birn();
        }
    }

    void Load()
    {
        byte typeN = S.SM.LoadByte(_idType) ?? 254;
        if (typeN == 254)
            _type = "new";
        else if (typeN == 255)
            _type = "none";
        else
            _type = S.Enemies.Types[typeN];
    }

    void GetId()
    {
        _sceneName = gameObject.scene.name;
        _id = S.ID("En", gameObject);
        _idType = S.IDM(_id, "typ");
    }

    IEnumerator Birn()
    {
        while (S.SM == null || S.FireflyObj == null)
            yield return new WaitForSeconds(0.2f);

        while (!S.Loader.Roots.ContainsKey(_sceneName) ||
            S.Loader.Roots[_sceneName] == null)
            yield return new WaitForSeconds(0.1f);

        _root = S.Loader.Roots[_sceneName];

        Load();

        if (_type.Equals("none"))
            NotExists();
        else if (_type == "new")
            DefineExistenz();
        else
            Summon();
    }

    void DefineExistenz()
    {
        float prob = 0;
        if (_sceneName.Contains("BR"))
            prob = 30; //HOLY COW THERE ARE TOO MUCH OF THEM WITH 100... Maybe
        else if (_sceneName.Contains("Income"))
            prob = 100;
        else if (_sceneName.Contains("TL"))
            prob = 100;
        else if (_sceneName.Contains("MR"))
            prob = 100;

        int n = S.RND.Next(100);
        if (n > prob)
        {
            byte b = 255;
            S.SM.Save(_idType, b);
            NotExists();
        }
        else
        {
            DefineType();
            Summon();
        }

        void DefineType()
        {
            int n = S.RND.Next(100);

            byte typeN = 0;

            if (true)
            {
                if (true)
                {
                    if (n < 25)
                        typeN = S.Enemies.TypeN("Zombella");
                    else if (n <= 50)
                        typeN = S.Enemies.TypeN("Baka");
                    else if (n <= 68)
                        typeN = S.Enemies.TypeN("Musculus");
                    else if (n <= 90)
                        typeN = S.Enemies.TypeN("Ghost");
                    else
                        typeN = S.Enemies.TypeN("Spider");
                }
            }

            _type = S.Enemies.Types[typeN];
            S.SM.Save(_idType, typeN);
        }
    }

    void NotExists()
    {
        GameObject obj = Instantiate(S.UnworkedSpawner, transform.position, transform.rotation, _root);

        MaterialPropertyBlock mpt = S.Fog.GetMPB(_sceneName);
        S.Fog.ApplyToGameObject(obj, mpt);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 20f))
            obj.transform.position = hit.point;

        Destroy(gameObject);
    }

    void Summon()
    {
        GameObject obj = null;

        if (_type == "Zombella")
            obj = Instantiate(S.Zombella, transform.position, transform.rotation, _root);
        else if (_type == "Baka")
            obj = Instantiate(S.Baka, transform.position, transform.rotation, _root);
        else if (_type == "Musculus")
            obj = Instantiate(S.Musculus, transform.position, transform.rotation, _root);
        else if (_type == "Ghost")
            obj = Instantiate(S.Ghost, transform.position, transform.rotation, _root);
        else if (_type == "Spider")
            obj = Instantiate(S.Spider, transform.position, transform.rotation, _root);
        else
        {
            S.Console.AddMessage($"Cannot spawn \"_type\" - no such type!", Color.red);
            Destroy(gameObject);
        }

        MaterialPropertyBlock mpb = S.Fog.GetMPB(_sceneName);
        S.Fog.ApplyToGameObject(obj, mpb);
        //Duplication but lets it be so

        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void EnsureMeshFilter()
    {
        if (GetComponent<MeshRenderer>() == null)
        {
            _unityEditorMeshRenderer = gameObject.AddComponent<MeshRenderer>();
            _unityEditorMeshRenderer.sharedMaterial = Materials.GetInEditor("DUMMY");
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

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 3f);
    }
#endif
}
