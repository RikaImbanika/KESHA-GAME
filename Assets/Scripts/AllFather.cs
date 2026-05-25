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
using System.Text;

public class AllFather : MonoBehaviour
{
	public OnStartPanel _osp;
	public List<GameObject> _spots;
	public int _enemyBulletSparklesCount;

	void Start()
	{
		S.AllFatherObj = gameObject;
		S.AllFather = this;
		_osp.enabled = true;
		_osp.gameObject.SetActive(true);

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

	public void Shuffle(int[] array)
	{
		for (int i = array.Length - 1; i > 0; i--)
		{
			int j = S.RND.Next(i + 1);
			(array[i], array[j]) = (array[j], array[i]);
		}
	}

	public string SelFromProb(List<(string Item, int Weight)> probabilities)
	{
		int total = probabilities.Sum(p => p.Weight);
		int rand = S.RND.Next(total);
		int cumulative = 0;

		foreach (var (item, weight) in probabilities)
		{
			cumulative += weight;
			if (rand < cumulative)
				return item;
		}

		return null; // Should never reach here if input is valid
	}

	public EnemyParams GetEnemyParams(string name)
	{
		EnemyParams ep = new EnemyParams();

		if (name == "zombie")
		{
			ep._screamerX = 0;
			ep._screamerY = -3.9f;
			ep._screamerZ = 0.60f;
			ep._screamerSounds = new string[] { "Screamer 1", "Screamer 2", "Screamer 3", "Screamer 4", "Screamer 5", "Screamer 6", "Screamer 7" };
		}
		else if (name == "bakalavr")
		{
			ep._screamerX = 0;
			ep._screamerY = -4.2f;
			ep._screamerZ = 0.65f;
			ep._screamerSounds = new string[] { "Screamer 1", "Screamer 2", "Screamer 3", "Screamer 4", "Screamer 5", "Screamer 6", "Screamer 7" };
		}
		else if (name == "musculus")
		{
			ep._screamerX = 0;
			ep._screamerY = -4.6f;
			ep._screamerZ = 2.7f;
			ep._screamerSounds = new string[] { "Screamer 1", "Screamer 2", "Screamer 3", "Screamer 4", "Screamer 5", "Screamer 6", "Screamer 7" };
		}
		else if (name == "ghost")
		{
			ep._screamerX = 0;
			ep._screamerY = -3;
			ep._screamerZ = 1.5f;
			ep._screamerSounds = new string[] { "Screamer 1", "Screamer 2", "Screamer 3", "Screamer 4", "Screamer 5", "Screamer 6", "Screamer 7" };
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