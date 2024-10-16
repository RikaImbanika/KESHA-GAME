using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IsTrader : MonoBehaviour
{
	public List<Trade> _trades;
	public Texture2D _tradeTexture;
	private Canvas _canvas;
	private Inventory _inventory;
	private List<GameObject> _panels;
	private AudioManager _audioManager;

	void Start()
	{
		_canvas = GameObject.FindObjectOfType<Canvas>();
		_inventory = _canvas.GetComponent<Inventory>();
		_panels = new List<GameObject>();
		_inventory._trader = this;
	}

	public void OpenMarket()
	{
		if (_inventory._marketOpened == false && _trades.Count > 0)
		{
			for (int i = _trades.Count - 1; i >= 0; i--)
				if (_trades[i]._tradeCount <= 0)
					_trades.RemoveAt(i);					

			_inventory._marketOpened = true;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;

			if (_audioManager == null)
			{
				GameObject go = GameObject.FindGameObjectWithTag("AudioManager");
				_audioManager = go.GetComponent<AudioManager>();
			}
			_audioManager.Play("inventory", 1);

			float spacing = 0.09f;
			int screenWidth = Screen.currentResolution.width;
			int screenHeight = Screen.currentResolution.height;

			float halfheight = (_trades.Count - 1) / 2f;

			for (int i = 0; i < _trades.Count; i++)
			{
				GameObject panelObject = new GameObject("Panel");
				_panels.Add(panelObject);
				Image panelImage = panelObject.AddComponent<Image>();
				panelImage.sprite = Sprite.Create(_tradeTexture, new Rect(0, 0, _tradeTexture.width, _tradeTexture.height), new Vector2(0.5f, 0.5f));
				panelObject.transform.SetParent(_canvas.transform);
				TradeClick tradeClick = panelObject.AddComponent<TradeClick>();
				tradeClick._trade = _trades[i];
				tradeClick._inventory = _inventory;
				tradeClick._tradeNumber = i;
				tradeClick._trader = this;

				RectTransform rectTransform = panelObject.GetComponent<RectTransform>();
				rectTransform.sizeDelta = new Vector2(screenWidth * 0.2f, screenHeight * 0.15f);
				rectTransform.anchoredPosition = new Vector2(0, screenHeight * (i - halfheight) * spacing);

				GameObject sellObject = new GameObject("Sell");
				Image sellImage = sellObject.AddComponent<Image>();
				sellImage.sprite = Sprite.Create(_trades[i]._selledTexture, new Rect(0, 0, _trades[i]._selledTexture.width, _trades[i]._selledTexture.height), new Vector2(0.5f, 0.5f));
				sellObject.transform.SetParent(panelObject.transform);

				RectTransform rectTransformSell = sellObject.GetComponent<RectTransform>();
				rectTransformSell.sizeDelta = new Vector2(screenWidth * 0.05f, screenHeight * 0.09f);
				rectTransformSell.anchoredPosition = new Vector2(-screenWidth * 0.06f, 0);


				GameObject buyObject = new GameObject("Buy");
				Image buyImage = buyObject.AddComponent<Image>();
				buyImage.sprite = Sprite.Create(_trades[i]._buyedTexture, new Rect(0, 0, _trades[i]._buyedTexture.width, _trades[i]._buyedTexture.height), new Vector2(0.5f, 0.5f));
				buyObject.transform.SetParent(panelObject.transform);

				RectTransform rectTransformBuy = buyObject.GetComponent<RectTransform>();
				rectTransformBuy.sizeDelta = new Vector2(screenWidth * 0.05f, screenHeight * 0.09f);
				rectTransformBuy.anchoredPosition = new Vector2(screenWidth * 0.06f, 0);





				GameObject TOBuy = new GameObject("TextMeshProObject");
				TextMeshProUGUI TMPBuy = TOBuy.AddComponent<TextMeshProUGUI>();
				TMPBuy.text = $"{GetText(_trades[i]._buyedCount)}";
				TOBuy.transform.SetParent(buyObject.transform);
				RectTransform RTBuy = TMPBuy.GetComponent<RectTransform>();
				RTBuy.anchorMin = Vector2.zero;
				RTBuy.anchorMax = Vector2.one;
				RTBuy.sizeDelta = Vector2.zero;
				float c = 1f / 1377f * screenWidth;
				RTBuy.anchoredPosition = new Vector2(20 * c, -20 * c);
				TMPBuy.fontSize = 24f * c;
				TMPBuy.alignment = TextAlignmentOptions.Center;
				TMPBuy.enableWordWrapping = false;
				TMPBuy.fontStyle = FontStyles.Bold;
				TMPBuy.outlineWidth = 0.5f;
				Material matBuy = TMPBuy.fontSharedMaterial;
				matBuy.shaderKeywords = new string[] { "OUTLINE_ON" };


				GameObject TOSell = new GameObject("TextMeshProObject");
				TextMeshProUGUI TMPSell = TOSell.AddComponent<TextMeshProUGUI>();
				TMPSell.text = $"{GetText(_trades[i]._selledCount)}";
				TOSell.transform.SetParent(sellObject.transform);
				RectTransform RTSell = TMPSell.GetComponent<RectTransform>();
				RTSell.anchorMin = Vector2.zero;
				RTSell.anchorMax = Vector2.one;
				RTSell.sizeDelta = Vector2.zero;
				//float c = 1f / 1377f * screenWidth;
				RTSell.anchoredPosition = new Vector2(20 * c, -20 * c);
				TMPSell.fontSize = 24f * c;
				TMPSell.alignment = TextAlignmentOptions.Center;
				TMPSell.enableWordWrapping = false;
				TMPSell.fontStyle = FontStyles.Bold;
				TMPSell.outlineWidth = 0.5f;
				Material matSell = TMPSell.fontSharedMaterial;
				matSell.shaderKeywords = new string[] { "OUTLINE_ON" };



				string GetText(int count)
				{
					if (count > 1)
						return $"{count}";
					else
						return "";
				}
			}
		}
	}

	public void RemoveTrade(int i)
	{
		_trades[i]._tradeCount--;
		_trades[i].Save();

		if (_trades[i]._tradeCount <= 0)
		{
			_trades.RemoveAt(i);

			if (_inventory._marketOpened)
			{
				CloseMarket();
				OpenMarket();
			}
		}
	}

	void Update()
	{
		if (_inventory._marketOpened)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
				CloseMarket();
			if (Input.GetKeyDown(KeyCode.E))
				CloseMarket();
			if (Input.GetKeyDown(KeyCode.I))
				CloseMarket();
		}
	}

	public void CloseMarket()
	{
		_inventory._marketOpened = false;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		while (_panels.Count > 0)
		{
			Destroy(_panels[0]);
			_panels.RemoveAt(0);
		}
	}
}
