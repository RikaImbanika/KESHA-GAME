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
        while (S.SM == null)
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
        _size = S.Backrooms._lightersSizes[sizeN];
        byte colorN = S.SM.LoadByte(_idColor) ?? 104;
        _color = S.Backrooms._lightersColors[colorN];
    }

    void DefineExistenz()
    {
        //Debug.LogError($"L #1");
        float prob = S.Backrooms._lightersProbabilities[_sceneName];
        int n = S.RND.Next(100);
        if (n > prob)
        {
            S.SM.Save(_idPos, Vector3.down);
            NotExists();
        }
        else
        {
            DefinePos();
            DefineSize();
            DefineColor();
            Summon();
        }

        void DefinePos()
        {
            _pos = transform.position;
            S.SM.Save(_idPos, _pos);
        }

        void DefineSize()
        {
            int n = S.RND.Next(100);

            byte sizeN = 0;

            if (n < 35)
                sizeN = 0;
            else if (n < 60)
                sizeN = 1;
            else if (n < 85)
                sizeN = 2;
            else
                sizeN = 3;

            _size = S.Backrooms._lightersSizes[sizeN];
            S.SM.Save(_idScale, sizeN);
        }
        
        void DefineColor()
        {
            int n = S.RND.Next(100);

            byte colorN = 0;

            if (n < 80)
                colorN = 0;
            else if (n <= 90) /////////
                colorN = 1;
            else
                colorN = 2;

            _color = S.Backrooms._lightersColors[colorN];
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
        Debug.LogError("L #4");
        SetSize();
        SetColor();
        SetPosition();
        SetId();

        var ren = GetComponent<MeshRenderer>();

        if (ren != null)
            Destroy(ren);
            
        Destroy(this);

        void SetPosition()
        {
            obj.transform.SetParent(transform);
            obj.transform.position = _pos;
        }

        void SetId()
        {
            lighter._id = _id;
            lighter._idPos = _idPos;
        }

        void SetColor()
        {
            if (_color != "Yellow")
            {
                MeshRenderer renderer = lighter._vis.GetComponent<MeshRenderer>();
                renderer.sharedMaterial = Materials.Get($"Sparkles/Sparkle{_color}");
                Debug.LogError("L #5");
            }
        }

        void SetSize()
        {
            if (_size != 1)
            {
                var sc = lighter._vis.transform.localScale;
                lighter._vis.transform.localScale = new Vector3(sc.x * _size, sc.y * _size, sc.z * _size);
                Debug.LogError("L #6");
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
