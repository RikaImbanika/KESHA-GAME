using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround2 : MonoBehaviour
{
    public GameObject playerObject;
    public CheckGround checkGround;

    private void OnTriggerStay(Collider other)
    {
        if (other != playerObject)
            if (!other.isTrigger)
                checkGround.grounded = 5;
    }
}
