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

    void Start()
    {
        if (_active)
        {
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
        if (_active)
        {
            Vector3 direction = Camera.main.transform.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            _sparkleVis.transform.rotation = lookRotation;

            transform.localScale *= (1 - _minimisingSpeed * Time.deltaTime * Random.Range(0f, 2f));

            if (transform.localScale.x < _minScale)
                Destroy(gameObject);
        }
    }
}
