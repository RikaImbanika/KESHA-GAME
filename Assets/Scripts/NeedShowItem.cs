using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedShowItem : MonoBehaviour
{
	public bool[] need;

	public void Start()
	{
		need = new bool[50];

		for (int i = 0; i < 50; i++)
			need[i] = true;
	}

	public int ItemId(string name)
	{
		switch (name)
		{
			case "cucumber":
				return 0;
			default:
				return -1;
		}
	}

	public bool NeedOnce(string name)
	{
		int id = ItemId(name);

		if (id < 0)
			return false;
		else
		{
			if (need[id])
			{
				need[id] = false;
				return true;
			}
			else
				return false;
		}
	}
}
