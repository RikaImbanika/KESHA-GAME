using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IsShuffle : MonoBehaviour
{
	public float _maxMove;
	public string _audioName;
	public bool _locked;
	public Vector3 _3dDirection;
	public float _speed;

	public ItemP _item;

	float direction;
	Vector3 zp;
	float cp;

	private string _id;

	private void Start()
	{
		if (_speed == 0)
			_speed = 1;
		_speed *= 0.05f;

		direction = 0f;
		zp = transform.position;

		_id = "" + transform.position.x + transform.position.y + transform.position.z;

		if (S.AllFather.Contains(_id))
		{
			Save s = S.AllFather.Load(_id);

			if (s._opened)
			{
				cp = _maxMove;
				transform.position = zp + _3dDirection * _maxMove;
			}

			_locked = s._locked;
		}
	}

	public void ToggleLock(bool locked)
	{
		Save s = new Save();

		if (S.AllFather.Contains(_id))
			s = S.AllFather.Load(_id);

		_locked = locked;
		s._locked = locked;

		S.AllFather.Save(_id, s);
	}

	public void Move()
	{
		if (!_locked)
		{
			if (_item != null)
				_item.ToggleLock(false);

			S.AudioManager.Play(_audioName, 1);			

			bool opened = false;

			if (cp < _maxMove / 2f)
			{
				direction = _speed;
				opened = true;
			}
			else
			{
				direction = -_speed;
				opened = false;
			}

			Save s = new Save();
			if (S.AllFather.Contains(_id))
				s = S.AllFather.Load(_id);

			s._opened = opened;

			S.AllFather.Save(_id, s);
		}
		else
			S.AudioManager.Play("notEnoughCash", 1);
	}

	public void Close()
	{
		direction = -_speed;

		Save s = new Save();
		if (S.AllFather.Contains(_id))
			s = S.AllFather.Load(_id);

		s._opened = false;

		S.AllFather.Save(_id, s);
	}

	public bool Closed
	{
		get
		{
			return cp < _maxMove / 2f;
		}
	}

	public void Update()
	{
		if (direction != 0)
		{
			if (direction > 0 && cp >= _maxMove)
			{
				direction = 0f;
				if (_item != null)
					_item.ToggleLock(false);
			}
			if (direction < 0 && cp <= 0)
			{
				direction = 0f;
				if (_item != null)
					_item.ToggleLock(true);
			}


			cp += direction * Time.deltaTime * 60;
			transform.position = zp + _3dDirection * cp;
		}
	}
}
