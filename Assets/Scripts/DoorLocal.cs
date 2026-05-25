using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DoorLocal : MonoBehaviour
{
	public float _maxAngle;
	public string _audioName;
	public bool _locked;
	public bool _direction;

	public ItemP _item;

	float zrx;
	float zry;
	float zrz;
	float cr;
	private AudioManager _audioManager;

	private string _id;

	private bool _isMoving;
	private float _moveStartTime;
	private float _moveDuration;
	private float _startAngle;
	private float _targetAngle;

	private void Start()
	{
		StartCoroutine(Start0());

		IEnumerator Start0()
		{
			zrx = transform.rotation.eulerAngles.x;
			zry = transform.rotation.eulerAngles.y;
			zrz = transform.rotation.eulerAngles.z;

			_id = S.ID("DL", gameObject);

			while (S.SM == null)
			{
				yield return new WaitForSeconds(0.1f);
				Debug.Log("IsDoor waiting for S.SaveManager");
			}

			bool opened = S.SM.LoadBool(S.IDM(_id, "opened")) ?? false;
			if (opened)
			{
				if (!_direction)
					cr = -_maxAngle;
				else
					cr = _maxAngle;

				transform.rotation = Quaternion.Euler(zrx, zry, zrz + cr);
			}

			_locked = S.SM.LoadBool(S.IDM(_id, "locked")) ?? _locked;

			// if (string.IsNullOrEmpty(_audioName))
			// 	_audioName = "toiletDoor";
			_audioName = "Toilet Door"; //TO DO: More sounds.
		}
	}

	public void ToggleLock(bool locked)
	{
		_locked = locked;
		S.SM.Save(S.IDM(_id, "locked"), _locked);
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

			_audioManager.Play("Toilet Door");

			float targetAngle;
			bool opened;
			if (!_direction)
			{
				if (cr < -(_maxAngle / 2))
				{
					targetAngle = 0;
					opened = false;
				}
				else
				{
					targetAngle = -_maxAngle;
					opened = true;
				}
			}
			else
			{
				if (cr > (_maxAngle / 2))
				{
					targetAngle = 0;
					opened = false;
				}
				else
				{
					targetAngle = _maxAngle;
					opened = true;
				}
			}

			// Запускаем синусоидальное движение
			StartMovement(targetAngle);

			S.SM.Save(S.IDM(_id, "opened"), opened);
		}
		else
			_audioManager.Play("Not Enough Cash");
	}

	public void Close()
	{
		StartMovement(0);
		S.SM.Save(S.IDM(_id, "opened"), false);
	}

	private void StartMovement(float targetAngle)
	{
		_startAngle = cr;
		_targetAngle = targetAngle;
		_moveStartTime = Time.time;
		float distance = Mathf.Abs(targetAngle - cr);

		_moveDuration = distance / 90f;

		if (_moveDuration < 0.01f)
		{
			cr = targetAngle;
			transform.rotation = Quaternion.Euler(zrx, zry, zrz + cr);
			_isMoving = false;
			if (targetAngle == 0 && _item != null)
				_item.ToggleLock(true);
		}
		else
		{
			_isMoving = true;
		}
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
		if (_isMoving)
		{
			float t = (Time.time - _moveStartTime) / _moveDuration;
			if (t >= 1f)
			{
				t = 1f;
				_isMoving = false;

				if (_targetAngle == 0 && _item != null)
					_item.ToggleLock(true);
			}

			float smoothT = (1f - Mathf.Cos(t * Mathf.PI)) / 2f;
			cr = Mathf.Lerp(_startAngle, _targetAngle, smoothT);
			transform.rotation = Quaternion.Euler(zrx, zry, zrz + cr);
		}
	}
}