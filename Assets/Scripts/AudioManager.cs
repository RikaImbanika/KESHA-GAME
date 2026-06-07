// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	private Dictionary<string, AudioSource> _audioSources;

	public bool _muted;

	public void Start()
	{
		_audioSources = new Dictionary<string, AudioSource>();

		foreach (Transform child in transform)
		{
			var source = child.GetComponent<AudioSource>();
			if (source == null) continue;

			string key = child.name;

			_audioSources[key] = source;
		}

		S.AudioManager = this;
	}

	public void Play(string name)
	{
		if (!CanPlay(name))
			return;

		float pitch = DefinePitch(name);
		SetPitch(name, pitch);
		_audioSources[name].Play();
	}

	public void Play(string name, float pitch)
	{
		if (!CanPlay(name))
			return;

		SetPitch(name, pitch);
		_audioSources[name].Play();
	}

	public void Play(string name, float minPitch, float maxPitch)
	{
		if (!CanPlay(name))
			return;

		SetPitch(name, Random.Range(minPitch, maxPitch));
		_audioSources[name].Play();
	}
	
	void SetPitch(string name, float pitch)
	{
		_audioSources[name].pitch = pitch;
	}

	bool CanPlay(string name)
	{
		if (_muted)
			return false;

		if (!_audioSources.ContainsKey(name))
		{
			Debug.LogError($"No such audioSource {name} in code!");
			return false;
		}

		return true;
	}

	float DefinePitch(string name)
	{
		switch (name)
		{
			case "Door":
				return Random.Range(0.9f, 1.1f);
			case "Pick Up":
				return Random.Range(0.9f, 1.1f);
			case "Throw":
				return Random.Range(0.2f, 0.4f);
			case "Plasma":
				return Random.Range(0.9f, 1.1f);
			case "Money":
				return Random.Range(0.9f, 1.1f);
			case "Not Enough Cash":
				return 0.9f;
			case "No Ammo":
				return 0.9f;
			case "Toilet Door":
				return Random.Range(0.9f, 1.1f);
			case "Reload": //This is literally take ammo.
				return 1f;
			case "Toilet":
				return 1f;
			case "damage":
				return Random.Range(1.1f, 1.175f); //TO DO: Optimise
			case "Screamer 1":
				return Random.Range(0.9f, 1.1f);
			case "Screamer 2":
				return Random.Range(0.9f, 1.1f);
			case "Screamer 3":
				return Random.Range(0.9f, 1.1f);
			case "Screamer 4":
				return Random.Range(0.9f, 1.1f);
			case "Screamer 5":
				return Random.Range(0.9f, 1.1f);
			case "Screamer 6":
				return Random.Range(0.9f, 1.1f);
			case "Screamer 7":
				return Random.Range(0.9f, 1.1f);

			case "First Zombella Theme 1":
				return 1f;
			case "First Zombella Theme 2":
				return 1f;
			case "Income 1":
				return 1f;
			case "Income 2":
				return 1f;

			default:
				return A[name].pitch;
		}
	}

	/// <summary>
	/// Audio
	/// </summary>
	public Dictionary<string, AudioSource> A
	{
		get
		{
			return _audioSources;
		}
	}
}