using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

public class Door : MonoBehaviour
{
	public AllFather _allFather;
	public Collider col;
	public string nextSceneName;
	public Vector3 position;
	public float _rotation;
	private AudioManager audioManager;
	public string audioName;
	public bool _locked;
	public GameObject _stamp;
	public float _stampAnimationTimeLeft;
	private GameObject _sparkle;
	public int _sparklesCount;
	AudioManager _audioManager;

	public void Start()
	{
		_allFather = GameObject.Find("AllFather").GetComponent<AllFather>();
		_sparkle = _allFather._sparkle;
		_sparklesCount = 50;

		GameObject go = GameObject.FindGameObjectWithTag("AudioManager");
		_audioManager = go.GetComponent<AudioManager>();
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
				audioManager.Play(audioName, 0);

				string sceneName = _allFather.Load("sceneName")._scene;
				List<string> loadScenesNames = Loader.l._map[nextSceneName];
				List<string> unloadScenesNames = Loader.l._map[sceneName];

				foreach (string name in loadScenesNames)
					if (!unloadScenesNames.Contains(name))
					{
						SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
						Loader.l._scenesToLoad.Add(name);
					}

				Loader.l.AddictiveLoadAsync();

				foreach (string name in unloadScenesNames)
					if (!loadScenesNames.Contains(name))
						SceneManager.UnloadSceneAsync(name);

				StartCoroutine(WaitLoad(playerObject));
			}
			else
			{
				GameObject ph = playerObject.transform.parent.gameObject;
				Rigidbody rb = ph.GetComponent<Rigidbody>();
				Vector3 direction = ph.transform.position - transform.position;
				direction = new Vector3(direction.x, 0, direction.y).normalized;
				direction *= 2f;
				direction += new Vector3(0, 1, 0);
				rb.AddForce(direction * 30f, ForceMode.Impulse);
				PlayerStorage ps = ph.GetComponent<PlayerStorage>();
				ps.Damage(10);

				for (int i = 0; i < _sparklesCount; i++)
				{
					GameObject sparkle = Instantiate(_sparkle);
					sparkle.transform.position = transform.position;
					sparkle.transform.rotation = Quaternion.LookRotation(direction);
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
		while (!SceneCurrentlyLoaded(nextSceneName))
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
		_allFather.Save("sceneName", s);

		yield return null;
	}

	bool SceneCurrentlyLoaded(string name)
	{
		for (int i = 0; i < SceneManager.sceneCount; ++i)
		{
			Scene scene = SceneManager.GetSceneAt(i);
			if (scene.name == name)
				return scene.isLoaded;
		}

		return false;
	}
}