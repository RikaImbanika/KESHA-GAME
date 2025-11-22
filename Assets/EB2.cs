using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB2 : MonoBehaviour
{
    void Update()
    {
        try
        {
            gameObject.transform.LookAt(S.Camera.transform.position);
            gameObject.transform.Rotate(0f, 0f, Random.Range(-180f, 180f));
        }
        catch
        {
            ////////////////////////
        }
    }
}
