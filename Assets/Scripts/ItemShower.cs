// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemShower : MonoBehaviour
{
    public Camera _showingCamera;
    public GameObject _showingItem;
    public Vector3 _showingRotation;
    public RawImage _showingPanel;
    public GameObject _showingOverlay;
    public TextMeshProUGUI _showingText;
    public float _showingStartTime;
    public Vector3 _showingStartScale;
    
    void Start()
    {
        _showingItem = null;
        _showingCamera.enabled = false;
        _showingCamera.gameObject.SetActive(false);
        _showingPanel.gameObject.SetActive(false);
        _showingOverlay.SetActive(false);

        S.ItemShower = this;
    }

    public void ShowItem(string itemName, string text, float offset, float delay, Vector3 startRotation, Vector3 rotation)
    {
        if (!string.IsNullOrEmpty(itemName))
        {
            if (S.Inventory.opened)
                S.Inventory.SwitchInventory();
            if (S.Inventory._marketOpened)
                S.Inventory._trader.CloseMarket(); //TO DO fix when we have more traders

            StartCoroutine(LateShow());

            IEnumerator LateShow()
            {
                _showingCamera.enabled = true;

                yield return new WaitForSeconds(delay);

                S.Inventory.smallInventoryPanel.SetActive(false);

                _showingStartTime = Time.time;

                Vector3 position = _showingCamera.transform.position;
                Vector3 direction = _showingCamera.transform.forward * 3;
                Vector3 offsetV = new Vector3(0, offset, 0);

                var prefab = Prefabs.Get(itemName);

                _showingItem = Instantiate(prefab, position + direction + offsetV, Quaternion.identity);
                _showingStartScale = _showingItem.transform.localScale;
                _showingItem.transform.localScale = Vector3.zero;

                _showingItem.transform.eulerAngles = startRotation;

                S.AM.Play("Gong");
                S.AM.Play("Crowd Is Happy"); //TO DO more sounds in future

                _showingCamera.enabled = true;
                _showingCamera.gameObject.SetActive(true);
                _showingPanel.gameObject.SetActive(true);
                _showingOverlay.SetActive(true);
                _showingText.text = text;
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
            _showingCamera.enabled = false;
            _showingCamera.gameObject.SetActive(false);
            _showingPanel.gameObject.SetActive(false);
            _showingOverlay.SetActive(false);
            S.Inventory.smallInventoryPanel.SetActive(true);
            S.AM.Play("Pick Up", 1.3f);
        }
    }

    public void TryShow(string itemName)
    {
        if (!(S.SM.LoadBool("Cucumber showed") ?? false))
            if (itemName == "Cucumber")
            {
                S.SM.Save("Cucumber showed", true);
                ShowItem(itemName, "You obtain a cucumber!!!", 0.7f, 0.35f, new Vector3(0, 36, 0), new Vector3(0, 0, 7));
            }
        if (!(S.SM.LoadBool("Gun showed") ?? false))
            if (itemName == "Gun")
            {
                S.SM.Save("Gun showed", true);
                ShowItem(itemName, "You obtain a gun!!!", 1.6f, 0.35f, new Vector3(90, 0, 0), new Vector3(0, 0, 7));
            }
    }

    void Update()
    {
        if (_showingItem != null)
        {
            float k = Time.deltaTime * 60f;
            
            _showingItem.transform.Rotate(_showingRotation.x * k, _showingRotation.y * k, _showingRotation.z * k, Space.World);
            float scaleCoef = 1 - (1 / ((Time.time - _showingStartTime) * 8f + 1f));
            _showingItem.transform.localScale = _showingStartScale * scaleCoef;
        }
    }
}
