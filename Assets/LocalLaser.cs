using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System;

public class LocalLaser : MonoBehaviour
{
    [Header("Timings")]
    public float _forwardTime = 2f;
    public float _forwardPause = 0f;
    public float _backwardTime = 2f;
    public float _backwardPause = 0f;
    public byte _currentState = 0;
    public float _currentTime;

    [Header("Distance Settings")]
    public Vector3 _movementDirection;
    public float _forwardDistance = 5f;
    public float _backwardDistance = 5f;

    [Header("Laser Settings")]
    public Vector3 _laserDirection;
    public float _laserLimit = 50f;

    [Header("Privates")]
    private Vector3 _forwardPosition;
    private Vector3 _backwardPosition;
    private GameObject _leftLaserObj;
    private GameObject _rightLaserObj;
    private GameObject _leftBotObj;
    private GameObject _rightBotObj;
    private Vector3 _leftDelta;
    private Vector3 _rightDelta;
    private float _updateInterval;
    private float _deltaTime;
    private float _fpsDeltaTime;
    private bool _playerInScene;
    private bool _needUpdate;
    private int _fpsD;
    private float _minFps = 1 / 12f;
    private float _maxFps = 1 / 120f;
    private float _period;
    private float _frequency;
    private int _layerMask;


    void Start()
    {
        _period = (_forwardTime + _backwardTime + _forwardPause + _backwardPause);
        _frequency = 2 * MathF.PI / _period;
        _updateInterval = 0.1f;
        _forwardPosition = transform.position + _movementDirection * _forwardDistance;
        _backwardPosition = transform.position - _movementDirection * _backwardDistance;

        _leftLaserObj = Instantiate(S.RedLaser, transform);
        _rightLaserObj = Instantiate(S.RedLaser, transform);

        _layerMask = 1 << LayerMask.NameToLayer("Player") |
                         1 << LayerMask.NameToLayer("Static") |
                         1 << LayerMask.NameToLayer("Enemies") |
                         1 << LayerMask.NameToLayer("Items") |
                         1 << LayerMask.NameToLayer("Default");

        InitLaserBots();
        UpdateLaserVisuals();
    }

    void Update()
    {
        if (FPS())
        {
            UpdateMovement();
            UpdateLaserVisuals();

            _deltaTime = 0;
        }
    }

    void UpdateMovement()
    {
        _currentTime += _deltaTime;
        float smoothed = (MathF.Sin(_currentTime * _frequency) + 1) / 2f;
        transform.position = Vector3.Lerp(_forwardPosition, _backwardPosition, smoothed);
    }

    void InitLaserBots()
    {
        GameObject botPrefab = Resources.Load<GameObject>($"Prefabs/LaserBot");

        _leftBotObj = Instantiate(botPrefab, transform.position, Quaternion.LookRotation(_laserDirection), transform);
        _leftBotObj.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
        _rightBotObj = Instantiate(botPrefab, transform.position, Quaternion.LookRotation(-_laserDirection), transform);
        _rightBotObj.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

        Vector3 leftHitPoint = GetLaserHitPoint(transform.position, -_laserDirection);
        Vector3 rightHitPoint = GetLaserHitPoint(transform.position, _laserDirection);
        _leftDelta = leftHitPoint - transform.position;
        _rightDelta = rightHitPoint - transform.position;
    }

    void UpdateLaserVisuals()
    {
        Vector3 leftHitPoint = transform.position + _leftDelta;
        Vector3 rightHitPoint = transform.position + _rightDelta;
        Vector3 leftHitPoint2 = GetLaserHitPoint(leftHitPoint, _laserDirection);
        Vector3 rightHitPoint2 = GetLaserHitPoint(rightHitPoint, -_laserDirection);

        UpdateSingleLaser(_leftLaserObj, _leftBotObj, leftHitPoint, leftHitPoint2);
        UpdateSingleLaser(_rightLaserObj, _rightBotObj, rightHitPoint, rightHitPoint2);
    }

    Vector3 GetLaserHitPoint(Vector3 from, Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(from, direction, out hit, _laserLimit, _layerMask))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
                S.PS.Damage(0.60f);

            return hit.point;
        }

        return transform.position + direction * _laserLimit;
    }

    void UpdateSingleLaser(GameObject laserObj, GameObject botObj, Vector3 startPoint, Vector3 endPoint)
    {
        if (laserObj == null) return;

        Vector3 middlePoint = (startPoint + endPoint) / 2f;
        laserObj.transform.position = middlePoint;

        // Rotate laser towards points
        Vector3 direction = endPoint - startPoint;
        if (direction == Vector3.zero)
            return;

        laserObj.transform.rotation = Quaternion.LookRotation(direction);

        // Scale laser by length
        float distance = Vector3.Distance(startPoint, endPoint);
        laserObj.transform.localScale = new Vector3(
            laserObj.transform.localScale.x,
            laserObj.transform.localScale.y,
            distance
        );

        botObj.transform.position = startPoint;
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