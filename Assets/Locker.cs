using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour
{
	public DoorLocal _door;
	public string _correctKey;

	string _id;

	public void Start()
	{
        StartCoroutine(Start0());

        IEnumerator Start0()
		{
		
			_id = "" + transform.position.x + transform.position.y + transform.position.z;

			while (S.SM == null)
			{
				yield return new WaitForSeconds(0.1f);
                Debug.Log("Locker waiting for S.SaveManager");
            }

			if (S.SM.LoadBool(S.ID(_id, "destroyed")) ?? false)
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
			S.Inventory.Remove(key, 1);

			if (_door != null)
				_door.ToggleLock(false);

			S.SM.Save(S.ID(_id, "destroyed"), true);

			S.AudioManager.Play("kill", 1);

			Destroy(gameObject);
		}
		else
			S.AudioManager.Play("notEnoughCash", 1);
	}
}
