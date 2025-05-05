using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public bool _playerInToilet;
    public string _toiletPhase = "silence";
    static Vector3 _toiletCenter = new Vector3(-131f, -2.5f, 223.5f);
    static Vector3 _toiletStart = new Vector3(-174f, -2.5f, 109f);
    float _maxToiletDistance = Vector3.Distance(_toiletCenter, _toiletStart);

    public bool _firstZombieBattle;
    public float _fztVolume;
    public bool _fztFadeIn;
    public bool _fztFadeOut;
    public bool _fztKilled;

    void Start()
    {
        S.MM = this;
        _fztVolume = 0;
    }

    void Update()
    {
        float time = Time.deltaTime;
        float normalTime = 1 / 60f;
        float d = time / normalTime;

        if (_playerInToilet)
            IfPlayerInToilet(d);
        else if (_firstZombieBattle)
            FirstZombieBattle(d);
    }

    public void IfPlayerInToilet(float d)
    {
        if (S.AM._toiletMusic1.time > 80)
        {
            S.AM._toiletMusic1.Stop();
            S.AM._toiletMusic2.time = 0;
            S.AM._toiletMusic2.Play();
        }
        if (S.AM._toiletMusic2.time > 80)
        {
            S.AM._toiletMusic2.Stop();
            S.AM._toiletMusic1.time = 0;
            S.AM._toiletMusic1.Play();
        }

        if (_toiletPhase == "leaving")
        {
            if (S.AM._toiletMusic1.volume >= 0.003f * d)
                S.AM._toiletMusic1.volume -= 0.003f * d;
            if (S.AM._toiletMusic2.volume >= 0.003f * d)
                S.AM._toiletMusic2.volume -= 0.003f * d;

            if (S.AM._toiletMusic1.volume < 0.003f && S.AM._toiletMusic2.volume < 0.003f)
            {
                S.AM._toiletMusic1.Stop();
                S.AM._toiletMusic2.Stop();
                _toiletPhase = "silence";
                _playerInToilet = false; //?
            }
        }
        else if (_toiletPhase == "entering")
        {
            float distance = Vector3.Distance(_toiletCenter, transform.position);
            float volume = (_maxToiletDistance - distance) / _maxToiletDistance;
            if (volume < 0)
                volume = 0;

            S.AM._toiletMusic1.volume = volume;
            S.AM._toiletMusic2.volume = volume;
        }
    }

    public void FirstZombieBattle(float d)
    {
        if (S.AM._fzt1.time > 80)
        {
            S.AM._fzt2.time = 0;
            S.AM._fzt2.Play();
        }
        else if (S.AM._fzt1.time > 80)
        {
            S.AM._fzt1.time = 0;
            S.AM._fzt1.Play();
        }

        if (_fztFadeIn)
        {
            if (_fztVolume < 1)
            {
                _fztVolume += 0.03f * d;
                S.AM._fzt1.volume = _fztVolume;
                S.AM._fzt2.volume = _fztVolume;
            }
            else
            {
                _fztVolume = 1f;
                _fztFadeIn = false;
                S.AM._fzt1.volume = _fztVolume;
                S.AM._fzt2.volume = _fztVolume;
            }
        }
        else if (_fztFadeOut)
        {
            if (_fztVolume > 0.03f)
            {
                _fztVolume -= 0.01f * d;
                S.AM._fzt1.volume = _fztVolume;
                S.AM._fzt2.volume = _fztVolume;
            }
            else
            {
                _fztVolume = 0;
                _fztFadeOut = false;
                S.AM._fzt1.volume = _fztVolume;
                S.AM._fzt2.volume = _fztVolume;

                if (_fztKilled)
                    _firstZombieBattle = false;
            }
        }
    }

    public void FirstZombieEntersHall()
    {
        _firstZombieBattle = true;
        _fztFadeIn = false;
        _fztFadeOut = false;
        _fztVolume = 1;
        S.AM._fzt1.volume = _fztVolume;
        S.AM._fzt2.volume = _fztVolume;
        S.AM._fzt1.time = 0;
        S.AM._fzt2.time = 0;
        S.AM.Play("FZT1");
    }

    public void PlayerMeetFirstZombie()
    {
        _fztFadeIn = true;
        _fztFadeOut = false;

        S.AM._fzt1.volume = _fztVolume;
        S.AM._fzt2.volume = _fztVolume;

        if (!_firstZombieBattle)
        {
            _firstZombieBattle = true;
            if (!S.AM._fzt1.isPlaying && !S.AM._fzt2.isPlaying)
            {
                S.AM.Play("FZT1"); /////////////////
            }
        }
    }

    public void PlayerLeavesFirstZombie()
    {
        _fztFadeOut = true;
        _fztFadeIn = false;
    }

    public void PlayerKillsFirstZombie()
    {
        _fztFadeOut = true;
        _fztFadeIn = false;
        _fztKilled = true;
    }

    public void EnterToilet()
    {
        if (_toiletPhase == "" || _toiletPhase == "silence")
        {
            _toiletPhase = "entering";
            _playerInToilet = true;
            S.AM._toiletMusic1.volume = 0;
            S.AM._toiletMusic2.volume = 0;
            S.AM._toiletMusic1.time = 0;
            S.AM._toiletMusic1.Play();
        }
        else if (_toiletPhase == "leaving")
        {
            _toiletPhase = "entering";
        }
    }

    public void DeepEnterToilet()
    {
        _toiletPhase = "inside";
        S.AM._toiletMusic1.volume = 1;
        S.AM._toiletMusic2.volume = 1;
    }

    public void LeaveToilet()
    {
        if (_toiletPhase != "silence")
            _toiletPhase = "leaving";
    }
}
