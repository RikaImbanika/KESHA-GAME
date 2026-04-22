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
    private string _idWingsType;
    private string _idWingsFrequency;
    private string _idWingsAmplitude;
    private string _sceneName;
    private Vector3 _pos;
    private string _color;
    private float _size;
    private string _wingsType;
    private float _wingsAmplitude;
    private float _wingsFrequency;
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
        _idWingsType = S.ID(_id, "wing");
        _idWingsFrequency = S.ID(_id, "wfrq");
        _idWingsAmplitude = S.ID(_id, "wamp");
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
        _wingsType = S.SM.LoadString(_idWingsType) ?? "";
        _wingsFrequency = S.SM.LoadFloat(_idWingsFrequency) ?? 0;
        _wingsAmplitude = S.SM.LoadFloat(_idWingsAmplitude) ?? 0;
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
            DefineWings();
            Summon();
        }
        
        void DefineWings()
        {
            //Defining doesn't mean summoning

            if (_color == "Bakalavrus" || _color == "Zombella")
            {
                int n = S.RND.Next(2);

                if (n == 0)
                    _wingsType = "crow";
                else
                    _wingsType = "angel";

                S.SM.Save(_idWingsType, _wingsType);

                _wingsAmplitude = 45f;
                _wingsFrequency = 1.79f;

                int g0 = S.RND.Next(3);

                if (g0 == 1)
                    _wingsFrequency = 2.42f;
                else if (g0 == 2)
                    _wingsFrequency = 3.12f;

                int g = S.RND.Next(3);

                if (g == 0)
                    _wingsAmplitude = 55f;
                else if (g == 1)
                    _wingsAmplitude = 35f;

                S.SM.Save(_idWingsAmplitude, _wingsAmplitude);
                S.SM.Save(_idWingsFrequency, _wingsFrequency);
            }
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

                if ((_color == "Zombella" || _color == "Bakalavrus") && sizeN == 0 || sizeN == 1)
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
                if (_sceneName != "BR 5" && _sceneName != "BR 7" && _sceneName != "BR 7R" && _sceneName != "BR 6" && _sceneName != "BR 6R")
                {
                    probs.Add(new("Yellow", 75));
                    probs.Add(new("Red", 2));
                    probs.Add(new("Blue", 1));
                    probs.Add(new("Purple", 1));
                    probs.Add(new("Green", 2));
                    probs.Add(new("RainbowSlow", 2));
                    probs.Add(new("RainbowFast", 1));
                    probs.Add(new("Bakalavrus", 8));
                    probs.Add(new("Zombella", 8));
                }
                else
                {
                    probs.Add(new("Yellow", 1));
                    probs.Add(new("Red", 2));
                    probs.Add(new("Blue", 69));
                    probs.Add(new("Purple", 2));
                    probs.Add(new("Green", 1));
                    probs.Add(new("RainbowSlow", 5));
                    probs.Add(new("RainbowFast", 4));
                    probs.Add(new("Bakalavrus", 8));
                    probs.Add(new("Zombella", 8));
                }
            }
            else if (_sceneName.Contains("MR"))
            {
                probs.Add(new("Yellow", 2));
                probs.Add(new("Red", 1));
                probs.Add(new("Blue", 70));
                probs.Add(new("Purple", 4));
                probs.Add(new("Green", 3));
                probs.Add(new("RainbowSlow", 4));
                probs.Add(new("RainbowFast", 3));
                probs.Add(new("Bakalavrus", 6));
                probs.Add(new("Zombella", 7));
            }
            else if (_sceneName.Contains("Income"))
            {
                probs.Add(new("Yellow", 87));
                probs.Add(new("Red", 1));
                probs.Add(new("Blue", 0));
                probs.Add(new("Purple", 0));
                probs.Add(new("Green", 2));
                probs.Add(new("RainbowSlow", 2));
                probs.Add(new("RainbowFast", 1));
                probs.Add(new("Bakalavrus", 3));
                probs.Add(new("Zombella", 4));
              }
            else if (_sceneName.Contains("TL"))
            {
                probs.Add(new("Yellow", 0));
                probs.Add(new("Red", 1));
                probs.Add(new("Blue", 70));
                probs.Add(new("Purple", 4));
                probs.Add(new("Green", 1));
                probs.Add(new("RainbowSlow", 5));
                probs.Add(new("RainbowFast", 4));
                probs.Add(new("Bakalavrus", 7));
                probs.Add(new("Zombella", 8));
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
        SetWings();

        var ren = GetComponent<MeshRenderer>();

        if (ren != null)
            Destroy(ren);

        Destroy(this);

        void SetWings()
        {
            if (!string.IsNullOrEmpty(_wingsType))
            {
                GameObject _leftWing;
                GameObject _rightWing;

                if (_wingsType == "crow")
                {
                    _leftWing = GameObject.Instantiate(S.CrowWing, lighter._vis.transform.position, lighter._vis.transform.rotation);
                    _rightWing = GameObject.Instantiate(S.CrowWing, lighter._vis.transform.position, lighter._vis.transform.rotation);
                }
                else
                {
                    _leftWing = GameObject.Instantiate(S.AngelWing, lighter._vis.transform.position, lighter._vis.transform.rotation);
                    _rightWing = GameObject.Instantiate(S.AngelWing, lighter._vis.transform.position, lighter._vis.transform.rotation);
                }

                _leftWing.transform.localScale = 0.0008f * lighter._vis.transform.localScale;
                _rightWing.transform.localScale = 0.0008f * new Vector3(lighter._vis.transform.localScale.x, lighter._vis.transform.localScale.y, -lighter._vis.transform.localScale.z);

                _leftWing.transform.position -= 0.5f * lighter._vis.transform.right * _size;
                _rightWing.transform.position += 0.5f * lighter._vis.transform.right * _size;

                _leftWing.transform.position += 0.25f * lighter._vis.transform.up * _size;
                _rightWing.transform.position += 0.25f * lighter._vis.transform.up * _size;

                _leftWing.transform.SetParent(lighter._vis.transform, true);
                _rightWing.transform.SetParent(lighter._vis.transform, true);

                lighter._wingLeft = _leftWing;
                lighter._wingRight = _rightWing;

                lighter._wingAmplitude = _wingsAmplitude;
                lighter._wingFrequency = _wingsFrequency;
            }
        }
        
        void SetSway()
        {
            if (_color == "Zombella" || _color == "Bakalavrus")
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
