using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System;
using Unity.VisualScripting;

public class AllFather : MonoBehaviour
{
	public List<GameObject> _spots;
	public int _enemyBulletSparklesCount;

	void Start()
	{
		S.AllFatherObj = gameObject;
		S.AllFather = this;

		StartCoroutine(LoggerOfTheSave());

        IEnumerator Start0()
		{
			while (S.SaveManager == null)
			{
				yield return new WaitForSeconds(0.1f);
				Debug.Log("AllFather waiting for S.SaveManager");
			}

            S.SaveManager.ReinitCurrentSave();

            _spots = new List<GameObject>();
        }

		StartCoroutine(FirstSave(7f));

		IEnumerator FirstSave(float delay)
		{
			yield return new WaitForSeconds(delay);
			while (S.SaveManager == null)
				yield return new WaitForSeconds(0.1f);

			S.SaveManager.SaveCurrentToLast();
		}
	}

	IEnumerator LoggerOfTheSave()
	{
		float delay = 10f;

		while (S.SaveManager == null)
			yield return new WaitForSeconds(0.1f);

		string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
									   "Kesha_Game_Save_Log.txt");

		while (true)
		{
			yield return new WaitForSeconds(delay);

			string logContent = "=========================\r\n";
			logContent += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";
			logContent += S.SaveManager.CurrentSave.ToString();
			logContent += "\r\n=======================\r\n\r\n";

			File.WriteAllText(savePath, logContent);
		}
	}

	public Dictionary<string, OldSave> DeepCopyDictionary(Dictionary<string, OldSave> original)
	{
		Dictionary<string, OldSave> copy = new Dictionary<string, OldSave>();

		foreach (var entry in original)
		{
			string key = entry.Key;
			OldSave originalValue = entry.Value;

			string json = JsonUtility.ToJson(originalValue);

			OldSave copyValue = JsonUtility.FromJson<OldSave>(json);

			copy.Add(key, copyValue);
		}

		return copy;
	}

	public EnemyParams GetEnemyParams(string name)
	{
		EnemyParams ep = new EnemyParams();

		if (name == "zombie")
		{
			ep._screamerX = 0;
			ep._screamerY = -3.9f;
			ep._screamerZ = 0.60f;
			ep._screamerSounds = new string[] { "screamer1", "screamer2", "screamer3", "screamer4", "screamer5", "screamer6", "screamer7" };
		}
		else if (name == "professor")
		{
			ep._screamerX = 0;
			ep._screamerY = -4.2f;
			ep._screamerZ = 0.65f;
			ep._screamerSounds = new string[] { "screamer1", "screamer2", "screamer3", "screamer4", "screamer5", "screamer6", "screamer7" };
		}
		else if (name == "musculus")
		{
			ep._screamerX = 0;
			ep._screamerY = -4.6f;
			ep._screamerZ = 2.7f;
			ep._screamerSounds = new string[] { "screamer1", "screamer2", "screamer3", "screamer4", "screamer5", "screamer6", "screamer7" };
		}
		else if (name == "ghost")
		{
			ep._screamerX = 0;
			ep._screamerY = -3;
			ep._screamerZ = 1.5f;
			ep._screamerSounds = new string[] { "screamer1", "screamer2", "screamer3", "screamer4", "screamer5", "screamer6", "screamer7" };
		}

		return ep;
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