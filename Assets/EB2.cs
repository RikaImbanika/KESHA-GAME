using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB2 : MonoBehaviour
{
    private Optimiser _opti;

    void Start()
    {
        _opti = new Optimiser(gameObject.scene.name);
    }
    
    void Update()
    {
        if (_opti.Optimise(transform.position))
        {
            gameObject.transform.LookAt(S.Camera.transform.position);
            gameObject.transform.Rotate(0f, 0f, Random.Range(-180f, 180f));
            _opti.Reset();
        }
    }
}