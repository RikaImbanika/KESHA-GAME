using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class S : object
{
	//Тут все типа статические общедоступные штуки
	//Объявить и заполнить один раз
	//Обращаться через свойства
	//Всё в одном месте

	private static GameObject _allFatherObj;
	private static AllFather _allFather;
	private static Loader _loader;
	private static II _II;
	private static Inventory _inventory;
	private static GameObject _canvasObj;
	private static GameObject _pObj;
	private static GameObject _ph;
	private static PlayerStorage _ps;
	private static Canvas _canvas;
	private static AudioManager _audioManager;
	private static GameObject _negative;
	private static IsGun _isGun;

	public static void Init()
	{
		_allFatherObj = GameObject.FindGameObjectWithTag("AllFather");
		_allFather = _allFatherObj.GetComponent<AllFather>();
		_loader = _allFatherObj.GetComponent<Loader>();
		_II = _allFatherObj.GetComponent<II>();
		_pObj = GameObject.FindGameObjectWithTag("Player");
		_canvasObj = GameObject.FindGameObjectWithTag("Canvas");
		_canvas = _canvasObj.GetComponent<Canvas>();
		_inventory = _canvasObj.GetComponent<Inventory>();
		_ph = _pObj.transform.parent.gameObject;
		_ps = _ph.GetComponent<PlayerStorage>();
		GameObject go2 = GameObject.FindGameObjectWithTag("AudioManager");
		_audioManager = go2.GetComponent<AudioManager>();
		_negative = _inventory._negative;
		_isGun = _allFatherObj.GetComponent<IsGun>(); //Вот так вот!
	}

	public static AudioManager AudioManager
	{
		get
		{
			return _audioManager;
		}
	}

	public static IsGun IsGun
	{
		get
		{
			return _isGun;
		}
	}

	public static GameObject Negative
	{
		get
		{
			return _negative;
		}
	}

	public static Canvas Canvas
	{
		get
		{
			return _canvas;
		}
	}

	public static Camera Camera
	{
		get
		{
			return Camera.main;
		}
	}

	public static Loader Loader
	{
		get
		{
			return _loader;
		}
	}

	public static AllFather AllFather
	{
		get
		{
			return _allFather;
		}
	}

	public static II II
	{
		get
		{
			return _II;
		}
	}

	public static Inventory Inventory
	{
		get
		{
			return _inventory;
		}
	}

	public static PlayerStorage PS
	{
		get
		{
			return _ps;
		}
	}

	public static GameObject Ph
	{
		get
		{
			return _ph;
		}
	}

	public static GameObject PObj
	{
		get
		{
			return _pObj;
		}
	}
}