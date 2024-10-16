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
	AllFather _allFather;

	private string _id;

	private void Start()
	{
		_allFather = GameObject.Find("AllFather").GetComponent<AllFather>();

		direction = 0f;
		zrx = transform.rotation.eulerAngles.x;
		zry = transform.rotation.eulerAngles.y;
		zrz = transform.rotation.eulerAngles.z;

		_id = "" + transform.position.x + transform.position.y + transform.position.z;

		if (_allFather.Contains(_id))
		{
			Save s = _allFather.Load(_id);

			if (s._opened)
			{
				if (!_direction)
					cr = -_maxAngle;
				else
					cr = _maxAngle;

				transform.rotation = Quaternion.Euler(zrx, zry, zrz + cr);
			}

			_locked = s._locked;
		}
	}

	public void ToggleLock(bool locked)
	{
		Save s = new Save();

		if (_allFather.Contains(_id))
			s = _allFather.Load(_id);

		_locked = locked;
		s._locked = locked;

		_allFather.Save(_id, s);
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

			Save s = new Save();
			if (_allFather.Contains(_id))
				s = _allFather.Load(_id);

			s._opened = opened;

			_allFather.Save(_id, s);
		}
		else
			_audioManager.Play("notEnoughCash", 1);
	}

	public void Close()
	{
		if (!_direction)
			direction = 1.5f;
		else
			direction = -1.5f;

		Save s = new Save();
		if (_allFather.Contains(_id))
			s = _allFather.Load(_id);

		s._opened = false;

		_allFather.Save(_id, s);
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
