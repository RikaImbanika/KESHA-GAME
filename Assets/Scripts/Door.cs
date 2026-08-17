// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using System;
using Unity.VisualScripting;

public class Door : MonoBehaviour
{
	public int _number;
	public Collider col;
	public string _audioName;
	public bool _locked;
	public Stamp _stamp;
	public int _sparklesCount;
	public DoorModel _doorModel;
	private string _sceneName;
	private bool _needArrow;
	private bool _arrowPlaced;
	private Transform _root;
	private int _layerMask;

	public void Start()
	{
		_sparklesCount = 35; //They are strange.

		_layerMask = 1 << LayerMask.NameToLayer("Static") |
			 1 << LayerMask.NameToLayer("Default");

		// if (string.IsNullOrEmpty(_audioName))
		// 	_audioName = "Door";
		_audioName = "Door"; //TO DO: More sounds.

		StartCoroutine(Wait());

		IEnumerator Wait()
		{
			_sceneName = gameObject.scene.name;

			float delay = UnityEngine.Random.Range(0, 0.5f);

			while (S.Loader._rooms == null)
				yield return new WaitForSeconds(delay);

			while (!S.Loader._rooms.ContainsKey(_sceneName))
				yield return new WaitForSeconds(delay);

			RoomModel roomModel = S.Loader._rooms[_sceneName];

			_doorModel = roomModel._doors[_number];
			_doorModel._door = this;
			_doorModel._coordinates = transform.position;
			_doorModel._right = transform.right;
			_locked = _doorModel._locked;
			_needArrow = _doorModel._needArrow;

			while (!S.Loader.Roots.ContainsKey(_sceneName))
				yield return new WaitForSeconds(delay);

			_root = S.Loader.Roots[_sceneName];

			if (_needArrow)
			{
				PlaceArrowAndExitSignAsync();

				while (!_arrowPlaced)
					yield return new WaitForSeconds(delay);
			}

			if (_locked)
				PlaceStampAsync();
		}
	}

	void PlaceStampAsync()
	{
		Vector3 point0 = transform.position + transform.right * 3f; ///

		Quaternion rot = Quaternion.LookRotation(transform.right);

		StartCoroutine(SetParent());

		IEnumerator SetParent()
		{
			while (true)
			{
				while (S.Loader.Roots == null ||
					!S.Loader.Roots.ContainsKey(_sceneName) ||
					S.Loader.Roots[_sceneName] == null)
					yield return new WaitForSeconds(0.25f);

				GameObject stampObj = Instantiate(S.Stamp, point0, rot, _root);

				try
				{
					RaycastHit hit;
					if (Physics.Raycast(point0, Vector3.down, out hit, 20f))
					{
						Vector3 point1 = hit.point;
						point1 += Vector3.up * 3.75f;
						stampObj.transform.position = point1;

						RaycastHit hit2;
						Vector3 point2 = point0 + Vector3.up * 10f;
						if (Physics.Raycast(point2, -transform.right, out hit2, 5f))
						{
							Vector3 point3 = hit2.point;
							point3.y = point1.y;
							point3 += transform.right * 0.55f;
							stampObj.transform.position = point3;
						}
					}
					else
					{
						throw new Exception("Can't raycast");
					}
				}
				catch (Exception ex)
				{
					S.Console.AddMessage($"I can't place stamp (#1)! Scene: {_sceneName}, door number: {_number}", Color.red);
					continue;
				}

				try
				{
					Vector3 originalScale = S.Stamp.transform.localScale;
					Vector3 parentScale = S.Loader.Roots[_sceneName].lossyScale;

					stampObj.transform.localScale = new Vector3(
						originalScale.x / parentScale.x,
						originalScale.y / parentScale.y,
						originalScale.z / parentScale.z
					);

					_stamp = stampObj.GetComponent<Stamp>();
					_stamp._door = this;
				}
				catch (Exception ex)
				{
					S.Console.AddMessage($"I can't place stamp (#2)! Scene: {_sceneName}, door number: {_number}", Color.red);
					continue;
				}

				break;
			}
		}
	}

	void PlaceArrowAndExitSignAsync()
	{
		Vector3 point0 = transform.position + transform.right * 3f;

		Quaternion rot = Quaternion.LookRotation(transform.right);

		StartCoroutine(SetParent0());

		IEnumerator SetParent0()
		{
			while (true)
			{
				while (S.Loader.Roots == null ||
					!S.Loader.Roots.ContainsKey(_sceneName) ||
					S.Loader.Roots[_sceneName] == null)
					yield return new WaitForSeconds(0.25f);

				float hueShiftSpeed = 0f;
				if (_sceneName == "BR 7" || _sceneName == "BR 7R")
					hueShiftSpeed = 0.1667f;
				else if (_sceneName == "BR 6" || _sceneName == "BR 6R")
					hueShiftSpeed = 0.08f;

				GameObject arrowObj = GameObject.Instantiate(S.Arrow, point0, rot, S.Loader.Roots[_sceneName]);

				Vector3 point1 = point0;

				RaycastHit hit;
				if (Physics.Raycast(point0, Vector3.down, out hit, 20f))
				{
					point1 = hit.point;
					arrowObj.transform.position = point1;
				}
				else
				{
					S.Console.AddMessage("Arrow not placed one time (#1)", Color.red);
					yield return new WaitForSeconds(1f);
					continue;
				}

				Vector3 originalScale = S.Arrow.transform.localScale;
				Vector3 parentScale = S.Loader.Roots[_sceneName].lossyScale;

				arrowObj.transform.localScale = new Vector3(
					originalScale.x / parentScale.x,
					originalScale.y / parentScale.y,
					originalScale.z / parentScale.z
				);

				Color arrowTint = GetSurfaceColor(
					arrowObj.transform,
					Vector3.down,
					arrowObj.transform.up,
					arrowObj.transform.right,
					hueShiftSpeed
				);
				ApplyTintToObject(arrowObj, arrowTint, hueShiftSpeed > 0f, hueShiftSpeed);
		
				Vector3 point2 = point1 + new Vector3(0, 14, 0);

				GameObject exitObj = GameObject.Instantiate(S.Exit, point0, rot, S.Loader.Roots[_sceneName]);

				if (Physics.Raycast(point2, -transform.right, out hit, 20f))
				{
					Vector3 point3 = hit.point + new Vector3(0, -5, 0);
					exitObj.transform.position = point3;
				}
				else
				{
					S.Console.AddMessage("Arrow not placed one time (#2)", Color.red);
					yield return new WaitForSeconds(1f);
					continue;
				}

				Vector3 originalScale2 = S.Exit.transform.localScale;

				exitObj.transform.localScale = new Vector3(
					originalScale2.x / parentScale.x,
					originalScale2.y / parentScale.y,
					originalScale2.z / parentScale.z
				);

				Color exitTint = GetSurfaceColor(
					exitObj.transform,
					-transform.right,
					exitObj.transform.up,
					exitObj.transform.forward,
					hueShiftSpeed
				);
				ApplyTintToObject(exitObj, exitTint, hueShiftSpeed > 0f, hueShiftSpeed);
		
				_arrowPlaced = true;
				break;
			}
		}
	}

	Color GetSurfaceColor(Transform objTransform, Vector3 rayDirection, Vector3 basis1, Vector3 basis2, float hueShiftSpeed)
	{
		Vector3[] points = new Vector3[]
		{
		objTransform.position,
		objTransform.position + basis1 + basis2,
		objTransform.position + basis1 - basis2,
		objTransform.position - basis1 + basis2,
		objTransform.position - basis1 - basis2,
		objTransform.position + basis1,
		objTransform.position - basis1,
		objTransform.position + basis2,
		objTransform.position - basis2
		};

		Color[] colors = S.WallColorCapturer.CaptureAtPoints(points, rayDirection, _layerMask);

		Color brightest = colors[0];
		for (int i = 1; i < colors.Length; i++)
			if (colors[i].grayscale > brightest.grayscale)
				brightest = colors[i];

		if (hueShiftSpeed > 0f)
		{
			float timeShift = Mathf.Repeat(Time.timeSinceLevelLoad * hueShiftSpeed, 1f);
			Color.RGBToHSV(brightest, out float h, out float s, out float v);
			h = (h - timeShift + 1f) % 1f;
			brightest = Color.HSVToRGB(h, s, v);
		}

		return BrightenAndDesaturateColor(brightest, 0.3f, 0.2f);
	}

	public Color BrightenAndDesaturateColor(Color baseColor, float b = 0.2f, float st = 0.2f)
	{
		//Here are no any problems

		Color.RGBToHSV(baseColor, out float h, out float s, out float v);

		v = Mathf.Pow(v, 1 - b);

		s = Mathf.Pow(s, 1 + st);

		return Color.HSVToRGB(h, s, v);
	}

	void ApplyTintToObject(GameObject obj, Color tint, bool useHueShiftShader, float hueShiftSpeed)
	{
		Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
		foreach (Renderer rend in renderers)
		{
			Material originalMat = rend.sharedMaterial;
			if (originalMat == null) continue;

			string originalShaderName = originalMat.shader.name;
			bool isTransparent = originalShaderName.Contains("Transparent") || originalShaderName.Contains("Alpha");

			string shaderName;
			if (isTransparent)
			{
				shaderName = useHueShiftShader ? "HueShiftUnlitTransparentF" : "UnlitTransparentF";
			}
			else
			{
				shaderName = useHueShiftShader ? "HueShiftUnlitF" : "UnlitF";
			}

			Shader shader = Shader.Find(shaderName);
			if (shader == null)
			{
				shader = Shader.Find("Custom/" + shaderName);
				if (shader == null) continue;
			}

			Material newMat = new Material(shader);
			newMat.mainTexture = originalMat.mainTexture;
			newMat.color = tint;
			if (useHueShiftShader)
				newMat.SetFloat("_Speed", hueShiftSpeed);

			rend.material = newMat;
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("PlayerHub"))
		{
			Go();
		}
	}

	public void Go()
	{
		if (!_locked)
		{
			S.AudioManager.Play(_audioName); //Was set before

			S.Loader.GoTo(_doorModel._nextSceneName, _doorModel._nextDoorId, transform.right);
		}
		else
		{
			PlayerMovement pm = S.Ph.GetComponent<PlayerMovement>();
			Vector3 direction = S.Camera.transform.position - transform.position;
			direction = new Vector3(direction.x, 0, direction.z).normalized;
			direction *= 1400f;
			direction += new Vector3(0, 25, 0);
			pm.Push(direction);
			S.Ps.Damage(10);
			S.Inventory._negated = 0.3f;

			for (int i = 0; i < _sparklesCount; i++)
			{
				GameObject sparkle = Instantiate(S.BlueSparkle, _root);
				sparkle.transform.position = transform.position;
				sparkle.transform.rotation = Quaternion.LookRotation(direction);
				sparkle.transform.localScale *= 1.1f;
			}
		}
	}

	public void Unlock()
	{
		if (_locked)
		{
			_locked = false;
			_doorModel._locked = false;
			S.Loader._rooms[_doorModel._nextSceneName]._doors[_doorModel._nextDoorId]._door.Unlock();
			_stamp.Unlock();
		}
	}
}