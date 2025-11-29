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
    private float _period;
    private float _frequency;
    private int _layerMask;

    private Optimiser _opti;

    void Start()
    {
        _opti = new Optimiser(gameObject.scene.name);
        _opti.MinFps = 1 / 24f;
        
        _period = (_forwardTime + _backwardTime + _forwardPause + _backwardPause);
        _frequency = 2 * MathF.PI / _period;
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
        if (_opti.Optimise(transform.position))
        {
            UpdateMovement();
            UpdateLaserVisuals();

            _opti.Reset();
        }
    }

    void UpdateMovement()
    {
        _currentTime += _opti.DeltaTime;
        float smoothed = (MathF.Sin(_currentTime * _frequency) + 1) / 2f;
        transform.position = Vector3.Lerp(_forwardPosition, _backwardPosition, smoothed);
    }

    void InitLaserBots()
    {
        GameObject botPrefab = Prefabs.Get("LaserBot");

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
                S.PS.Damage(16f * _opti.DeltaTime);

            return hit.point;
        }

        return transform.position + direction * _laserLimit;
    }

    void UpdateSingleLaser(GameObject laserObj, GameObject botObj, Vector3 startPoint, Vector3 endPoint)
    {
        if (laserObj == null) return;

        laserObj.transform.position = startPoint;

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
}