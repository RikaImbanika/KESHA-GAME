using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Inventory : MonoBehaviour
{
	[Header("Keybinds")]

	public GameObject inventoryPanel;
	public GameObject smallInventoryPanel;
	public bool opened;

	public int selectedId;
	public int selId;
	public int ultraSelectedId;

	public Item[] items;

	public GameObject[] panels;
	public GameObject[] smallPanels;

	public TextMeshProUGUI[] numberLabels;
	public TextMeshProUGUI[] smallNumberLabels;

	public GameObject selectorPanel;
	public GameObject selectorPanelPlus; //œŒÕﬂÀ¿
	public GameObject cursorPanel;
	public GameObject inventoryPanelParentBig;
	public GameObject inventoryPanelParentSmall; //Õ≈ œŒÕﬂÀ¿, ›“Œ ◊“Œ??
	//¿¿¿ ›“Œ —¿Ã¿  ¿–“»Õ ¿
	//◊Œ?

	public Camera _camera;
	public Camera showingCamera;
	public Rigidbody prb;

	public LayerMask notTransperent;

	public GameObject numberLabelExample;

	public float throwTime;
	public float throwSize;
	public bool throwing;
	public int throwCombo;

	public Canvas canvas;

	public long lastKeyTime;
	public string state;

	public float fps;

	public GameObject allFather;

	public GameObject throwPanel;
	public GameObject throwPanelBlack;
	public GameObject throwPanelRed;

	public GameObject _showingItem;
	public Vector3 _showingRotation;
	public RawImage showingPanel;
	public GameObject showingOverlay;
	public TextMeshProUGUI showingText;
	public float showingStartTime;
	public float _negated;

	public GameObject _playerObject;

	Sprite _empty;

	public bool _marketOpened;

	public bool _somethingClicked;

	public IsTrader _trader;

	public PlayerStorage _playerStorage;

	public GameObject _circleCursor;

	public GameObject _negative;

	public int CountOfItem(string name)
	{
		int count = 0;

		for (int i = 0; i < 36; i++)
			if (items[i] != null)
				if (items[i]._name == name)
					count += items[i]._count;

		return count;
	}

	public void Remove(string name, int count)
	{
		for (int i = 0; i < 36; i++)
			if (items[i] != null)
				if (items[i]._name == name)
				{
					if (items[i]._count > count)
					{
						items[i]._count -= count;
						Visualise(i);
						SaveOneItem(i);
						return;
					}
					else if (items[i]._count == count)
					{
						items[i]._name = "";
						ultraSelectedId = -1;
						selectorPanelPlus.SetActive(false);
						Visualise(i);
                        SaveOneItem(i);
                        return;
					}
					else if (items[i]._count < count)
					{
						count -= items[i]._count; //CORRECT
						items[i]._name = "";
						ultraSelectedId = -1;
						selectorPanelPlus.SetActive(false);
						Visualise(i);
                        SaveOneItem(i);
                        continue;
					}
				}
	}

	public void Start()
	{
		S.Inventory = this;
		S.Negative = _negative;

		_empty = Resources.Load<Sprite>("Sprites/Items/Empty");

		StartCoroutine(LateStart(0.3f));

		IEnumerator LateStart(float waitTime)
		{
            while (S.AudioManager == null)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log("Inventory waiting for S.AudioManager");
            }

            S.AudioManager.muted = true;

            items = new Item[36]; //Kinda hardcode
            numberLabels = new TextMeshProUGUI[36];
            smallNumberLabels = new TextMeshProUGUI[9];
            opened = true;
            state = "not started";

            _showingItem = null;
            showingCamera.enabled = false;
            showingCamera.gameObject.SetActive(false);
            showingPanel.gameObject.SetActive(false);
            showingOverlay.SetActive(false);

            yield return new WaitForSeconds(waitTime);

			while (S.AllFather == null)
			{
				yield return new WaitForSeconds(0.1f);
                Debug.Log("Inventory waiting for S.AllFather");
            }

			Vector2 canvasScale = new Vector2(canvas.transform.lossyScale.x, canvas.transform.lossyScale.y);

			for (int i = 0; i < numberLabels.Length; i++)
			{
				Transform t = panels[i].transform;
				var obj = Instantiate(numberLabelExample);
				obj.transform.parent = t;
				obj.transform.position = t.position + new Vector3(8 * canvasScale.x, 140 * canvasScale.y, 0);
				obj.transform.localScale = new Vector3(0.22f, 0.65f);

				numberLabels[i] = obj.GetComponent<TextMeshProUGUI>();
				items[i] = S.AllFather.gameObject.AddComponent<Item>();
				items[i]._name = "";
				items[i]._count = 0;
				//Debug.Log($"i {i} item {items[i]}");
			}

			for (int i = 0; i < smallNumberLabels.Length; i++)
			{
				Transform t = smallPanels[i].transform;
				var obj = Instantiate(numberLabelExample);
				obj.transform.parent = t;
				obj.transform.position = t.position + new Vector3(7 * canvasScale.x, 103 * canvasScale.y, 0);
				obj.transform.localScale = new Vector3(0.22f, 0.65f);

				smallNumberLabels[i] = obj.GetComponent<TextMeshProUGUI>();
			}

			ultraSelectedId = -1;

			SwitchInventory();
			SwitchInventory();
			SwitchInventory();

			selectorPanelPlus.SetActive(false);

			numberLabelExample.SetActive(false);

			Visualize();

			S.AudioManager.muted = false;
		}
	}

	public void Update()
	{
		if (S.Negative == null)
			return;

		if (_negated > 0)
		{
			S.Negative.SetActive(System.MathF.Sin(_negated * 220) > -0.2f);
			_negated -= Time.deltaTime;
		}
		else
			S.Negative.SetActive(false);

		MyInput();

		if (S.FpsTMP == null)
		{
			Debug.Log("Inventory wainitg for S.FpsTMP");
			return;
		}

		fps = MathF.Round(fps * 0.5f + 0.5f / Time.deltaTime);
		S.FpsTMP.text = fps.ToString();

		if (_showingItem != null)
			_showingItem.transform.Rotate(_showingRotation.x, _showingRotation.y, _showingRotation.z, Space.World);

		if (!_marketOpened && !opened)
		{
			bool clickable = false;

			Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 7f, notTransperent))
			{
				Locker locker = hit.collider.gameObject.GetComponent<Locker>();
				if (locker != null)
				{
					clickable = true;
					goto render;
				}

				FrerardHolder fh = hit.collider.gameObject.GetComponent<FrerardHolder>();
				if (fh != null)
				{
					clickable = true;
					goto render;
				}

				IsTrader trader = hit.collider.gameObject.GetComponent<IsTrader>();
				if (trader != null)
				{
					clickable = true;
					goto render;
				}

				Door door = hit.collider.gameObject.GetComponent<Door>();
				if (door != null)
				{
					clickable = true;
					goto render;
				}

				ItemP itemP = hit.collider.gameObject.GetComponent<ItemP>();
				if (itemP != null)
				{
					clickable = true;
					goto render;
				}

				Button1 button1 = hit.collider.gameObject.GetComponent<Button1>();
				if (button1 != null)
				{
					clickable = true;
					goto render;
				}

				SaverTypewriter saver = hit.collider.gameObject.GetComponent<SaverTypewriter>();
				if (saver != null)
				{
					clickable = true;
					goto render;
				}

				IsDoor isDoor = hit.collider.gameObject.GetComponent<IsDoor>();
				if (isDoor != null)
				{
					clickable = true;
					goto render;
				}

				IsShuffle isShuffle = hit.collider.gameObject.GetComponent<IsShuffle>();
				if (isShuffle != null)
				{
					clickable = true;
					goto render;
				}
			}

			render:

			if (clickable)
			{
				cursorPanel.SetActive(false);
				_circleCursor.SetActive(true);
			}
			else
			{
				cursorPanel.SetActive(true);
				_circleCursor.SetActive(false);
			}
		}
	}

	public void Visualize()
	{
		for (int i = 0; i < panels.Length; i++)
			Visualise(i);
	}

	public void Visualise(int id)
	{
		if (!IsEmpty(items[id]))
		{
			Debug.Log($"Setting sprite on {id}");

			string spriteName = S.II.Get(items[id]._name)._spriteName;
			Sprite sprite = Resources.Load<Sprite>($"Sprites/Items/{spriteName}");
			panels[id].GetComponent<Image>().sprite = sprite;

			Debug.Log($"Sprite name on {id} is {spriteName}");

			if (items[id]._count > 1)
				numberLabels[id].text = Align(items[id]._count.ToString());
			else
				numberLabels[id].text = "";

			if (id < 9)
			{
				smallPanels[id].GetComponent<Image>().sprite = sprite;

				if (items[id]._count > 1)
					smallNumberLabels[id].text = Align(items[id]._count.ToString());
				else
					smallNumberLabels[id].text = "";
			}

			Debug.Log($"Set sprite on {id}");
		}
		else
		{
			Debug.Log($"Clearing sprite on {id}");

			panels[id].GetComponent<Image>().sprite = _empty;
			numberLabels[id].text = "";
			if (id < 9)
			{
				smallPanels[id].GetComponent<Image>().sprite = _empty;
				smallNumberLabels[id].text = "";
			}

			Debug.Log($"Cleared sprite on {id}");
		}

		string Align(string s)
		{
			if (s.Length >= 3)
				return s;
			else if (s.Length == 2)
				return ' ' + s;
			else
				return "  " + s;
		}
	}

	private void MyInput()
	{
		if (_showingItem != null)
		{
			if (showingStartTime < Time.time - 1)
				if (Input.anyKey)
					HideShowingItem();
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
				SwitchInventory();
			if (Input.GetKeyDown(KeyCode.Escape) && opened)
				SwitchInventory();

			if (opened)
			{
				if (Input.GetAxis("Mouse ScrollWheel") < 0f)
					SelectItem(selectedId + 1, false);
				else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
					SelectItem(selectedId - 1, false);

				if (Input.GetKeyDown(KeyCode.Return))
				{
					SelectItem(selectedId, true);
					state = "not started";
				}
				else
				{
					long time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
					long deltaTime = time - lastKeyTime;

					bool atLeastOne = false;

					if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
						Method(1, KeyCode.D);
					if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
						Method(-1, KeyCode.A);
					if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
						Method(-9, KeyCode.W);
					if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
						Method(9, KeyCode.S);

					if (!atLeastOne)
						state = "not started";

					void Method(int idDelta, KeyCode keyCode)
					{
						if (state == "not started")
						{
							state = "starting";
							SelectItem(selectedId + idDelta, false);
							lastKeyTime = time;
						}
						else if (state == "starting" && deltaTime > 250)
						{
							state = "started";
							SelectItem(selectedId + idDelta, false);
							lastKeyTime = time;
						}
						else if (state == "started" && deltaTime > 45)
						{
							lastKeyTime = time;
							SelectItem(selectedId + idDelta, false);
							lastKeyTime = time;
						}

						atLeastOne = true;
					}
				}
			}
			else
			{
				if (Input.GetAxis("Mouse ScrollWheel") > 0f)
					SelectItem(selId - 1, false);
				if (Input.GetAxis("Mouse ScrollWheel") < 0f)
					SelectItem(selId + 1, false);

				if (Input.GetMouseButtonDown(0))
					Click();

				if (Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(1))
					_camera.fieldOfView = 28;
				if (Input.GetKeyUp(KeyCode.C) || Input.GetMouseButtonUp(1))
					_camera.fieldOfView = 76; ///////////////////////////////////
			}

			bool throwable = false;

			if (items.Length <= selectedId || IsNull(items[selectedId]))
			{
				Debug.Log($"Inventory waiting for items");
				return;
			}
			else
			{
				if (!string.IsNullOrEmpty(items[selectedId]._name))
					throwable = S.II.Get(items[selectedId]._name)._throwable;

				if (!string.IsNullOrEmpty(items[selectedId]._name))
				{
					if (!opened) //So, why the hell it is throwing??
					{
						if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Q) ||
						(Input.GetMouseButtonDown(0) && !_somethingClicked))
						{
							throwTime = 0;
							throwing = true;
							if (throwable)
								throwPanelBlack.transform.localScale = new Vector3(1, 1, 0);
							else
								throwPanelRed.transform.localScale = new Vector3(1, 1, 0);
						}
					}
					else if (throwable) //Okay, so...
						if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Q))
							Throw(selectedId, 300);
				}

				if (throwing)
				{
					if (!string.IsNullOrEmpty(items[selectedId]._name))
					{
						if (throwable)
						{
							throwTime += Time.deltaTime * Math.Max(Math.Min(MathF.Pow(throwCombo, 0.35f), 10), 1);

							throwSize = Math.Max(throwTime * 1.2f, 0);

							throwPanel.transform.localScale = new Vector3(throwSize, 1, 0);
						}
					}
					else
						StopCombo();
				}

				if (throwing && (Input.GetKeyUp(KeyCode.R) || Input.GetKeyUp(KeyCode.Q) || Input.GetMouseButtonUp(0)))
				{
					if (throwable && (throwCombo == 0 || throwSize > 0.25f))
						Throw(selectedId, throwSize * 1500);

					throwing = false;
					throwPanel.transform.localScale = new Vector3(0, 0, 0);
					throwPanelBlack.transform.localScale = new Vector3(0, 0, 0);
					throwPanelRed.transform.localScale = new Vector3(0, 0, 0);
					throwTime = 0;
					throwCombo = 0;

					throwSize = 0;
				}

				if (throwing && throwSize > 1)
				{
					if (!string.IsNullOrEmpty(items[selectedId]._name))
					{
						if (throwable)
						{
							Throw(selectedId, throwSize * 1500);
							throwCombo++;
						}
						else
							StopCombo();
					}
					else
						StopCombo();

					throwTime = 0;
					throwSize = 0;
				}

				void StopCombo()
				{
					throwing = false;
					throwPanel.transform.localScale = new Vector3(0, 0, 0);
					throwPanelBlack.transform.localScale = new Vector3(0, 0, 0);

					throwCombo = 0;
					throwTime = 0;
					throwSize = 0;
				}

				if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
					SelectItem(0, false);
				else if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
					SelectItem(1, false);
				else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
					SelectItem(2, false);
				else if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))
					SelectItem(3, false);
				else if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5))
					SelectItem(4, false);
				else if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6))
					SelectItem(5, false);
				else if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7))
					SelectItem(6, false);
				else if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8))
					SelectItem(7, false);
				else if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9))
					SelectItem(8, false);
			}
		}
	}

	public void Click()
	{
		_somethingClicked = true;

		if (!_marketOpened && !opened)
		{
			Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 7f, notTransperent))
			{
				Locker locker = hit.collider.gameObject.GetComponent<Locker>();
				if (locker != null)
				{
					if (!string.IsNullOrEmpty(items[selectedId]._name))
						locker.Unlock(items[selectedId]._name);
					else
						S.AudioManager.Play("notEnoughCash", 1);
				}

				FrerardHolder fh = hit.collider.gameObject.GetComponent<FrerardHolder>();
				if (fh != null)
				{
					if (!string.IsNullOrEmpty(items[selectedId]._name))
					{
						if (items[selectedId]._name.Contains("Frerard"))
						{
							fh.Take(items[selectedId]);
							items[selectedId] = null;
							Visualise(selectedId);
							return;
						}
						else
							fh.Take(null);
					}
					else
					{
						fh.Take(null);
					}
				}

				IsTrader trader = hit.collider.gameObject.GetComponent<IsTrader>();
				if (trader != null)
				{
					trader.OpenMarket();
					return;
				}

				Door door = hit.collider.gameObject.GetComponent<Door>();
				if (door != null)
				{
					if (_playerObject.tag == "Player")
						door.Go();
					return;
				}

				ItemP itemP = hit.collider.gameObject.GetComponent<ItemP>();
				if (itemP != null)
				{
					Take(itemP);
					return;
				}

				Button1 button1 = hit.collider.gameObject.GetComponent<Button1>();
				if (button1 != null)
				{
					button1.Go(_camera);
					return;
				}

				SaverTypewriter saver = hit.collider.gameObject.GetComponent<SaverTypewriter>();
				if (saver != null)
				{
					saver.Save();
					return;
				}
			}

			Item item = items[selectedId];
			if (item != null)
			{
				if (item._name == "FirstAidKit")
				{
					if (_playerStorage._health < 100)
					{
						_playerStorage.Heal(100);
						Remove("FirstAidKit", 1);
						S.AudioManager.Play("heal", 1);
						return;
					}
				}

				if (item._name == "Gun")
				{
					S.IsGun.Fire();
					return;
				}
			}

			if (Physics.Raycast(ray, out hit, 7f, notTransperent))
			{
				IsDoor isDoor = hit.collider.gameObject.GetComponent<IsDoor>();
				if (isDoor != null)
				{
					isDoor.Move();
					return;
				}
			}

			if (Physics.Raycast(ray, out hit, 7f, notTransperent))
			{
				IsShuffle isShuffle = hit.collider.gameObject.GetComponent<IsShuffle>();
				if (isShuffle != null)
				{
					isShuffle.Move();
					return;
				}
			}

			_somethingClicked = false;
		}
	}

	public void Hack(GameObject go)
	{
		var i = go.GetComponent<ItemP>();
		if (i._locked)
			i.ToggleLock(false);
		Take(i);
	}

	public void Take(GameObject go)
	{
		Take(go.GetComponent<ItemP>());
	}

	public void Take(ItemP itemP)
	{
		Take(itemP._name, itemP._count);
		itemP.Destroy();
	}

	public void Take(string name, int count)
	{
		string an = S.II.Get(name)._pickUpA;
		if (string.IsNullOrEmpty(items[selId]._name))
		{
			items[selId]._name = name;
			items[selId]._count = count;
			Visualise(selId);
			S.AudioManager.Play(an, 1);
			CheckShowing(selId);
            SaveOneItem(selId);
			CheckQuests(name);
            Debug.Log($"Taked {name} to {selId} *1");
			return;
		}

		if (items[selId]._name == name)
		{
			items[selId]._count += count;
			Visualise(selId);
			S.AudioManager.Play(an, 1);
			CheckShowing(selId);
            SaveOneItem(selId);
            CheckQuests(name);
            Debug.Log($"Taked {name} to {selId} *2");
			return;
		}

		int id = 0;
		for (; id < 36; id++)
			if (!string.IsNullOrEmpty(items[id]._name))
				if (items[id]._name == name)
				{
					items[id]._count += count;
					Visualise(id);
					S.AudioManager.Play(an, 1);
					CheckShowing(id);
                    SaveOneItem(id);
                    CheckQuests(name);
                    Debug.Log($"Taked {name} to {id} *3");
					return;
				}

		if (id >= 36)
			for (id = 0; id < 36; id++)
				if (string.IsNullOrEmpty(items[id]._name))
				{
					items[id]._name = name;
					items[id]._count = count;
					Visualise(id);
					S.AudioManager.Play(an, 1);
					CheckShowing(id);
                    SaveOneItem(id);
                    CheckQuests(name);
                    Debug.Log($"Taked {name} to {id} *4");
					return;
				}
	}

	public void CheckQuests(string name)
	{
		if (name == "GreenKey")
			S.SM.Save("greenKeyTaken", true);
    }

	public void CheckShowing(int id)
	{
		if (!(S.SM.LoadBool("Cucumber showed") ?? false))
			if (items[id]._name == "Cucumber")
			{
				S.SM.Save("Cucumber showed", true);
				ShowItem(id, "You obtain a cucumber!!!", "gong", 0.5f, 0.35f, new Vector3(0, 36, 0), new Vector3(0, 0, 7));
			}
		if (!(S.SM.LoadBool("Gun showed") ?? false))
			if (items[id]._name == "Gun")
			{
                S.SM.Save("Gun showed", true);
                ShowItem(id, "You obtain a gun!!!", "gong", 0.5f, 0.35f, new Vector3(0, 36, 0), new Vector3(0, 0, 7));
			}
	}

	public void ShowItem(int id, string text, string audioName, float offset, float delay, Vector3 startRotation, Vector3 rotation)
	{
		if (!string.IsNullOrEmpty(items[id]._name))
		{
			if (opened)
				SwitchInventory();
			if (_marketOpened)
				_trader.CloseMarket();

			//Debug.Log($"Name {items[id]._name} id {id}");

			StartCoroutine(LateShow());

			IEnumerator LateShow()
			{
				showingCamera.enabled = true;

				yield return new WaitForSeconds(delay);

				smallInventoryPanel.SetActive(false);

				showingStartTime = Time.time;

				Vector3 position = showingCamera.transform.position;
				Vector3 direction = showingCamera.transform.forward * 3;
				Vector3 offsetV = new Vector3(0, offset, 0);

				var prefab = Prefabs.Get(items[id]._name);
				//Debug.Log($"Prefab {prefab}, name {items[id]._name}, id {id}");

				_showingItem = Instantiate(prefab, position + direction + offsetV, Quaternion.identity);

				_showingItem.transform.eulerAngles = startRotation;

				S.AudioManager.Play(audioName, 1);

				showingCamera.enabled = true;
				showingCamera.gameObject.SetActive(true);
				showingPanel.gameObject.SetActive(true);
				showingOverlay.SetActive(true);
				showingText.text = text;
				_showingRotation = rotation;
			}
		}
	}

	public void HideShowingItem()
	{
		if (_showingItem != null)
		{
			Destroy(_showingItem);
			_showingItem = null;
			showingCamera.enabled = false;
			showingCamera.gameObject.SetActive(false);
			showingPanel.gameObject.SetActive(false);
			showingOverlay.SetActive(false);
			smallInventoryPanel.SetActive(true);
			S.AudioManager.Play("pickUp", 1.3f);
		}
	}

	public void Throw(int id, float power)
	{
		if (!string.IsNullOrEmpty(items[id]._name))
		{
			Vector3 position = _camera.transform.position;
			Vector3 direction = _camera.transform.forward;

			int buffer = items[id]._count;
			items[id]._count = 1;

			items[id].Throw(position, direction, power, prb.velocity, _camera.transform.rotation); //

			items[id]._count = buffer - 1;

			if (items[id]._count <= 0)
			{
				items[id]._name = "";
				ultraSelectedId = -1;
				selectorPanelPlus.SetActive(false);
			}

			Visualise(id);
			S.AudioManager.Play("throw", MathF.Min(MathF.Max(MathF.Pow(throwCombo, 0.1f), 1), 5));

			SaveOneItem(id);
		}
	}

	public void SwitchInventory()
	{
		if (!_marketOpened)
		{
			opened = !opened;

			if (opened)
			{
				Cursor.lockState = CursorLockMode.None;
				SelectItem(selectedId, false);
			}
			else
			{
				Cursor.lockState = CursorLockMode.Locked;
				SelectItem(selId, false);
				ultraSelectedId = -1;
				selectorPanelPlus.SetActive(false); //
			}

			Cursor.visible = opened;

			inventoryPanel.gameObject.SetActive(opened);
			smallInventoryPanel.gameObject.SetActive(!opened);
			cursorPanel.gameObject.SetActive(!opened);
			_circleCursor.SetActive(!opened);
		}
	}

	public void SelectItem(int id, bool permanently)
	{
		throwing = false;
		throwPanel.transform.localScale = new Vector3(0, 0, 0);
		throwPanelBlack.transform.localScale = new Vector3(0, 0, 0);
		throwPanelRed.transform.localScale = new Vector3(0, 0, 0);

		//Vector2 canvasScale = new Vector2(canvas.transform.lossyScale.x, canvas.transform.lossyScale.y);

		S.AudioManager.Play("inventory", 1.3f);

		if (opened)
		{
			Cut(36);

			if (id < 9)
				SelectSmall();

			SelectBig();
		}
		else
		{
			Cut(9);
			SelectBig();
			SelectSmall();
		}

		void SelectSmall()
		{
			float s = 1.3f;

			selectorPanel.transform.SetParent(inventoryPanelParentSmall.transform);
			selectorPanel.transform.SetSiblingIndex(0); //?

			selectorPanel.transform.position = smallPanels[id].transform.position;// + new Vector3(0, 1, 0);
			selectorPanel.transform.localScale = new Vector3(s, s, 0);

			selId = id;
		}

		void SelectBig()
		{
			float s = 2.6f;

			selectorPanel.transform.SetParent(inventoryPanelParentBig.transform);

			selectorPanel.transform.SetSiblingIndex(1); //?

			selectorPanel.transform.position = panels[id].transform.position;
			selectorPanel.transform.localScale = new Vector3(s, s, 0);

			selectedId = id;

			if (opened && permanently)
			{
				if (ultraSelectedId != -1)
				{
					if (ultraSelectedId != id)
					{
						if (items[id].IsUnityNull())
						{
							items[id] = new Item();
							Debug.Log($"!!! Here was null item on id {id}. I fixed.");
						}
						Debug.Log($"Item name: {items[id]._name}");
						Debug.Log($"Is name empty: {string.IsNullOrEmpty(items[id]._name)}");
						if (!string.IsNullOrEmpty(items[id]._name))
						{
							if (!items[id]._name.Equals(items[ultraSelectedId]._name))
							{
								Item buffer = new Item();
								buffer.CloneFrom(items[ultraSelectedId]);
								items[ultraSelectedId] = items[id];
								items[id] = buffer;

                                SaveTwoItems();

                                Debug.Log("ITEMS SWAPPED.");
							}
							else
							{
								items[id]._count += items[ultraSelectedId]._count;
								items[ultraSelectedId]._name = "";

                                SaveTwoItems();

                                Debug.Log("ITEMS MERGED.");
                            }
						}
						else
						{
							items[id]._name = items[ultraSelectedId]._name;
                            items[id]._count = items[ultraSelectedId]._count;
                            items[ultraSelectedId]._name = "";
                            items[ultraSelectedId]._count = 0;

							SaveTwoItems();

							Debug.Log("ITEMS MOVED.");
						}

						Visualise(id);
						Visualise(ultraSelectedId);
					}

					ultraSelectedId = -1;
					selectorPanelPlus.SetActive(false);

					SaveOneItem(id);

                    Debug.Log("DESELECTED.");
                }
				else if (!string.IsNullOrEmpty(items[id]._name))
				{
					float f = 1.8f;

					ultraSelectedId = id;

					selectorPanelPlus.SetActive(true);
					selectorPanelPlus.transform.SetParent(inventoryPanelParentBig.transform);
					selectorPanel.transform.SetSiblingIndex(0);
					selectorPanelPlus.transform.SetSiblingIndex(0);
					selectorPanelPlus.transform.position = panels[id].transform.position;
					selectorPanelPlus.transform.localScale = new Vector3(f, f, 0);

                    Debug.Log("SELECTED.");
                }
			}
		}

		void Cut(int count)
		{
			id = (count + id) % count;
		}
	}

	public bool IsEmpty(Item item)
	{
		try
		{
			string aboba = item._name;
			return string.IsNullOrEmpty(aboba);
		}
		catch
		{
			return true;
		}
	}

	public bool IsNull(Item item)
	{
		bool so = false;

        try
        {
            string aboba = item._name;
            return false;
        }
        catch
        {
            return true;
        }
    }

    public void SaveTwoItems()
    {
        SaveOneItem(selectedId);
        SaveOneItem(ultraSelectedId);
    }

    public void SaveOneItem(int id0)
    {
        S.SM.Save(S.ID("INV name ", id0), items[id0]._name);
        S.SM.Save(S.ID("INV count ", id0), items[id0]._count);
    }
}
