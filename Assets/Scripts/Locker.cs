using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour
{
	public DoorLocal _door;
	public string _correctKey;

	string _id;
	string _idDestroyed;

	public void Start()
	{
		StartCoroutine(Start0());

		IEnumerator Start0()
		{
			GetId();

			while (S.SM == null)
			{
				yield return new WaitForSeconds(0.1f);
				Debug.Log("Locker waiting for S.SaveManager");
			}

			if (S.SM.LoadBool(_idDestroyed) ?? false)
				Destroy(gameObject);
		}
	}
	
	public void GetId()
	{
		_id = S.ID("LK", gameObject);
		_idDestroyed = S.IDM(_id, "destroyed");
	}

	public void Cheat()
	{
		Unlock(_correctKey);
	}

	public void Unlock(string key)
	{
		if (key == _correctKey)
		{		
			S.Inventory.Remove(key, 1);

			if (_door != null)
				_door.ToggleLock(false);

			S.SM.Save(_idDestroyed, true);

			S.AM.Play("Kill", 1);

			Destroy(gameObject);
		}
		else
			S.AM.Play("Not Enough Cash", 1);
	}
}
