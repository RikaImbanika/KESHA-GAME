using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LifeBars : MonoBehaviour
{
    public GameObject _textObj;
    public TextMeshPro _tmp;
    public float _timeLeft;

    void Start()
    {
        _tmp = _textObj.GetComponent<TextMeshPro>();
        Color color = Color.white;
        color.a = 0;
        _tmp.color = color;
        S.LifeBars = this;
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
                Color color = Color.white;

                float t = _timeLeft / 2.5f;
                color.a = Mathf.SmoothStep(0, 1, t);
                _tmp.color = color;
            }
        }
    }

    public void Show(string text)
    {
        _timeLeft = 3f;
        _tmp.text = text;
        Color color = Color.white;
        _tmp.color = color;
    }
}