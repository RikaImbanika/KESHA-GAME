// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemNameShower : MonoBehaviour
{
    public GameObject _textObj;
    private TextMeshPro _tmp;
    private float _timeLeft;

    void Start()
    {
        _tmp = _textObj.GetComponent<TextMeshPro>();
        Color color = Color.white;
        color.a = 0;
        _tmp.color = color;
        S.ItemNameShower = this;
    }

    void Update()
    {
        if (_timeLeft > 0f)
        {
            _timeLeft -= Time.deltaTime;
            if (_timeLeft < 0)
                _timeLeft = 0f;

            if (_timeLeft < 2.5f)
            {
                Color color = _tmp.color;

                float t = _timeLeft / 2.5f;
                float alpha = Mathf.SmoothStep(0, 1, t);
                color.a = alpha;
                _tmp.color = color;
            }
        }
    }

    public void Show(string text, Color color)
    {
        _timeLeft = 3f;
        _tmp.text = text;
        _tmp.color = color;
    }
}