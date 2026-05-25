using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optimiser
{
    private string _sceneName;
    private float _fpsUpdatePeriod = 0.2f;
    private float _fpsRecalcPeriod = 0.6f;
    private float _maxPeriodForDistance;
    private float _maxPeriodForRotation;
    private float _maxPeriodForScene;
    private float _minPeriod;
    private float _fpsDeltaTime;
    private float _updateInterval;
    private float _recalcDeltaTime;
    private bool _playerReallyInScene;
    private bool _needUpdate;
    private float _deltaTime;
    
    public Optimiser(string sceneName)
    {
        _sceneName = sceneName;
        _fpsUpdatePeriod = 0.2f;
        _fpsRecalcPeriod = 0.6f;
        _maxPeriodForDistance = 1 / 12f;
        _maxPeriodForScene = 2f;
        _maxPeriodForRotation = 1 / 2f;
        _minPeriod = 1 / 120f;
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

    public float MaxPeriodForDistance
    {
        get
        {
            return _maxPeriodForDistance;
        }
        set
        {
            _maxPeriodForDistance = value;
        }
    }

    public float MaxPeriodForScene
    {
        get
        {
            return _maxPeriodForScene;
        }
        set
        {
            _maxPeriodForScene = value;
        }
    }

    public float MaxPeriodForRotation
    {
        get
        {
            return _maxPeriodForRotation;
        }
        set
        {
            _maxPeriodForRotation = value;
        }
    }


    public bool Optimise(Vector3 pos, Vector3? pos2 = null, Vector3? pos3 = null)
    {
        _deltaTime += Time.deltaTime;
        _fpsDeltaTime += Time.deltaTime;
        _recalcDeltaTime += Time.deltaTime;

        Vector3 targetPos = S.PlayerTarget(_sceneName);
        Vector3 targetForward = S.PlayerTargetForward(_sceneName);

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
            float dist = GetDist();
            float t = (dist - lim) / step;

            float smoothed = 1;
            if (t > 0 && S.Camera.fieldOfView > 35f)
                smoothed = Mathf.SmoothStep(0f, 1f, 1f / (t * t));

            float coef1 = Mathf.Lerp(_maxPeriodForDistance, _minPeriod, smoothed);

            float k = GetK();
            float coef2 = 1f;
            if (dist > 30f)
                coef2 = Mathf.Clamp(k, 1f, _maxPeriodForRotation / _minPeriod);

            _updateInterval = coef1 * coef2;

            _needUpdate = false;

            return true;
        }

        float GetDist()
        {
            float a1 = Vector3.Distance(pos, targetPos);

            if (pos2 == null)
                return a1;
            else
            {
                float a2 = Vector3.Distance((Vector3)pos2, targetPos);

                if (pos3 == null)
                    return MathF.Min(a1, a2);
                else
                {
                    float a3 = Vector3.Distance((Vector3)pos3, targetPos);
                    return MathF.Min(MathF.Min(a1, a2), a3);
                }
            }
        }

        float GetK()
        {

            float angle = Vector3.Angle(targetForward, (pos - targetPos)); //.normalised
            //+ visible - invisible
            float dif1 = S.Camera.fieldOfView - angle;

            if (pos2 == null)
                return 0.5f - dif1 / 30f; //degrees
            else
            {
                angle = Vector3.Angle(targetForward, (Vector3)pos2 - targetPos); //.normalised
                                                                                                                  //+ visible - invisible
                float dif2 = S.Camera.fieldOfView - angle;

                if (pos3 == null)
                {
                    if (dif1 < dif2)
                        return 0.5f - dif1 / 30f; //degrees
                    else
                        return 0.5f - dif2 / 30f; //degrees
                }
                else
                {
                    angle = Vector3.Angle(targetForward, (Vector3)pos3 - targetPos); //.normalised
                                                                                                                      //+ visible - invisible
                    float dif3 = S.Camera.fieldOfView - angle;

                    if (dif1 < dif2)
                        if (dif1 < dif3)
                            return 0.5f - dif1 / 30f; //degrees
                        else
                            return 0.5f - dif3 / 30f; //degrees
                    else
                        if (dif2 < dif3)
                            return 0.5f - dif2 / 30f; //degrees
                        else
                            return 0.5f - dif3 / 30f; //degrees
                }
            }
        }

        bool Check()
        {
            if (S.PS == null)
                return false;

            bool playerReallyInScene = S.PS._currentSceneName == _sceneName;

            if (!_playerReallyInScene && playerReallyInScene)
                _needUpdate = true;

            _playerReallyInScene = playerReallyInScene;

            bool playerInScene = _playerReallyInScene || S.FakePlayerScene == _sceneName;

            if (playerInScene)
                return _deltaTime > _updateInterval;
            else
                return _deltaTime > _maxPeriodForScene;
        }
    }
    
    public void Reset()
    {
        _deltaTime = 0;
    }
}