using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public string _savesDirectory;
    private TheSave _currentSave;
    public TheSave _lastSave;

    public void Start()
    {
        S.SM = this;
        _savesDirectory = Application.persistentDataPath;
        _currentSave = new TheSave();
        _lastSave = new TheSave();
    }

    public void SaveCurrentToLast()
    {
        _currentSave.SaveFloat("playerXRot", S.PlayerCamScript.xRotation);
        _currentSave.SaveFloat("playerYRot", S.PlayerCamScript.yRotation);
        _currentSave.SaveVector3("playerPosition", S.Ph.transform.position);
        _currentSave.SaveString("sceneName", S.PS._currentSceneName);
        _currentSave.SaveFloat("health", S.PS._health);

        Destroy(_lastSave);
        _lastSave = _currentSave.DeepCopy();
    }

    public void ReinitCurrentSave()
    {
        _currentSave.SaveFloat("playerXRot", 404.0f);
        _currentSave.SaveFloat("playerYRot", 404.0f);
        _currentSave.SaveVector3("playerPosition", new Vector3(0, 0, 0));
        _currentSave.SaveString("sceneName", "Income");
        //S.Inventory.SaveL2();
    }

    public TheSave CurrentSave
    {
        get { return _currentSave; }
    }

    void ReinitLastSave()
    {
        _lastSave = new TheSave();
    }

    public bool? LoadBool(string key)
    {
        return _currentSave.LoadBool(key);
    }

    public int? LoadInt(string key)
    {
        return _currentSave.LoadInt(key);
    }

    public float? LoadFloat(string key)
    {
        return _currentSave.LoadFloat(key);
    }

    public string LoadString(string key)
    {
        return _currentSave.LoadString(key);
    }

    public List<string> LoadListString(string key)
    {
        return _currentSave.LoadListString(key);
    }

    public List<bool> LoadListBool(string key)
    {
        return _currentSave.LoadListBool(key);
    }

    public List<int> LoadListInt(string key)
    {
        return _currentSave.LoadListInt(key);
    }

    public List<float> LoadListFloat(string key)
    {
        return _currentSave.LoadListFloat(key);
    }

    public Vector3? LoadVector3(string key)
    {
        return _currentSave.LoadVector3(key);
    }

    public Quaternion? LoadQuaternion(string key)
    {
        return _currentSave.LoadQuaternion(key);
    }

    public void RemoveBool(string key)
    {
        _currentSave.RemoveBool(key);
    }

    public void RemoveInt(string key)
    {
        _currentSave.RemoveInt(key);
    }

    public void RemoveFloat(string key)
    {
        _currentSave.RemoveFloat(key);
    }

    public void RemoveString(string key)
    {
        _currentSave.RemoveString(key);
    }

    public void RemoveVector3(string key)
    {
        _currentSave.RemoveVector3(key);
    }

    public void RemoveQuaternion(string key)
    {
        _currentSave.RemoveQuaternion(key);
    }

    public void RemoveListString(string key)
    {
        _currentSave.RemoveListString(key);
    }

    public void RemoveFromList(string key, object value)
    {
        _currentSave.RemoveFromList(key, value);
    }

    public void AddToList(string key, object value)
    {
        _currentSave.AddToList(key, value);
    }

    public void Save(string key, object value)
    {
        Type type = value.GetType();
        if (type == typeof(bool))
        {
            _currentSave.SaveBool(key, (bool)value);
        }
        else if (type == typeof(int))
        {
            _currentSave.SaveInt(key, (int)value);
        }
        else if (type == typeof(float))
        {
            _currentSave.SaveFloat(key, (float)value);
        }
        else if (type == typeof(string))
        {
            _currentSave.SaveString(key, (string)value);
        }
        else if (type == typeof(Vector3))
        {
            _currentSave.SaveVector3(key, (Vector3)value);
        }
        else if (type == typeof(List<string>))
        {
            _currentSave.SaveListString(key, (List<string>)value);
        }
        else if (type == typeof(List<bool>))
        {
            _currentSave.SaveListBool(key, (List<bool>)value);
        }
        else if (type == typeof(List<int>))
        {
            _currentSave.SaveListInt(key, (List<int>)value);
        }
        else if (type == typeof(List<float>))
        {
            _currentSave.SaveListFloat(key, (List<float>)value);
        }
        else if (type == typeof(Quaternion))
        {
            _currentSave.SaveQuaternion(key, (Quaternion)value);
        }
    }

    public void LoadLastSave()
    {
        Destroy(_currentSave);
        _currentSave = _lastSave.DeepCopy();

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene0 = SceneManager.GetSceneAt(i);

            if (scene0.name != "Start")
                SceneManager.UnloadSceneAsync(scene0);
        }

        LoadInventory();

        S.PS._health = LoadFloat("health") ?? 50;
        S.PS.VisualiseHealth();
        S.PS._currentSceneName = LoadString("sceneName");

        List<string> map = new List<string>();
        map.AddRange(S.Loader._map[S.PS._currentSceneName]);
        map.Add(S.PS._currentSceneName);

        foreach (string sceneName in map)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            S.Loader.PleaseLoadScene(sceneName);
        }

        StartCoroutine(CR());

        IEnumerator CR()
        {
            while (!SceneManager.GetSceneByName(S.PS._currentSceneName).isLoaded)
                yield return new WaitForSeconds(0.2f);
            while (!S.Loader.LoadedScene(S.PS._currentSceneName))
                yield return new WaitForSeconds(0.2f); //Okay okay

            S.Ph.transform.position = S.SaveManager.CurrentSave.LoadVector3("playerPosition") ?? Vector3.zero; /////////////
            float playerXRot = S.SaveManager.CurrentSave.LoadFloat("playerXRot") ?? 0f;
            float playerYRot = S.SaveManager.CurrentSave.LoadFloat("playerYRot") ?? 0f;
            S.PlayerCamScript.StaticRotate(playerXRot, playerYRot);

            Debug.Log("SAVE LOADED!!!");
        }
    }

    private void LoadInventory()
    {
        for (int i = 0; i < 36; i++)
        {
            string itemName = LoadString(S.ID("INV name ", i));

            if (!string.IsNullOrEmpty(itemName))
            {
                S.Inventory.items[i]._name = itemName;
                S.Inventory.items[i]._count = LoadInt(S.ID("INV count ", i)) ?? 404;
            }
            else
            {
                S.Inventory.items[i]._name = "";
                S.Inventory.items[i]._count = 0;
            }
        }
        S.Inventory.Visualize();
    }
}
