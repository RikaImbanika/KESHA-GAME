using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeSpawner : MonoBehaviour
{
    private string _id;
    private string _idHealth;
    private string _idType;
    private string _sceneName;
    private float _health;
    private byte _type;
    public GameObject[] _corners;


    void Start()
    {
        StartCoroutine(Start0());

        IEnumerator Start0()
        {
            GetId();
            yield return Birn();
        }
    }

    void GetId()
    {
        _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
        _id = _sceneName + transform.position.x + transform.position.y + transform.position.z;
    }

    IEnumerator Birn()
    {
        while (S.SM == null)
            yield return new WaitForSeconds(0.2f);

        _idHealth = S.ID(_id, "snakeHp");
        _idType = S.ID(_id, "snakeType");

        _health = S.SM.LoadFloat(_idHealth) ?? -404f;

        if (_health == -404f)
            DefineExistenz();
        else if (_health <= 0f)
            Destroy(gameObject);
        else
            Summon();
    }

    void DefineExistenz()
    {
        if (S.Backrooms._snakes.ContainsKey(_sceneName))
        {
            _type = S.Backrooms._snakes[_sceneName];
            S.SM.Save(_idType, _type);
            Summon();
        }
        else
        {
            S.SM.Save(_idType, "skipped");
            S.SM.Save(_idHealth, "0");
            Destroy(gameObject);
        }
    }

    void Summon()
    {
        GameObject obj = null;

        obj = Instantiate(S.Snakes[_type - 1], transform.position, transform.rotation, transform);

        Transform snakeBrainObj = obj.transform.Find("SnakeBrain");

        SnakeBrain snakeBrain = snakeBrainObj.GetComponent<SnakeBrain>();
        snakeBrain._id = _id;
        snakeBrain._idHealth = _idHealth;

        if (_health != -404)
            snakeBrain._health = _health;

        snakeBrain._points = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            snakeBrain._points[i] = _corners[i].transform.position;

            Destroy(_corners[i]);
        }

        Destroy(this);
    }
}
