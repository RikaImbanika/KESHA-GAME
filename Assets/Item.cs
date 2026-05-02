using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class Item : MonoBehaviour
{
    public string _name;
    public int _count;

    public Item()
    {
        _name = "";
        _count = 0;
        //Should help?
    }

    public void Load(string id)
    {
        _name = S.SM.LoadString(S.ID(id, "name"));
        _count = S.SM.LoadInt(S.ID(id, "count")) ?? 0;
    }

    public void Save(string id)
    {
        S.SM.Save(S.ID(id, "name"), _name);
        S.SM.Save(S.ID(id, "count"), _count);
    }

    public void Throw(Vector3 position, Vector3 direction, float power, Vector3 playerVelocity, Quaternion rotation)
    {
        ItemInfo ii = S.II.Get(_name);
        string prefabName = ii._prefabName;
        GameObject prefab = Prefabs.Get(prefabName);
        Quaternion rot = Quaternion.identity * Quaternion.Euler(ii._throwRotX, ii._throwRotY, ii._throwRotZ);

        if (!S.Loader.Roots.ContainsKey(S.PS._currentSceneName))
            return;

        Transform root = S.Loader.Roots[S.PS._currentSceneName];
        GameObject obj = Instantiate(prefab, position + direction, rot, root);
        ItemP itemP = obj.GetComponent<ItemP>();
        itemP._count = _count;
        itemP._forLoader = true; //Seriously, why there wasn't this stroke for SO LONG???
        //And there wasn't setting of scene... Just all to start...
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
