// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using System;
using Unity.VisualScripting;

public class Door : MonoBehaviour
{
	public int _number;
	public Collider col;
	public string _audioName;
	public bool _locked;
	public Stamp _stamp;
	public int _sparklesCount;
	public DoorModel _doorModel;
	private string _sceneName;
	private bool _needArrow;
	private bool _arrowPlaced;

	public void Start()
	{
		_sparklesCount = 75;

		// if (string.IsNullOrEmpty(_audioName))
		// 	_audioName = "Door";
		_audioName = "Door"; //TO DO: More sounds.

		StartCoroutine(Wait());

		IEnumerator Wait()
		{
			_sceneName = gameObject.scene.name;

			float delay = UnityEngine.Random.Range(0, 0.5f);

			while (S.Loader._rooms == null)
				yield return new WaitForSeconds(delay);

			while (!S.Loader._rooms.ContainsKey(_sceneName))
				yield return new WaitForSeconds(delay);

			RoomModel roomModel = S.Loader._rooms[_sceneName];

			_doorModel = roomModel._doors[_number];
			_doorModel._door = this;
			_doorModel._coordinates = transform.position;
			_doorModel._right = transform.right;
			_locked = _doorModel._locked;
			_needArrow = _doorModel._needArrow;

			if (_needArrow)
			{
				PlaceArrowAndExitSignAsync();

				while (!_arrowPlaced)
					yield return new WaitForSeconds(delay);
			}

			if (_locked)
				PlaceStampAsync();
		}
	}

	void PlaceStampAsync()
	{
		Vector3 point0 = transform.position + transform.right * 3f; ///

		Quaternion rot = Quaternion.LookRotation(transform.right);

		StartCoroutine(SetParent());

		IEnumerator SetParent()
		{
			while (true)
			{
				while (S.Loader.Roots == null ||
					!S.Loader.Roots.ContainsKey(_sceneName) ||
					S.Loader.Roots[_sceneName] == null)
					yield return new WaitForSeconds(0.25f);

				GameObject stampObj = GameObject.Instantiate(S.Stamp, point0, rot, S.Loader.Roots[_sceneName]);

				try
				{
					RaycastHit hit;
					if (Physics.Raycast(point0, Vector3.down, out hit, 20f))
					{
						Vector3 point1 = hit.point;
						point1 += Vector3.up * 3.75f;
						stampObj.transform.position = point1;

						RaycastHit hit2;
						Vector3 point2 = point0 + Vector3.up * 10f;
						if (Physics.Raycast(point2, -transform.right, out hit2, 5f))
						{
							Vector3 point3 = hit2.point;
							point3.y = point1.y;
							point3 += transform.right * 0.55f;
							stampObj.transform.position = point3;
						}
					}
					else
					{
						throw new Exception("Can't raycast");
					}
				}
				catch (Exception ex)
				{
					S.Console.AddMessage($"I can't place stamp (#1)! Scene: {_sceneName}, door number: {_number}", Color.red);
					continue;
				}

				try
				{
					Vector3 originalScale = S.Stamp.transform.localScale;
					Vector3 parentScale = S.Loader.Roots[_sceneName].lossyScale;

					stampObj.transform.localScale = new Vector3(
						originalScale.x / parentScale.x,
						originalScale.y / parentScale.y,
						originalScale.z / parentScale.z
					);

					_stamp = stampObj.GetComponent<Stamp>();
					_stamp._door = this;
				}
				catch (Exception ex)
				{
					S.Console.AddMessage($"I can't place stamp (#2)! Scene: {_sceneName}, door number: {_number}", Color.red);
					continue;
				}

				break;
			}
		}
	}

	void PlaceArrowAndExitSignAsync()
	{
		Vector3 point0 = transform.position + transform.right * 3f; ///

		Quaternion rot = Quaternion.LookRotation(transform.right);

		StartCoroutine(SetParent0());

		IEnumerator SetParent0()
		{
			while (true)
			{
				while (S.Loader.Roots == null ||
					!S.Loader.Roots.ContainsKey(_sceneName) ||
					S.Loader.Roots[_sceneName] == null)
					yield return new WaitForSeconds(0.25f);

				GameObject arrowObj = GameObject.Instantiate(S.Arrow, point0, rot, S.Loader.Roots[_sceneName]);

				Vector3 point1 = point0;

				RaycastHit hit;
				if (Physics.Raycast(point0, Vector3.down, out hit, 20f))
				{
					point1 = hit.point;
					arrowObj.transform.position = point1;
				}
				else
				{
					S.Console.AddMessage("Arrow not placed one time (#1)", Color.red);
					yield return new WaitForSeconds(1f);
					continue;
				}

				Vector3 originalScale = S.Arrow.transform.localScale;
				Vector3 parentScale = S.Loader.Roots[_sceneName].lossyScale;

				arrowObj.transform.localScale = new Vector3(
					originalScale.x / parentScale.x,
					originalScale.y / parentScale.y,
					originalScale.z / parentScale.z
				);

				Vector3 point2 = point1 + new Vector3(0, 14, 0);

				GameObject exitObj = GameObject.Instantiate(S.Exit, point0, rot, S.Loader.Roots[_sceneName]);

				if (Physics.Raycast(point2, -transform.right, out hit, 20f))
				{
					Vector3 point3 = hit.point + new Vector3(0, -5, 0);
					exitObj.transform.position = point3;
				}
				else
				{
					S.Console.AddMessage("Arrow not placed one time (#2)", Color.red);
					yield return new WaitForSeconds(1f);
					continue;
				}

				Vector3 originalScale2 = S.Exit.transform.localScale;

				exitObj.transform.localScale = new Vector3(
					originalScale2.x / parentScale.x,
					originalScale2.y / parentScale.y,
					originalScale2.z / parentScale.z
				);

				_arrowPlaced = true;

				break;
			}
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("PlayerHub"))
		{
			Go();
		}
	}

	public void Go()
	{
		if (!_locked)
		{
			S.AudioManager.Play(_audioName); //Was set before

			S.Loader.GoTo(_doorModel._nextSceneName, _doorModel._nextDoorId, transform.right);
		}
		else
		{
			PlayerMovement pm = S.Ph.GetComponent<PlayerMovement>();
			Vector3 direction = S.Camera.transform.position - transform.position;
			direction = new Vector3(direction.x, 0, direction.z).normalized;
			direction *= 1400f;
			direction += new Vector3(0, 25, 0);
			pm.Push(direction);
			S.Ps.Damage(10);
			S.Inventory._negated = 0.3f;

			for (int i = 0; i < _sparklesCount; i++)
			{
				GameObject sparkle = Instantiate(S.BlueSparkle);
				sparkle.transform.position = transform.position;
				sparkle.transform.rotation = Quaternion.LookRotation(direction);
				sparkle.transform.localScale *= 4f;
			}
		}
	}

	public void Unlock()
	{
		if (_locked)
		{
			_locked = false;
			_doorModel._locked = false;
			Debug.LogError($"sc {_doorModel._nextSceneName} did {_doorModel._nextDoorId}");
			S.Loader._rooms[_doorModel._nextSceneName]._doors[_doorModel._nextDoorId]._door.Unlock();
			_stamp.Unlock();
		}
	}
}