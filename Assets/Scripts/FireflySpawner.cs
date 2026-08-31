// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FireflySpawner : MonoBehaviour
{
    public bool _instant;
    private string _id;
    private string _idPos;
    private string _idScale;
    private string _idColor;
    private string _idWingsType;
    private string _idWingsFrequency;
    private string _idWingsAmplitude;
    private string _idMirrored;
    private string _sceneName;
    private Vector3 _pos;
    private string _color;
    private float _size;
    private string _wingsType;
    private float _wingsAmplitude;
    private float _wingsFrequency;
    private bool _mirrored;
    private MeshRenderer _unityEditorMeshRenderer;
    private MeshFilter _unityEditorMeshFilter;


    void Start()
    {
        StartCoroutine(Start0());

        IEnumerator Start0()
        {
            GetId();

            while (!S.Loader.Roots.ContainsKey(_sceneName))
                yield return new WaitForSeconds(0.25f);

            yield return Birn();
        }
    }

    void GetId()
    {
        _sceneName = gameObject.scene.name;
        _id = S.ID("FF", gameObject);
        _idPos = S.IDM(_id, "pos");
        _idColor = S.IDM(_id, "clr");
        _idScale = S.IDM(_id, "scl");
        _idWingsType = S.IDM(_id, "wing");
        _idWingsFrequency = S.IDM(_id, "wfrq");
        _idWingsAmplitude = S.IDM(_id, "wamp");
        _idMirrored = S.IDM(_id, "mirr");
    }

    IEnumerator Birn()
    {
        while (S.SM == null || S.FireflyObj == null)
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
        _size = S.Fireflies._firefliesSizes[sizeN];
        byte colorN = S.SM.LoadByte(_idColor) ?? 104;
        _color = S.Fireflies._firefliesColors[colorN];
        _wingsType = S.SM.LoadString(_idWingsType) ?? "";
        _wingsFrequency = S.SM.LoadFloat(_idWingsFrequency) ?? 0;
        _wingsAmplitude = S.SM.LoadFloat(_idWingsAmplitude) ?? 0;
        byte mirroredByte = S.SM.LoadByte(_idMirrored) ?? 0;
        _mirrored = mirroredByte == 1;
    }

    void DefineExistenz()
    {
        float prob = 0;
        if (_instant)
            prob = 100;
        else if (_sceneName.Contains("BR"))
            prob = S.Backrooms._firefliesProbabilities[_sceneName];
        else if (_sceneName.Contains("Income"))
            prob = 40;
        else if (_sceneName.Contains("TL"))
            prob = 30;
        else if (_sceneName.Contains("MR"))
            prob = prob = S.Mushrooms._firefliesProbabilities[_sceneName];

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
            DefineMirroring();
            Summon();
        }

        void DefineMirroring()
        {
            if (S.Fireflies.IsZombieFirefly(_color))
                if (S.Fireflies._canMirror[_color])
                {
                    _mirrored = S.RND.Next(2) == 0;
                }
            S.SM.Save(_idMirrored, (byte)(_mirrored ? 1 : 0));
        }

        void DefineWings()
        {
            //Defining doesn't mean summoning

            if (S.Fireflies.IsZombieFirefly(_color))
            {
                int n = S.RND.Next(7);

                if (n == 0)
                    _wingsType = "crow";
                else if (n == 1)
                    _wingsType = "angel";
                else if (n == 2)
                    _wingsType = "bat";
                else if (n == 3)
                    _wingsType = "rainbow1";
                else if (n == 4)
                    _wingsType = "rainbow2";
                else if (n == 5)
                    _wingsType = "ice1";
                else if (n == 6)
                    _wingsType = "ice2";

                S.SM.Save(_idWingsType, _wingsType);

                _wingsAmplitude = 48f;
                _wingsFrequency = 2.62f;

                int g0 = S.RND.Next(3);

                if (g0 == 1)
                    _wingsFrequency = 2.94f;
                else if (g0 == 2)
                    _wingsFrequency = 3.23f;

                int g = S.RND.Next(3);

                if (g == 0)
                    _wingsAmplitude = 57f;
                else if (g == 1)
                    _wingsAmplitude = 40f;

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
                else
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

                if (S.Fireflies.IsZombieFirefly(_color)
                    && sizeN == 0 || sizeN == 1)
                    continue;
                else
                    break;
            }

            _size = S.Fireflies._firefliesSizes[sizeN];
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
                    probs.Add(new("Yellow", 750));
                    probs.Add(new("Red", 20));
                    probs.Add(new("Blue", 10));
                    probs.Add(new("Purple", 10));
                    probs.Add(new("Green", 20));
                    probs.Add(new("RainbowSlow", 20));
                    probs.Add(new("RainbowFast", 10));
                    probs.Add(new("Baka", 60));
                    probs.Add(new("Zombella", 60));
                    probs.Add(new("FatBaka", 30));
                    probs.Add(new("FatZombella", 30));
                    probs.Add(new("Illuminaty", 25));
                    probs.Add(new("Kuplinov1", 30));
                    probs.Add(new("Kuplinov2", 30));
                    probs.Add(new("Boykisser1", 15));
                    probs.Add(new("Boykisser2", 15));
                }
                else
                {
                    probs.Add(new("Yellow", 10));
                    probs.Add(new("Red", 20));
                    probs.Add(new("Blue", 690));
                    probs.Add(new("Purple", 20));
                    probs.Add(new("Green", 10));
                    probs.Add(new("RainbowSlow", 50));
                    probs.Add(new("RainbowFast", 40));
                    probs.Add(new("Baka", 60));
                    probs.Add(new("Zombella", 60));
                    probs.Add(new("FatBaka", 20));
                    probs.Add(new("FatZombella", 20));
                    probs.Add(new("Illuminaty", 14));
                    probs.Add(new("Kuplinov1", 30));
                    probs.Add(new("Kuplinov2", 30));
                    probs.Add(new("Boykisser1", 15));
                    probs.Add(new("Boykisser2", 15));
                }
            }
            else if (_sceneName.Contains("MR"))
            {
                probs.Add(new("Yellow", 20));
                probs.Add(new("Red", 10));
                probs.Add(new("Blue", 700));
                probs.Add(new("Purple", 40));
                probs.Add(new("Green", 30));
                probs.Add(new("RainbowSlow", 40));
                probs.Add(new("RainbowFast", 30));
                probs.Add(new("Baka", 40));
                probs.Add(new("Zombella", 40));
                probs.Add(new("FatBaka", 20));
                probs.Add(new("FatZombella", 30));
                probs.Add(new("Illuminaty", 20));
                probs.Add(new("Kuplinov1", 30));
                probs.Add(new("Kuplinov2", 30));
                probs.Add(new("Boykisser1", 20));
                probs.Add(new("Boykisser2", 20));
            }
            else if (_sceneName.Contains("Income"))
            {
                probs.Add(new("Yellow", 870));
                probs.Add(new("Red", 10));
                probs.Add(new("Blue", 0));
                probs.Add(new("Purple", 0));
                probs.Add(new("Green", 20));
                probs.Add(new("RainbowSlow", 20));
                probs.Add(new("RainbowFast", 10));
                probs.Add(new("Baka", 30));
                probs.Add(new("Zombella", 40));
                probs.Add(new("FatBaka", 2));
                probs.Add(new("FatZombella", 2));
                probs.Add(new("Illuminaty", 2));
                probs.Add(new("Kuplinov1", 1));
                probs.Add(new("Kuplinov2", 1));
                probs.Add(new("Boykisser1", 1));
                probs.Add(new("Boykisser2", 1));
            }
            else if (_sceneName.Contains("TL"))
            {
                probs.Add(new("Yellow", 0));
                probs.Add(new("Red", 10));
                probs.Add(new("Blue", 700));
                probs.Add(new("Purple", 40));
                probs.Add(new("Green", 10));
                probs.Add(new("RainbowSlow", 50));
                probs.Add(new("RainbowFast", 40));
                probs.Add(new("Baka", 50));
                probs.Add(new("Zombella", 50));
                probs.Add(new("FatBaka", 20));
                probs.Add(new("FatZombella", 30));
                probs.Add(new("Illuminaty", 20));
                probs.Add(new("Kuplinov1", 30));
                probs.Add(new("Kuplinov2", 30));
                probs.Add(new("Boykisser1", 20));
                probs.Add(new("Boykisser2", 20));
            }
            else
            {
                probs.Add(new("Yellow", 100));
                probs.Add(new("Red", 100));
                probs.Add(new("Blue", 100)); //300
                probs.Add(new("Purple", 100));
                probs.Add(new("Green", 80));
                probs.Add(new("RainbowSlow", 100));
                probs.Add(new("RainbowFast", 100));
                probs.Add(new("Baka", 80));
                probs.Add(new("Zombella", 80));
                probs.Add(new("FatBaka", 80));
                probs.Add(new("FatZombella", 80));
                probs.Add(new("Illuminaty", 40));
                probs.Add(new("Kuplinov1", 30));
                probs.Add(new("Kuplinov2", 30));
                probs.Add(new("Boykisser1", 20));
                probs.Add(new("Boykisser2", 20));
            }
            colorN = S.Fireflies.ColorN[S.AllFather.SelFromProb(probs)];

            _color = S.Fireflies._firefliesColors[colorN];
            S.SM.Save(_idColor, colorN);
        }
    }

    void NotExists()
    {
        Destroy(gameObject);
    }

    void Summon()
    {
        GameObject obj = Instantiate(S.FireflyObj, _pos, transform.rotation, transform);
        Firefly firefly = obj.GetComponent<Firefly>();

        SetSize();
        SetColor();
        SetPosition();
        SetId();
        SetSway();
        SetWings();
        SetFog();
        SetMirroring();

        var ren = GetComponent<MeshRenderer>();

        if (ren != null)
            Destroy(ren);

        Destroy(this);

        void SetFog()
        {
            MaterialPropertyBlock mpt = S.Fog.GetMPB(_sceneName);
            S.Fog.ApplyToGameObject(obj, mpt);
        }

        void SetWings()
        {
            if (!string.IsNullOrEmpty(_wingsType))
            {
                GameObject _leftWing;
                GameObject _rightWing;

                _leftWing = GameObject.Instantiate(S.Wings[_wingsType], firefly._vis.transform.position, firefly._vis.transform.rotation);
                _rightWing = GameObject.Instantiate(S.Wings[_wingsType], firefly._vis.transform.position, firefly._vis.transform.rotation);

                _leftWing.transform.localScale = 0.0008f * firefly._vis.transform.localScale;
                _rightWing.transform.localScale = 0.0008f * new Vector3(firefly._vis.transform.localScale.x, firefly._vis.transform.localScale.y, -firefly._vis.transform.localScale.z);

                _leftWing.transform.position -= 0.5f * firefly._vis.transform.right * _size;
                _rightWing.transform.position += 0.5f * firefly._vis.transform.right * _size;

                _leftWing.transform.position += 0.25f * firefly._vis.transform.up * _size;
                _rightWing.transform.position += 0.25f * firefly._vis.transform.up * _size;

                _leftWing.transform.SetParent(firefly._vis.transform, true);
                _rightWing.transform.SetParent(firefly._vis.transform, true);

                firefly._wingLeft = _leftWing;
                firefly._wingRight = _rightWing;

                firefly._wingAmplitude = _wingsAmplitude;
                firefly._wingFrequency = _wingsFrequency;
            }
        }

        void SetSway()
        {
            if (S.Fireflies.IsZombieFirefly(_color))
            {
                firefly._swayAmplitude = 35f;
                firefly._swayFrequency = 1f;

                if (S.RND.Next(3) == 0)
                    firefly._swayFrequency = 2f;

                int g = S.RND.Next(5);

                if (g == 0)
                    firefly._swayAmplitude = 45f;
                else if (g == 1)
                    firefly._swayAmplitude = 25f;
            }
        }

        void SetPosition()
        {
            obj.transform.SetParent(transform);
            obj.transform.position = _pos;
        }

        void SetId()
        {
            firefly._id = _id;
            firefly._idPos = _idPos;
            firefly._sceneName = _sceneName;
        }

        void SetColor()
        {
            if (_color != "Yellow")
            {
                MeshRenderer renderer = firefly._vis.GetComponent<MeshRenderer>();

                renderer.sharedMaterial = S.Fireflies._materials[_color];
            }
        }

        void SetSize()
        {
            if (_size != 1)
            {
                var sc = firefly._vis.transform.localScale;
                firefly._vis.transform.localScale = new Vector3(sc.x * _size, sc.y * _size, sc.z * _size);
            }
        }

        void SetMirroring()
        {
            if (_mirrored)
            {
                MeshRenderer renderer = firefly._vis.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    Material originalMat = renderer.sharedMaterial;
                    if (originalMat != null)
                    {
                        Material mirroredMat = new Material(originalMat);
                        mirroredMat.SetTextureScale("_MainTex", new Vector2(-1f, 1f));
                        mirroredMat.SetTextureOffset("_MainTex", new Vector2(1f, 0f));
                        renderer.material = mirroredMat;
                    }
                }
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