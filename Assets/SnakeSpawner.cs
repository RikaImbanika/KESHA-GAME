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
        GameObject obj = null;

        obj = Instantiate(S.Snakes[_type - 1], transform.position, transform.rotation, transform);

        //Debug.LogError("XXX 0");

        Transform snakeBrainObj = obj.transform.Find("SnakeBrain");
        if (snakeBrainObj != null)
        {
            //Debug.LogError("XXX 1");

            SnakeBrain snakeBrain = snakeBrainObj.GetComponent<SnakeBrain>();

            snakeBrain._points = new Vector3[4];
            //Debug.LogError("XXX 3");
            for (int i = 0; i < 4; i++)
            {
                snakeBrain._points[i] = _corners[i].transform.position;
                Destroy(_corners[i]);
            }
            //Debug.LogError("XXX 4");
        }
        else
            //Debug.LogError("XXX 2");

        Destroy(this);
    }
}
