using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Prefabs : object
{
	public static GameObject Get(string name)
	{
		return Resources.Load<GameObject>($"Prefabs/{name}");
	}
}
