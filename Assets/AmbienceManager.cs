using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AmbienceManager : MonoBehaviour
{
    public string _toiletPhase;
    public string _toiletSwapPhase;
    public float _toiletTime;
    public float _toiletAmbienceDuration;


    void Start()
    {
        _toiletPhase = "silence";
        _toiletSwapPhase = "1";
        _toiletAmbienceDuration = 113.5f; //Do not forget

        S.AmbienceManager = this;
    }

    void Update()
    {
        float time = Time.deltaTime;
        float normalTime = 1 / 60f;
        float d = time / normalTime;

        Bathrooms(d);
    }

    void Bathrooms(float d)
    {
        if (_toiletPhase == "leaving")
        {
            if (S.AM._toiletAmbience1.volume > 0)
                S.AM._toiletAmbience1.volume -= 0.08f * d;
            if (S.AM._toiletAmbience2.volume > 0)
                S.AM._toiletAmbience2.volume -= 0.08f * d;

            if (S.AM._toiletAmbience2.volume <= 0 && S.AM._toiletAmbience1.volume <= 0)
            {
                _toiletPhase = "silence";

                if (_toiletSwapPhase == "1" || _toiletSwapPhase == "2to1")
                    _toiletTime = S.AM._toiletAmbience1.time;
                else
                    _toiletTime = S.AM._toiletAmbience2.time;

                S.AM._toiletAmbience1.Pause();
                S.AM._toiletAmbience2.Pause();
            }
        }
        else if (_toiletPhase == "entering")
        {
            if (_toiletSwapPhase == "1" || _toiletSwapPhase == "2to1")
            {
                S.AM._toiletAmbience1.volume += 0.08f * d;

                if (S.AM._toiletAmbience1.volume >= 1)
                    _toiletPhase = "entered";
            }
            else if (_toiletSwapPhase == "2" || _toiletSwapPhase == "1to2")
            {
                S.AM._toiletAmbience2.volume += 0.08f * d;

                if (S.AM._toiletAmbience2.volume >= 1)
                    _toiletPhase = "entered";
            }
        }
        else
        {
            if (_toiletSwapPhase == "1")
                if (S.AM._toiletAmbience1.time > _toiletAmbienceDuration)
                {
                    _toiletSwapPhase = "1to2";
                    S.AM._toiletAmbience2.time = 0;
                    S.AM._toiletAmbience2.volume = 1;
                    S.AM._toiletAmbience2.Play();
                }

            if (_toiletSwapPhase == "2")
                if (S.AM._toiletAmbience2.time > _toiletAmbienceDuration)
                {
                    _toiletSwapPhase = "2to1";
                    S.AM._toiletAmbience1.time = 0;
                    S.AM._toiletAmbience1.volume = 1;
                    S.AM._toiletAmbience1.Play();
                }

            if (_toiletSwapPhase == "1to2")
            {
                S.AM._toiletAmbience1.volume -= 0.02f * d;
                if (S.AM._toiletAmbience1.volume <= 0)
                {
                    S.AM._toiletAmbience1.Stop();
                    _toiletSwapPhase = "2";
                }
            }

            if (_toiletSwapPhase == "2to1")
            {
                S.AM._toiletAmbience2.volume -= 0.02f * d;
                if (S.AM._toiletAmbience2.volume <= 0)
                {
                    S.AM._toiletAmbience2.Stop();
                    _toiletSwapPhase = "1";
                }
            }
        }
    }

    public void EnterToilet()
    {
        _toiletPhase = "entering";

        if (_toiletSwapPhase == "1" || _toiletSwapPhase == "2to1")
        {
            if (!S.AM._toiletAmbience1.isPlaying)
            {
                S.AM._toiletAmbience1.Play();
                S.AM._toiletAmbience1.time = _toiletTime;
            }
        }
        else if (_toiletSwapPhase == "2" || _toiletSwapPhase == "1to2")
        {
            if (!S.AM._toiletAmbience2.isPlaying)
            {
                S.AM._toiletAmbience2.Play();
                S.AM._toiletAmbience2.time = _toiletTime;
            }
        }
    }
    
    public void LeaveToilet()
    {
        _toiletPhase = "leaving";
    }
}
