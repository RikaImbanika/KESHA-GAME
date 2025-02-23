using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

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

				string sceneName = S.AllFather.Load("sceneName")._scene;
				List<string> loadScenesNames = S.Loader._map[nextSceneName];
				loadScenesNames.Add(nextSceneName);
				List<string> unloadScenesNames = S.Loader._map[sceneName];
				unloadScenesNames.Add(sceneName);

				foreach (string name in loadScenesNames)
					if (!unloadScenesNames.Contains(name))
					{
						SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
						S.Loader._scenesToLoad.Add(name);
					}

				foreach (string name in unloadScenesNames)
					if (!loadScenesNames.Contains(name))
						SceneManager.UnloadSceneAsync(name);

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
					GameObject sparkle = Instantiate(S.AllFather._sparkle);
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

			float randomX = Random.Range(0f, 360f);
			float randomY = Random.Range(0f, 360f);
			float randomZ = Random.Range(0f, 360f);

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

		GameObject playerHub = playerObject.transform.parent.gameObject;
		PlayerMovement pm = playerHub.GetComponent<PlayerMovement>();

		Vector3 v = new Vector3(0, 0, 0);
		if (pm.isCrouching)
			v = new Vector3(0, -2.2f, 0);

		playerHub.transform.position = position + v;
		PlayerCamScript pcs = Camera.main.GetComponent<PlayerCamScript>();
		pcs.Rotate(_rotation);

		PlayerStorage ps = playerHub.GetComponent<PlayerStorage>();
		ps._currentSceneName = nextSceneName;

		Save s = new Save();
		s._scene = nextSceneName;
		S.AllFather.Save("sceneName", s);

		yield return null;
	}
}