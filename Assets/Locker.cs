using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour
{
	public IsDoor _door;
	public string _correctKey;

	private AudioManager _audioManager;
	AllFather _allFather;
	string _id;
	Inventory _inventory;

	public void Start()
	{
		_inventory = GameObject.Find("Canvas").GetComponent<Inventory>();

		if (_audioManager == null)
		{
			GameObject go = GameObject.FindGameObjectWithTag("AudioManager");
			_audioManager = go.GetComponent<AudioManager>();
		}

		_allFather = GameObject.Find("AllFather").GetComponent<AllFather>();
		_id = "" + transform.position.x + transform.position.y + transform.position.z;

		if (_allFather.Contains(_id))
		{
			Save s = _allFather.Load(_id);

			if (s._destroyed)
				Destroy(gameObject);
		}
	}

	public void Cheat()
	{
		Unlock(_correctKey);
	}

	public void Unlock(string key)
	{
		if (key == _correctKey)
		{		
			_inventory.Remove(key, 1);

			if (_door != null)
				_door.ToggleLock(false);

			Save s = new Save();
			if (_allFather.Contains(_id))
				s = _allFather.Load(_id);

			s._destroyed = true;

			_allFather.Save(_id, s);

			_audioManager.Play("kill", 1);

			Destroy(gameObject);
		}
		else
			_audioManager.Play("notEnoughCash", 1);
	}
}
