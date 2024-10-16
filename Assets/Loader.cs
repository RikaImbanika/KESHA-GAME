using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader : object
{
    public static Loader0 l;
    static AllFather _allFather;

    public static void Init()
    {
        l = new Loader0();
    }
}

public class Loader0 : MonoBehaviour
{
    public AllFather _allFather;
    public Dictionary<string, List<string>> _map;
    public List<string> _scenesToLoad;

    public void Start()
    {
        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();

        SceneManager.LoadSceneAsync("Income", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Corridor", LoadSceneMode.Additive);

        Save s = new Save();
        s._scene = "Income";
        _allFather.Save("sceneName", s);

        _map = new Dictionary<string, List<string>>();
        _scenesToLoad = new List<string>();

        AddValue("Income", "Corridor");

        AddValue("Corridor", "Income");
        AddValue("Corridor", "Hall");

        AddValue("Hall", "Corridor");
        AddValue("Hall", "BR 1");
        AddValue("Hall", "TL 0");
        AddValue("Hall", "MR 2");

        AddValue("MR 2", "Hall");
        AddValue("MR 2", "MR 1");

        AddValue("MR 1", "MR 2");
        AddValue("BR 1", "MR 3");

        AddValue("MR 3", "MR 1");
        AddValue("MR 3", "MR 4");

        AddValue("MR 4", "MR 3");

        AddValue("TL 0", "Hall");
        AddValue("TL 0", "TL 1");

        AddValue("TL 1", "TL 0");
        AddValue("TL 1", "TL 2");

        AddValue("TL 2", "TL 1");

        AddValue("BR 1", "Hall");
        AddValue("BR 1", "BR 2");
        AddValue("BR 1", "BR 4R"); //1

        AddValue("BR 2", "BR 1");
        AddValue("BR 2", "BR 3");
        AddValue("BR 2", "BR 3R"); //2

        AddValue("BR 3", "BR 2");
        AddValue("BR 3", "BR 4");
        AddValue("BR 3", "BR 2R"); //3

        AddValue("BR 4", "BR 3");
        AddValue("BR 4", "BR 5");
        AddValue("BR 4", "BR 8"); //4

        AddValue("BR 5", "BR 4");
        AddValue("BR 5", "BR 6");
        AddValue("BR 5", "BR 1R");
        AddValue("BR 5", "BR 7"); //5

        AddValue("BR 6", "BR 5");
        AddValue("BR 6", "BR 7R"); //6

        AddValue("BR 7R", "BR 6");
        AddValue("BR 7R", "BR 8");
        AddValue("BR 7R", "BR 2R"); //7

        AddValue("BR 8", "BR 7R");
        AddValue("BR 8", "BR 4");
        AddValue("BR 8", "BR 1R"); //8

        AddValue("BR 7", "BR 2R");
        AddValue("BR 7", "BR 4R");
        AddValue("BR 7", "BR 5"); //9

        AddValue("BR 2R", "BR 7");
        AddValue("BR 2R", "BR 7R");
        AddValue("BR 2R", "BR 3"); //10

        AddValue("BR 6R", "BR 4R");
        AddValue("BR 6R", "BR 3R"); //11

        AddValue("BR 3R", "BR 6R");
        AddValue("BR 3R", "BR 1R");
        AddValue("BR 3R", "BR 2"); //12

        AddValue("BR 1R", "BR 3R");
        AddValue("BR 1R", "BR 8");
        AddValue("BR 1R", "BR 5"); //13

        AddValue("BR 4R", "BR 6R");
        AddValue("BR 4R", "BR 1");
        AddValue("BR 4R", "BR 7"); //14
    }

    public void AddValue(string key, string value)
    {
        if (!_map.ContainsKey(key))
            _map[key] = new List<string>();

        _map[key].Add(value);
    }

    public void AddictiveLoadAsync()
    {
        StartCoroutine(ALA());

        IEnumerator ALA()
        {
            while (_scenesToLoad.Count > 0)
            {
                string scene = _scenesToLoad[0];

                while (!SceneCurrentlyLoaded(scene))
                    yield return new WaitForSeconds(0.05f);

                Scene targetScene = SceneManager.GetSceneByName(scene);
                SceneManager.SetActiveScene(targetScene);

                List<string> ids = _allFather.Load(scene)._strings;
                for (int i = 0; i < ids.Count; i++)
                {
                    Save s = _allFather.Load(ids[i]);
                    if (s._unnatural)
                    {
                        GameObject prefab = Prefabs.Get(s._name);
                        GameObject obj = Instantiate(prefab, s._position, s._rotation);
                        ItemP itemP = obj.GetComponent<ItemP>();
                        itemP._unnatural = s._unnatural;
                    }
                }

                _scenesToLoad.RemoveAt(0);
            }

            Scene tScene = SceneManager.GetSceneByName(_allFather.Load("sceneName")._scene);
            SceneManager.SetActiveScene(tScene);
        }
    }

    public bool SceneCurrentlyLoaded(string name)
    {
        for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == name)
                return scene.isLoaded;
        }

        return false;
    }
}
