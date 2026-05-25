using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameVersionPlacer : MonoBehaviour
{
    public string _version;
    public TextMeshProUGUI _tmpOnStartPanel;
    public TextMeshProUGUI _tmpInInventory;

    void Start()
    {
        _tmpOnStartPanel.text = $"v {_version}";
        _tmpInInventory.text = $"Kesha Game, v{_version}, Made by Rika Imbanika";
    }
}
