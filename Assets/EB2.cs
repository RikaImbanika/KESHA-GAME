using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB2 : MonoBehaviour
{
    public Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        try
        {
            gameObject.transform.LookAt(_camera.transform.position);
            gameObject.transform.Rotate(0f, 0f, Random.Range(-180f, 180f));
        }
        catch
        {
        }
    }
}
