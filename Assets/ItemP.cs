using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemP : MonoBehaviour
{
    public string _id;
    public string _name;
    public int _count;
    public bool _unnatural;
    public bool _destroyed;
    public bool _locked;

    AllFather _allFather;
    string _sceneName;
    public GameObject _obj;

    void Start()
    {
        _obj = gameObject;

        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();
        _sceneName = gameObject.scene.name;
        Save s = _allFather.Load(_id);

        bool _new = (s._position == Vector3.zero);

        if (_new)
            _id = "" + transform.position.x + transform.position.y + transform.position.z;
        else if (_unnatural)
        {
            //MEANS IT WAS LOADED
            //AFTER CREATING BY PLAYER
            transform.position = s._position;
            transform.rotation = s._rotation;
            _count = s._count;
            _name = s._name;
        }
        else
        {
            //MEANS IT WAS LOADED
            //AFTER CREATING BY SCENE
            //MY LOADER SHOULD NOT LOAD NATURAL
            //OBJECTS BECOUSE SCENE WILL DO IT
            //SO IT WAS CREATED BY SCENE
            if (s._destroyed)
                Destroy();
            transform.position = s._position;
            transform.rotation = s._rotation;
        }
        StartCoroutine(Saver(UnityEngine.Random.Range(9f, 11f)));
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
        Save s = new Save();
        s._name = _name;
        s._count = _count;
        s._position = transform.position;
        s._rotation = transform.rotation;
        s._scene = _sceneName;
        s._unnatural = _unnatural;
        s._locked = _locked;
        _allFather.Save(_id, s);

        Save s2 = _allFather.Load(_sceneName);
        if (s2._strings == null)
            s2._strings = new List<string>();
        if (!s2._strings.Contains(_id))
            s2._strings.Add(_id);
        _allFather.Save(_sceneName, s2);
    }

    public void Destroy()
    {
        Debug.Log($"Destroyed {_name}");
        if (_unnatural)
            _allFather.Destroy(_id);
        else
        {
            Save s = new Save();
            s._destroyed = true;
            _allFather.Save(_id, s);
        }
        Destroy(gameObject);
    }

    public Item ToItem(int id)
    {
        return ToItem(id.ToString());
    }

    public Item ToItem(string id)
    {
        Item res = new GameObject(_name).AddComponent<Item>();
        res._name = _name;
        res._count = _count;
        res._id = id;
        return res;
    }
}