// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Drawer : MonoBehaviour
{
	public bool _opened;
	public float _maxMove;
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
		StartCoroutine(Start0());

		IEnumerator Start0()
		{
			if (_speed == 0)
				_speed = 1;
			_speed *= 0.05f;

			direction = 0f;
			zp = transform.position;

			_id = S.ID("Dr", gameObject);

			while (S.SM == null)
			{
				yield return new WaitForSeconds(0.1f);
				Debug.Log("IsShuffle waiting for S.SaveManager");
			}

			bool opened = S.SM.LoadBool(S.IDM(_id, "opened")) ?? _opened;
			_locked = S.SM.LoadBool(S.IDM(_id, "locked")) ?? _locked;

			if (opened)
			{
				cp = _maxMove;
				transform.position = zp + _3dDirection * _maxMove;
			}
		}
	}

	public void ToggleLock(bool locked)
	{
		_locked = locked;
		S.SM.Save(S.IDM(_id, "locked"), locked);
	}

	public void Move()
	{
		if (!_locked)
		{
			if (_item != null)
				_item.ToggleLock(false);

			bool opened = false;

			if (cp < _maxMove / 2f)
			{
				direction = _speed;
				opened = true;
				S.AudioManager.Play("Drawer Open");
			}
			else
			{
				direction = -_speed;
				opened = false;
				S.AudioManager.Play("Drawer Close");
			}

			S.SM.Save(S.IDM(_id, "opened"), opened);
		}
		else
			S.AudioManager.Play("Not Enough Cash");
	}

	public void Close()
	{
		direction = -_speed;

		S.SM.Save(S.IDM(_id, "opened"), false);
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

			float t = cp / _maxMove;
			float sinT = Mathf.Sin(t * Mathf.PI - Mathf.PI / 2) / 2 + 0.5f;
			transform.position = zp + _3dDirection * (_maxMove * sinT);
		}
	}
}