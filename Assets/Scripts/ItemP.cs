using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemP : MonoBehaviour
{
    public string _id;
    private string _idPos;
    private string _idRot;
    private string _idCount;
    private string _idName;
    private string _idScene;
    private string _idLocked;
    private string _idDestroyed;
    public string _name;
    public int _count = 1;
    public bool _forLoader;
    //If item was in scene, scene will load it
    //If item wasn't in scene it should be loaded by Loader
    public bool _locked; //Means untakeable
    public string _sceneName;

    void Start()
    {
        _sceneName = gameObject.scene.name;
        StartCoroutine(Start0());

        IEnumerator Start0()
        {
            while (S.SM == null)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log("ItemP waiting for S.SaveManager");
            }

            GetId();
            Load();

            StartCoroutine(Saver(UnityEngine.Random.Range(5f, 6f)));
        }
    }

    void Load()
    {
        Vector3 position = S.SM.LoadVector3(S.IDM(_id, "pos")) ?? transform.position;
        Quaternion rotation = S.SM.LoadQuaternion(S.IDM(_id, "rot")) ?? transform.rotation;

        if (_forLoader)
        {
            transform.position = position;
            transform.rotation = rotation;
            //Better leave them here or in Loader?
            _count = S.SM.LoadInt(S.IDM(_id, "count")) ?? _count;
            _name = S.SM.LoadString(S.IDM(_id, "name")) ?? _name;
        }
        else
        {
            if (S.SM.LoadBool(_idDestroyed) ?? false)
                Destroy();
            else if (!string.IsNullOrEmpty(_name))
            {
                transform.position = position;
                transform.rotation = rotation;
                _locked = S.SM.LoadBool(_idLocked) ?? false;
            }
        }
    }

    void GetId()
    {
        bool hasNoId = string.IsNullOrWhiteSpace(_id);

        if (hasNoId)
        {
            if (!_forLoader)
                _id = S.ID("IT", gameObject);
            else
                _id = S.ID("IT");
        }

        _idName = S.IDM(_id, "name");
        _idPos = S.IDM(_id, "pos");
        _idRot = S.IDM(_id, "rot");
        _idCount = S.IDM(_id, "count");
        _idLocked = S.IDM(_id, "lckd");
        _idDestroyed = S.IDM(_id, "destroyed");
        _idScene = S.IDM(_id, "sc"); //Why?
    }

    IEnumerator Saver(float t)
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            if (transform.position.y < -700)
                Destroy();
            Save();
            yield return new WaitForSeconds(t);
        }
    }

    public void ToggleLock(bool locked)
    {
        _locked = locked;
    }

    public void Save()
    {
        S.SM.Save(_idName, _name);
        S.SM.Save(_idCount, _count);
        S.SM.Save(_idPos, transform.position);
        S.SM.Save(_idRot, transform.rotation);
        S.SM.Save(_idScene, _sceneName); //Why?
        S.SM.Save(_idLocked, _locked);

        if (_forLoader)
            S.SM.AddToList(S.IDM(_sceneName, "ids"), _id); //Seriously...
    }

    public void Destroy()
    {
        S.SM.RemoveString(_idName);
        S.SM.RemoveInt(_idCount);
        S.SM.RemoveVector3(_idPos);
        S.SM.RemoveQuaternion(_idRot);
        S.SM.RemoveString(_idScene);
        S.SM.RemoveBool(_idLocked);

        if (_forLoader)
            S.SM.RemoveFromList(S.IDM(_sceneName, "ids"), _id);
        else
            S.SM.Save(_idDestroyed, true);

        Debug.Log($"Destroyed ItemP {_name}.");
        
        Destroy(gameObject);
    }
}