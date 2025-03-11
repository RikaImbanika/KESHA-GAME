using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstZombie : MonoBehaviour
{
    AllFather _allFather;

    void Start()
    {
        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();

        if (!(S.SM.LoadBool("greenKeyTaken") ?? false))
            Destroy(this.gameObject);
        else if ((S.SM.LoadBool("gunWasBuyed") ?? false) && (S.SM.LoadBool("ammoWasBuyed") ?? false))
            Destroy(this.gameObject);
        else
        {
            var zombie = GetComponent<Zombie>();
            zombie._active = true;
        }
    }
}