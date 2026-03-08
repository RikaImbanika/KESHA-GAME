using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstZombie2 : MonoBehaviour
{
    public Zombie _zombie;
    bool _playerInHall;
    bool _actuallySummoned;
    bool _dead;

    void Start()
    {
        S.FirstZombie2 = this;

        StartCoroutine(DelayCoroutine());

        IEnumerator DelayCoroutine()
        {
            yield return new WaitForSeconds(0.2f);

            if ((S.SM.LoadBool("gunWasBuyed") ?? false) && (S.SM.LoadBool("ammoWasBuyed") ?? false))
            {
                Summon();
                if (S.SM.LoadBool("firstZombieIsDead") ?? false)
                {
                    _dead = true;
                    S.MM._fztKilled = true;
                }
            }
        }
    }

    void Update()
    {
        bool playerInHall = S.PS._currentSceneName == "Hall";

        if (_actuallySummoned)
        {
            if (_zombie._health <= 0 && !_dead)
            {
                _dead = true;
                _zombie._followPlayer = false;
                S.MM.PlayerKillsFirstZombie();
                S.SM.Save("firstZombieIsDead", true);
                Debug.Log("Dead in hall");
            }
            
            if (!_playerInHall && playerInHall && !_dead)
            {
                _playerInHall = true;
                _zombie._followPlayer = true;
                S.MM.PlayerMeetFirstZombie();
                Debug.Log("Meet");
            }
            else if (_playerInHall && !playerInHall)
            {
                _zombie._followPlayer = false;
                _playerInHall = false;
                S.MM.PlayerLeavesFirstZombie();
                Debug.Log("Leave");
            }
        }
    }

    public void FirstZombieEntersHall()
    {
        StartCoroutine(DelayCoroutine());

        IEnumerator DelayCoroutine()
        {
            S.SM.Save("firstZombieInHall", true);

            yield return new WaitForSeconds(2f);

            S.AudioManager.Play("Door");

            yield return new WaitForSeconds(0.3f);

            S.MM.FirstZombieEntersHall(); //Yes.
            Summon();
            _zombie._followPlayer = true;
        }
    }

    public void Summon()
    {
        GameObject zombie = GameObject.Instantiate(Prefabs.Get("FirstZombie"), transform.position, transform.rotation, transform);
        _zombie = zombie.GetComponent<Zombie>();
        _zombie._ani.Rebind();
        _actuallySummoned = true;
        Debug.LogError("Summoned in hall");
    }
}
