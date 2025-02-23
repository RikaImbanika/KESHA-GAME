using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trade : MonoBehaviour
{
    public string _id;
    public int _tradeCount;
    public string _selledItemName;
    public int _selledCount;
    public string _buyedItemName;
    public int _buyedCount;

    public void Start()
    {
        _id = "" + transform.position.x + transform.position.y + transform.position.z;

        if (S.AllFather.Contains(_id))
        {
            Save s = S.AllFather.Load(_id);
            _tradeCount = s._count;
        }
    }

    public void Save()
    {
        Save s = new Save();
        if (S.AllFather.Contains(_id))
            s = S.AllFather.Load(_id);
        
        s._count = _tradeCount;

        S.AllFather.Save(_id, s);
    }
}
