using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LighterSpawner : MonoBehaviour
{
    private string _id;
    private string _idPos;
    private string _idScale;
    private string _idColor;
    private string _sceneName;
    private Vector3 _pos;
    private string _color;
    private float _size;
    private MeshRenderer _unityEditorMeshRenderer;
    private MeshFilter _unityEditorMeshFilter;


    void Start()
    {
        StartCoroutine(Start0());

        IEnumerator Start0()
        {
            GetId();
            yield return Birn();
        }
    }

    void GetId()
    {
        _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
        _id = _sceneName + transform.position.x + transform.position.y + transform.position.z;
        _idPos = S.ID(_id, "pos");
        _idColor = S.ID(_id, "clr");
        _idScale = S.ID(_id, "scl");
    }

    IEnumerator Birn()
    {
        while (S.SM == null || S.LighterObj == null)
            yield return new WaitForSeconds(0.2f);

        _pos = S.SM.LoadVector3(_idPos) ?? Vector3.zero;

        if (_pos.Equals(Vector3.zero))
            DefineExistenz();
        else if (_pos.Equals(Vector3.down))
            NotExists();
        else
        {
            Load();
            Summon();
        }
    }

    void Load()
    {
        byte sizeN = S.SM.LoadByte(_idScale) ?? 104;
        _size = S.Lighters._lightersSizes[sizeN];
        byte colorN = S.SM.LoadByte(_idColor) ?? 104;
        _color = S.Lighters._lightersColors[colorN];
    }

    void DefineExistenz()
    {
        float prob = 0;
        if (_sceneName.Contains("BR"))
            prob = S.Backrooms._lightersProbabilities[_sceneName];
        else if (_sceneName.Contains("Income"))
            prob = 40;
        else if (_sceneName.Contains("TL"))
            prob = 30;
        else if (_sceneName.Contains("MR"))
            prob = prob = S.Mushrooms._lightersProbabilities[_sceneName];

        int n = S.RND.Next(100);
        if (n > prob)
        {
            S.SM.Save(_idPos, Vector3.down);
            NotExists();
        }
        else
        {
            DefinePos();
            DefineColor();
            DefineSize();          
            Summon();
        }

        void DefinePos()
        {
            _pos = transform.position;
            S.SM.Save(_idPos, _pos);
        }

        void DefineSize()
        {
            byte sizeN = 0;

            while (true)
            {
                int n = S.RND.Next(100);
            
                if (_sceneName.Contains("BR"))
                {
                    if (n < 35)
                        sizeN = 0;
                    else if (n < 60)
                        sizeN = 1;
                    else if (n < 85)
                        sizeN = 2;
                    else
                        sizeN = 3;
                }
                else if (_sceneName.Contains("MR"))
                {
                    if (n < 10)
                        sizeN = 0;
                    else if (n < 25)
                        sizeN = 1;
                    else if (n < 50)
                        sizeN = 2;
                    else
                        sizeN = 3;
                }
                else if (_sceneName.Contains("TL"))
                {
                    if (n < 8)
                        sizeN = 0;
                    else if (n < 28)
                        sizeN = 1;
                    else if (n < 55)
                        sizeN = 2;
                    else
                        sizeN = 3;
                }
                else if (_sceneName.Contains("Income"))
                {
                    if (n < 12)
                        sizeN = 0;
                    else if (n < 25)
                        sizeN = 1;
                    else if (n < 55)
                        sizeN = 2;
                    else
                        sizeN = 3;
                }

                if ((_color == "Zombella" || _color == "Bakalavr") && sizeN == 0)
                    continue;
                else
                    break;
            }

            _size = S.Lighters._lightersSizes[sizeN];
            S.SM.Save(_idScale, sizeN);
        }
        
        void DefineColor()
        {
            int n = S.RND.Next(100);

            byte colorN = 0;

            List<(string, int)> probs = new List<(string, int)>();

            if (_sceneName.Contains("BR"))
            {
                if (_sceneName != "BR 7" && _sceneName != "BR 7R" && _sceneName != "BR 6" && _sceneName != "BR 6R")
                {
                    probs.Add(new("Yellow", 82));
                    probs.Add(new("Red", 2));
                    probs.Add(new("Blue", 1));
                    probs.Add(new("Purple", 1));
                    probs.Add(new("Green", 2));
                    probs.Add(new("Bakalavr", 6));
                    probs.Add(new("Zombella", 6));
                }
                else
                {
                    probs.Add(new("Yellow", 1));
                    probs.Add(new("Red", 2));
                    probs.Add(new("Blue", 80));
                    probs.Add(new("Purple", 2));
                    probs.Add(new("Green", 1));
                    probs.Add(new("Bakalavr", 7));
                    probs.Add(new("Zombella", 7));
                }
            }
            else if (_sceneName.Contains("MR"))
            {
                probs.Add(new("Yellow", 2));
                probs.Add(new("Red", 1));
                probs.Add(new("Blue", 77));
                probs.Add(new("Purple", 5));
                probs.Add(new("Green", 3));
                probs.Add(new("Bakalavr", 6));
                probs.Add(new("Zombella", 6));
            }
            else if (_sceneName.Contains("Income"))
            {
                probs.Add(new("Yellow", 91));
                probs.Add(new("Red", 1));
                probs.Add(new("Blue", 0));
                probs.Add(new("Purple", 0));
                probs.Add(new("Green", 2));
                probs.Add(new("Bakalavr", 3));
                probs.Add(new("Zombella", 3));
              }
            else if (_sceneName.Contains("TL"))
            {
                probs.Add(new("Yellow", 0));
                probs.Add(new("Red", 1));
                probs.Add(new("Blue", 82));
                probs.Add(new("Purple", 4));
                probs.Add(new("Green", 1));
                probs.Add(new("Bakalavr", 6));
                probs.Add(new("Zombella", 6));
            }
            colorN = S.Lighters.ColorN[S.AllFather.SelFromProb(probs)];

            _color = S.Lighters._lightersColors[colorN];
            S.SM.Save(_idColor, colorN);
        }
    }

    void NotExists()
    {
        Destroy(gameObject);
    }

    void Summon()
    {
        GameObject obj = Instantiate(S.LighterObj, _pos, transform.rotation, transform);
        Lighter lighter = obj.GetComponent<Lighter>();

        SetSize();
        SetColor();
        SetPosition();
        SetId();
        SetSway();

        var ren = GetComponent<MeshRenderer>();

        if (ren != null)
            Destroy(ren);

        Destroy(this);
        
        void SetSway()
        {
            if (_color == "Zombella" || _color == "Bakalavr")
            {
                lighter._swayAmplitude = 35f;
                lighter._swayFrequency = 1f;

                if (S.RND.Next(3) == 0)
                    lighter._swayFrequency = 2f;

                int g = S.RND.Next(5);

                if (g == 0)
                    lighter._swayAmplitude = 45f;
                else if (g == 1)
                    lighter._swayAmplitude = 25f;
            }
        }

        void SetPosition()
        {
            obj.transform.SetParent(transform);
            obj.transform.position = _pos;
        }

        void SetId()
        {
            lighter._id = _id;
            lighter._idPos = _idPos;
            lighter._sceneName = _sceneName;
        }

        void SetColor()
        {
            if (_color != "Yellow")
            {
                MeshRenderer renderer = lighter._vis.GetComponent<MeshRenderer>();

                renderer.sharedMaterial = S.Lighters._materials[_color];
            }
        }

        void SetSize()
        {
            if (_size != 1)
            {
                var sc = lighter._vis.transform.localScale;
                lighter._vis.transform.localScale = new Vector3(sc.x * _size, sc.y * _size, sc.z * _size);
            }
        }
    }

#if UNITY_EDITOR
    private void EnsureMeshFilter()
    {
        if (GetComponent<MeshRenderer>() == null)
        {
            _unityEditorMeshRenderer = gameObject.AddComponent<MeshRenderer>();
            _unityEditorMeshRenderer.sharedMaterial = Materials.GetInEditor("DUMMY_YELLOW");
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

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
    #endif
}
