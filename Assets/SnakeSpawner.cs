using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeSpawner : MonoBehaviour
{
    private string _id;
    private string _sceneName;
    private float _life;
    private byte _type;


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

        _life = S.SM.LoadFloat(S.ID(_id, "snakieLife")) ?? -404f;

        if (_life == -404f)
            DefineExistenz();
        else if (_life <= 0f)
            Destroy(gameObject);
        else
            Summon();
    }

    void DefineExistenz()
    {
        if (S.Backrooms._snakes.ContainsKey(_sceneName))
        {
            _type = S.Backrooms._snakes[_sceneName];
            S.SM.Save(S.ID(_id, "snakieLife"), 300);
            S.SM.Save(S.ID(_id, "snakieType"), _type);
            Summon();
        }
        else
        {
            S.SM.Save(S.ID(_id, "snakieLife"), -404);
            Destroy(gameObject);
        }
    }

    void Summon()
    {
        if (_type == 1)
            Instantiate(S.Snakie1, transform.position, transform.rotation, transform.parent);
        else if (_type == 2)
            Instantiate(S.Snakie2, transform.position, transform.rotation, transform.parent);
        else if (_type == 3)
            Instantiate(S.Snakie3, transform.position, transform.rotation, transform.parent);
        Destroy(gameObject);
    }
}
