using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomsDeepEnterAgain : MonoBehaviour
{
	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			if (S.SM.LoadBool($"Was in MR") ?? false)
				S.MM.EnterMushrooms();
		}
	}
}
