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
        _id = "Trade_" + transform.position.x + transform.position.z + _selledItemName + _buyedItemName;
        _tradeCount = S.SM.LoadInt(S.ID(_id, "TradeCount")) ?? _tradeCount;
    }

    public void Save()
    {
        S.SM.Save(S.ID(_id, "TradeCount"), _tradeCount);
    }
}
