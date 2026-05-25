using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parent : MonoBehaviour
{
    public GameObject _parent;
    Vector3 _deltaPosition;

    void Start()
    {
        _deltaPosition = gameObject.transform.position - _parent.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _parent.transform.position + _deltaPosition;
    }
}
