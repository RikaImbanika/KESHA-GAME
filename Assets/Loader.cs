using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using System;
using Unity.VisualScripting;

public class Loader : MonoBehaviour
{
    public Dictionary<string, RoomModel> _rooms;
    public Dictionary<string, List<string>> _map;
    private List<string> _scenesToLoad;
    private bool _workingOnIt;
    private Dictionary<string, Transform> _sceneRoots;

    public void Start()
    {
        _sceneRoots = new Dictionary<string, Transform>();
        _scenesToLoad = new List<string>();
        WaitLoad();
    }

    public Dictionary<string, Transform> Roots
    {
        get
        {
            return _sceneRoots;
        }
        set
        {
            _sceneRoots = value;
        }
    }

    public Dictionary<string, Transform> SceneRoots
    {
        get
        {
            return _sceneRoots;
        }
        set
        {
            _sceneRoots = value;
        }
    }

    void WaitLoad()
    {
        StartCoroutine(MT());

        IEnumerator MT()
        {
            S.Loader = this;

            while (S.AllFather == null)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log("Loader waiting for S.AllFater");
            }

            InitMap();

            SceneManager.LoadSceneAsync("Income", LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync("Corridor", LoadSceneMode.Additive);

            GameObject pObj = GameObject.FindGameObjectWithTag("Player");
            GameObject playerHub = pObj.transform.parent.gameObject;
            PlayerMovement pm = playerHub.GetComponent<PlayerMovement>();

            while (!S.AllFather.SceneCurrentlyLoaded("Income"))
                yield return new WaitForSeconds(0.2f);

            Vector3 v = new Vector3(0, 0, 0);
            if (pm.isCrouching)
                v = new Vector3(0, -2.2f, 0);

            playerHub.transform.position = new Vector3(6.08f, -13.18f, -852.67f) + v;

            while (S.PS == null)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log("Loader waiting for S.PlayerStorage");
            }

            while (S.PS == null)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log("Loader waiting for S.PlayerStorage");
            }

            S.PS._currentSceneName = "Income";

            while (S.SM == null)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log("Loader waiting for S.SaverManager");
            }

            S.SM.Save("sceneName", "Income");

            yield return null;
        }
    }

    public void GoTo(string sceneName, string nextSceneName)
    {
        if (nextSceneName == "Start")
            return;

        List<string> loadScenesNames = new List<string>();
        loadScenesNames.AddRange(S.Loader._map[nextSceneName]);
        loadScenesNames.Add(nextSceneName); //

        List<string> unloadScenesNames = new List<string>();
        if (sceneName != "Start")
        {
            unloadScenesNames.AddRange(S.Loader._map[sceneName]);
            unloadScenesNames.Add(sceneName); //?
        }

        foreach (string name in loadScenesNames)
            if (!unloadScenesNames.Contains(name))
            {
                SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
                S.Loader.PleaseLoadScene(name);
            }

        foreach (string name in unloadScenesNames)
            if (!loadScenesNames.Contains(name))
                try
                {
                    SceneManager.UnloadSceneAsync(name);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error unloading scene {name}: {ex.Message}");
                }
    }

    public void InitMap()
    {
        _map = new Dictionary<string, List<string>>();
        _rooms = new Dictionary<string, RoomModel>();

        AddValue("Income", "Corridor", 1, 1);

        AddValue("Corridor", "Income", 1, 1);
        AddValue("Corridor", "Hall", 2, 1);

        AddValue("Hall", "Corridor", 1, 2);
        AddValue("Hall", "BR 1", 2, 1);
        AddValue("Hall", "TL 0", 3, 1, true);
        AddValue("Hall", "MR 1", 4, 1);

        AddValue("MR 1", "Hall", 1, 4);
        AddValue("MR 1", "MR 2", 2, 1, true);

        AddValue("MR 2", "MR 1", 1, 2, true);
        AddValue("MR 2", "MR 3", 2, 1);

        AddValue("MR 3", "MR 2", 1, 2);
        AddValue("MR 3", "MR 4", 2, 1);

        AddValue("MR 4", "MR 3", 1, 2);

        AddValue("TL 0", "Hall", 1, 3, true);
        AddValue("TL 0", "TL 1", 2, 1);
        AddValue("TL 0", "TL 1", 3, 2);

        AddValue("TL 1", "TL 0", 1, 2);
        AddValue("TL 1", "TL 0", 2, 3);
        AddValue("TL 1", "TL 2", 3, 1);

        AddValue("TL 2", "TL 1", 1, 3);

        AddValue("BR 1", "Hall", 1, 2);
        AddValue("BR 1", "BR 2", 3, 1);
        AddValue("BR 1", "BR 4R", 2, 3); //1

        AddValue("BR 2", "BR 1", 1, 3);
        AddValue("BR 2", "BR 3", 3, 1);
        AddValue("BR 2", "BR 3R", 2, 1); //2

        AddValue("BR 3", "BR 2", 1, 3);
        AddValue("BR 3", "BR 4", 3, 1);
        AddValue("BR 3", "BR 2R", 2, 3); //3

        AddValue("BR 4", "BR 3", 1, 3);
        AddValue("BR 4", "BR 5", 3, 1);
        AddValue("BR 4", "BR 8", 2, 2); //4

        AddValue("BR 5", "BR 4", 1, 3);
        AddValue("BR 5", "BR 6", 3, 1);
        AddValue("BR 5", "BR 1R", 4, 1);
        AddValue("BR 5", "BR 7", 2, 3); //5

        AddValue("BR 6", "BR 5", 1, 3);
        AddValue("BR 6", "BR 7R", 2, 1); //6

        AddValue("BR 7R", "BR 6", 1, 2);
        AddValue("BR 7R", "BR 8", 3, 1);
        AddValue("BR 7R", "BR 2R", 2, 1); //7

        AddValue("BR 8", "BR 7R", 1, 3);
        AddValue("BR 8", "BR 4", 2, 2);
        AddValue("BR 8", "BR 1R", 3, 2); //8

        AddValue("BR 7", "BR 2R", 1, 2);
        AddValue("BR 7", "BR 4R", 2, 1);
        AddValue("BR 7", "BR 5", 3, 2); //9

        AddValue("BR 2R", "BR 7", 2, 1);
        AddValue("BR 2R", "BR 7R", 1, 2);
        AddValue("BR 2R", "BR 3", 3, 2); //10

        AddValue("BR 6R", "BR 4R", 1, 2);
        AddValue("BR 6R", "BR 3R", 2, 3); //11

        AddValue("BR 3R", "BR 6R", 3, 2);
        AddValue("BR 3R", "BR 1R", 2, 3);
        AddValue("BR 3R", "BR 2", 1, 2); //12

        AddValue("BR 1R", "BR 3R", 3, 2);
        AddValue("BR 1R", "BR 8", 2, 3);
        AddValue("BR 1R", "BR 5", 1, 4); //13

        AddValue("BR 4R", "BR 6R", 2, 1);
        AddValue("BR 4R", "BR 1", 3, 2);
        AddValue("BR 4R", "BR 7", 1, 2); //14
    }

    public void Update()
    {
        if (_scenesToLoad.Count > 0)
            AddictiveLoadAsync();
    }

    public void PleaseLoadScene(string sceneName)
    {
        if (sceneName == "Start")
            return;

        if (!_scenesToLoad.Contains(sceneName))
            _scenesToLoad.Add(sceneName);
        else
            Debug.LogError($"Sorre, scene {sceneName} already in queue.");
    }

    public void AddValue(string sceneName, string nextSceneName, int doorId1, int doorId2, bool locked = false)
    {
        if (!_map.ContainsKey(sceneName))
            _map.Add(sceneName, new List<string>());

        if (!_map[sceneName].Contains(nextSceneName))
            _map[sceneName].Add(nextSceneName);

        if (!_rooms.ContainsKey(sceneName))
            _rooms.Add(sceneName, new RoomModel(sceneName));

        if (!_rooms[sceneName]._doors.ContainsKey(doorId1))
            _rooms[sceneName]._doors.Add(doorId1, new DoorModel(locked));

        _rooms[sceneName]._doors[doorId1]._nextDoorId = doorId2;
        _rooms[sceneName]._doors[doorId1]._nextSceneName = nextSceneName;
    }

    private void AddictiveLoadAsync()
    {
        if (!_workingOnIt)
        {
            _workingOnIt = true;
            StartCoroutine(ALA());
        }

        IEnumerator ALA()
        {
            string _sceneName = _scenesToLoad[0];

            float elapsed = 0;

            while (S.AllFather == null)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log("Loader waiting for S.AllFater");
            }

            while (!SceneRoots.ContainsKey(_sceneName))
            {
                elapsed += 0.1f;

                if (elapsed > 20f)
                    yield break;

                yield return new WaitForSeconds(0.1f);
            }

            while (SceneRoots[_sceneName] == null)
            {
                //Null left from previous unloaded root

                elapsed += 0.1f;

                if (elapsed > 20f)
                    yield break;

                yield return new WaitForSeconds(0.1f);
            }

            Transform root = SceneRoots[_sceneName];

            while (!S.AllFather.SceneCurrentlyLoaded(_sceneName))
            {
                yield return new WaitForSeconds(0.1f);
            }

            List<string> ids = S.SM.LoadListString(S.ID(_sceneName, "ids"));
            if (ids != null)
                for (int i = 0; i < ids.Count; i++)
                {
                    string name = S.SM.LoadString(S.ID(ids[i], "name"));
                    try
                    {
                        GameObject prefab = Prefabs.Get(S.II.Get(name)._prefabName);
                        Vector3 pos = S.SM.LoadVector3(S.ID(ids[i], "position")) ?? Vector3.zero;
                        Quaternion rot = S.SM.LoadQuaternion(S.ID(ids[i], "rotation")) ?? Quaternion.identity;
                        GameObject obj = Instantiate(prefab, root);                        
                        ItemP itemP = obj.GetComponent<ItemP>();
                        itemP._id = ids[i];
                        obj.transform.position = pos;
                        obj.transform.rotation = rot;
                        itemP._forLoader = true;
                    }
                    catch (NullReferenceException ex)
                    {
                        Debug.LogError($"NULL IS HERE! ({name}) ({ex.Message})");
                    }
                }

            _scenesToLoad.RemoveAt(0);

            while (S.SM == null)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log("Loader waiting for S.SaverManager");
            }

            _workingOnIt = false;
        }
    }

    public bool LoadedScene(string sceneName)
    {
        return !_scenesToLoad.Contains(sceneName);
    }
}
