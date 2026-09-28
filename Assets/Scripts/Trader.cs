// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Trader : MonoBehaviour
{
	public List<Trade> _trades;

	[Header("Trade panel materials (back -> front order)")]
	public Material _tradeBackMaterial;
	public Material _tradeFrontMaterial;
	public Material _tradeOverlayMaterial;

	[Tooltip("Visual scale of the panel quads (hitbox is unaffected)")]
	public float _panelScale = 1.18f;

	private List<GameObject> _panels;

	[Header("World-space trade UI")]
	[Tooltip("Distance from S.Intercam along forward")]
	public float _distance = 10f;

	[Tooltip("How much closer to the camera icons/text are compared to the panel")]
	public float _frontOffset = 0.02f;

	private TextMeshPro _numberLabelExample;
	private GameObject _root;

	// For each trade: hit rectangle in camera-local XY at Z = _distance.
	// Rect.x = left, Rect.y = bottom, Rect.width = panel width, Rect.height = panel height.
	private List<Rect> _panelBounds;

	private int _openedFrame = -1;

	void Start()
	{
		_panels = new List<GameObject>();
		_panelBounds = new List<Rect>();
		S.Inventory._trader = this;

		_numberLabelExample = S.NumberLabelExample.GetComponent<TextMeshPro>();
	}

	static Shader TradeShader => Shader.Find("Custom/AlphaUnlitSingleSideWithAlphaMultiplier");

	GameObject CreateQuad(string name, Material mat, Transform parent,
						  Vector3 localPos, float width, float height)
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
		go.name = name;
		go.layer = 13;

		go.transform.SetParent(parent, false);
		go.transform.localPosition = localPos;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = new Vector3(width, height, 1f);

		var mr = go.GetComponent<MeshRenderer>();
		mr.sharedMaterial = mat;
		mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		mr.receiveShadows = false;

		return go;
	}

	static Material CreateItemMaterial(Texture2D tex)
	{
		var mat = new Material(TradeShader);
		mat.mainTexture = tex;
		return mat;
	}

	TextMeshPro CreateNumberLabel(string text, Transform parent, Vector3 localPos)
	{
		TextMeshPro tmp = Instantiate(_numberLabelExample, parent);
		tmp.name = "NumberLabel";
		tmp.text = text;

		tmp.transform.localPosition = localPos;
		tmp.transform.localRotation = Quaternion.identity;
		tmp.transform.localScale = _numberLabelExample.transform.localScale;

		tmp.gameObject.layer = 13;
		return tmp;
	}

	static void SetLayerRecursive(GameObject go, int layer)
	{
		go.layer = layer;
		foreach (Transform child in go.transform)
			SetLayerRecursive(child.gameObject, layer);
	}

	public void OpenMarket()
	{
		for (int i = _trades.Count - 1; i >= 0; i--)
			if (_trades[i]._tradeCount <= 0)
				_trades.RemoveAt(i);

		if (S.Inventory._marketOpened == false && _trades.Count > 0)
		{
			S.Inventory._marketOpened = true;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;

			S.AM.Play("Inventory", 1);

			Camera cam = S.Intercam;
			Transform camT = cam.transform;

			float fov = cam.fieldOfView;

			const float aspect = 16f / 9f;
			float vFovRad = fov * Mathf.Deg2Rad;
			float visibleHeight = 2f * _distance * Mathf.Tan(vFovRad * 0.5f);
			float visibleWidth = visibleHeight * aspect;

			float spacing = 0.24f;
			float halfheight = (_trades.Count - 1) / 2f;

			float panelW = visibleWidth * 0.32f;
			float panelH = visibleHeight * 0.24f;

			// Visual size of the panel quads (glow may overlap between rows).
			float panelVisW = panelW * _panelScale;
			float panelVisH = panelH * _panelScale;

			float itemW = visibleWidth * 0.10f * 0.8f;
			float itemH = visibleHeight * 0.18f * 0.8f;
			float itemXOff = visibleWidth * 0.12f * 0.8f;

			const float rootY = 1f;

			// Precompute per-call invariants.
			float step = visibleHeight * spacing;
			float halfPanelW = panelW * 0.5f;
			float halfPanelH = panelH * 0.5f;
			float panelLeft = -halfPanelW;

			// Depth layers (camera-local Z; +Z is away from the camera).
			float zStep = _frontOffset;
			float zBack = 0f;
			float zFront = -zStep;
			float zItems = -2f * zStep;
			float zOverlay = -3f * zStep;
			float zText = -4f * zStep;

			Vector3 sellOffset = new Vector3(-itemXOff, 0f, zItems);
			Vector3 buyOffset = new Vector3(itemXOff, 0f, zItems);
			Vector3 sellTextOffset = new Vector3(0.4f, -0.8f, zText - zItems);
			Vector3 buyTextOffset = new Vector3(0.6f, -0.8f, zText - zItems);

			_root = new GameObject("TradeRoot");
			_root.layer = 13;
			_root.transform.SetParent(camT, false);
			_root.transform.localPosition = new Vector3(0f, rootY, _distance);
			_root.transform.localRotation = Quaternion.identity;
			_root.transform.localScale = Vector3.one;

			_panelBounds.Clear();

			for (int i = 0; i < _trades.Count; i++)
			{
				float y = step * (i - halfheight);
				Vector3 panelPos = new Vector3(0f, y, 0f);

				float localCenterY = rootY + y;

				// Hitbox uses the un-scaled panel size, so clicks stay inside
				// the visible core of the panel and don't reach into the glow.
				_panelBounds.Add(new Rect(
					panelLeft,
					localCenterY - halfPanelH,
					panelW,
					panelH));

				GameObject panelBack = CreateQuad("PanelBack", _tradeBackMaterial, _root.transform,
												  new Vector3(0f, y, zBack), panelVisW, panelVisH);
				_panels.Add(panelBack);

				GameObject panelFront = CreateQuad("PanelFront", _tradeFrontMaterial, _root.transform,
												   new Vector3(0f, y, zFront), panelVisW, panelVisH);
				_panels.Add(panelFront);

				string sellSpriteName = S.II.Get(_trades[i]._selledItemName)._spriteName;
				Texture2D sellTex = Resources.Load<Texture2D>($"Textures/Items/{sellSpriteName}");

				Vector3 sellPos = panelPos + sellOffset;
				GameObject sellObject = CreateQuad("Sell", CreateItemMaterial(sellTex), _root.transform,
												   sellPos, itemW, itemH);
				_panels.Add(sellObject);

				string buySpriteName = S.II.Get(_trades[i]._buyedItemName)._spriteName;
				Texture2D buyTex = Resources.Load<Texture2D>($"Textures/Items/{buySpriteName}");

				Vector3 buyPos = panelPos + buyOffset;
				GameObject buyObject = CreateQuad("Buy", CreateItemMaterial(buyTex), _root.transform,
												  buyPos, itemW, itemH);
				_panels.Add(buyObject);

				GameObject panelOverlay = CreateQuad("PanelOverlay", _tradeOverlayMaterial, _root.transform,
													 new Vector3(0f, y, zOverlay), panelVisW, panelVisH);
				_panels.Add(panelOverlay);

				string sellText = GetCountText(_trades[i]._selledCount);
				if (sellText.Length > 0)
				{
					Vector3 tPos = sellPos + sellTextOffset;
					CreateNumberLabel(sellText, _root.transform, tPos);
				}

				string buyText = GetCountText(_trades[i]._buyedCount);
				if (buyText.Length > 0)
				{
					Vector3 tPos = buyPos + buyTextOffset;
					CreateNumberLabel(buyText, _root.transform, tPos);
				}
			}

			SetLayerRecursive(_root, 13);

			_openedFrame = Time.frameCount;
		}
	}

	static string GetCountText(int count)
	{
		return count > 1 ? count.ToString() : "";
	}

	void HandleTradeClick(Trade trade)
	{
		if (S.Inventory.CountOfItem(trade._selledItemName) >= trade._selledCount)
		{
			S.Inventory.Remove(trade._selledItemName, trade._selledCount);
			S.Inventory.Take(trade._buyedItemName, trade._buyedCount);
			RemoveTrade(trade);

			S.AM.Play("Money", 1);

			if (trade._buyedItemName == "Gun")
			{
				S.SM.Save("gunWasBought", true);
				if (S.SM.LoadBool("ammoWasBought") ?? false)
					S.FirstZombella2.FirstZombieEntersHall();
			}
			if (trade._buyedItemName == "Ammo")
			{
				S.SM.Save("ammoWasBought", true);
				if (S.SM.LoadBool("gunWasBought") ?? false)
					S.FirstZombella2.FirstZombieEntersHall();
			}
		}
		else
			S.AM.Play("Not Enough Cash", 1);
	}

	public void RemoveTrade(Trade trade)
	{
		trade._tradeCount--;
		trade.Save();

		if (trade._tradeCount <= 0)
		{
			_trades.Remove(trade);

			if (S.Inventory._marketOpened)
			{
				CloseMarket();
				OpenMarket();
			}
		}
	}

	bool TryGetClickLocalPoint(out Vector2 localXY)
	{
		localXY = Vector2.zero;

		Camera cam = S.Intercam;
		if (cam == null) return false;
		Transform camT = cam.transform;

		Ray ray = cam.ScreenPointToRay(Input.mousePosition);

		Vector3 planeNormal = camT.forward;
		Vector3 planePoint = camT.position + planeNormal * _distance;

		float denom = Vector3.Dot(planeNormal, ray.direction);
		if (Mathf.Abs(denom) < 1e-6f) return false;

		float t = Vector3.Dot(planePoint - ray.origin, planeNormal) / denom;
		if (t < 0f) return false;

		Vector3 worldPoint = ray.origin + ray.direction * t;
		Vector3 localPoint = camT.InverseTransformPoint(worldPoint);

		localXY = new Vector2(localPoint.x, localPoint.y);
		return true;
	}

	void Update()
	{
		if (!S.Inventory._marketOpened) return;

		if (Input.GetKeyDown(KeyCode.Escape)) CloseMarket();
		if (Input.GetKeyDown(KeyCode.E)) CloseMarket();
		if (Input.GetKeyDown(KeyCode.I)) CloseMarket();

		// Skip clicks on the very frame the market opened, so the same input
		// that opened the market cannot immediately hit a panel.
		if (Time.frameCount == _openedFrame) return;

		if (Input.GetMouseButtonDown(0))
		{
			if (TryGetClickLocalPoint(out Vector2 localXY))
			{
				for (int i = 0; i < _panelBounds.Count && i < _trades.Count; i++)
				{
					if (_panelBounds[i].Contains(localXY))
					{
						HandleTradeClick(_trades[i]);
						break;
					}
				}
			}
		}
	}

	public void CloseMarket()
	{
		if (S.Inventory._marketOpened)
		{
			S.Inventory._marketOpened = false;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

			if (_root != null)
			{
				Destroy(_root);
				_root = null;
			}
			_panels.Clear();
			_panelBounds.Clear();

			S.AM.Play("Inventory", 1);

			if (!(S.SM.LoadBool("SaidGunAmmoMessage") ?? false))
			{
				S.SM.Save("SaidGunAmmoMessage", true);

				bool gunBought = S.SM.LoadBool("gunWasBought") ?? false;
				bool ammoBought = S.SM.LoadBool("ammoWasBought") ?? false;

				string message = null;
				if (!gunBought && !ammoBought)
					message = "Rika: I need to buy a gun and ammo!!!!!!";
				else if (gunBought && !ammoBought)
					message = "Rika: I also need to buy ammo!!!!!!";
				else if (!gunBought && ammoBought)
					message = "Rika: I also need to buy a gun!!!!!!";

				if (message != null)
					StartCoroutine(SayNeedGunAmmo(message));
			}
		}
	}

	IEnumerator SayNeedGunAmmo(string message)
	{
		yield return new WaitForSeconds(3);
		S.Console.AddMessage(message, Color.magenta);
	}
}