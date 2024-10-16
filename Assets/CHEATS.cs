using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

public class CHEATS : MonoBehaviour
{
    public AllFather _allFather;
    bool _cheats;
    PlayerStorage _playerStorage;
    GameObject _canvas;
    Inventory _inventory;
    List<string> _cheated;
    GameObject _e;
    GameObject _d;
    public float _message;

    public void Remember(string sceneName)
    {
        _cheated.Add(sceneName);
    }

    public bool IsCheated(string sceneName)
    {
        return _cheated.Contains(sceneName);
    }

    void Start()
    {
        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();
        _playerStorage = gameObject.GetComponent<PlayerStorage>();
        _canvas = GameObject.Find("Canvas");
        _inventory = _canvas.GetComponent<Inventory>();

        Transform canvasTransform = _canvas.transform; // Получите Transform Canvas
        Transform e = canvasTransform.Find("Cheats enabled");
        _e = e.gameObject;
        Transform d = canvasTransform.Find("Cheats disabled");
        _d = d.gameObject;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Y))
            transform.position += new Vector3(0, 8, 0);

		if ((Input.GetKeyDown(KeyCode.P) && Input.GetKey(KeyCode.O)) || (Input.GetKey(KeyCode.P) && Input.GetKeyDown(KeyCode.O)))
		{
			_cheats = !_cheats;
			_d.SetActive(!_cheats);
			_e.SetActive(_cheats);
			_message = 1f;
		}

		if (_cheats)
        {
            if (_playerStorage._health < 100)
                _playerStorage._health = 100;

            if (_playerStorage._currentSceneName == "Income" && !IsCheated("Income"))
                StartCoroutine(IncomeCheat());
            if (_playerStorage._currentSceneName == "Corridor" && !IsCheated("Corridor"))
                StartCoroutine(CorridorCheat());
            if (_playerStorage._currentSceneName == "Hall" && !IsCheated("Hall"))
                StartCoroutine(HallCheat());
        }

        if (_message > 0)
        {
            _message -= Time.deltaTime;
            if (_message <= 0)
            {
                _e.SetActive(false);
                _d.SetActive(false);
            }
        }
    }

    void Save()
    {
        for (int i = 0; i < _cheated.Count; i++)
        {
            Save s = new Save();
            s._cheated = true;
            _allFather.Save(_cheated[i], s);
        }
    }

    void Load()
    {
    }

    IEnumerator CorridorCheat()
    {
        yield return TakeItemWithDelay("apple 0 cor");
        yield return TakeItemWithDelay("apple 1 cor");
        yield return TakeItemWithDelay("apple 2 cor");
        yield return TakeItemWithDelay("apple 3 cor");
        yield return TakeItemWithDelay("apple 4 cor");
        yield return TakeItemWithDelay("apple 5 cor");
        yield return TakeItemWithDelay("apple 6 cor");
        yield return TakeItemWithDelay("cuc cor");
        Remember("Corridor");
    }

    IEnumerator IncomeCheat()
    {
        GameObject.Find("LimeLocker").GetComponent<Locker>().Cheat();
        yield return new WaitForSeconds(0.15f);
        GameObject.Find("BlueLocker").GetComponent<Locker>().Cheat();
        yield return new WaitForSeconds(0.15f);
        GameObject.Find("RedLocker").GetComponent<Locker>().Cheat();
        yield return new WaitForSeconds(0.15f);
        GameObject.Find("PurpleLocker").GetComponent<Locker>().Cheat();
        yield return new WaitForSeconds(0.15f);

        GameObject.Find("DoorBlue").GetComponent<IsDoor>().Move();
        yield return new WaitForSeconds(0.15f);
        GameObject.Find("DoorRed").GetComponent<IsDoor>().Move();
        yield return new WaitForSeconds(0.15f);
        GameObject.Find("DoorLime").GetComponent<IsDoor>().Move();
        yield return new WaitForSeconds(0.15f);
        GameObject.Find("DoorNone").GetComponent<IsDoor>().Move();
        yield return new WaitForSeconds(0.15f);

        yield return TakeItemWithDelay("BlueKey");
        yield return TakeItemWithDelay("RedKey");
        yield return TakeItemWithDelay("PurpleKey");
        yield return TakeItemWithDelay("Cucumber Income");
        yield return TakeItemWithDelay("Apple 1 Income");
        yield return TakeItemWithDelay("Apple 2 Income");

        Remember("Income");
    }

    IEnumerator HallCheat()
    {
        yield return TakeItemWithDelay("LimeKey");
        yield return TakeItemWithDelay("Cuc Hall 0");
        yield return TakeItemWithDelay("Cuc Hall 1");
        yield return TakeItemWithDelay("Cuc Hall 2");
        yield return TakeItemWithDelay("Cuc Hall 3");
        yield return TakeItemWithDelay("Cuc Hall 4");
        yield return TakeItemWithDelay("Cuc Hall 5");
        yield return TakeItemWithDelay("Apple Hall");
        yield return TakeItemWithDelay("Frerard3");

        Remember("Hall");
    }

    private IEnumerator TakeItemWithDelay(string itemName)
    {
        _inventory.Hack(GameObject.Find(itemName));
        yield return new WaitForSeconds(0.15f);
    }
}
