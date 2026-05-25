using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondCam : MonoBehaviour
{
    void Start()
    {
        //Camera.main.depth = 1;
        Camera itSelf = gameObject.GetComponent<Camera>();
        itSelf.depth = 0;
    }
}
