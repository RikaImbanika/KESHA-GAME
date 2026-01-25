using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparkle3 : MonoBehaviour
{
    public bool _active; 
    public float _pushNormal;
    public float _pushRandom;
    public float _minScale;
    public float _minimisingSpeed;
    public float _gravity;
    public float _period;
    public int _count;
    public float _width;
    public float _sizeRandom;
    public float _friction;

    [Header("Privates")]
    private Rigidbody _rb;
    private float _decreaser;
    private int _counter;
    private float _p;
    private Vector3 _direction;
    private float _velocity;
    private int _layerMask;
    private GameObject[] _visual;
    private int _prevIndex;
    private int _index;
    private bool _instantiated;
    private int _deadLine;
    private float _deltaTime;


    void Start()
    {
        if (_active)
        {
            _deadLine = -1;

            _layerMask = 1 << LayerMask.NameToLayer("Player") |
            1 << LayerMask.NameToLayer("Static") |
            1 << LayerMask.NameToLayer("Enemies") |
            1 << LayerMask.NameToLayer("Items") |
            1 << LayerMask.NameToLayer("Default");

            _visual = new GameObject[_count];

            _decreaser = _minimisingSpeed * (Random.Range(0f, 0.4f) +
            Random.Range(0f, 0.4f) +
            Random.Range(0f, 0.4f) +
            Random.Range(0f, 0.4f) +
            Random.Range(0f, 0.4f));

            _rb = gameObject.GetComponent<Rigidbody>();

            _direction = transform.right * _pushRandom * Random.Range(-1f, 1f);
            _direction += transform.up * _pushRandom * Random.Range(-1f, 1f);
            _direction += transform.forward * _pushNormal * Random.Range(0f, 1f);

            _velocity = _direction.magnitude;
            Insta();

            _visual[0].transform.position = transform.position + _direction;
            _visual[0].transform.rotation = Quaternion.LookRotation(-_direction);

            _p = _width * Random.Range(1f, _sizeRandom);
            _visual[0].transform.localScale = new Vector3(_p, _p, _velocity);
        }
    }

    void Insta()
    {
        if (!_instantiated)
        {
            if (_index == _count - 1)
                _instantiated = true;

            _visual[_index] = GameObject.Instantiate(S.RedLaser); /////////////////
        }
    }

    bool Dying()
    {
        if (_deadLine == -1)
        {
            if (_p < _minScale)
            {
                _deadLine = _prevIndex;

                Destroy(_visual[_index]);
            }
            else
                return false;
        }
        else
        {
            Destroy(_visual[_index]);

            if (_index == _deadLine)
                Destroy(gameObject); //The End.
        }

        return true;
    }

    void Update()
    {
        Do();

        void Do()
        {
            if (_active)
            {
                _deltaTime += Time.deltaTime;

                if (_deltaTime > _period)
                {
                    _decreaser += _deltaTime * 0.2f;
                    _direction += new Vector3(0, _gravity * _deltaTime, 0); ///

                    _velocity = _direction.magnitude;

                    _p *= 1 - _deltaTime * _decreaser;

                    _prevIndex = _index;
                    _index++;
                    if (_index >= _count)
                        _index = 0;
                    else
                        Insta();

                    if (Dying())
                        return;

                    _visual[_index].transform.localScale = new Vector3(_p, _p, _velocity);

                    Ray ray = new Ray(_visual[_prevIndex].transform.position, _direction);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, _velocity, _layerMask))
                    {
                        _visual[_index].transform.position = hit.point;
                        _visual[_index].transform.transform.rotation = Quaternion.LookRotation(-_direction);
                        _direction = Vector3.Reflect(_direction, hit.normal.normalized) * _friction;
                    }
                    else
                    {
                        _visual[_index].transform.position = _visual[_prevIndex].transform.position + _direction;
                        _visual[_index].transform.transform.rotation = Quaternion.LookRotation(-_direction);
                    }

                    _deltaTime = 0;
                }
            }
        }
    }
}