using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TradeClick : MonoBehaviour, IPointerClickHandler
{
    public Trade _trade;
    public int _tradeNumber;
    public Inventory _inventory;
    public Trader _trader;
    private AudioManager _audioManager;
    AllFather _allFather;

    public void Start()
    {
        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();
        GameObject go = GameObject.FindGameObjectWithTag("AudioManager");
        _audioManager = go.GetComponent<AudioManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_inventory.CountOfItem(_trade._selledItemName) >= _trade._selledCount)
        {
            _inventory.Remove(_trade._selledItemName, _trade._selledCount);
            _inventory.Take(_trade._buyedItemName, _trade._buyedCount);
            _trader.RemoveTrade(_tradeNumber);

            _audioManager.Play("money", 1);

            if (_trade._buyedItemName == "Gun")
            {
                S.SM.Save("gunWasBuyed", true);
                if (S.SM.LoadBool("ammoWasBuyed") ?? false)
                    S.FirstZombie2.FirstZombieEntersHall();
            }
            if (_trade._buyedItemName == "Ammo")
            {
                S.SM.Save("ammoWasBuyed", true);
                if (S.SM.LoadBool("gunWasBuyed") ?? false)
                    S.FirstZombie2.FirstZombieEntersHall();
            }
        }
        else
            _audioManager.Play("notEnoughCash", 1);
    }
}
