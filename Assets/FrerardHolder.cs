using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrerardHolder : MonoBehaviour
{
    string _id;
    public int _number;
    public string _waitItem; 
    public Frerard _frerard;
    ItemP _placedItem;
    int _currentRotation;
    int _rotations;

    void Start()
    {
        _id = $"FrerHolder {_number}";

        string name = S.SM.LoadString(S.ID(_id, "name"));

        if (!string.IsNullOrEmpty(name))
        {
            GameObject prefab = Prefabs.Get(name);
            GameObject obj = Instantiate(prefab, transform.position, transform.rotation);
            obj.transform.localScale = transform.localScale;

            _placedItem = obj.GetComponent<ItemP>();

            _rotations = S.SM.LoadInt(S.ID(_id, "rot")) ?? 0;
            _currentRotation = S.SM.LoadInt(S.ID(_id, "curRot")) ?? 0;

            for (int i = 0; i < _currentRotation; i++)
                Rotate(_placedItem.gameObject);
        }
    }

    public void Do(Item item)
    {
        if (!_frerard._activated)
        {
            if (!S.Inventory.IsNullOrEmpty(item))
            {
                if (_placedItem != null)
                    Swap(item);
                else
                    Put(item);
            }
            else if (_placedItem != null)
                Interact();
           
            if (_placedItem != null)
            {
                bool ok = _placedItem.name == _waitItem && _currentRotation == 0;
                _frerard.Set(_number, ok);
            }
        }
	}

    void Swap(Item item)
    {
        Debug.Log("Frerard swap");
        string name = _placedItem._name;
        _placedItem.Destroy();
        Put(item);
        S.Inventory.Take(name, 1);
    }

    void Put(Item item)
    {
        Debug.Log("Frerard put");

        S.SM.Save(S.ID(_id, "name"), item._name);

        GameObject prefab = Prefabs.Get(item._name);
        GameObject obj = Instantiate(prefab, transform.position, transform.rotation);
        obj.transform.localScale = transform.localScale;

        _placedItem = obj.GetComponent<ItemP>();

        Debug.Log($"I put {_placedItem._name} to frerard!");

        int newRotations = UnityEngine.Random.Range(0, 4);
        _currentRotation = 0; //ok

        for (int i = 0; i < newRotations; i++)
            Rotate(obj);

        _rotations = 0; //ok

        S.Inventory.Remove(item._name, 1);

        S.AudioManager.Play("kill", 0.7f);
    }

    void Interact()
    {
        Debug.Log("Frerard interact");
        if (_rotations >= 3)
            Pick();
        else
        {
            Rotate(_placedItem._obj);
            Debug.Log($"I rotated frerard!");
            S.AudioManager.Play("kill", 0.7f);
        }
    }

    void Pick()
    {
        S.Inventory.Take(_placedItem);
        S.SM.Save(S.ID(_id, "name"), "");
        _placedItem = null;
    }

    void Rotate(GameObject obj)
    {
        obj.transform.Rotate(0, 0, 90);
        _rotations++;
        _currentRotation++;

        if (_currentRotation == 4)
            _currentRotation = 0;

        S.SM.Save(S.ID(_id, "rot"), _rotations);
        S.SM.Save(S.ID(_id, "curRot"), _currentRotation);
    }
}