using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstZombie : MonoBehaviour
{
    void Start()
    {
        if (!(S.SM.LoadBool("greenKeyTaken") ?? false))
            Destroy(this.gameObject);
        else if ((S.SM.LoadBool("gunWasBuyed") ?? false) && (S.SM.LoadBool("ammoWasBuyed") ?? false))
            Destroy(this.gameObject);
        else
        {
            GameObject zombie = GameObject.Instantiate(Prefabs.Get("FirstZombie"), transform.position, transform.rotation, transform);
            Zombie zomb = zombie.GetComponent<Zombie>();
            zomb._health = 300f;
            zomb._maxHealth = 300f;
        }
    }
}