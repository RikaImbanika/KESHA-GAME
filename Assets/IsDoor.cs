using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IsDoor : MonoBehaviour
{
	public float _maxAngle;
	public string _audioName;
	public bool _locked;
	public bool _direction;

	public ItemP _item;

	float direction;
	float zrx;
	float zry;
	float zrz;
	float cr;
	private AudioManager _audioManager;

	private string _id;

	private void Start()
	{
		StartCoroutine(Start0());

		IEnumerator Start0()
		{
			direction = 0f;
			zrx = transform.rotation.eulerAngles.x;
			zry = transform.rotation.eulerAngles.y;
			zrz = transform.rotation.eulerAngles.z;

			_id = "" + transform.position.x + transform.position.y + transform.position.z;

			while (S.SM == null)
			{
				yield return new WaitForSeconds(0.1f);
                Debug.Log("IsDoor waiting for S.SaveManager");
            }

			bool opened = S.SM.LoadBool(S.ID(_id, "opened")) ?? false;
			if (opened)
			{
				if (!_direction)
					cr = -_maxAngle;
				else
					cr = _maxAngle;

				transform.rotation = Quaternion.Euler(zrx, zry, zrz + cr);
			}

			_locked = S.SM.LoadBool(S.ID(_id, "locked")) ?? _locked;
		}
	}

	public void ToggleLock(bool locked)
	{
		_locked = locked;
		S.SM.Save(S.ID(_id, "locked"), _locked);
	}

	public void Move()
	{
		if (_audioManager == null)
		{
			GameObject go = GameObject.FindGameObjectWithTag("AudioManager");
			_audioManager = go.GetComponent<AudioManager>();
		}
		if (!_locked)
		{
			if (_item != null)
				_item.ToggleLock(false);

			_audioManager.Play(_audioName, 1);

			Quaternion rotation = transform.rotation;

			bool opened = false;

			if (!_direction)
			{
				if (cr < -(_maxAngle / 2))
				{
					direction = 1.5f;
					opened = false;
				}
				else
				{
					direction = -1.5f;
					opened = true;
				}
			}
			else
			{
				if (cr > (_maxAngle / 2))
				{
					direction = -1.5f;
					opened = false;
				}
				else
				{
					direction = 1.5f;
					opened = true;
				}
			}

			S.SM.Save(S.ID(_id, "opened"), opened);
		}
		else
			_audioManager.Play("notEnoughCash", 1); //
	}

	public void Close()
	{
		if (!_direction)
			direction = 1.5f;
		else
			direction = -1.5f;

        S.SM.Save(S.ID(_id, "opened"), false);
    }

	public bool Closed
	{
		get
		{
			if (!_direction)
				return cr >= 0;
			else
				return cr <= 0;
		}
	}

	public void Update()
	{
		if (direction != 0)
		{
			if (!_direction)
			{
				if (direction > 0 && cr >= 0)
				{
					direction = 0f;
					if (_item != null)
						_item.ToggleLock(true);
				}
				if (direction < 0 && cr <= -_maxAngle)
					direction = 0f;
			}
			else
			{
				if (direction < 0 && cr <= 0)
				{
					direction = 0f;
					if (_item != null)
						_item.ToggleLock(true);
				}
				if (direction > 0 && cr >= _maxAngle)
					direction = 0f;
			}

			cr += direction * Time.deltaTime * 60;
			transform.rotation = Quaternion.Euler(zrx, zry, zrz + cr);
		}
	}
}
