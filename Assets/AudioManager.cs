using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _shot;
    
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
    public AudioSource _ohNo;
    public AudioSource _wrong;
    public AudioSource _wrong2;

    public AudioSource _toiletMusic1;
	public AudioSource _toiletMusic2;

    public AudioSource _fzt1;
    public AudioSource _fzt2;

	public AudioSource _incomeOST1;
	public AudioSource _incomeOST2;

	public AudioSource _adelaidaOST1;
	public AudioSource _labyrinth;
	public AudioSource _fenomen;
	public AudioSource _greatMix;
	public AudioSource _riddik;

	public AudioSource _maylo;
	public AudioSource _theRoom;

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
		name = name.ToLower();
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
			case "door":
				pitch = 0.9f + (float)rnd.NextDouble() * 0.2f;
				_door.pitch = pitch;
				_door.Play();
				break;
			case "pickup":
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
			case "notenoughcash":
				pitch = 0.9f;
				_notEnoughCash.pitch = pitch;
				_notEnoughCash.Play();
				break;
			case "noammo":
				pitch = 0.9f;
				_noAmmo.pitch = pitch;
				_noAmmo.Play();
				break;
			case "toiletdoor":
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
				_kill.pitch = pitch;
				_kill.volume = 0.75f;
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
            case "ohno":
                pitch = 1;
                _ohNo.pitch = pitch;
                _ohNo.Play();
                break;
            case "wrong":
                pitch = 1f;
                _wrong.pitch = pitch;
                _wrong.Play();
                break;
            case "wrong2":
                pitch = 1;
                _wrong2.pitch = pitch;
                _wrong2.Play();
                break;
            case "fzt1":
                pitch = 1;
                _fzt1.pitch = pitch;
                _fzt1.Play();
                break;
            case "fzt2":
                pitch = 1;
                _fzt2.pitch = pitch;
                _fzt2.Play();
                break;
			case "incomeOST1":
				pitch = 1;
				_incomeOST1.pitch = pitch;
				_incomeOST1.Play();
				break;
			case "incomeOST2":
				pitch = 1;
				_incomeOST2.pitch = pitch;
				_incomeOST2.Play();
				break;
			default:
				Debug.Log($"No such audioSource {name} in code! Maybe you want to add it into AudioManager?");
				break;
		}
	}
}