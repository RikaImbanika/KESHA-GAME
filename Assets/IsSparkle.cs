using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsSparkle : MonoBehaviour
{
    public float _speed;
    public float _moveNormal;
    public float _moveRandom;
    public GameObject _sparkleVis;
    public float _minScale;
    public float _minimisingSpeed;

    [Header("Privates")]
    private Rigidbody _rb;
    private float _decreaser;
    private int _counter;

    private Optimiser _opti;

    void Start()
    {
        _opti = new Optimiser(gameObject.scene.name);

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

    void Update()
    {
        if (_opti.Optimise(transform.position))
        {
            Do();
            _opti.Reset();
        }

        void Do()
        {
            if (_counter == 0)
            {
                Vector3 direction = Camera.main.transform.position - transform.position;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                _sparkleVis.transform.rotation = lookRotation;

                _decreaser += _opti.DeltaTime * 0.2f;
            }

            _counter++;

            if (_counter >= 4)
                _counter = 0;

            transform.localScale *= 1 - _opti.DeltaTime * _decreaser;

            if (transform.localScale.x < _minScale)
                Destroy(gameObject);
        }
    }
}