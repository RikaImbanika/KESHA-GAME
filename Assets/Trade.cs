using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trade : MonoBehaviour
{
    public string _id;
    public int _tradeCount;
    public Texture2D _buyedTexture;
    public Texture2D _selledTexture;
    public string _selledItemName;
    public string _buyedItemName;
    public int _selledCount;
    public int _buyedCount;
    public Item _item;
    AllFather _allFather;

    public void Start()
    {
        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();

        _id = "" + transform.position.x + transform.position.y + transform.position.z;

        if (_allFather.Contains(_id))
        {
            Save s = _allFather.Load(_id);
            _tradeCount = s._count;
        }
    }

    public void Save()
    {
        Save s = new Save();
        if (_allFather.Contains(_id))
            s = _allFather.Load(_id);
        
        s._count = _tradeCount;

        _allFather.Save(_id, s);
    }
}
