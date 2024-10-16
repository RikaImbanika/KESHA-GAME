using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using Newtonsoft.Json;

public class Item : MonoBehaviour
{
    public string _id;
    public string _name;
    public int _count;

    AllFather _allFather;

    public void Start()
    {
        GameObject obj = new GameObject("Item");
        this.transform.parent = obj.transform;

        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();

        Save s = new Save();

        if (_allFather.Contains(_id))
        {
            s = _allFather.Load(_id);
            _name = s._name;
            _count = s._count;
        }
        else
        {
            s = new Save();
            s._name = _name;
            s._count = _count;
            _allFather.Save(_id, s);
        }
    }

    public void Load()
    {
        if (_allFather.Contains(_id))
        {
            Save s = new Save();
            s = _allFather.Load(_id);
            _name = s._name;
            _count = s._count;
        }
    }

    public void Save()
    {
        Save s = new Save();
        s._name = _name;
        s._count = _count;
        _allFather.Save(_id, s);
    }

    public void Throw(Vector3 position, Vector3 direction, float power, Vector3 playerVelocity, Quaternion rotation)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{_name}");
        GameObject obj = Instantiate(prefab, position + direction, new Quaternion(0, 0, 0, 0));
        ItemP itemP = obj.GetComponent<ItemP>();
        obj.transform.rotation = rotation * Quaternion.Euler(II.Get(itemP._name)._throwRotX, II.Get(itemP._name)._throwRotY, II.Get(itemP._name)._throwRotZ);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.velocity = playerVelocity;
        rb.AddForce(direction * power);
    }

    public Item Clone()
    {
        Item clone = new Item();
        clone._id = _id;
        clone._name = _name;
        clone._count = _count;
        return clone;
    }

    public override string ToString()
    {
        return $"({_id} | {_name} | {_count})";
    }
}
