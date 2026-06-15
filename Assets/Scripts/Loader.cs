// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

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
    public Dictionary<string, string> _aliases;
    private List<string> _scenesToLoad;
    private bool _workingOnIt;
    private Dictionary<string, Transform> _roots;
    private Dictionary<string, SceneRoot> _sceneRoots;
    public bool _teleporting;

    public void Start()
    {
        _roots = new Dictionary<string, Transform>();
        _sceneRoots = new Dictionary<string, SceneRoot>();
        _scenesToLoad = new List<string>();
        FirstWaitLoad();
    }

    public Dictionary<string, Transform> Roots
    {
        get
        {
            return _roots;
        }
        set
        {
            _roots = value;
        }
    }

    public Dictionary<string, SceneRoot> SceneRoots
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

    void FirstWaitLoad()
    {
        StartCoroutine(MT());

        IEnumerator MT()
        {
            S.Loader = this;

            while (S.AllFather == null || S.Console == null)
                yield return new WaitForSeconds(0.1f);

            InitMap();
            FillAliases();

            SceneManager.LoadSceneAsync("Income", LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync("Corridor", LoadSceneMode.Additive);

            while (!S.AllFather.SceneCurrentlyLoaded("Income"))
                yield return new WaitForSeconds(0.2f);

            Vector3 v = new Vector3(0, 0, 0);
            if (S.Pm.isCrouching)
                v = new Vector3(0, -2.2f, 0);

            S.Ph.transform.position = new Vector3(6.08f, -13.18f, -852.67f) + v;

            while (!S.Loader.Roots.ContainsKey("Income"))
                yield return new WaitForSeconds(0.2f);

            S.Fog.SetFog("Income", Roots["Income"].gameObject);

            while (S.PS == null)
                yield return new WaitForSeconds(0.2f);

            S.PS._currentSceneName = "Income";

            while (S.SM == null)
                yield return new WaitForSeconds(0.2f);

            S.SM.Save("sceneName", "Income");

            yield return null;
        }
    }

    void FillAliases()
    {
        _aliases = new Dictionary<string, string>();

        foreach (string name in _rooms.Keys)
        {
            string a1 = name.ToLower();
            string a2 = S.TextProcessor.ToSnakeCase(name);
            string a3 = S.TextProcessor.ToPascalSnakeCase(name);
            //TO DO: a2 & a3 will not work here

            if (!_aliases.ContainsKey(a1))
                _aliases.Add(a1, name);

            if (!_aliases.ContainsKey(a2))
                _aliases.Add(a2, name);

            if (!_aliases.ContainsKey(a3))
                _aliases.Add(a3, name);
        }
    }

    public IEnumerator WaitLoad(string nextSceneName, int doorId, Vector3 dir)
    {
        float deadline = 0f;

        while (!S.AllFather.SceneCurrentlyLoaded(nextSceneName) && deadline < 5f)
        {
            yield return new WaitForSeconds(0.05f);
            deadline += 0.05f;
        }

        Vector3 v = new Vector3(0, -1.7f, 0);
        if (S.Pm.isCrouching)
            v = new Vector3(0, -3.9f, 0);

        var nextDoorModel = S.Loader._rooms[nextSceneName]._doors[doorId];
        float dRotation = -Vector3.SignedAngle(dir, -nextDoorModel._right, new Vector3(0, -1, 0));
        Vector3 offset = nextDoorModel._right * 2;

        S.Ph.transform.position = nextDoorModel._coordinates + v + offset;
        S.PlayerCamScript.Rotate(dRotation);

        S.SDC.RequestCleanup();

        _teleporting = false;
    }

    public IEnumerator WaitLoadPortal(string nextSceneName)
    {
        float deadline = 0f;

        while (!S.AllFather.SceneCurrentlyLoaded(nextSceneName) && deadline < 5f)
        {
            yield return new WaitForSeconds(0.2f);
            deadline += 0.2f;
        }

        _teleporting = false;
    }

    public void GoTo(string nextSceneName, int doorId, Vector3 dir)
    {
        StartCoroutine(GoToAsync());

        IEnumerator GoToAsync()
        {
            while (_teleporting)
                yield return new WaitForSeconds(0.1f);

            _teleporting = true;

            ImportantStaticShitToDo(nextSceneName);

            if (nextSceneName == "Start")
            {
                //We should not be here
                _teleporting = false;
                yield break;
            }

            List<string> newScenesNames = new List<string>();
            newScenesNames.AddRange(S.Loader._map[nextSceneName]);
            newScenesNames.Add(nextSceneName);

            List<string> oldScenesNames = new List<string>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
                if (SceneManager.GetSceneAt(i).name != "Start")
                    oldScenesNames.Add(SceneManager.GetSceneAt(i).name);

            foreach (string name in newScenesNames)
                if (!oldScenesNames.Contains(name))
                {
                    SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
                    S.Loader.PleaseLoadScene(name);
                }

            foreach (string name in oldScenesNames)
                if (!newScenesNames.Contains(name))
                    try
                    {
                        SceneManager.UnloadSceneAsync(name);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"Error unloading scene {name}: {ex.Message}");
                    }

            if (doorId > 0)
            {
                StartCoroutine(WaitLoad(nextSceneName, doorId, dir));
            }
            else
            {
                StartCoroutine(WaitLoadPortal(nextSceneName));
            }
        }
    }

    public void InitMap()
    {
        _map = new Dictionary<string, List<string>>();
        _rooms = new Dictionary<string, RoomModel>();

        _rooms.Add("Start", new RoomModel("Start"));

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

    public void ImportantStaticShitToDo(string nextSceneName)
    {
        S.PS._currentSceneName = nextSceneName;
        S.SaveManager.CurrentSave.SaveString("sceneName", nextSceneName);

        S.MM._playerOnIncome = (nextSceneName == "Income");

        if (S.MM._playerOnIncome)
            S.MM.EnterIncome();
        else
            S.MM.LeaveIncome(); //What do u want from me? .. IDK BRO

        if (nextSceneName.Contains("BR"))
            S.MM.EnterBackrooms();
        else
            S.MM.LeaveBackrooms();

        if (!nextSceneName.Contains("TL") || nextSceneName == "TL 0" || nextSceneName == "TL 1")
            S.MM.LeaveToilet();
        else
            S.MM.EnterToilet();

        if (nextSceneName.Contains("TL") && nextSceneName != "TL 0")
            S.AmbienceManager.EnterToilet();
        else
            S.AmbienceManager.LeaveToilet();

        if (!nextSceneName.Contains("MR"))
            S.MM.LeaveMushrooms();
        else if (nextSceneName != "MR 1" && nextSceneName != "MR 2")
            S.MM.EnterMushrooms();
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
            string sceneName = _scenesToLoad[0];
            float elapsed = 0;

            while (S.AllFather == null)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log("Loader waiting for S.AllFater");
            }

            while (!S.AllFather.SceneCurrentlyLoaded(sceneName) ||
                   Roots == null ||
                   !Roots.ContainsKey(sceneName) ||
                   Roots[sceneName] == null)
            {
                elapsed += 0.1f;
                if (elapsed > 20f)
                {
                    Debug.LogError($"Timeout loading scene {sceneName}");
                    _workingOnIt = false;
                    yield break;
                }
                yield return new WaitForSeconds(0.1f);
            }

            while (S.SM == null)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log("Loader waiting for S.SaverManager");
            }

            try
            {
                Transform root = Roots[sceneName];
                                
                S.Fog.SetFog(sceneName, root.gameObject);

                List<string> ids = S.SM.LoadListString(S.IDM(sceneName, "ids"));

                if (ids != null)
                {
                    MaterialPropertyBlock mpt = S.Fog.GetMPB(sceneName);

                    for (int i = 0; i < ids.Count; i++)
                    {
                        string currentId = ids[i];
                        
                        if (currentId.Length < 3)
                        {
                            Debug.LogError($"Invalid id '{currentId}' – too short, skipping");
                            continue;
                        }

                        string prefix = currentId.Remove(2);
                        if (prefix == "IT")
                        {
                            InstaItem(); 
                        }
                        else if (prefix == "ZM")
                        {
                            InstaZombie();
                        }
                        else if (prefix == "SN")
                        {
                            InstaSnake();
                        }

                        void InstaSnake()
                        {
                            try
                            {
                                Vector3 pos = S.SM.LoadVector3(S.IDM(currentId, "pos")) ?? Vector3.zero;
                                GameObject go = Instantiate(S.SnakeSpawner, pos, Quaternion.identity, root);
                                SnakeSpawner spawner = go.GetComponent<SnakeSpawner>();
                                spawner._id = currentId;
                            }
                            catch (Exception ex)
                            {
                                S.Console.AddMessage($"Can't instantiate \"{currentId}\" ({ex.Message})", Color.red);
                            }
                        }

                        void InstaZombie()
                        {
                            try
                            {
                                string prefabName = S.SM.LoadString(S.IDM(currentId, "name"));
                                Vector3 pos = S.SM.LoadVector3(S.IDM(currentId, "pos")) ?? Vector3.zero;
                                GameObject go = Instantiate(Prefabs.Get(prefabName), pos, Quaternion.identity, root);
                                Zombie zombie = go.GetComponent<Zombie>();
                                zombie._id = currentId;
                                zombie._forLoader = true;
                                S.Fog.ApplyToGameObject(go, mpt);
                            }
                            catch (Exception ex)
                            {
                                S.Console.AddMessage($"Can't instantiate \"{currentId}\" ({ex.Message})", Color.red);
                            }
                        }

                        void InstaItem()
                        {
                            string itemName = "not set yet";
                            try
                            {
                                itemName = S.SM.LoadString(S.IDM(currentId, "name"));
                                GameObject prefab = Prefabs.Get(S.II.Get(itemName)._prefabName);
                                GameObject obj = Instantiate(prefab, root);
                                ItemP itemP = obj.GetComponent<ItemP>();
                                itemP._id = currentId;
                                itemP._forLoader = true;
                                itemP._sceneName = sceneName;
                                S.Fog.ApplyToGameObject(obj, mpt);
                            }
                            catch (Exception ex)
                            {
                                S.Console.AddMessage($"Can't instantiate item \"{itemName}\" ({ex.Message})", Color.red);
                            }
                        }
                    }
                }

                _scenesToLoad.RemoveAt(0);
            }
            catch (Exception e)
            {
                S.Console.AddMessage($"Critical error loading scene {sceneName}: {e}", Color.red);
                Debug.LogError($"Critical error loading scene {sceneName}: {e}");
                if (_scenesToLoad.Count > 0)
                    _scenesToLoad.RemoveAt(0);
            }
            finally
            {
                _workingOnIt = false; 
            }
        }
    }

    public bool LoadedScene(string sceneName)
    {
        return !_scenesToLoad.Contains(sceneName);
    }
}
