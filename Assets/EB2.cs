using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB2 : MonoBehaviour
{
    private float _updateInterval;
    private float _deltaTime;
    private float _fpsDeltaTime;
    private bool _playerInScene;
    private bool _needUpdate;
    private int _fpsD;
    private float _minFps = 1 / 6f;
    private float _maxFps = 1 / 120f;

    void Update()
    {
        if (FPS())
        {
            gameObject.transform.LookAt(S.Camera.transform.position);
            gameObject.transform.Rotate(0f, 0f, Random.Range(-180f, 180f));
            _deltaTime = 0;
        }
    }

    bool FPS()
    {
        _deltaTime += Time.deltaTime;
        _fpsDeltaTime += Time.deltaTime;

        bool check = Check();

        if (_fpsDeltaTime > 0.2f || _needUpdate)
            return RecalcFPS();
        else
            return check;

        bool RecalcFPS()
        {
            _fpsDeltaTime = 0;
            _fpsD++;

            if (check || _fpsD >= 3)
            {
                _fpsD = 0;

                float lim = 20f;
                float step = 40f;
                float dist = Vector3.Distance(transform.position, S.Ph.transform.position);
                float t = (dist - lim) / step;

                float smoothed = 1;
                if (t > 0 && S.Camera.fieldOfView > 35f)
                    smoothed = Mathf.SmoothStep(0f, 1f, 1f / (t * t));

                float coef1 = Mathf.Lerp(_minFps, _maxFps, smoothed);

                float angle = Vector3.Angle(S.Camera.transform.forward, (transform.position - S.Camera.transform.position).normalized);
                //+ visible - invisible
                float k = 0.5f - (S.Camera.fieldOfView - angle) / 30f; //degrees
                float coef2 = 0.05f;
                if (dist > 30f)
                    coef2 = Mathf.Clamp(k, 0.05f, 1) * 20f;

                _updateInterval = coef1 * coef2;

                _needUpdate = false;

                return true;
            }
            else
                return false;
        }

        bool Check()
        {
            bool buf = S.PS._currentSceneName == gameObject.scene.name;

            if (!_playerInScene && buf)
                _needUpdate = true;

            _playerInScene = buf;

            float coef3 = 20f;

            if (_playerInScene)
                coef3 = 1f;

            return _deltaTime > _updateInterval * coef3;
        }
    }
}
