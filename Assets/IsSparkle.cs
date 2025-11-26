using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsSparkle : MonoBehaviour
{
    public bool _active;
    private Rigidbody _rb;
    public float _speed;
    public float _moveNormal;
    public float _moveRandom;
    public GameObject _sparkleVis;
    public float _minScale;
    public float _minimisingSpeed;
    private float _updateInterval;
    private float _deltaTime;
    private float _fpsDeltaTime;
    private int _fpsD;
    private bool _playerInScene;
    private bool _needUpdate;
    private float _minFps = 1 / 12f;
    private float _maxFps = 1 / 120f;
    private float _decreaser;
    private int _counter;

    void Start()
    {
        if (_active)
        {
            _decreaser = _minimisingSpeed * (Random.Range(0f, 0.4f) +
            Random.Range(0f, 0.4f) +
            Random.Range(0f, 0.4f) +
            Random.Range(0f, 0.4f) +
            Random.Range(0f, 0.4f));

            _rb = gameObject.GetComponent<Rigidbody>();
            _rb.AddForce(transform.forward * Random.Range(0, _speed));

            _rb.AddForce(transform.right * _speed * Random.Range(-1f, 1f));
            _rb.AddForce(transform.up * _speed * Random.Range(-1f, 1f));
            _rb.AddForce(transform.forward * _speed * Random.Range(0f, 1f));

            Vector3 forward = transform.forward.normalized * _moveNormal;
            Vector3 randomDirection = Random.insideUnitSphere * _moveRandom;

            transform.position += forward + transform.right * randomDirection.x + transform.up * randomDirection.y;

            float scale = Random.Range(0.1f, 1f);
            transform.localScale = new Vector3(transform.localScale.x * scale, transform.localScale.y * scale, transform.localScale.z * scale);
        }
    }

    void Update()
    {
        if (FPS())
        {
            Do();
            _deltaTime = 0;
        }

        void Do()
        {
            if (_active)
            {
                if (_counter == 0)
                {
                    Vector3 direction = Camera.main.transform.position - transform.position;
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    _sparkleVis.transform.rotation = lookRotation;

                    _decreaser += _deltaTime * 0.2f;
                }

                _counter++;

                if (_counter >= 4)
                    _counter = 0;

                transform.localScale *= 1 - _deltaTime * _decreaser;

                if (transform.localScale.x < _minScale)
                    Destroy(gameObject);
            }
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
            if (S.PS == null)
                return false;

            bool buf = S.PS._currentSceneName == gameObject.scene.name;

            if (!_playerInScene && buf)
            {
                _needUpdate = true;
                _counter = 0;
            }

            _playerInScene = buf;

            float coef3 = 20f;

            if (_playerInScene)
                coef3 = 1f;

            return _deltaTime > _updateInterval * coef3;
        }
    }
}
