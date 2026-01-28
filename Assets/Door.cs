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
	public int _nextDoorId;
	private string _nextSceneName;
	public Collider col;
	public string audioName;
	public bool _locked;
	public Stamp _stamp;
	public float _stampAnimationTimeLeft;
	public int _sparklesCount;
	public DoorModel _doorModel;
	public DoorModel __nextDoorModel;
	private string _sceneName;

	public void Start()
	{
		_sparklesCount = 100;
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
					if (!roomModel._doors.ContainsKey(_number))
						roomModel._doors.Add(_number, new DoorModel(transform.position.x, transform.position.y, transform.position.z, transform.right));
					else
					{
						DoorModel doorModel = roomModel._doors[_number];
						doorModel._coordinates = new Vector3(transform.position.x, transform.position.y, transform.position.z);
						doorModel._right = transform.right;
					}
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
			S.AudioManager.Play(audioName, 0);

			_doorModel = S.Loader._rooms[_sceneName]._doors[_number];
			_nextSceneName = _doorModel._nextSceneName;
			_nextDoorId = _doorModel._nextDoorNumber;

			S.Teleporter.ImportantStaticShitToDo(_nextSceneName);

			List<string> loadScenesNames = new List<string>();
			loadScenesNames.AddRange(S.Loader._map[_nextSceneName]);
			loadScenesNames.Add(_nextSceneName); //

			List<string> unloadScenesNames = new List<string>();
			unloadScenesNames.AddRange(S.Loader._map[_sceneName]);
			unloadScenesNames.Add(_sceneName); //?

			foreach (string name in loadScenesNames)
				if (!unloadScenesNames.Contains(name))
				{
					SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
					S.Loader.PleaseLoadScene(name);
				}

			foreach (string name in unloadScenesNames)
				if (!loadScenesNames.Contains(name))
					try
					{
						SceneManager.UnloadSceneAsync(name);
					}
					catch (System.Exception ex)
					{
						Debug.LogError($"Error unloading scene {name}: {ex.Message}");
					}

			StartCoroutine(S.Teleporter.WaitLoad(_nextSceneName, _nextDoorId, transform.right));
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
		_stampAnimationTimeLeft = 1f;
		_locked = false;
	}

	public void Update()
	{
		if (_stampAnimationTimeLeft > 0)
		{
			_stampAnimationTimeLeft -= Time.deltaTime;

			float randomX = UnityEngine.Random.Range(0f, 360f);
			float randomY = UnityEngine.Random.Range(0f, 360f);
			float randomZ = UnityEngine.Random.Range(0f, 360f);

			_stamp._go.transform.rotation = Quaternion.Euler(randomX, randomY, randomZ);

			if (_stampAnimationTimeLeft <= 0)
				Destroy(_stamp._go);

			_locked = false;
		}
	}
}