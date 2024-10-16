using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsRay : MonoBehaviour
{
    public bool _active;
    public Camera _mainCamera;
    public float _timeLeft;

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        if (_active)
        {
            _timeLeft -= Time.deltaTime;

            if (_timeLeft < 0)
                Destroy(gameObject);

            Vector3 a = transform.forward;
            Vector3 b = _mainCamera.transform.position;
            Vector3 projection = Vector3.Dot(a, b) / Vector3.Dot(b, b) * b;

            float angle = Angle(transform.up, projection);
            transform.Rotate(new Vector3(0, 0, angle));

            float Angle(Vector3 vector1, Vector3 vector2)
            {
                float dotProduct = Vector3.Dot(vector1, vector2);
                float magnitude1 = Length(vector1);
                float magnitude2 = Length(vector2);

                float cosAngle = dotProduct / (magnitude1 * magnitude2);
                float angleInRadians = (float)Math.Acos(cosAngle);
                float angleInDegrees = angleInRadians * (180f / (float)Math.PI);

                return angleInDegrees;
            }

            float Length(Vector3 v)
            {
                return MathF.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z); //
            }
        }
    }
}
