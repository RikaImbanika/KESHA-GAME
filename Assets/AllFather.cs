using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

public class AllFather : MonoBehaviour
{
	public Dictionary<string, Save> _theSave;
	public Dictionary<string, Save> _theSaveOfTheSave;
	public GameObject _spot;
	public GameObject _sparkle;
	public GameObject _redSparkle;
	public List<GameObject> _spots;
	public AudioSource _caboom;
	public AudioSource _shot;
	public Camera _camera;
	public Inventory _inventory;
	public AudioManager _audioManager;
	public GameObject _playerObject;
	private PlayerStorage _playerStorage;
	private PlayerMovement _playerMovement;
	private GameObject _playerHub;
	private PlayerCamScript _playerCamScript;

	public int _enemyBulletSparklesCount;

	void Start()
	{
		S.Init();
		StartCoroutine(Logger(10f));

		_theSave = new Dictionary<string, Save>();
		_spots = new List<GameObject>();
		_spot = GameObject.Find("TheSpot");
		_spot.transform.SetParent(gameObject.transform);
		_sparkle = GameObject.Find("Sparkle");
		_sparkle.transform.SetParent(gameObject.transform);

		_playerHub = _playerObject.transform.parent.gameObject;
		_playerStorage = _playerHub.GetComponent<PlayerStorage>();
		_playerMovement = _playerHub.GetComponent<PlayerMovement>();
		_playerCamScript = Camera.main.GetComponent<PlayerCamScript>();

		StartCoroutine(FirstSave(7f));

		IEnumerator FirstSave(float delay)
		{
			yield return new WaitForSeconds(delay);
			SaveTheSave();
		}
	}

	IEnumerator Logger(float delay)
	{
		while (true)
		{
			yield return new WaitForSeconds(delay);
			string save = "=========================";
			foreach (var kvp in _theSave)
				save += "\r\n" + kvp.Key + " : " + kvp.Value;
			save += "\r\n=======================";
			Debug.Log(save);
		}
	}

	public Dictionary<string, Save> DeepCopyDictionary(Dictionary<string, Save> original)
	{
		Dictionary<string, Save> copy = new Dictionary<string, Save>();

		foreach (var entry in original)
		{
			string key = entry.Key;
			Save originalValue = entry.Value;

			string json = JsonUtility.ToJson(originalValue);

			Save copyValue = JsonUtility.FromJson<Save>(json);

			copy.Add(key, copyValue);
		}

		return copy;
	}

	public void SaveTheSave()
	{
		Save("playerXRot", new Save(_playerCamScript.xRotation));
		Save("playerXRot", new Save(_playerCamScript.yRotation));
		Save("playerPosition", new Save(_playerHub.transform.position));
		Save s = new Save();
		s._scene = _playerStorage._currentSceneName;
		Save("sceneName", s);

		_inventory.SaveTheSave();
		_theSaveOfTheSave = DeepCopyDictionary(_theSave);
	}

	public void LoadTheSave()
	{
		_theSave = DeepCopyDictionary(_theSaveOfTheSave);

		_playerHub.transform.position = Load("playerPosition")._position;
		_playerStorage._health = Load("health")._value;
		_playerStorage._currentSceneName = Load("sceneName")._name;

		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			Scene scene0 = SceneManager.GetSceneAt(i);

			if (scene0.name != "Start")
				SceneManager.UnloadSceneAsync(scene0);
		}

		LoadSavedScenes();
	}

	private void LoadSavedScenes()
	{
		Thread mt = new Thread(MT);
		mt.Start();

		void MT()
		{
			_inventory.LoadTheSave();

			string currentScene = Load("sceneName")._scene;
			var map = S.Loader._map[currentScene];
			map.Add(currentScene);

			foreach (string sceneName in map)
			{
				SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
				S.Loader._scenesToLoad.Add(sceneName);
			}

			while (!SceneManager.GetSceneByName(currentScene).isLoaded)
				Thread.Sleep(200);
			while (!S.Loader._scenesToLoad.Contains(currentScene))
				Thread.Sleep(200);
			_playerHub.transform.position = Load("playerPosition")._position;
			_playerCamScript.StaticRotate(Load("playerXRot")._value, Load("playerYRot")._value);

			Debug.Log("SAVE LOADED!!!");
		}
	}

	public void Save(string key, Save save)
	{
		if (_theSave.ContainsKey(key))
			_theSave[key] = save;
		else
			_theSave.Add(key, save);
	}

	public Save Load(string key)
	{
		if (_theSave.ContainsKey(key))
			return _theSave[key];
		else
			return new Save();
	}

	public void Destroy(string id)
	{
		_theSave.Remove(id);
	}

	public bool Contains(string key)
	{
		return _theSave.ContainsKey(key);
	}

	public EnemyParams GetEnemyParams(string name)
	{
		EnemyParams ep = new EnemyParams();

		if (name == "zombie")
		{
			ep._screamerX = 0;
			ep._screamerY = -3.9f;
			ep._screamerZ = 0.60f;
			ep._screamerSounds = new string[] { "screamer1", "screamer2", "screamer3", "screamer4", "screamer5", "screamer6", "screamer7" };
		}
		else if (name == "professor")
		{
			ep._screamerX = 0;
			ep._screamerY = -4.2f;
			ep._screamerZ = 0.65f;
			ep._screamerSounds = new string[] { "screamer1", "screamer2", "screamer3", "screamer4", "screamer5", "screamer6", "screamer7" };
		}
		else if (name == "musculus")
		{
			ep._screamerX = 0;
			ep._screamerY = -4.6f;
			ep._screamerZ = 2.7f;
			ep._screamerSounds = new string[] { "screamer1", "screamer2", "screamer3", "screamer4", "screamer5", "screamer6", "screamer7" };
		}
		else if (name == "ghost")
		{
			ep._screamerX = 0;
			ep._screamerY = -3;
			ep._screamerZ = 1.5f;
			ep._screamerSounds = new string[] { "screamer1", "screamer2", "screamer3", "screamer4", "screamer5", "screamer6", "screamer7" };
		}

		return ep;
	}

	public bool SceneCurrentlyLoaded(string name)
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