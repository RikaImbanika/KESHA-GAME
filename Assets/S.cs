using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class S : object
{
	private static System.Random _rnd;
	private static GameObject _allFatherObj;
	private static AllFather _allFather;
	private static Loader _loader;
	private static II _II;
	private static Inventory _inventory;
	private static GameObject _canvasObj;
	private static GameObject _pObj;
	private static GameObject _ph;
	private static PlayerStorage _ps;
	private static PlayerMovement _playerMovement;
	private static Canvas _canvas;
	private static AudioManager _audioManager;
	private static GameObject _negative;
	private static IsGun _isGun;
	private static SaveManager _saveManager;
	private static PlayerCamScript _playerCamScript;
	private static Camera _camera;
    private static GameObject fpsObj;
    private static TextMeshProUGUI fpsTMP;
    private static GameObject _spot;
    private static GameObject _blueSparkle;
	private static GameObject _redSparkle;
	private static GameObject _blueOldSparkle;
	private static GameObject _redOldSparkle;
	private static FirstZombie2 _firstZombie2;
	private static SceneDuplicateCleaner _sceneDuplicateCleaner;
	private static MusicManager _musicManager;
	private static GameObject _loot;
	private static SceneSelector _sceneSelector;
	private static Teleporter _teleporter;
	private static CHEATS _cheats;
	private static GaymeBroker _gaymeBroker;
	private static GameObject _redLaser;
	private static GameObject _redPoint;
	private static GameObject _blueLaser;
	private static GameObject _blueRay;
	private static AudioSource _shot;
	private static GameObject _enemyBullet;
	private static List<Material> _snakeBallMaterials;
	private static GameObject _snakeBody;
	private static AudioSource _caboom;
	private static Backrooms _backrooms;
	private static GameObject _lighterObj;
	private static GameObject _snakie1;
	private static GameObject _snakie2;
	private static GameObject _snakie3;
	private static Mushrooms _mushrooms;
	private static Lighters _lighters;
	private static RandomRotation _randomRotations;
	private static GameObject _blueHitPoint;
	private static GameObject _redHitPoint;
	private static PortalsBase _portalsBase;
	private static VisibleLayers _visibleLayers;
	private static Transform _fakePlayer; //for checking player on the other side of portal
	private static Transform _fakePlayerCamera;
	private static string _fakePlayerScene;
	private static float _portalToPlayerDistance;
	private static float _fakePlayerLastUpdated;
	private static Enemies _enemies;
	private static GameObject _zombie;
	private static GameObject _bakalavr;
	private static GameObject _musculus;
	private static GameObject _ghost;
	private static GameObject _spider;
	private static GameObject _stamp;

	public static GameObject Stamp
	{
		get
		{
			return _stamp;
		}
		set
		{
			_stamp = value;
		}
	}

	public static GameObject Zombie
	{
		get
		{
			return _zombie;
		}
		set
		{
			_zombie = value;
		}
	}

	public static GameObject Bakalavr
	{
		get
		{
			return _bakalavr;
		}
		set
		{
			_bakalavr = value;
		}
	}

	public static GameObject Musculus
	{
		get
		{
			return _musculus;
		}
		set
		{
			_musculus = value;
		}
	}

	public static GameObject Ghost
	{
		get
		{
			return _ghost;
		}
		set
		{
			_ghost = value;
		}
	}

	public static GameObject Spider
	{
		get
		{
			return _spider;
		}
		set
		{
			_spider = value;
		}
	}

	public static Enemies Enemies
	{
		get
		{
			return _enemies;
		}
		set
		{
			_enemies = value;
		}
	}

	public static float FakePlayerLastUpdated
	{
		get
		{
			return _fakePlayerLastUpdated;
		}
		set
		{
			_fakePlayerLastUpdated = value;
		}
	}

	public static float PortalToPlayerDistance
	{
		get
		{
			return _portalToPlayerDistance;
		}
		set
		{
			_portalToPlayerDistance = value;
		}
	}

	public static string FakePlayerScene
	{
		get
		{
			return _fakePlayerScene;
		}
		set
		{
			_fakePlayerScene = value;
		}
	}

	public static Transform FakePlayer
	{
		get
		{
			return _fakePlayer;
		}
		set
		{
			_fakePlayer = value;
		}
	}

	public static Transform FakePlayerCamera
	{
		get
		{
			return _fakePlayerCamera;
		}
		set
		{
			_fakePlayerCamera = value;
		}
	}

	public static VisibleLayers VisibleLayers
	{
		get
		{
			return _visibleLayers;
		}
		set
		{
			_visibleLayers = value;
		}
	}

	public static PlayerMovement PlayerMovement
	{
		get
		{
			return _playerMovement;
		}
		set
		{
			_playerMovement = value;
		}
	}

	public static PlayerMovement PM
	{
		get
		{
			return _playerMovement;
		}
		set
		{
			_playerMovement = value;
		}
	}


	public static PlayerMovement Pm
	{
		get
		{
			return _playerMovement;
		}
		set
		{
			_playerMovement = value;
		}
	}

	public static PortalsBase PortalsBase
	{
		get
		{
			return _portalsBase;
		}
		set
		{
			_portalsBase = value;
		}
	}

	public static GameObject BlueHitPoint
	{
		get
		{
			return _blueHitPoint;
		}
		set
		{
			_blueHitPoint = value;
		}
	}

	public static GameObject RedHitPoint
	{
		get
		{
			return _redHitPoint;
		}
		set
		{
			_redHitPoint = value;
		}
	}

	public static RandomRotation RandRot
	{
		get
		{
			return _randomRotations;
		}
		set
		{
			_randomRotations = value;
		}
	}

	public static Lighters Lighters
	{
		get
		{
			return _lighters;
		}
		set
		{
			_lighters = value;
		}
	}

	public static Mushrooms Mushrooms
	{
		get
		{
			return _mushrooms;
		}
		set
		{
			_mushrooms = value;
		}
	}

	public static GameObject Snakie1
	{
		get
		{
			return _snakie1;
		}
		set
		{
			_snakie1 = value;
		}
	}

	public static GameObject Snakie2
	{
		get
		{
			return _snakie2;
		}
		set
		{
			_snakie2 = value;
		}
	}

	public static GameObject Snakie3
	{
		get
		{
			return _snakie3;
		}
		set
		{
			_snakie3 = value;
		}
	}

	public static System.Random RND
	{
		get
		{
			return _rnd;
		}
		set
		{
			_rnd = value;
		}
	}

	public static GameObject LighterObj
	{
		get
		{
			return _lighterObj;
		}
		set
		{
			_lighterObj = value;
		}
	}
	
	public static Backrooms Backrooms
	{
		get
		{
			return _backrooms;
		}
		set
		{
			_backrooms = value;
		}
	}

	public static AudioSource Caboom
    {
		get
        {
			return _caboom;
        }
		set
        {
			_caboom = value;
        }
    }

	public static GameObject SnakeBody
    {
		get
		{
			return _snakeBody;
		}
		set
        {
			_snakeBody = value;
        }
    }

	public static List<Material> SnakeBallMaterials
	{
		get
		{
			return _snakeBallMaterials;
		}
		set
		{
			_snakeBallMaterials = value;
		}
	}

	public static GameObject EnemyBullet
    {
		get
		{
			return _enemyBullet;
		}
		set
		{
			_enemyBullet = value;
		}
    }

    public static AudioSource Shot
	{
		get
		{
			return _shot;
		}
		set
		{
			_shot = value;
		}
	}

	public static GameObject RedLaser
	{
		get
		{
			return _redLaser;
		}
		set
		{
			_redLaser = value;
		}
	}

	public static GameObject BlueLaser
	{
		get
		{
			return _blueLaser;
		}
		set
		{
			_blueLaser = value;
		}
	}

	public static GameObject RedPoint
	{
		get
		{
			return _redPoint;
		}
		set
		{
			_redPoint = value;
		}
	}

	public static GaymeBroker GaymeBroker
	{
		get
		{
			return _gaymeBroker;
		}
		set
		{
			_gaymeBroker = value;
		}
	}

	public static CHEATS Cheats
	{
		get
		{
			return _cheats;
		}
		set
		{
			_cheats = value;
		}
	}

	public static Teleporter Teleporter
	{
		get
		{
			return _teleporter;
		}
		set
		{
			_teleporter = value;
		}
	}


	public static SceneSelector SceneSelector
	{
		get
		{
			return _sceneSelector;
		}
		set
		{
			_sceneSelector = value;
		}
	}

	public static GameObject Loot
	{
		get
		{
			return _loot;
		}
		set
		{
			_loot = value;
		}
	}
	
	public static AudioManager AM
	{
		get
		{
			return _audioManager;
		}
		set
		{
			_audioManager = value;
		}
	}

    public static MusicManager MM
    {
        get
        {
            return _musicManager;
        }
        set
        {
            _musicManager = value;
        }
    }

    public static MusicManager MusicManager
	{
		get
		{
			return _musicManager;
		}
		set
		{
			_musicManager = value;
		}
	}

    public static SceneDuplicateCleaner SDC
    {
        get
        {
            return _sceneDuplicateCleaner;
        }
        set
        {
            _sceneDuplicateCleaner = value;
        }
    }
    
	public static SceneDuplicateCleaner SceneDuplicateCleaner
	{
		get
		{
			return _sceneDuplicateCleaner;
		}
		set
		{
			_sceneDuplicateCleaner = value;
		}
	}


	public static FirstZombie2 FirstZombie2
	{
		get
		{
			return _firstZombie2;
		}
		set
		{
			_firstZombie2 = value;
		}
	}

	public static GameObject BlueRay
	{
		get
		{
			return _blueRay;
		}
		set
		{
			_blueRay = value;
		}
	}

	public static GameObject BlueSparkle
	{
		get
		{
			return _blueSparkle;
		}
		set
		{
			_blueSparkle = value;
		}
	}

	public static GameObject RedSparkle
	{
		get
		{
			return _redSparkle;
		}
		set
		{
			_redSparkle = value;
		}
	}

	public static GameObject BlueOldSparkle
	{
		get
		{
			return _blueOldSparkle;
		}
		set
		{
			_blueOldSparkle = value;
		}
	}

	public static GameObject RedOldSparkle
	{
		get
		{
			return _redOldSparkle;
		}
		set
		{
			_redOldSparkle = value;
		}
	}

	public static GameObject Spot
	{
		get
		{
			return _spot;
		}
		set
		{
			_spot = value;
		}
	}

    public static AudioManager AudioManager
	{
		get
		{
			return _audioManager;
		}
		set
		{
			_audioManager = value;
		}
	}

	public static IsGun IsGun
	{
		get
		{
			return _isGun;
		}
		set
		{
			_isGun = value;
		}
	}

	public static GameObject Negative
	{
		get
		{
			return _negative;
		}
		set
		{
			_negative = value;
		}
	}

	public static GameObject AllFatherObj
	{
		get
		{
			return _allFatherObj;
		}
		set
		{
			_allFatherObj = value;
		}
	}

	public static GameObject CanvasObj
	{
		get
		{
			return _canvasObj;
		}
		set
		{
			_canvasObj = value;
		}
	}

	public static GameObject PObj
	{
		get
		{
			return _pObj;
		}
		set
		{
			_pObj = value;
		}
	}

	public static GameObject Ph
	{
		get
		{
			return _ph;
		}
		set
		{
			_ph = value;
		}
	}

	public static PlayerStorage Ps
	{
		get
		{
			return _ps;
		}
		set
		{
			_ps = value;
		}
	}

    public static PlayerStorage PS
    {
        get
        {
            return _ps;
        }
        set
        {
            _ps = value;
        }
    }

    public static Canvas Canvas
	{
		get
		{
			return _canvas;
		}
		set
		{
			_canvas = value;
		}
	}

	public static Loader Loader
	{
		get
		{
			return _loader;
		}
		set
		{
			_loader = value;
		}
	}

	public static AllFather AllFather
	{
		get
		{
			return _allFather;
		}
		set
		{
			_allFather = value;
		}
	}

	public static II II
	{
		get
		{
			return _II;
		}
		set
		{
			_II = value;
		}
	}

	public static Inventory Inventory
	{
		get
		{
			return _inventory;
		}
		set
		{
			_inventory = value;
		}
	}

	public static PlayerCamScript PlayerCamScript
	{
		get
		{
			return _playerCamScript;
		}
		set
		{
			_playerCamScript = value;
		}
	}

	public static Camera Camera
	{
		get
		{
			return _camera;
		}
		set
		{
			_camera = value;
		}
	}

	public static SaveManager SaveManager
	{
		get
		{
			return _saveManager;
		}
		set
		{
			_saveManager = value;
		}
	}

    public static SaveManager SM
    {
        get
        {
            return _saveManager;
        }
        set
        {
            _saveManager = value;
        }
    }

	public static TextMeshProUGUI FpsTMP
	{
		get
		{
			return fpsTMP;
		}
		set
		{
			fpsTMP = value;
		}
	}

    public static GameObject FpsObj
	{
		get
		{
			return fpsObj;
		}
		set
		{
			fpsObj = value;
		}
	}

	public static Vector3 PlayerTarget(string sceneName)
	{
		if (sceneName == PS._currentSceneName)
			return Camera.transform.position;
		else if (sceneName == _fakePlayerScene)
			return FakePlayerCamera.position;
		else
			return Camera.transform.position; //
	}

	public static Vector3 PlayerTargetForward(string sceneName)
	{
		if (sceneName == PS._currentSceneName)
			return Camera.transform.forward;
		else if (sceneName == _fakePlayerScene)
			return FakePlayerCamera.forward;
		else
			return Camera.transform.forward; //
	}

	public static string ID(object id1, object id2)
	{
		return $"{id1.ToString()} {id2.ToString()}";
	}

	public static string ID(GameObject pos)
	{
		return $"{pos.transform.position.x} {pos.transform.position.y} {pos.transform.position.z}";
	}
}