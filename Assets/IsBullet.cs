using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsBullet : MonoBehaviour
{
    public bool _active;
    public float _speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_active)
        {
            float moveDistance = _speed * Time.deltaTime;
            transform.Translate(Vector3.forward * moveDistance);
        }
    }
}
