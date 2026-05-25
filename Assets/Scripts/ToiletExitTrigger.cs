using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletExitTrigger : MonoBehaviour
{
	private void OnTriggerEnter(Collider collider)
	{
		Go(collider.gameObject);
	}

	public void Go(GameObject playerObject)
	{
		if (playerObject.tag == "Player")
			S.MM.LeaveToilet();
	}
}
