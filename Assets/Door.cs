using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using System;

public class Door : MonoBehaviour
{
	public Collider col;
	public string nextSceneName;
	public Vector3 position;
	public float _rotation;
	public string audioName;
	public bool _locked;
	public GameObject _stamp;
	public float _stampAnimationTimeLeft;
	public int _sparklesCount;

	public void Start()
	{
		_sparklesCount = 100;
	}

	private void OnTriggerEnter(Collider collider)
	{
		Go(collider.gameObject);
	}

	public void Go(GameObject playerObject)
	{
		if (playerObject.tag == "Player")
		{
			if (!_locked)
			{
				S.AudioManager.Play(audioName, 0);

				string sceneName = S.SM.LoadString("sceneName");
				List<string> loadScenesNames = new List<string>();
				loadScenesNames.AddRange(S.Loader._map[nextSceneName]);
				loadScenesNames.Add(nextSceneName);

				List<string> unloadScenesNames = new List<string>();
				unloadScenesNames.AddRange(S.Loader._map[sceneName]);
				unloadScenesNames.Add(sceneName);

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

                StartCoroutine(WaitLoad(playerObject));
			}
			else
			{
				GameObject ph = playerObject.transform.parent.gameObject;
				Rigidbody rb = ph.GetComponent<Rigidbody>();
				PlayerMovement pm = ph.GetComponent<PlayerMovement>();
				Vector3 direction = S.Camera.transform.position - transform.position;
				direction = new Vector3(direction.x, 0, direction.z).normalized;
				direction *= 1800f;
				direction += new Vector3(0, 25, 0);
				pm.Push(direction);
				PlayerStorage ps = ph.GetComponent<PlayerStorage>();
				ps.Damage(10);
				S.Inventory._negated = 0.3f;

				for (int i = 0; i < _sparklesCount; i++)
				{
					GameObject sparkle = Instantiate(S.Sparkle);
					sparkle.transform.position = transform.position;
					sparkle.transform.rotation = Quaternion.LookRotation(direction);
					sparkle.transform.localScale *= 4f;
					sparkle.GetComponent<IsSparkle>()._active = true;
				}
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

			_stamp.transform.rotation = Quaternion.Euler(randomX, randomY, randomZ);

			if (_stampAnimationTimeLeft <= 0)
				Destroy(_stamp);

			_locked = false;
		}
	}

	IEnumerator WaitLoad(GameObject playerObject)
	{
		while (!S.AllFather.SceneCurrentlyLoaded(nextSceneName))
			yield return new WaitForSeconds(0.2f);

		PlayerMovement pm = S.Ph.GetComponent<PlayerMovement>();

		Vector3 v = new Vector3(0, 0, 0);
		if (pm.isCrouching)
			v = new Vector3(0, -2.2f, 0);

		S.Ph.transform.position = position + v;
		S.PlayerCamScript.Rotate(_rotation);

		S.PS._currentSceneName = nextSceneName;
		S.SaveManager.CurrentSave.SaveString("sceneName", S.PS._currentSceneName);
	}
}