using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletEnterTrigger : MonoBehaviour
{
	private AudioManager _audioManager;

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			if (_audioManager == null)
			{
				GameObject go = GameObject.FindGameObjectWithTag("AudioManager");
				_audioManager = go.GetComponent<AudioManager>();
			}

			_audioManager.EnterToilet();
		}
	}
}
