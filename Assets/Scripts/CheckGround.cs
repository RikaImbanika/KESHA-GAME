using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    public GameObject playerObject;
    public PlayerMovement playerMovement;
    public int grounded;

    public void Update()
	{
        if (grounded > 0)
        {
            playerMovement.grounded = true;
            grounded--;
        }
        else
            playerMovement.grounded = false;
	}

    private void OnTriggerStay(Collider other)
    {
        if (other != playerObject)
            if (!other.isTrigger)
                grounded = 5;
    }
}
