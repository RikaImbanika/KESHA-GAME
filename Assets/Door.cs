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
	public float _stampAnimationTimeLeft;
	public int _sparklesCount;
	public DoorModel _doorModel;
	private string _sceneName;
	private bool _needArrow;
	private Vector3 _stampStartScale;

	public void Start()
	{
		_sparklesCount = 100;

		// if (string.IsNullOrEmpty(_audioName))
		// 	_audioName = "Door";
		_audioName = "Door"; //TO DO: More sounds.

		StartCoroutine(Wait());

		IEnumerator Wait()
		{
			_sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
			
			bool ok = false;

			while (true)
			{
				try
				{
					if (S.Loader._rooms == null)
						Debug.Log("Wtf#1");

					if (S.Loader._rooms[_sceneName] == null)
						Debug.Log($"Wtf#2 sceneName {_sceneName} obj {S.Loader._rooms[_sceneName]}");

					RoomModel roomModel = S.Loader._rooms[_sceneName];
					_doorModel = roomModel._doors[_number];
					_doorModel._door = this;
					_doorModel._coordinates = transform.position;
					_doorModel._right = transform.right;
					_locked = _doorModel._locked;
					_needArrow = _doorModel._needArrow;
					ok = true;
				}
				catch
				{
					ok = false;
				}

				if (ok)
					break;
				else
					yield return new WaitForSeconds(0.5f);
			}

			if (_needArrow)
			{
				Vector3 point0 = transform.position + transform.right * 3f; ///

				Quaternion rot = Quaternion.LookRotation(transform.right);

				StartCoroutine(SetParent0());

				IEnumerator SetParent0()
				{
					while (S.Loader.Roots[_sceneName] == null)
						yield return new WaitForSeconds(0.25f);

					GameObject arrowObj = GameObject.Instantiate(S.Arrow, point0, rot, S.Loader.Roots[_sceneName]);

					Vector3 point1 = point0;

					RaycastHit hit;
					if (Physics.Raycast(point0, Vector3.down, out hit, 20f))
					{
						point1 = hit.point;
						arrowObj.transform.position = point1;
					}

					Vector3 originalScale = S.Arrow.transform.localScale;
					Vector3 parentScale = S.Loader.Roots[_sceneName].lossyScale;

					arrowObj.transform.localScale = new Vector3(
						originalScale.x / parentScale.x,
						originalScale.y / parentScale.y,
						originalScale.z / parentScale.z
					);

					Vector3 point2 = point1 + new Vector3(0, 14, 0);

					Vector3 point3 = point2;

					GameObject exitObj = GameObject.Instantiate(S.Exit, point0, rot, S.Loader.Roots[_sceneName]);

					if (Physics.Raycast(point2, -transform.right, out hit, 20f))
					{
						point3 = hit.point + new Vector3(0, -5, 0);
						exitObj.transform.position = point3;
					}

					Vector3 originalScale2 = S.Exit.transform.localScale;

					exitObj.transform.localScale = new Vector3(
						originalScale2.x / parentScale.x,
						originalScale2.y / parentScale.y,
						originalScale2.z / parentScale.z
					);
				}
			}

			if (_locked)
			{
				Vector3 point0 = transform.position + transform.right * 0.5f;

				Quaternion rot = Quaternion.LookRotation(transform.right);

				StartCoroutine(SetParent());

				IEnumerator SetParent()
				{
					while (S.Loader.Roots[_sceneName] == null)
						yield return new WaitForSeconds(0.25f);

					GameObject stampObj = GameObject.Instantiate(S.Stamp, point0, rot, S.Loader.Roots[_sceneName]);

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

					Vector3 originalScale = S.Stamp.transform.localScale;
					Vector3 parentScale = S.Loader.Roots[_sceneName].lossyScale;

					stampObj.transform.localScale = new Vector3(
						originalScale.x / parentScale.x,
						originalScale.y / parentScale.y,
						originalScale.z / parentScale.z
					);

					_stamp = stampObj.GetComponent<Stamp>();
					_stamp._door = this;
					_stampStartScale = stampObj.transform.localScale;
				}
			}			
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
			Go();
	}

	public void Go()
	{
		if (!_locked)
		{
			S.AudioManager.Play(_audioName); //Was set before

			_doorModel = S.Loader._rooms[_sceneName]._doors[_number];

			S.Loader.GoTo(_doorModel._nextSceneName, _doorModel._nextDoorId, transform.right);
		}
		else
		{
			Rigidbody rb = S.Ph.GetComponent<Rigidbody>();
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
			_stampAnimationTimeLeft = 1.75f;
			_locked = false;
			_doorModel._locked = false;
			Debug.LogError($"sc {_doorModel._nextSceneName} did {_doorModel._nextDoorId}");
			S.Loader._rooms[_doorModel._nextSceneName]._doors[_doorModel._nextDoorId]._locked = false;
			S.Loader._rooms[_doorModel._nextSceneName]._doors[_doorModel._nextDoorId]._door.Unlock();
			_stamp.Unlock();
		}
	}

	public void Update()
	{
		if (_stampAnimationTimeLeft > 0)
		{
			_stampAnimationTimeLeft -= Time.deltaTime;

			float totalTime = 1.75f;
			float progress = _stampAnimationTimeLeft / totalTime;

			float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

			float currentScale = Mathf.Lerp(0f, 1f, smoothProgress);

			_stamp._go.transform.localScale = _stampStartScale * currentScale;

			float randomX = UnityEngine.Random.Range(0f, 360f);
			float randomY = UnityEngine.Random.Range(0f, 360f);
			float randomZ = UnityEngine.Random.Range(0f, 360f);
			_stamp._go.transform.rotation = Quaternion.Euler(randomX, randomY, randomZ);

			if (_stampAnimationTimeLeft <= 0)
				Destroy(_stamp._go);
		}
	}
}