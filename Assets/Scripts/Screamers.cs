using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screamers : MonoBehaviour
{
    public float _timeBeforeNextScreamerLeft;
    public float _lastDelay;
    public float _lastScreamerTime;
    public int _queueSize;

    void Start()
    {
        S.Screamers = this;
    }

    public bool TryToScream()
    {
        if (_queueSize < 3)
            _timeBeforeNextScreamerLeft = _lastDelay - (Time.time - _lastScreamerTime);
        else if (_queueSize < 10)
            _timeBeforeNextScreamerLeft = _lastDelay / 2f - (Time.time - _lastScreamerTime);
        else
            _timeBeforeNextScreamerLeft = _lastDelay / 3f - (Time.time - _lastScreamerTime);

        if (_timeBeforeNextScreamerLeft < 0)
        {
            _lastDelay = GetDelay();
            _lastScreamerTime = Time.time;
            _queueSize = 0;
            return true;
        }
        else
        {
            _queueSize++;
            return false;
        }
    }

    public float GetDelay()
    {
        return 5f + Random.Range(0, 55f);
    }
}
