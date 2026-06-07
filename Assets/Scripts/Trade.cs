// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trade : MonoBehaviour
{
    public string _id;
    string _idTradeCount;
    public int _tradeCount;
    public string _selledItemName;
    public int _selledCount;
    public string _buyedItemName;
    public int _buyedCount;

    public void Start()
    {
        _id = S.ID("Trade", gameObject);
        _idTradeCount = S.IDM(_id, "count");
        _tradeCount = S.SM.LoadInt(_idTradeCount) ?? _tradeCount;
    }

    public void Save()
    {
        S.SM.Save(_idTradeCount, _tradeCount);
    }
}
