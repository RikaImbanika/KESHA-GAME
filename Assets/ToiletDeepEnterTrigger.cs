using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletDeepEnterTrigger : MonoBehaviour
{
	private AudioManager _audioManager;

	private void OnTriggerEnter(Collider collider)
	{
		Go(collider.gameObject);
	}

	public void Go(GameObject playerObject)
	{
		if (playerObject.tag == "Player")
		{
			if (_audioManager == null)
			{
				GameObject go = GameObject.FindGameObjectWithTag("AudioManager");
				_audioManager = go.GetComponent<AudioManager>();
			}

			_audioManager.DeepEnterToilet();
		}
	}
}
