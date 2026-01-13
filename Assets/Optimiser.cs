using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optimiser
{
    private string _sceneName;
    private float _fpsUpdatePeriod = 0.2f;
    private float _fpsRecalcPeriod = 0.6f;
    private float _minFps = 1 / 12f;
    private float _maxFps = 1 / 120f;
    private float _ifNotInScene = 0.75f;
    private float _fpsDeltaTime;
    private float _updateInterval;
    private float _recalcDeltaTime;
    private bool _playerInScene;
    private bool _needUpdate;
    private float _deltaTime;
    
    public Optimiser(string sceneName)
    {
        _sceneName = sceneName;
        _fpsUpdatePeriod = 0.2f;
        _fpsRecalcPeriod = 0.6f;
        _minFps = 1 / 12f;
        _maxFps = 1 / 120f;
        _ifNotInScene = 0.75f;
    }
    
    public float DeltaTime
    {
        get
        {
            return _deltaTime;
        }
        set
        {
            _deltaTime = value;
        }
    }
    
    public string SceneName
    {
        get
        {
            return _sceneName;
        }
        set
        {
            _sceneName = value;
        }
    }
    
    public float FpsUpdatePeriod
    {
        get
        {
            return _fpsUpdatePeriod;
        }
        set
        {
            _fpsUpdatePeriod = value;
        }
    }
    
    public float FpsRecalcPeriod
    {
        get
        {
            return _fpsRecalcPeriod;
        }
        set
        {
            _fpsRecalcPeriod = value;
        }
    }

    public float IfNotInScene
    {
        get
        {
            return _ifNotInScene;
        }
        set
        {
            _ifNotInScene = value;
        }
    }

    public float MinFps
    {
        get
        {
            return _minFps;
        }
        set
        {
            _minFps = value;
        }
    }
    
    public float MaxFps
    {
        get
        {
            return _maxFps;
        }
        set
        {
            _maxFps = value;
        }
    }

    public bool Optimise(Vector3 pos)
    {
        _deltaTime += Time.deltaTime;
        _fpsDeltaTime += Time.deltaTime;
        _recalcDeltaTime += Time.deltaTime;

        bool check = Check();

        if (_needUpdate)
        {
            _fpsDeltaTime = 0;
            _recalcDeltaTime = 0;
            RecalcFPS();
            return true;
        }
        else if (_fpsDeltaTime > _fpsUpdatePeriod)
        {
            _fpsDeltaTime = 0;
            if (check)
                RecalcFPS();
            return check;
        }
        else if (_recalcDeltaTime >= _fpsRecalcPeriod)
        {
            _fpsDeltaTime = 0;
            _recalcDeltaTime = 0;
            RecalcFPS();
            return true;
        }
        else
            return check;

        bool RecalcFPS()
        {
            _recalcDeltaTime = 0;

            float lim = 20f;
            float step = 40f;
            float dist = Vector3.Distance(pos, S.Camera.transform.position);
            float t = (dist - lim) / step;

            float smoothed = 1;
            if (t > 0 && S.Camera.fieldOfView > 35f)
                smoothed = Mathf.SmoothStep(0f, 1f, 1f / (t * t));

            float coef1 = Mathf.Lerp(_minFps, _maxFps, smoothed);

            float angle = Vector3.Angle(S.Camera.transform.forward, (pos - S.Camera.transform.position).normalized);
            //+ visible - invisible
            float k = 0.5f - (S.Camera.fieldOfView - angle) / 30f; //degrees
            float coef2 = 0.05f;
            if (dist > 30f)
                coef2 = Mathf.Clamp(k, 0.05f, 1) * 20f;

            _updateInterval = coef1 * coef2;

            _needUpdate = false;

            return true;
        }

        bool Check()
        {
            if (S.PS == null)
                return false;
                
            bool playerInScene = S.PS._currentSceneName == _sceneName;

            if (!_playerInScene && playerInScene)
                _needUpdate = true;

            _playerInScene = playerInScene;

            float coef3 = 20f;

            if (_playerInScene)
                coef3 = 1f;

            if (_playerInScene)
                return _deltaTime > _updateInterval;
            else
                return _deltaTime > _ifNotInScene;
        }
    }
    
    public void Reset()
    {
        _deltaTime = 0;
    }
}