using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

public class Button1 : MonoBehaviour
{
	AllFather _allFather;
	string _sceneName;
	string _key;

	private AudioManager _audioManager;
	public string _audioName;
	private bool _pressed;
	private bool _finished;
	public GameObject _obj;
	private float _startY;
	public float _deltaY;
	private float _finalDeltaY;
	public Camera _camera;
	private Camera _playerCamera;
	public IsDoor _isDoor;
	public ToiletZombie _toiletZombie;

	public void Start()
	{
		_finalDeltaY = -10.63f;
		_startY = _obj.transform.position.y;

		_allFather = GameObject.Find("AllFather").GetComponent<AllFather>();
		_sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
		_key = _sceneName + transform.position.x + transform.position.y + transform.position.z;

		if (_allFather.Contains(_key))
		{
			Save es = _allFather.Load(_key);

			if (es._pressed)
			{
				_pressed = true;
				_finished = true;
				_toiletZombie._active = false;
				_obj.transform.position = new Vector3(_obj.transform.position.x, _startY + _finalDeltaY, _obj.transform.position.z);
			}
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
			_playerCamera.gameObject.SetActive(false);

			_pressed = true;

			_startY = _obj.transform.position.y;

			if (_audioManager == null)
			{
				GameObject go = GameObject.FindGameObjectWithTag("AudioManager");
				_audioManager = go.GetComponent<AudioManager>();
			}

			_audioManager.Play(_audioName, 1);

			_isDoor.Close();

			Thread.Sleep(300);

			Save save = new Save();
			save._pressed = true;
			_allFather.Save(_key, save);

			yield return null;
		}
	}

	public void Update()
	{
		if (_pressed && !_finished)
		{
			//Debug.Log($"{_obj.transform.position.y} > {_startY} + {_finalDeltaY}");
			if (_obj.transform.position.y > _startY + _finalDeltaY)
			{
				if (_isDoor.Closed)
					_obj.transform.position = _obj.transform.position + new Vector3(0, _deltaY * Time.deltaTime * 60, 0);
			}
			else
			{
				_obj.transform.position = new Vector3(_obj.transform.position.x, _startY + _finalDeltaY, _obj.transform.position.z);
				_finished = true;

				_camera.gameObject.SetActive(false);
				_playerCamera.gameObject.SetActive(true);

				Thread.Sleep(500);
			}
		}
	}
}