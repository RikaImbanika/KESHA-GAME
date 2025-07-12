using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System.Linq;

public class Teleporter : MonoBehaviour
{
    public void Start()
    {
        S.Teleporter = this;
    }

    public void SoLetsFuckingTeleportSomewhereRightNow(string nextSceneName)
    {
        string current = S.SM.LoadString("sceneName");
        GoFuckIt();

        void GoFuckIt()
        {
            List<Scene> loadedScenes = new List<Scene>();

            List<string> loadScenesNames = new List<string>();
            loadScenesNames.AddRange(S.Loader._map[nextSceneName]);
            loadScenesNames.Add(nextSceneName);

            List<string> unloadScenesNames = new List<string>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
                if (SceneManager.GetSceneAt(i).name != "Start")
                    unloadScenesNames.Add(SceneManager.GetSceneAt(i).name);

            ImportantStaticShitToDo(name);

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

            StartCoroutine(WaitLoad(nextSceneName, 1, new Vector3(0, 0, -1)));
        }

        IEnumerator UnloadSceneAsync(Scene scene)
        {
            if (scene.isLoaded)
            {
                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);
                while (!asyncUnload.isDone) yield return null;
                Debug.Log($"UNLOADED DUPLICATE SCENE: {scene.name}!");
            }
        }
    }

    public void ImportantStaticShitToDo(string nextSceneName)
    {
        if (nextSceneName != "Start")
        {
            S.PS._currentSceneName = nextSceneName;
            S.SaveManager.CurrentSave.SaveString("sceneName", nextSceneName);
        }

        S.MM._playerOnIncome = (nextSceneName == "Income");

        if (S.MM._playerOnIncome)
            S.MM.EnterIncome();
        else
            S.MM.LeaveIncome(); //What do u want from me?

        if (nextSceneName.Contains("BR"))
            S.MM.EnterBackrooms();
        else
            S.MM.LeaveBackrooms();
    }

    public IEnumerator WaitLoad(string nextSceneName, int doorId, Vector3 dir)
    {
        while (!S.AllFather.SceneCurrentlyLoaded(nextSceneName))
            yield return new WaitForSeconds(0.2f);

        PlayerMovement pm = S.Ph.GetComponent<PlayerMovement>();

        Vector3 v = new Vector3(0, -1.7f, 0);
        if (pm.isCrouching)
            v = new Vector3(0, -3.9f, 0);

        var nextDoorModel = S.Loader._rooms[nextSceneName]._doors[doorId];
        float dRotation = -Vector3.SignedAngle(dir, -nextDoorModel._right, new Vector3(0, -1, 0));
        Vector3 offset = nextDoorModel._right * 2;

        Debug.Log($"Forward is {offset.x} {offset.y} {offset.z}.");

        S.Ph.transform.position = nextDoorModel._coordinates + v + offset;
        S.PlayerCamScript.Rotate(dRotation);

        Debug.Log($"GONE TO SCENE {nextSceneName}");

        S.SDC.RequestCleanup();
    }
}
