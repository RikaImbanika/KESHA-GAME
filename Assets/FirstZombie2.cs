using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstZombie2 : MonoBehaviour
{
    public Zombie _zombie;
    //bool _summoned;
    bool _inBattle;
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

            if (!(S.SM.LoadBool("gunWasBuyed") ?? false) || !(S.SM.LoadBool("ammoWasBuyed") ?? false))
            {
                _zombie.gameObject.SetActive(false);
                _zombie._active = false;
            }
            else
            {
                if (!(S.SM.LoadBool("firstZombieIsDead") ?? false))
                {
                    Summon();
                }
                else
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
        if (!_playerInHall && playerInHall && !_dead && _actuallySummoned)
        {
            _playerInHall = true;
            _zombie._followPlayer = true;
            S.MM.PlayerMeetFirstZombie();
        }
        else if (_playerInHall && !playerInHall && _actuallySummoned)
        {
            _zombie._followPlayer = false;
            _playerInHall = false;
            S.MM.PlayerLeavesFirstZombie();
        }

        if (_zombie._health <= 0 && _actuallySummoned)
        {
            _dead = true;
            S.MM.PlayerKillsFirstZombie();
            S.SM.Save("firstZombieIsDead", true);
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
            if (!_zombie.gameObject.activeInHierarchy)
                _zombie.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.3f);

            S.MM.FirstZombieEntersHall(); //Yes.
            Summon();
            _zombie._followPlayer = true;
        }
    }

    public void Summon()
    {
        _zombie.gameObject.SetActive(true);
        _zombie._active = true;
        _zombie._ani.Rebind();
        _actuallySummoned = true;
    }
}
