using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class Item : MonoBehaviour
{
    public string _name;
    public int _count;

    public void Load(string id)
    {
        if (S.AllFather.Contains(id))
        {
            Save s = new Save();
            s = S.AllFather.Load(id);
            _name = s._name;
            _count = s._count;
        }
    }

    public void Save(string id)
    {
        Save s = new Save();
        s._name = _name;
        s._count = _count;
        S.AllFather.Save(id, s);
    }

    public void Throw(Vector3 position, Vector3 direction, float power, Vector3 playerVelocity, Quaternion rotation)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{_name}");
        GameObject obj = Instantiate(prefab, position + direction, new Quaternion(0, 0, 0, 0));
        ItemP itemP = obj.GetComponent<ItemP>();
        itemP._count = _count;
        obj.transform.rotation = rotation * Quaternion.Euler(S.II.Get(itemP._name)._throwRotX, S.II.Get(itemP._name)._throwRotY, S.II.Get(itemP._name)._throwRotZ);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.velocity = playerVelocity;
        rb.AddForce(direction * power);
    }

    public void CloneFrom(Item from)
    {
        if (string.IsNullOrEmpty(from._name))
            from._name = "";
        _name = from._name;
        _count = from._count;
    }

    public override string ToString()
    {
        return $"({_name} | {_count})";
    }
}
