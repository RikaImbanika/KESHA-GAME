using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Prefabs : object
{
	public static GameObject Get(string name)
	{
		return Resources.Load<GameObject>($"Prefabs/{name}");
	}

	public static AudioSource GetAudioSource(string name)
	{
		return Resources.Load<AudioSource>($"Prefabs/AudioSources/{name}");
	}

	public static Material GetMaterial(string name)
	{
		return Resources.Load<Material>($"Materials/{name}");
	}
}