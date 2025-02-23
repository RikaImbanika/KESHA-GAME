using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CHEATS : MonoBehaviour
{
    bool _cheats;
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
        _cheated = new List<string>();
        Transform canvasTransform = S.Canvas.transform; // Получите Transform Canvas
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
            if (S.PS._health < 100)
                S.PS._health = 100;

            if (S.PS._currentSceneName == "Income" && !IsCheated("Income"))
                StartCoroutine(IncomeCheat());
            if (S.PS._currentSceneName == "Corridor" && !IsCheated("Corridor"))
                StartCoroutine(CorridorCheat());
            if (S.PS._currentSceneName == "Hall" && !IsCheated("Hall"))
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
            S.AllFather.Save(_cheated[i], s);
        }
    }

    void Load()
    {
    }

    IEnumerator CorridorCheat()
    {
        Remember("Corridor");

        yield return TakeItemWithDelay("apple 0 cor");
        yield return TakeItemWithDelay("apple 1 cor");
        yield return TakeItemWithDelay("apple 2 cor");
        yield return TakeItemWithDelay("apple 3 cor");
        yield return TakeItemWithDelay("apple 4 cor");
        yield return TakeItemWithDelay("apple 5 cor");
        yield return TakeItemWithDelay("apple 6 cor");
        yield return TakeItemWithDelay("cuc cor");
    }

    IEnumerator IncomeCheat()
    {
        Remember("Income");

        var obj = GameObject.Find("LimeLocker");
        Debug.Log(obj);
        var locker = obj.GetComponent<Locker>();
        Debug.Log(locker);
        locker.Cheat();
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
    }

    IEnumerator HallCheat()
    {
        Remember("Hall");

        yield return TakeItemWithDelay("GreenKey");
        yield return TakeItemWithDelay("Cuc Hall 0");
        yield return TakeItemWithDelay("Cuc Hall 1");
        yield return TakeItemWithDelay("Cuc Hall 2");
        yield return TakeItemWithDelay("Cuc Hall 3");
        yield return TakeItemWithDelay("Cuc Hall 4");
        yield return TakeItemWithDelay("Cuc Hall 5");
        yield return TakeItemWithDelay("Apple Hall");
        yield return TakeItemWithDelay("Frerard3");
    }

    private IEnumerator TakeItemWithDelay(string itemName)
    {
        S.Inventory.Hack(GameObject.Find(itemName));
        yield return new WaitForSeconds(0.15f);
    }
}
