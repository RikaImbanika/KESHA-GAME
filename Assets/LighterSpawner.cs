using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LighterSpawner : MonoBehaviour
{
    private string _id;
    private string _idPos;
    private string _sceneName;
    private Vector3 _pos;
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
            Summon();
    }

    void DefineExistenz()
    {
        float prob = S.Backrooms._lightersProbabilities[_sceneName];
        int n = S.RND.Next(100);
        if (n > prob)
        {
            S.SM.Save(_idPos, Vector3.down);
            NotExists();
        }
        else
        {
            _pos = transform.position;
            S.SM.Save(_idPos, _pos);
            Summon();
        }
    }

    void NotExists()
    {
        Destroy(gameObject);
    }

    void Summon()
    {
        Instantiate(S.LighterObj, _pos, transform.rotation, transform.parent);
        Destroy(gameObject);
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
