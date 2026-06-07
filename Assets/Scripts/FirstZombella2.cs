// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstZombella2 : MonoBehaviour
{
    public Zombie _zombella;
    bool _playerInHall;
    bool _actuallySummoned;
    bool _dead;

    void Start()
    {
        S.FirstZombella2 = this;

        StartCoroutine(DelayCoroutine());

        IEnumerator DelayCoroutine()
        {
            yield return new WaitForSeconds(0.2f);

            if ((S.SM.LoadBool("gunWasBuyed") ?? false) && (S.SM.LoadBool("ammoWasBuyed") ?? false))
            {
                Summon();
                if (S.SM.LoadBool("firstZombellaIsDead") ?? false)
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
            if (_zombella._health <= 0 && !_dead)
            {
                _dead = true;
                _zombella._followPlayer = false;
                S.MM.PlayerKillsFirstZombella();
                S.SM.Save("firstZombellaIsDead", true);
                S.Granny.Calming();
                Debug.Log("Dead in hall");
            }
            
            if (!_playerInHall && playerInHall && !_dead)
            {
                _playerInHall = true;
                _zombella._followPlayer = true;
                S.MM.PlayerMeetFirstZombella();
                Debug.Log("Meet");
                //Not ideal but work so okay
            }
            else if (_playerInHall && !playerInHall)
            {
                _zombella._followPlayer = false;
                _playerInHall = false;
                S.MM.PlayerLeavesFirstZombella();
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

            S.MM.FirstZombellaEntersHall();
            Summon();
            _zombella._followPlayer = true;

            S.Granny.StartFleeing();
        }
    }

    public void Summon()
    {
        GameObject zombella = GameObject.Instantiate(Prefabs.Get("Zombella"), transform.position, transform.rotation, transform);
        _zombella = zombella.GetComponent<Zombie>();
        _zombella._health = 500f;
        _zombella._maxHealth = 500f;
        _zombella._ani.Rebind();
        _actuallySummoned = true;
        Debug.LogError("Summoned in hall");
    }
}
