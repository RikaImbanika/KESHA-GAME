using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TradeClick : MonoBehaviour, IPointerClickHandler
{
    public Trade _trade;
    public int _tradeNumber;
    public Trader _trader;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (S.Inventory.CountOfItem(_trade._selledItemName) >= _trade._selledCount)
        {
            S.Inventory.Remove(_trade._selledItemName, _trade._selledCount);
            S.Inventory.Take(_trade._buyedItemName, _trade._buyedCount);
            _trader.RemoveTrade(_tradeNumber);

            S.AM.Play("Money", 1);

            if (_trade._buyedItemName == "Gun")
            {
                S.SM.Save("gunWasBuyed", true);
                if (S.SM.LoadBool("ammoWasBuyed") ?? false)
                    S.FirstZombella2.FirstZombieEntersHall();
            }
            if (_trade._buyedItemName == "Ammo")
            {
                S.SM.Save("ammoWasBuyed", true);
                if (S.SM.LoadBool("gunWasBuyed") ?? false)
                    S.FirstZombella2.FirstZombieEntersHall();
            }
        }
        else
            S.AM.Play("Not Enough Cash", 1);
    }
}
