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
        _id = S.ID("SN", gameObject);
    }

    IEnumerator Birn()
    {
        while (S.SM == null)
            yield return new WaitForSeconds(0.2f);

        _idHealth = S.IDM(_id, "hp");
        _idType = S.IDM(_id, "type");

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

        if (snakeBrain._head._type == "Silent")
        {
            snakeBrain._lookNameColor = new Color(0.4f, 0.063f, 1f);
            snakeBrain._lifeBarNameColor = new Color(0.4f, 0.063f, 1f);
            snakeBrain._lifeColor = Color.black;
        }
        else if (snakeBrain._head._type == "Nature")
        {
            snakeBrain._lookNameColor = new Color(0.2f, 1f, 0.1f);
            snakeBrain._lifeBarNameColor = new Color(0.2f, 1f, 0.1f);
            snakeBrain._lifeColor = Color.black;
        }
        else if (snakeBrain._head._type == "Ice")
        {
            snakeBrain._lookNameColor = new Color(0.07f, 0.847f, 1f);
            snakeBrain._lifeBarNameColor = new Color(0.07f, 0.847f, 1f);
            snakeBrain._lifeColor = Color.black;
        }
        else
        {
            snakeBrain._lookNameColor = new Color(1f, 0.92f, 0.152f);
            snakeBrain._lifeBarNameColor = new Color(1f, 0.92f, 0.152f);
            snakeBrain._lifeColor = Color.black;
        }

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
