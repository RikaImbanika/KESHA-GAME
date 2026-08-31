// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextPlacerFromConole : MonoBehaviour
{
    public string _commandName;
    
    void Start()
    {
        TextMeshPro tmp = GetComponent<TextMeshPro>();
        string text = "???????????????????";
        StartCoroutine(PlaceAsync());

        IEnumerator PlaceAsync()
        {
            while (S.Console == null)
                yield return new WaitForSeconds(1f);

            if (_commandName == "Items")
                text = S.Console.ItemsText();
            else if (_commandName == "Scenes")
                text = S.Console.ScenesText();

            tmp.text = text;
        }
    }
}
