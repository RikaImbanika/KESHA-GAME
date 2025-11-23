using UnityEngine;
using System.Collections;

public class LocalLaser : MonoBehaviour
{
    [Header("Timings")]
    public float _forwardTime = 2f;
    public float _forwardPause = 0.5f;
    public float _backwardTime = 2f;
    public float _backwardPause = 0.5f;
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

    void Start()
    {
        _forwardPosition = transform.position + _movementDirection * _forwardDistance;
        _backwardPosition = transform.position - _movementDirection * _backwardDistance;

        _leftLaserObj = Instantiate(S.RedLaser, transform);
        _rightLaserObj = Instantiate(S.RedLaser, transform);

        InitLaserBots();
        UpdateLaserVisuals();
    }

    void Update()
    {
        UpdateMovement();
        UpdateLaserVisuals();
    }

    void UpdateMovement()
    {
        _currentTime += Time.deltaTime;

        switch (_currentState)
        {
            case 0:
                MoveToTarget(0);
                break;

            case 1:
                if (_currentTime >= _forwardPause)
                {
                    _currentState = 2;
                    _currentTime = 0f;
                }
                break;

            case 2:
                MoveToTarget(2);
                break;

            case 3:
                if (_currentTime >= _backwardPause)
                {
                    _currentState = 0;
                    _currentTime = 0f;
                }
                break;
        }
    }

    void MoveToTarget(byte state)
    {
        float t = _currentTime / GetMoveTime(state);
        t = Mathf.Clamp01(t);

        //Sinusoidal
        float smoothed = Mathf.SmoothStep(0f, 1f, t);
        if (state == 0)
           transform.position = Vector3.Lerp(_forwardPosition, _backwardPosition, smoothed);
        else
           transform.position = Vector3.Lerp(_backwardPosition, _forwardPosition, smoothed);

        if (_currentTime >= GetMoveTime(state))
        {
            if (_currentState == 3)
                _currentState = 0;
            else
                _currentState++;
            _currentTime = 0f;
        }
    }

    float GetMoveTime(byte state)
    {
        if (state == 0)
            return _forwardTime;
        else if (state == 1)
            return _forwardPause;
        else if (state == 2)
            return _backwardTime;
        else if (state == 3)
            return _backwardPause;
        else return 10;
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
        if (Physics.Raycast(from, direction, out hit, _laserLimit))
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
}