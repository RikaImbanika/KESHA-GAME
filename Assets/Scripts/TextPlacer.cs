// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextPlacer : MonoBehaviour
{
    public string _textFilePath;

    void Start()
    {
        TextMeshPro tmp = GetComponent<TextMeshPro>();
        TextAsset text = Resources.Load<TextAsset>($"Texts/{_textFilePath}");
        tmp.text = text.text;
    }
}
