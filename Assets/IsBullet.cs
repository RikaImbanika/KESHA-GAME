using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsBullet : MonoBehaviour
{
    public bool _active;
    public float _speed;

    void Update()
    {
        if (_active)
        {
            float moveDistance = _speed * Time.deltaTime;
            transform.Translate(Vector3.forward * moveDistance);
        }
    }
}
