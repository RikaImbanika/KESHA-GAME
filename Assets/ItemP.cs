using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemP : MonoBehaviour
{
    public string _id;
    public string _name;
    public int _count = 1;
    public bool _unnatural;
    public bool _locked;

    string _sceneName;
    public GameObject _obj;

    void Start()
    {
        _obj = gameObject;
        _sceneName = gameObject.scene.name;
        StartCoroutine(Start0());

        IEnumerator Start0()
        {
            while (S.SM == null)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log("ItemP waiting for S.SaveManager");
            }

            bool hasNoId = string.IsNullOrWhiteSpace(_id);
            Vector3 position = transform.position;

            if (!hasNoId)
            {
                position = S.SM.LoadVector3(S.ID(_id, "position")) ?? transform.position;
                hasNoId = position == transform.position;
            }

            if (hasNoId)
                _id = $"{transform.position.x}{transform.position.y}{transform.position.z}";

            _name = S.SM.LoadString(S.ID(_id, "name")) ?? _name;

            Quaternion rotation = S.SM.LoadQuaternion(S.ID(_id, "rotation")) ?? transform.rotation;

            _unnatural = S.SM.LoadBool(S.ID(_id, "unnatural")) ?? _unnatural;
            ////Need or not?

            if (_unnatural)
            {
                transform.position = position;
                transform.rotation = rotation;
                _count = S.SM.LoadInt(S.ID(_id, "count")) ?? _count;
            }
            else
            {
                if (S.SM.LoadBool(S.ID(_id, "destroyed")) ?? false)
                    Destroy();
                else if (!string.IsNullOrEmpty(_name))
                {
                    transform.position = position;
                    transform.rotation = rotation;
                }
            }
            StartCoroutine(Saver(UnityEngine.Random.Range(9f, 11f)));
        }
    }

    IEnumerator Saver(float t)
    {
        while (true)
        {
            yield return new WaitForSeconds(t);
            if (transform.position.y < -700)
                Destroy();
            Save();
        }
    }

    public void ToggleLock(bool locked)
    {
        _locked = locked;
    }

    public void Save()
    {
        S.SM.Save(S.ID(_id, "name"), _name);
        S.SM.Save(S.ID(_id, "count"), _count);
        S.SM.Save(S.ID(_id, "position"), transform.position);
        S.SM.Save(S.ID(_id, "rotation"), transform.rotation);
        S.SM.Save(S.ID(_id, "scene"), _sceneName);
        S.SM.Save(S.ID(_id, "unnatural"), _unnatural);
        S.SM.Save(S.ID(_id, "locked"), _locked);

        S.SM.AddToList(S.ID(_sceneName, "ids"), _id);
    }

    public void Destroy()
    {
        Debug.Log($"Destroyed {_name}");
        if (_unnatural)
        {
            S.SM.RemoveString(S.ID(_id, "name"));
            S.SM.RemoveInt(S.ID(_id, "count"));
            S.SM.RemoveVector3(S.ID(_id, "position"));
            S.SM.RemoveQuaternion(S.ID(_id, "rotation"));
            S.SM.RemoveString(S.ID(_id, "scene"));
            S.SM.RemoveBool(S.ID(_id, "unnatural"));
            S.SM.RemoveBool(S.ID(_id, "locked"));

            S.SM.RemoveFromList(S.ID(_sceneName, "ids"), _id);
        }
        else
        {
            S.SM.RemoveString(S.ID(_id, "name"));
            S.SM.RemoveInt(S.ID(_id, "count"));
            S.SM.RemoveVector3(S.ID(_id, "position"));
            S.SM.RemoveQuaternion(S.ID(_id, "rotation"));
            S.SM.RemoveString(S.ID(_id, "scene"));
            S.SM.RemoveBool(S.ID(_id, "unnatural"));
            S.SM.RemoveBool(S.ID(_id, "locked"));

            S.SM.Save(S.ID(_id, "destroyed"), true);
        }
        Destroy(gameObject);
    }
}