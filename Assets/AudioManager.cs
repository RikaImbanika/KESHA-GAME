using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioSource _gong;
	public AudioSource _arfa;
	public AudioSource _door;
	public AudioSource _pickUp;
	public AudioSource _throw;
	public AudioSource _inventory;
	public AudioSource _plasma;
	public AudioSource _money;
	public AudioSource _notEnoughCash;
	public AudioSource _toiletDoor;
	public AudioSource _noAmmo;
	public AudioSource _reload;
	public AudioSource _toilet;
	public AudioSource _kill;
	public AudioSource _damage;
	public AudioSource _heal;
	public AudioSource _screamer1;
	public AudioSource _screamer2;
	public AudioSource _screamer3;
	public AudioSource _screamer4;
	public AudioSource _screamer5;
	public AudioSource _screamer6;
	public AudioSource _screamer7;

	public AudioSource _toiletMusic1;
	public AudioSource _toiletMusic2;
	public int _toiletPhase = 0;
	static Vector3 _toiletCenter = new Vector3(-131f, -2.5f, 223.5f);
	static Vector3 _toiletStart = new Vector3(-174f, -2.5f, 109f);
	float _maxToiletDistance = Vector3.Distance(_toiletCenter, _toiletStart);

	public bool muted;

	public void Start()
	{
		S.AudioManager = this;
	}

	public void Play(string name)
	{
		Play(name, 1);
	}

	public void Play(string name, float pitch)
	{
		if (muted)
			return;

		System.Random rnd = new System.Random();

		switch (name)
		{		
			case "gong":
				_gong.pitch = pitch;
				_gong.Play();
				break;
			case "arfa":
				_arfa.pitch = pitch;
				_arfa.Play();
				break;
			case "Door":
				pitch = 0.9f + (float)rnd.NextDouble() * 0.2f;
				_door.pitch = pitch;
				_door.Play();
				break;
			case "pickUp":
				pitch += -0.1f + (float)rnd.NextDouble() * 0.2f;
				_pickUp.pitch = pitch;
				_pickUp.Play();
				break;
			case "throw":
				pitch += 0.2f + (float)rnd.NextDouble() * 0.2f;
				_throw.pitch = pitch;
				_throw.Play();
				break;
			case "inventory":
				_inventory.pitch = pitch;
				_inventory.Play();
				break;
			case "plasma":
				pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				_plasma.pitch = pitch;
				_plasma.Play();
				break;
			case "money":
				pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				_money.pitch = pitch;
				_money.Play();
				break;
			case "notEnoughCash":
				pitch = 0.9f;
				_notEnoughCash.pitch = pitch;
				_notEnoughCash.Play();
				break;
			case "noAmmo":
				pitch = 0.9f;
				_noAmmo.pitch = pitch;
				_noAmmo.Play();
				break;
			case "toiletDoor":
				pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				_toiletDoor.pitch = pitch;
				_toiletDoor.Play();
				break;
			case "reload":
				pitch = 1f;
				_reload.pitch = pitch;
				_reload.Play();
				break;
			case "toilet":
				pitch = 1f;
				_toilet.pitch = pitch;
				_toilet.Play();
				break;
			case "kill":
				pitch = 1f;
				_kill.pitch = pitch;
				_kill.Play();
				break;
			case "damage":
				pitch = 1.1f;
				_damage.pitch = pitch;
				_damage.Play();
				break;
			case "heal":
				_heal.Play();
				break;
			case "screamer1":
				pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				_screamer1.pitch = pitch;
				_screamer1.Play();
				break;
			case "screamer2":
				pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				_screamer2.pitch = pitch;
				_screamer2.Play();
				break;
			case "screamer3":
				pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				_screamer3.pitch = pitch;
				_screamer3.Play();
				break;
			case "screamer4":
				pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				_screamer4.pitch = pitch;
				_screamer4.Play();
				break;
			case "screamer5":
				pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				_screamer5.pitch = pitch;
				_screamer5.Play();
				break;
			case "screamer6":
				pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				_screamer6.pitch = pitch;
				_screamer6.Play();
				break;
			case "screamer7":
				pitch = UnityEngine.Random.Range(0.9f, 1.1f);
				_screamer7.pitch = pitch;
				_screamer7.Play();
				break;
			default:
				Debug.Log($"No such audioSource {name}!");
				break;
		}
	}

	public void Update()
	{
		if (_toiletPhase != 0)
		{
			if (_toiletMusic1.time > 80)
			{
				_toiletMusic1.Stop(); ////////////
				_toiletMusic2.time = 0;
				_toiletMusic2.Play();
			}
			if (_toiletMusic2.time > 80)
			{
				_toiletMusic2.Stop(); //////////////
				_toiletMusic1.time = 0;
				_toiletMusic1.Play();
			}
		}

		if (_toiletPhase == 3)
		{
			if (_toiletMusic1.volume >= 0.003f)
				_toiletMusic1.volume -= 0.003f;
			if (_toiletMusic2.volume >= 0.003f)
				_toiletMusic2.volume -= 0.003f;

			if (_toiletMusic1.volume < 0.003f && _toiletMusic2.volume < 0.003f)
			{
				_toiletMusic1.Stop();
				_toiletMusic2.Stop();
				_toiletPhase = 0;
			}
		}
		else if (_toiletPhase == 1)
		{
			float distance = Vector3.Distance(_toiletCenter, transform.position);
			float volume = (_maxToiletDistance - distance) / _maxToiletDistance;
			if (volume < 0)
				volume = 0;

			_toiletMusic1.volume = volume;
			_toiletMusic2.volume = volume;
		}
	}

	public void PlayFirstZombieTheme()
	{
		_toiletMusic1.volume = 1;
		_toiletMusic2.volume = 1;
		_toiletPhase = 2;
		_toiletMusic1.Play();
	}

	public void StopFirstZombieTheme()
	{
		_toiletPhase = 3;
	}

	public void EnterToilet()
	{
		if (_toiletPhase == 0)
		{
			_toiletPhase = 1;
			_toiletMusic1.time = 0;
			_toiletMusic1.Play();
		}
		else if (_toiletPhase == 3)
		{
			_toiletPhase = 1;
		}
	}

	public void DeepEnterToilet()
	{
		_toiletPhase = 2;
		_toiletMusic1.volume = 1;
		_toiletMusic2.volume = 1;
	}

	public void LeaveToilet()
	{
		if (_toiletPhase != 0)
			_toiletPhase = 3;
	}
}
