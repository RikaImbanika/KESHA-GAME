using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeSpawner : MonoBehaviour
{
    public string _id;
    private string _idHealth;
    private string _idType;
    private string _idPos;
    private string _sceneName;
    private float _health;
    private byte _type;
    public bool _instant;
    public bool _forLoader;

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

        if (string.IsNullOrEmpty(_id))
            _id = S.ID("SN", gameObject);

        _idHealth = S.IDM(_id, "hp");
        _idType = S.IDM(_id, "type");
        _idPos = S.IDM(_id, "pos");
    }

    IEnumerator Birn()
    {
        while (S.SM == null)
            yield return new WaitForSeconds(0.2f);

        _health = S.SM.LoadFloat(_idHealth) ?? -404f;

        if (_health == -404f)
            DefineExistenz();
        else if (_health <= 0f)
            Destroy(gameObject);
        else
            Load();
    }

    void DefineExistenz()
    {
        if (_instant)
        {
            if (_type == 0)
                _type = (byte)Random.Range(1, 4 + 1);

            S.SM.Save(_idType, _type);
            Summon();
        }
        else if (S.Backrooms._snakes.ContainsKey(_sceneName))
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

    void Load()
    {
        transform.position = S.SM.LoadVector3(_idPos) ?? transform.position;
        _type = S.SM.LoadByte(_idType) ?? 1;
        Summon();
    }

    void Summon()
    {
        GameObject obj = null;

        obj = Instantiate(S.Snakes[_type - 1], transform.position, transform.rotation, transform);

        Transform snakeBrainObj = obj.transform.Find("SnakeBrain");

        SnakeBrain snakeBrain = snakeBrainObj.GetComponent<SnakeBrain>();
        snakeBrain._id = _id;
        snakeBrain._forLoader = _forLoader;

        if (_health > 0)
            snakeBrain._health = _health;

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

        Destroy(this);
    }
}
