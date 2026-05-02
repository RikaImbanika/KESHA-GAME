using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrerardHolder : MonoBehaviour
{
    string _id;
    public int _number;
    public string _waitItem; 
    public Frerard _frerard;
    private ItemP _placedItem;
    int _realRotation;
    int _fakeRotation;

    void Start()
    {
        _id = S.ID("FrerardHolder", _number);

        string name = S.SM.LoadString(S.ID(_id, "name"));

        if (!string.IsNullOrEmpty(name))
        {
            GameObject prefab = Prefabs.Get(name);
            GameObject obj = Instantiate(prefab, transform.position, transform.rotation, S.Loader.SceneRoots["Hall"]);
            obj.transform.localScale = transform.localScale;
            obj.transform.SetParent(transform, true);

            _placedItem = obj.GetComponent<ItemP>();
            _placedItem._forLoader = false;

            int currentRotation = S.SM.LoadInt(S.ID(_id, "realRot")) ?? 0;

            for (int i = 0; i < currentRotation; i++)
                RotateReal(obj);

            _fakeRotation = S.SM.LoadInt(S.ID(_id, "fakeRot")) ?? 0;

            bool ok = _placedItem._name == _waitItem && _realRotation == 0;
            _frerard.Set(_number, ok);
            //SaveRotations();
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
                bool ok = _placedItem._name == _waitItem && _realRotation == 0;
                _frerard.Set(_number, ok);
                Debug.Log($"okay = {ok}, waitItem = {_waitItem} name = {_placedItem._name} rot = {_realRotation}");
            }
        }
	}

    void Swap(Item item)
    {
        Debug.Log("Frerard swap");
        string name = _placedItem._name;
        Destroy(_placedItem.gameObject);
        _placedItem = null;
        Put(item);
        S.Inventory.Take(name, 1);
        ForcedShowName();
    }

    void Put(Item item)
    {
        _realRotation = 0; //ok
        _fakeRotation = 0; //ok
        
        GameObject prefab = Prefabs.Get(item._name);
        GameObject obj = Instantiate(prefab, transform.position, transform.rotation, S.Loader.SceneRoots["Hall"]);
        obj.transform.localScale = transform.localScale;
        obj.transform.SetParent(transform, true);
        
        _placedItem = obj.GetComponent<ItemP>();
        
        SaveName();

        int newRotations = UnityEngine.Random.Range(0, 4);

        for (int i = 0; i < newRotations; i++)
            RotateReal(obj);

        SaveRotations();

        S.Inventory.Remove(item._name, 1);

        S.AudioManager.Play("kill", 0.7f);

        Debug.Log($"Frerard put, realRot: {_realRotation}, fakeRot: {_fakeRotation}");
    }

    void Interact()
    {
        Debug.Log("Frerard interact");
        if (_fakeRotation >= 3)
            Pick();
        else
        {
            RotateFake(_placedItem._obj);
            SaveRotations();
            Debug.Log($"I rotated frerard!");
            S.AudioManager.Play("kill", 0.85f);
        }
    }

    void Pick()
    {
        S.Inventory.Take(_placedItem._name, 1);
        ForcedShowName();
        Destroy(_placedItem.gameObject);
        SaveName("");
        _placedItem = null;

        Debug.Log($"Frerard pick");
    }

    void ForcedShowName()
    {
        S.Inventory.ForcedShowName();
    }
    
    void RotateReal(GameObject obj)
    {
        obj.transform.Rotate(0, 0, 90);
        
        _realRotation++;

        if (_realRotation == 4)
            _realRotation = 0;
    }

    void RotateFake(GameObject obj)
    {
        RotateReal(obj);
        
        _fakeRotation++;
    }
    
    void SaveName()
    {
        SaveName(_placedItem._name);
    }
    
    void SaveName(string name)
    {
        S.SM.Save(S.ID(_id, "name"), name);
    }
    
    void SaveRotations()
    {
        S.SM.Save(S.ID(_id, "fakeRot"), _fakeRotation);
        
        S.SM.Save(S.ID(_id, "realRot"), _realRotation);
    }
}