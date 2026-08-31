// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrerardHolder : MonoBehaviour
{
    string _id;
    public int _number;
    public string _waitItem; 
    public Frerard _frerard;
    private GameObject _placedItem;
    int _realRotation;
    int _fakeRotation;
    string _name;
    MaterialPropertyBlock _mpb;

    void Start()
    {
        _id = S.IDM("FrerardHolder", _number);

        _name = S.SM.LoadString(S.IDM(_id, "name"));

        _mpb = S.Fog.GetMPB("Hall");

        StartCoroutine(LateStart());

        IEnumerator LateStart()
        {
            if (!string.IsNullOrEmpty(_name))
            {
                while (S.Loader.Roots == null ||
                    !S.Loader.Roots.ContainsKey("Hall") ||
                    S.Loader.Roots["Hall"] == null)
                    yield return new WaitForSeconds(0.25f);

                GameObject prefab = Prefabs.Get(_name);
                _placedItem = Instantiate(prefab, transform.position, transform.rotation, S.Loader.Roots["Hall"]);
                _placedItem.transform.localScale = transform.localScale;
                _placedItem.transform.SetParent(transform, true);
                S.Fog.ApplyToGameObject(_placedItem, _mpb);
                ItemP itemP = _placedItem.GetComponent<ItemP>();
                Destroy(itemP);

                int currentRotation = S.SM.LoadInt(S.IDM(_id, "realRot")) ?? 0;

                for (int i = 0; i < currentRotation; i++)
                    RotateReal(_placedItem);

                _fakeRotation = S.SM.LoadInt(S.IDM(_id, "fakeRot")) ?? 0;

                //S.Console.AddMessage($"FH{_number} curRot = {currentRotation} realRot = {_realRotation} fakeRot = {_fakeRotation}");

                bool ok = _name == _waitItem && _realRotation == 0;
                _frerard.Set(_number, ok);
                //SaveRotations();
            }
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
                bool ok = _name == _waitItem && _realRotation == 0;
                _frerard.Set(_number, ok);
                //S.Console.AddMessage($"okay = {ok}, waitItem = {_waitItem} name = {_name} rot = {_realRotation}");
            }
        }
    }

    void TrySay()
    {
        if (!(S.SM.LoadBool("FirstFrerardPicPlaced") ?? false))
        {
            S.SM.Save("FirstFrerardPicPlaced", true);
            StartCoroutine(Say());
        }

        IEnumerator Say()
        {
            yield return new WaitForSeconds(1.8f);
            S.Console.AddMessage("Rika: I need to collect all of that picture parts!", Color.magenta);
        }
    }

    void Swap(Item item)
    {
        //S.Console.AddMessage("Frerard swap");
        string nameRemember = _name;
        Destroy(_placedItem.gameObject);
        _placedItem = null;
        _realRotation = 0;
        _fakeRotation = 0;
        Put(item);
        S.Inventory.Take(nameRemember, 1);
        ForcedShowName();
    }

    void Put(Item item)
    {
        TrySay();

        _realRotation = 0; //ok
        _fakeRotation = 0; //ok
        
        GameObject prefab = Prefabs.Get(item._name);
        _placedItem = Instantiate(prefab, transform.position, transform.rotation, S.Loader.Roots["Hall"]);
        S.Fog.ApplyToGameObject(_placedItem, _mpb);
        ItemP itemP = _placedItem.GetComponent<ItemP>();
        Destroy(itemP);
        
        _placedItem.transform.localScale = transform.localScale;
        _placedItem.transform.SetParent(transform, true);
        _name = item._name;
        
        SaveName();

        int newRotations = UnityEngine.Random.Range(0, 4);

        for (int i = 0; i < newRotations; i++)
            RotateReal(_placedItem);

        SaveRotations();

        S.Inventory.Remove(item._name, 1);

        S.AM.Play("Kill", 0.7f);

        //S.Console.AddMessage($"Frerard put, realRot: {_realRotation}, fakeRot: {_fakeRotation}");
    }

    void Interact()
    {
        //S.Console.AddMessage("Frerard interact");
        if (_fakeRotation >= 3)
            Pick();
        else
        {
            RotateFake(_placedItem.gameObject);
            SaveRotations();
            //S.Console.AddMessage($"I rotated frerard!");
            S.AM.Play("Kill", 0.85f);
        }
    }

    void Pick()
    {
        S.Inventory.Take(_name, 1);
        ForcedShowName();
        Destroy(_placedItem.gameObject);
        SaveName("");
        _realRotation = 0;
        _fakeRotation = 0;
        SaveRotations();
        _placedItem = null;
        _name = "empty";

        //S.Console.AddMessage($"Frerard pick");
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
        SaveName(_name);
    }
    
    void SaveName(string name)
    {
        S.SM.Save(S.IDM(_id, "name"), name);
    }
    
    void SaveRotations()
    {
        S.SM.Save(S.IDM(_id, "fakeRot"), _fakeRotation);        
        S.SM.Save(S.IDM(_id, "realRot"), _realRotation);
    }
}