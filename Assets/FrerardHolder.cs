using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrerardHolder : MonoBehaviour
{
    public int _number;
    public string _waitItem; 
    public Frerard _frerard;
    AllFather _allFather;
    Canvas _canvas;
    Inventory _inventory;
    ItemP _placedItem;
    int _currentRotation;
    int _rotations;

    void Start()
    {
        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();
        _canvas = GameObject.FindObjectOfType<Canvas>();
        _inventory = _canvas.GetComponent<Inventory>();
    }

    public void Take(Item item)
    {
        if (!_frerard._activated)
        {
            if (item != null)
            {
                GameObject prefab = Prefabs.Get(item._name);
                GameObject obj = Instantiate(prefab, transform.position, transform.rotation);
                obj.transform.localScale = transform.localScale;

                int rotations = UnityEngine.Random.Range(0, 4);
                _currentRotation = 0;

                for (int i = 0; i < rotations; i++)
                    Rotate(obj);
                _rotations = 0;

                if (_placedItem != null)
                    _inventory.Take(_placedItem);
                _placedItem = obj.GetComponent<ItemP>();
                //_placedItem.transform.SetParent(_frerard.transform);

                S.AudioManager.Play("kill", 0.7f);
            }
            else if (_placedItem != null)
            {
                if (_rotations >= 3)
                {
                    _inventory.Take(_placedItem);
                    _placedItem = null;
                }
                else
                    Rotate(_placedItem._obj);

                S.AudioManager.Play("kill", 0.7f);
            }

            if (_placedItem != null)
            {
                bool ok = _placedItem.name == _waitItem && _currentRotation == 0;
                _frerard.Set(_number, ok);
            }
        }
	}

    void Rotate(GameObject obj)
    {
        obj.transform.Rotate(0, 0, 90);
        _rotations++;
        _currentRotation++;

        if (_currentRotation == 4)
            _currentRotation = 0;
    }
}
