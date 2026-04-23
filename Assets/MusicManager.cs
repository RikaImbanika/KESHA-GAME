using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public bool _playerOnIncome;
    public string _incomeSwapPhase;
    public string _incomePhase;
    public string _backroomsPhase = "silence";
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
    public string _mushroomsPhase = "silence";

    void Start()
    {
        S.MM = this;

        _fztVolume = 0;
        _playerOnIncome = true;
        _incomeSwapPhase = "1";
        _backroomsTrackId = 0;
        _backroomsPrevTrackId = 11; //Track here should not be equals first one
        _mushroomsPrevTrackId = 1; //

        StartCoroutine(LateStart(0.3f));

        IEnumerator LateStart(float waitTime)
        {
            while (S.AudioManager == null)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log("MusicManager waiting for S.AudioManager");
            }

            int count = 7;
            int count2 = 12;

            _backroomsVolumes = new float[count];
            _backroomsVolumes[0] = 1;

            _backroomsSources = new AudioSource[count];
            _backroomsLengthes = new float[count];
                        
            _backroomsSources[0] = S.AM._goodTimes;
            _backroomsLengthes[0] = 310;

            _backroomsSources[1] = S.AM._rainbow;
            _backroomsLengthes[1] = 300;
                        
            _backroomsSources[2] = S.AM._adelaidaOST1;
            _backroomsLengthes[2] = 173;
                        
            _backroomsSources[3] = S.AM._fenomen;
            _backroomsLengthes[3] = 674;
                        
            _backroomsSources[4] = S.AM._riddik;
            _backroomsLengthes[4] = 1116;

            _backroomsSources[5] = S.AM._labyrinth;
            _backroomsLengthes[5] = 597;

            _backroomsSources[6] = S.AM._greatMix;
            _backroomsLengthes[6] = 2378;

            bool[] remember = new bool[count2];
            _backroomsOrder = new int[count2];

            var rnd = new System.Random();

            //Ordering

            //It's pretty complex but this
            //Only my own logic of music
            //I can't explain it

            _backroomsOrder[0] = 0;
            _backroomsOrder[1] = 1;

            if (rnd.Next(10) < 4)
            {
                _backroomsOrder[0] = 1;
                _backroomsOrder[1] = 0;
            }

            int[] localOrder = new int[3];
            localOrder[0] = 2;
            localOrder[1] = 3;
            localOrder[2] = 4;

            S.AllFather.Shuffle(localOrder);

            _backroomsOrder[2] = localOrder[0];

            _backroomsOrder[3] = _backroomsOrder[0];
            _backroomsOrder[4] = _backroomsOrder[1];

            _backroomsOrder[5] = localOrder[1];

            _backroomsOrder[6] = _backroomsOrder[0];

            _backroomsOrder[7] = localOrder[2];

            _backroomsOrder[8] = _backroomsOrder[1];

            _backroomsOrder[9] = 5;

            _backroomsOrder[10] = rnd.Next(2);

            _backroomsOrder[11] = 6;

            //Play

            S.AM._incomeOST1.Play();

            //MR

            _mushroomsVolumes = new float[2];
            _mushroomsVolumes[0] = 1;

            _mushroomsSources = new AudioSource[2];
            _mushroomsSources[0] = S.AM._maylo;
            _mushroomsSources[1] = S.AM._theRoom;

            _mushroomsLengthes = new float[2];
            _mushroomsLengthes[0] = 282;
            _mushroomsLengthes[1] = 128;
        }
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

        Income(d);
        Backrooms(d);
        Mushrooms(d);
    }

    public void Mushrooms(float d)
    {
        if (_mushroomsPhase == "silence")
            return;

        if (_mushroomsPhase == "leaving")
        {
            if (_mushroomVolume > 0)
                _mushroomVolume -= 0.005f * d;
            else
            {
                _mushroomsPhase = "silence";
                _mushroomVolume = 0;
                _mushroomsSources[_mushroomsTrackId].Pause();
                _mushroomsSources[_mushroomsPrevTrackId].Pause();
            }
        }
        else if (_mushroomsPhase == "entering")
        {
            _mushroomVolume += 0.005f * d;

            if (_mushroomVolume > 1)
            {
                _mushroomVolume = 1;
                _mushroomsPhase = "entered";
            }
        }

        if (_mushroomsSources[_mushroomsTrackId].time > _mushroomsLengthes[_mushroomsTrackId])
        {
            _mushroomsPrevTrackId = _mushroomsTrackId;

            _mushroomsTrackId += 1;
            if (_mushroomsTrackId >= 2)
                _mushroomsTrackId = 0;

            _mushroomsSources[_mushroomsTrackId].time = 0;
            _mushroomsVolumes[_mushroomsTrackId] = 1;
            _mushroomsSources[_mushroomsTrackId].volume = 1 * _mushroomVolume;
            _mushroomsSources[_mushroomsTrackId].Play();
        }

        if (_mushroomsVolumes[_mushroomsPrevTrackId] > 0)
            _mushroomsVolumes[_mushroomsPrevTrackId] -= 0.005f * d;
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
        if (_mushroomsPhase == "silence")
            _mushroomsSources[_mushroomsTrackId].Play();

        _mushroomsPhase = "entering";
    }

    public void LeaveMushrooms()
    {
        _mushroomsPhase = "leaving";
    }

    public void Backrooms(float d)
    {
        if (_backroomsPhase == "silence")
            return;

        int trackIdShuffled = _backroomsOrder[_backroomsTrackId];
        int prevTrackIdShuffled = _backroomsOrder[_backroomsPrevTrackId];

        if (_backroomsPhase == "leaving")
        {
            if (_backroomVolume > 0)
                _backroomVolume -= 0.005f * d;
            else
            {
                _backroomsPhase = "silence";
                _backroomVolume = 0;
                _backroomsSources[trackIdShuffled].Pause();
                _backroomsSources[prevTrackIdShuffled].Pause();
            }
        }
        else if (_backroomsPhase == "entering")
        {
            _backroomVolume += 0.005f * d;

            if (_backroomVolume > 1)
            {
                _backroomVolume = 1;
                _backroomsPhase = "entered";
            }
        }

        if (_backroomsSources[trackIdShuffled].time > _backroomsLengthes[trackIdShuffled])
        {
            _backroomsPrevTrackId = _backroomsTrackId;

            _backroomsTrackId += 1;
            if (_backroomsTrackId >= _backroomsOrder.Count())
                _backroomsTrackId = 0;

            trackIdShuffled = _backroomsOrder[_backroomsTrackId];
            prevTrackIdShuffled = _backroomsOrder[_backroomsPrevTrackId];

            _backroomsSources[trackIdShuffled].time = 0;
            _backroomsVolumes[trackIdShuffled] = 1;
            _backroomsSources[trackIdShuffled].volume = 1 * _backroomVolume;
            _backroomsSources[trackIdShuffled].Play();
        }

        if (_backroomsVolumes[prevTrackIdShuffled] > 0)
            _backroomsVolumes[prevTrackIdShuffled] -= 0.005f * d;
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

        if (_backroomsPhase == "silence")
        {
            _backroomsVolumes[trackIdShuffled] = 1;
            _backroomsSources[trackIdShuffled].volume = 1 * _backroomVolume;

            _backroomsSources[trackIdShuffled].Play();
        }

        _backroomsPhase = "entering";
    }

    public void LeaveBackrooms()
    {
        _backroomsPhase = "leaving";
    }

    public void EnterIncome()
    {
        _incomePhase = "entering";

        if (_incomeSwapPhase == "1" || _incomeSwapPhase == "2to1")
        {
            if (!S.AM._incomeOST1.isPlaying)
            {
                S.AM._incomeOST1.Play();
                S.AM._incomeOST1.time = _incomeTime;
            }
        }
        else if (_incomeSwapPhase == "2" || _incomeSwapPhase == "1to2")
        {
            if (!S.AM._incomeOST2.isPlaying)
            {
                S.AM._incomeOST2.Play();
                S.AM._incomeOST2.time = _incomeTime;
            }
        }
    }

    public void LeaveIncome()
    {
        _incomePhase = "leaving";
    }

    public void Income(float d)
    {
        if (_incomePhase == "leaving")
        {
            if (S.AM._incomeOST1.volume > 0)
                S.AM._incomeOST1.volume -= 0.005f * d;
            if (S.AM._incomeOST2.volume > 0)
                S.AM._incomeOST2.volume -= 0.005f * d;

            if (S.AM._incomeOST2.volume <= 0 && S.AM._incomeOST1.volume <= 0)
            {
                _incomePhase = "leaved";

                if (_incomeSwapPhase == "1" || _incomeSwapPhase == "2to1")
                    _incomeTime = S.AM._incomeOST1.time;
                else
                    _incomeTime = S.AM._incomeOST2.time;

                S.AM._incomeOST1.Pause();
                S.AM._incomeOST2.Pause();
            }
        }
        else if (_incomePhase == "entering")
        {
            if (_incomeSwapPhase == "1" || _incomeSwapPhase == "2to1")
            {
                S.AM._incomeOST1.volume += 0.005f * d;

                if (S.AM._incomeOST1.volume >= 1)
                    _incomePhase = "entered";
            }
            else if (_incomeSwapPhase == "2" || _incomeSwapPhase == "1to2")
            {
                S.AM._incomeOST2.volume += 0.005f * d;

                if (S.AM._incomeOST2.volume >= 1)
                    _incomePhase = "entered";
            }
        }
        else
        {
            if (_incomeSwapPhase == "1")
                if (S.AM._incomeOST1.time > 197)
                {
                    _incomeSwapPhase = "1to2";
                    S.AM._incomeOST2.time = 0;
                    S.AM._incomeOST2.volume = 1;
                    S.AM._incomeOST2.Play();
                }

            if (_incomeSwapPhase == "2")
                if (S.AM._incomeOST2.time > 197)
                {
                    _incomeSwapPhase = "2to1";
                    S.AM._incomeOST1.time = 0;
                    S.AM._incomeOST1.volume = 1;
                    S.AM._incomeOST1.Play();
                }

            if (_incomeSwapPhase == "1to2")
            {
                S.AM._incomeOST1.volume -= 0.003f * d;
                if (S.AM._incomeOST1.volume <= 0)
                {
                    S.AM._incomeOST1.Stop();
                    _incomeSwapPhase = "2";
                }
            }

            if (_incomeSwapPhase == "2to1")
            {
                S.AM._incomeOST2.volume -= 0.003f * d;
                if (S.AM._incomeOST2.volume <= 0)
                {
                    S.AM._incomeOST2.Stop();
                    _incomeSwapPhase = "1";
                }
            }
        }
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
            float distance = Vector3.Distance(_toiletCenter, S.Camera.transform.position);
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
        if (string.IsNullOrWhiteSpace(_toiletPhase) || _toiletPhase == "silence")
        {
            _toiletPhase = "entering";
            _playerInToilet = true;
            S.AM._toiletMusic1.volume = 0;
            S.AM._toiletMusic2.volume = 0;
            S.AM._toiletMusic1.time = 0;
            S.AM._toiletMusic2.time = 0;
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
