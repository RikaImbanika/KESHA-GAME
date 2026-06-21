// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using System;

public class Button1 : MonoBehaviour
{
	string _sceneName;
	string _key;

	private bool _pressed;
	private bool _finished;
	public GameObject _obj;
	private float _startY;
	public float _deltaY;
	private float _finalDeltaY;
	public Camera _camera;
	private Camera _playerCamera;
	public DoorLocal _doorLocal;
	public ToiletZombie _toiletZombie;
	public float _startTime;

	// ---------- Добавленные поля ----------
	[Header("Camera Animation")]
	public Vector3 _сameraTargetPosition;    // конечная позиция камеры
	public Vector3 _сameraTargetRotation;    // конечный поворот (углы Эйлера)

	private Vector3 _cameraStartPosition;
	private Quaternion _cameraStartRotation;
	// ---------------------------------------

	public void Start()
	{
		//-10.62 too low
		//-10.64 too high
		//-10.63 too low
		//-10.635 too low
		//-10.6375 too much
		_finalDeltaY = -10.63625f; //Hardcode but correct +-
		_startY = _obj.transform.position.y;

		_camera.depth = -100;

		_sceneName = gameObject.scene.name;
		_key = _sceneName + transform.position.x + transform.position.y + transform.position.z;

		bool pressed = S.SaveManager.CurrentSave.LoadBool("Button1Pressed") ?? false;

		if (pressed)
		{
			_pressed = true;
			_finished = true;
			_toiletZombie._active = false;
			_obj.transform.position = new Vector3(_obj.transform.position.x, _startY + _finalDeltaY, _obj.transform.position.z);
		}
	}

	public void Go(Camera playerCamera)
	{
		if (!_pressed)
			StartCoroutine(WaitLoad());

		IEnumerator WaitLoad()
		{
			_toiletZombie._active = false;

			_playerCamera = playerCamera;

			_camera.gameObject.SetActive(true);
			_camera.depth = 100;
			_playerCamera.gameObject.SetActive(false);

			S.Camera = _camera;

			_startY = _obj.transform.position.y;

			S.AM.Play("Gong");
			S.AM.Play("Toilet");

			_doorLocal.Close();

			while (!_doorLocal.Closed)
				yield return new WaitForSeconds(0.1f);

			_startTime = Time.time;
			_pressed = true;

			// Захват начальных позиции и поворота камеры
			_cameraStartPosition = _camera.transform.position;
			_cameraStartRotation = _camera.transform.rotation;

			S.SaveManager.CurrentSave.SaveBool("Button1Pressed", true);

			yield return null;
		}
	}

	public void Update()
	{
		if (_pressed && !_finished)
		{
			float nowTime = Time.time;
			float deltaTime = nowTime - _startTime;

			float duration = 3.5f;

			// --- Анимация камеры (линейная) ---
			if (_camera != null)
			{
				float tCam = Mathf.Clamp01(deltaTime / duration);
				_camera.transform.position = Vector3.Lerp(_cameraStartPosition, _сameraTargetPosition, tCam);
				_camera.transform.rotation = Quaternion.Lerp(_cameraStartRotation, Quaternion.Euler(_сameraTargetRotation), tCam);
			}
			// ---------------------------------

			float t = deltaTime / duration;
			float t2 = (-Mathf.Cos(t * 180 * Mathf.Deg2Rad) + 1) / 2f;

			if (t < 1)
			{
				_obj.transform.position = new Vector3(_obj.transform.position.x, _startY + _finalDeltaY * t2, _obj.transform.position.z);
			}
			else
			{
				_obj.transform.position = new Vector3(_obj.transform.position.x, _startY + _finalDeltaY, _obj.transform.position.z);
				_finished = true;

				_camera.gameObject.SetActive(false);
				_camera.depth = -100;
				_playerCamera.gameObject.SetActive(true);

				S.Camera = _playerCamera;

				Thread.Sleep(500);
			}
		}
	}
}