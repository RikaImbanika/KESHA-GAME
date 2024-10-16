using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstZombie : MonoBehaviour
{
    AllFather _allFather;

    void Start()
    {
        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();

        if (!_allFather.Load("greenKeyTaken")._bool)
            Destroy(this.gameObject);

        if (_allFather.Load("gunWasBuyed")._bool && _allFather.Load("ammoWasBuyed")._bool)
            Destroy(this.gameObject);
    }
}
