// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MusicPhase
{
    Silence,
    Entering,
    Leaving,
    Entered,
    Leaved,
    Inside
}

public enum IncomeSwapPhase
{
    First,
    Second,
    OneToTwo,
    TwoToOne
}

public class MusicManager : MonoBehaviour
{
    public bool _playerInToilet;
    public MusicPhase _toiletPhase = MusicPhase.Silence;

    static Vector3 _toiletCenter = new Vector3(-131f, -2.5f, 223.5f);
    static Vector3 _toiletStart = new Vector3(-174f, -2.5f, 109f);
    float _maxToiletDistance = Vector3.Distance(_toiletCenter, _toiletStart);

    public bool _firstZombellaBattle;
    public float _fztVolume;
    public bool _fztFadeIn;
    public bool _fztFadeOut;
    public bool _fztKilled;
    public bool _playerOnIncome;
    public IncomeSwapPhase _incomeSwapPhase;
    public MusicPhase _incomePhase = MusicPhase.Silence;
    public MusicPhase _backroomsPhase = MusicPhase.Silence;
    public float _incomeTime;
    public AudioSource _backroomsOldTrack;
    public AudioSource _backroomsNewTrack;
    public AudioSource[] _backroomsSources;
    public int[] _backroomsOrder;
    public float[] _backroomsLengthes;
    public float[] _backroomsVolumes;
    public float _backroomVolume;
    public int _backroomsTrackId;
    public int _backroomsPrevTrackId;
    //MR
    public AudioSource _mushroomsOldTrack;
    public AudioSource _mushroomsNewTrack;
    public AudioSource[] _mushroomsSources;
    public float[] _mushroomsLengthes;
    public float[] _mushroomsVolumes;
    public float _mushroomVolume;
    public int _mushroomsTrackId;
    public int _mushroomsPrevTrackId;
    public MusicPhase _mushroomsPhase = MusicPhase.Silence;

    public AudioSource _finalOldTrack;
    public AudioSource _finalNewTrack;
    public AudioSource[] _finalSources;
    public float[] _finalLengthes;
    public float[] _finalVolumes;
    public float _finalVolume;
    public int _finalTrackId;
    public int _finalPrevTrackId;
    public MusicPhase _finalPhase = MusicPhase.Silence;
    private float _backroomsTrackPlayTime;

    // Consts
    private const float _fadeSpeedNormal = 0.005f;
    private const float _fadeSpeedFast = 0.003f;
    private const float _fadeSpeedFirstZombellaIn = 0.03f;
    private const float _fadeSpeedFirstZombellaOut = 0.01f;
    private const float _incomeSwitchTime = 197f;
    private const float _toiletTrackLength = 80f;
    private const float _firstZombellaTrackLength = 80f;
    private const float _finalTrackTrimSeconds = 2f;
    private const float _referenceFramesPerSecond = 60f;
    private const float _waitDelaySeconds = 0.1f;
    private const float _lateStartDelay = 0.3f;

    void Start()
    {
        S.MM = this;

        _fztVolume = 0;
        _playerOnIncome = true;
        _incomeSwapPhase = IncomeSwapPhase.First;
        _mushroomsPrevTrackId = 1; //
        _finalPrevTrackId = 1;

        StartCoroutine(LateStart(_lateStartDelay));

        IEnumerator LateStart(float waitTime)
        {
            while (S.AudioManager == null)
            {
                yield return new WaitForSeconds(_waitDelaySeconds);
            }

            int count = 6;
            int count2 = 13;

            _backroomsVolumes = new float[count];
            _backroomsVolumes[0] = 1;

            _backroomsSources = new AudioSource[count];
            _backroomsLengthes = new float[count];
                        
            _backroomsSources[0] = S.AM.A["Good Times"];
            _backroomsLengthes[0] = 313;

            _backroomsSources[1] = S.AM.A["Rainbow"];
            _backroomsLengthes[1] = 303;
                        
            _backroomsSources[2] = S.AM.A["Adelaida"];
            _backroomsLengthes[2] = 176;
                        
            _backroomsSources[3] = S.AM.A["Fenomen"];
            _backroomsLengthes[3] = 677;
                        
            _backroomsSources[4] = S.AM.A["Riddik"];
            _backroomsLengthes[4] = 1119;

            _backroomsSources[5] = S.AM.A["White Fog"];
            _backroomsLengthes[5] = 317;

            bool[] remember = new bool[count2];
            _backroomsOrder = new int[count2];

            var rnd = new System.Random();

            //Ordering

            //It's pretty complex but this is
            //Only my own logic of music
            //I can't explain it

            _backroomsOrder[0] = 0;
            _backroomsOrder[1] = 1;
            _backroomsOrder[2] = 5;

            for (int i = 2; i > 0; i--)
            {
                int j = S.RND.Next(0, i + 1);
                int temp = _backroomsOrder[i];
                _backroomsOrder[i] = _backroomsOrder[j];
                _backroomsOrder[j] = temp;
            }

            _backroomsOrder[3] = _backroomsOrder[0];

            int[] localOrder = new int[3];
            localOrder[0] = 2;
            localOrder[1] = 3;
            localOrder[2] = 4;

            S.AllFather.Shuffle(localOrder);

            _backroomsOrder[4] = localOrder[0];

            _backroomsOrder[5] = _backroomsOrder[0];
            _backroomsOrder[6] = _backroomsOrder[1];

            _backroomsOrder[7] = localOrder[1];

            _backroomsOrder[8] = _backroomsOrder[2];
            _backroomsOrder[9] = _backroomsOrder[0];

            _backroomsOrder[10] = localOrder[2];

            _backroomsOrder[11] = _backroomsOrder[1];
            _backroomsOrder[12] = _backroomsOrder[2];

            _backroomsTrackId = 0;
            _backroomsPrevTrackId = 1; //Track here should not be equals first one

            //Play

            S.AM.A["Income 1"].Play();

            //MR

            _mushroomsVolumes = new float[2];
            _mushroomsVolumes[0] = 1;

            _mushroomsSources = new AudioSource[2];
            _mushroomsSources[0] = S.AM.A["Maylo"];
            _mushroomsSources[1] = S.AM.A["The Room"];

            _mushroomsLengthes = new float[2];
            _mushroomsLengthes[0] = 282;
            _mushroomsLengthes[1] = 128;

            _finalVolumes = new float[2];
            _finalVolumes[0] = 1;

            _finalSources = new AudioSource[2];
            _finalSources[0] = S.AM.A["Final 1"];
            _finalSources[1] = S.AM.A["Final 2"];

            _finalLengthes = new float[2];
            _finalLengthes[0] = _finalSources[0].clip.length - _finalTrackTrimSeconds;
            _finalLengthes[1] = _finalSources[1].clip.length - _finalTrackTrimSeconds;
        }
    }

    void Update()
    {
        float d = Time.deltaTime * _referenceFramesPerSecond;

        if (_playerInToilet)
            IfPlayerInToilet(d);
        else if (_firstZombellaBattle)
            FirstZombellaBattle(d);

        Income(d);
        Backrooms(d);
        Mushrooms(d);
        Final(d);
    }

    public void Mushrooms(float d)
    {
        if (_mushroomsPhase == MusicPhase.Silence)
            return;

        if (_mushroomsPhase == MusicPhase.Leaving)
        {
            if (_mushroomVolume > 0)
                _mushroomVolume -= _fadeSpeedNormal * d;
            else
            {
                _mushroomsPhase = MusicPhase.Silence;
                _mushroomVolume = 0;
                _mushroomsSources[_mushroomsTrackId].Pause();
                _mushroomsSources[_mushroomsPrevTrackId].Pause();
            }
        }
        else if (_mushroomsPhase == MusicPhase.Entering)
        {
            _mushroomVolume += _fadeSpeedNormal * d;

            if (_mushroomVolume > 1)
            {
                _mushroomVolume = 1;
                _mushroomsPhase = MusicPhase.Entered;
            }
        }

        if (_mushroomsSources[_mushroomsTrackId].time > _mushroomsLengthes[_mushroomsTrackId])
        {
            _mushroomsPrevTrackId = _mushroomsTrackId;

            _mushroomsTrackId += 1;
            if (_mushroomsTrackId >= _mushroomsSources.Length)
                _mushroomsTrackId = 0;

            _mushroomsSources[_mushroomsTrackId].time = 0;
            _mushroomsVolumes[_mushroomsTrackId] = 1;
            _mushroomsSources[_mushroomsTrackId].volume = 1 * _mushroomVolume;
            _mushroomsSources[_mushroomsTrackId].Play();
        }

        if (_mushroomsVolumes[_mushroomsPrevTrackId] > 0)
            _mushroomsVolumes[_mushroomsPrevTrackId] -= _fadeSpeedNormal * d;
        else
        {
            _mushroomsVolumes[_mushroomsPrevTrackId] = 0;
            _mushroomsSources[_mushroomsPrevTrackId].volume = 0;
            _mushroomsSources[_mushroomsPrevTrackId].Stop();
        }

        _mushroomsSources[_mushroomsTrackId].volume = _mushroomsVolumes[_mushroomsTrackId] * _mushroomVolume;
        _mushroomsSources[_mushroomsPrevTrackId].volume = _mushroomsVolumes[_mushroomsPrevTrackId] * _mushroomVolume;
    }

    public void EnterMushrooms()
    {
        if (_mushroomsPhase == MusicPhase.Silence)
            _mushroomsSources[_mushroomsTrackId].Play();

        _mushroomsPhase = MusicPhase.Entering;
    }

    public void LeaveMushrooms()
    {
        _mushroomsPhase = MusicPhase.Leaving;
    }

    public void Final(float d)
    {
        if (_finalPhase == MusicPhase.Silence)
            return;

        if (_finalPhase == MusicPhase.Leaving)
        {
            if (_finalVolume > 0)
                _finalVolume -= _fadeSpeedNormal * d;
            else
            {
                _finalPhase = MusicPhase.Silence;
                _finalVolume = 0;
                _finalSources[_finalTrackId].Pause();
                _finalSources[_finalPrevTrackId].Pause();
            }
        }
        else if (_finalPhase == MusicPhase.Entering)
        {
            _finalVolume += _fadeSpeedNormal * d;

            if (_finalVolume > 1)
            {
                _finalVolume = 1;
                _finalPhase = MusicPhase.Entered;
            }
        }

        if (_finalSources[_finalTrackId].time > _finalLengthes[_finalTrackId])
        {
            _finalPrevTrackId = _finalTrackId;

            _finalTrackId += 1;
            if (_finalTrackId >= _finalSources.Length)
                _finalTrackId = 0;

            _finalSources[_finalTrackId].time = 0;
            _finalVolumes[_finalTrackId] = 1;
            _finalSources[_finalTrackId].volume = 1 * _finalVolume;
            _finalSources[_finalTrackId].Play();
        }

        if (_finalVolumes[_finalPrevTrackId] > 0)
            _finalVolumes[_finalPrevTrackId] -= _fadeSpeedNormal * d;
        else
        {
            _finalVolumes[_finalPrevTrackId] = 0;
            _finalSources[_finalPrevTrackId].volume = 0;
            _finalSources[_finalPrevTrackId].Stop();
        }

        _finalSources[_finalTrackId].volume = _finalVolumes[_finalTrackId] * _finalVolume;
        _finalSources[_finalPrevTrackId].volume = _finalVolumes[_finalPrevTrackId] * _finalVolume;
    }

    public void EnterFinal()
    {
        if (_finalPhase == MusicPhase.Silence)
            _finalSources[_finalTrackId].Play();

        _finalPhase = MusicPhase.Entering;
    }

    public void LeaveFinal()
    {
        _finalPhase = MusicPhase.Leaving;
    }

    public void Backrooms(float d)
    {
        if (_backroomsPhase == MusicPhase.Silence)
            return;

        _backroomsTrackPlayTime += Time.deltaTime;

        int trackIdShuffled = _backroomsOrder[_backroomsTrackId];
        int prevTrackIdShuffled = _backroomsOrder[_backroomsPrevTrackId];

        if (_backroomsPhase == MusicPhase.Leaving)
        {
            if (_backroomVolume > 0)
                _backroomVolume -= _fadeSpeedNormal * d;
            else
            {
                _backroomsPhase = MusicPhase.Silence;
                _backroomVolume = 0;
                _backroomsSources[trackIdShuffled].Pause();
                _backroomsSources[prevTrackIdShuffled].Pause();
            }
        }
        else if (_backroomsPhase == MusicPhase.Entering)
        {
            _backroomVolume += _fadeSpeedNormal * d;

            if (_backroomVolume > 1)
            {
                _backroomVolume = 1;
                _backroomsPhase = MusicPhase.Entered;
            }
        }

        if (_backroomsTrackPlayTime > _backroomsLengthes[trackIdShuffled])
        {
            _backroomsPrevTrackId = _backroomsTrackId;

            _backroomsTrackId += 1;
            if (_backroomsTrackId >= _backroomsOrder.Length)
                _backroomsTrackId = 0;

            trackIdShuffled = _backroomsOrder[_backroomsTrackId];
            prevTrackIdShuffled = _backroomsOrder[_backroomsPrevTrackId];

            _backroomsSources[trackIdShuffled].time = 0;
            _backroomsVolumes[trackIdShuffled] = 1;
            _backroomsSources[trackIdShuffled].volume = 1 * _backroomVolume;
            _backroomsSources[trackIdShuffled].Play();
            _backroomsTrackPlayTime = 0;
        }

        if (_backroomsVolumes[prevTrackIdShuffled] > 0)
            _backroomsVolumes[prevTrackIdShuffled] -= _fadeSpeedNormal * d;
        else
        {
            _backroomsVolumes[prevTrackIdShuffled] = 0;
            _backroomsSources[prevTrackIdShuffled].volume = 0;
            _backroomsSources[prevTrackIdShuffled].Stop();
        }

        _backroomsSources[trackIdShuffled].volume = _backroomsVolumes[trackIdShuffled] * _backroomVolume;
        _backroomsSources[prevTrackIdShuffled].volume = _backroomsVolumes[prevTrackIdShuffled] * _backroomVolume;
    }

    public void EnterBackrooms()
    {
        int trackIdShuffled = _backroomsOrder[_backroomsTrackId];

        if (_backroomsPhase == MusicPhase.Silence)
        {
            _backroomsVolumes[trackIdShuffled] = 1;
            _backroomsSources[trackIdShuffled].volume = 1 * _backroomVolume;

            _backroomsSources[trackIdShuffled].Play();
        }

        _backroomsPhase = MusicPhase.Entering;
    }

    public void LeaveBackrooms()
    {
        _backroomsPhase = MusicPhase.Leaving;
    }

    public void EnterIncome()
    {
        _incomePhase = MusicPhase.Entering;

        if (_incomeSwapPhase == IncomeSwapPhase.First || _incomeSwapPhase == IncomeSwapPhase.TwoToOne)
        {
            if (!S.AM.A["Income 1"].isPlaying)
            {
                S.AM.A["Income 1"].Play();
                S.AM.A["Income 1"].time = _incomeTime;
            }
        }
        else if (_incomeSwapPhase == IncomeSwapPhase.Second || _incomeSwapPhase == IncomeSwapPhase.OneToTwo)
        {
            if (!S.AM.A["Income 2"].isPlaying)
            {
                S.AM.A["Income 2"].Play();
                S.AM.A["Income 2"].time = _incomeTime;
            }
        }
    }

    public void LeaveIncome()
    {
        _incomePhase = MusicPhase.Leaving;
    }

    public void Income(float d)
    {
        if (_incomePhase == MusicPhase.Leaving)
        {
            if (S.AM.A["Income 1"].volume > 0)
                S.AM.A["Income 1"].volume -= _fadeSpeedNormal * d;
            if (S.AM.A["Income 2"].volume > 0)
                S.AM.A["Income 2"].volume -= _fadeSpeedNormal * d;

            if (S.AM.A["Income 2"].volume <= 0 && S.AM.A["Income 1"].volume <= 0)
            {
                _incomePhase = MusicPhase.Leaved;

                if (_incomeSwapPhase == IncomeSwapPhase.First || _incomeSwapPhase == IncomeSwapPhase.TwoToOne)
                    _incomeTime = S.AM.A["Income 1"].time;
                else
                    _incomeTime = S.AM.A["Income 2"].time;

                S.AM.A["Income 1"].Pause();
                S.AM.A["Income 2"].Pause();
            }
        }
        else if (_incomePhase == MusicPhase.Entering)
        {
            if (_incomeSwapPhase == IncomeSwapPhase.First || _incomeSwapPhase == IncomeSwapPhase.TwoToOne)
            {
                S.AM.A["Income 1"].volume += _fadeSpeedNormal * d;

                if (S.AM.A["Income 1"].volume >= 1)
                    _incomePhase = MusicPhase.Entered;
            }
            else if (_incomeSwapPhase == IncomeSwapPhase.Second || _incomeSwapPhase == IncomeSwapPhase.OneToTwo)
            {
                S.AM.A["Income 2"].volume += _fadeSpeedNormal * d;

                if (S.AM.A["Income 2"].volume >= 1)
                    _incomePhase = MusicPhase.Entered;
            }
        }
        else
        {
            if (_incomeSwapPhase == IncomeSwapPhase.First)
                if (S.AM.A["Income 1"].time > _incomeSwitchTime)
                {
                    _incomeSwapPhase = IncomeSwapPhase.OneToTwo;
                    S.AM.A["Income 2"].time = 0;
                    S.AM.A["Income 2"].volume = 1;
                    S.AM.A["Income 2"].Play();
                }

            if (_incomeSwapPhase == IncomeSwapPhase.Second)
                if (S.AM.A["Income 2"].time > _incomeSwitchTime)
                {
                    _incomeSwapPhase = IncomeSwapPhase.TwoToOne;
                    S.AM.A["Income 1"].time = 0;
                    S.AM.A["Income 1"].volume = 1;
                    S.AM.A["Income 1"].Play();
                }

            if (_incomeSwapPhase == IncomeSwapPhase.OneToTwo)
            {
                S.AM.A["Income 1"].volume -= _fadeSpeedFast * d;
                if (S.AM.A["Income 1"].volume <= 0)
                {
                    S.AM.A["Income 1"].Stop();
                    _incomeSwapPhase = IncomeSwapPhase.Second;
                }
            }

            if (_incomeSwapPhase == IncomeSwapPhase.TwoToOne)
            {
                S.AM.A["Income 2"].volume -= _fadeSpeedFast * d;
                if (S.AM.A["Income 2"].volume <= 0)
                {
                    S.AM.A["Income 2"].Stop();
                    _incomeSwapPhase = IncomeSwapPhase.First;
                }
            }
        }
    }

    public void IfPlayerInToilet(float d)
    {
        if (S.AM.A["Toilet Music 1"].time > _toiletTrackLength)
        {
            S.AM.A["Toilet Music 1"].Stop();
            S.AM.A["Toilet Music 2"].time = 0;
            S.AM.A["Toilet Music 2"].Play();
        }
        if (S.AM.A["Toilet Music 2"].time > _toiletTrackLength)
        {
            S.AM.A["Toilet Music 2"].Stop();
            S.AM.A["Toilet Music 1"].time = 0;
            S.AM.A["Toilet Music 1"].Play();
        }

        if (_toiletPhase == MusicPhase.Leaving)
        {
            if (S.AM.A["Toilet Music 1"].volume >= _fadeSpeedFast * d)
                S.AM.A["Toilet Music 1"].volume -= _fadeSpeedFast * d;
            if (S.AM.A["Toilet Music 2"].volume >= _fadeSpeedFast * d)
                S.AM.A["Toilet Music 2"].volume -= _fadeSpeedFast * d;

            if (S.AM.A["Toilet Music 1"].volume < _fadeSpeedFast && S.AM.A["Toilet Music 2"].volume < _fadeSpeedFast)
            {
                S.AM.A["Toilet Music 1"].Stop();
                S.AM.A["Toilet Music 2"].Stop();
                _toiletPhase = MusicPhase.Silence;
                _playerInToilet = false; //?
            }
        }
        else if (_toiletPhase == MusicPhase.Entering)
        {
            float distance = Vector3.Distance(_toiletCenter, S.Camera.transform.position);
            float volume = (_maxToiletDistance - distance) / _maxToiletDistance;
            if (volume < 0)
                volume = 0;

            S.AM.A["Toilet Music 1"].volume = volume;
            S.AM.A["Toilet Music 2"].volume = volume;
        }
    }

    public void FirstZombellaBattle(float d)
    {
        if (S.AM.A["First Zombella Theme 1"].time > _firstZombellaTrackLength)
        {
            S.AM.A["First Zombella Theme 2"].time = 0;
            S.AM.A["First Zombella Theme 2"].Play();
        }
        else if (S.AM.A["First Zombella Theme 2"].time > _firstZombellaTrackLength)
        {
            S.AM.A["First Zombella Theme 1"].time = 0;
            S.AM.A["First Zombella Theme 1"].Play();
        }

        if (_fztFadeIn)
        {
            if (_fztVolume < 1)
            {
                _fztVolume += _fadeSpeedFirstZombellaIn * d;
                S.AM.A["First Zombella Theme 1"].volume = _fztVolume;
                S.AM.A["First Zombella Theme 2"].volume = _fztVolume;
            }
            else
            {
                _fztVolume = 1f;
                _fztFadeIn = false;
                S.AM.A["First Zombella Theme 1"].volume = _fztVolume;
                S.AM.A["First Zombella Theme 2"].volume = _fztVolume;
            }
        }
        else if (_fztFadeOut)
        {
            if (_fztVolume > _fadeSpeedFirstZombellaOut)
            {
                _fztVolume -= _fadeSpeedFirstZombellaOut * d;
                S.AM.A["First Zombella Theme 1"].volume = _fztVolume;
                S.AM.A["First Zombella Theme 2"].volume = _fztVolume;
            }
            else
            {
                _fztVolume = 0;
                _fztFadeOut = false;
                S.AM.A["First Zombella Theme 1"].volume = _fztVolume;
                S.AM.A["First Zombella Theme 2"].volume = _fztVolume;

                if (_fztKilled)
                    _firstZombellaBattle = false;
            }
        }
    }

    public void FirstZombellaEntersHall()
    {
        _firstZombellaBattle = true;
        _fztFadeIn = false;
        _fztFadeOut = false;
        _fztVolume = 1;
        S.AM.A["First Zombella Theme 1"].volume = _fztVolume;
        S.AM.A["First Zombella Theme 2"].volume = _fztVolume;
        S.AM.A["First Zombella Theme 1"].time = 0;
        S.AM.A["First Zombella Theme 2"].time = 0;
        S.AM.Play("First Zombella Theme 1");
    }

    public void PlayerMeetFirstZombella()
    {
        _fztFadeIn = true;
        _fztFadeOut = false;

        S.AM.A["First Zombella Theme 1"].volume = _fztVolume;
        S.AM.A["First Zombella Theme 2"].volume = _fztVolume;

        if (!_firstZombellaBattle)
        {
            _firstZombellaBattle = true;
            if (!S.AM.A["First Zombella Theme 1"].isPlaying && !S.AM.A["First Zombella Theme 2"].isPlaying)
            {
                S.AM.Play("First Zombella Theme 1"); /////////////////
            }
        }
    }

    public void PlayerLeavesFirstZombella()
    {
        _fztFadeOut = true;
        _fztFadeIn = false;
    }

    public void PlayerKillsFirstZombella()
    {
        _fztFadeOut = true;
        _fztFadeIn = false;
        _fztKilled = true;
    }

    public void EnterToilet()
    {
        if (_toiletPhase == MusicPhase.Silence)
        {
            _toiletPhase = MusicPhase.Entering;
            _playerInToilet = true;
            S.AM.A["Toilet Music 1"].volume = 0;
            S.AM.A["Toilet Music 2"].volume = 0;
            S.AM.A["Toilet Music 1"].time = 0;
            S.AM.A["Toilet Music 2"].time = 0;
            S.AM.A["Toilet Music 1"].Play();
        }
        else if (_toiletPhase == MusicPhase.Leaving)
        {
            _toiletPhase = MusicPhase.Entering;
        }
    }

    public void DeepEnterToilet()
    {
        _toiletPhase = MusicPhase.Inside;
        S.AM.A["Toilet Music 1"].volume = 1;
        S.AM.A["Toilet Music 2"].volume = 1;
    }

    public void LeaveToilet()
    {
        if (_toiletPhase != MusicPhase.Silence)
            _toiletPhase = MusicPhase.Leaving;
    }
}