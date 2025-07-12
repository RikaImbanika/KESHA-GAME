using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HorisontalToPlayer : MonoBehaviour
{
    void Update()
    {
        if (S.Camera == null)
            return;

        Vector3 p = S.Camera.transform.position;
        p = new Vector3(p.x, transform.position.y, p.z);
        gameObject.transform.LookAt(p);
    }
}
