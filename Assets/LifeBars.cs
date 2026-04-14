using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LifeBars : MonoBehaviour
{
    public GameObject _textObj;
    public GameObject _bar1;
    public GameObject _bar2;
    public GameObject _healthObj;
    public GameObject _healthObjChild;
    public GameObject _healthObjBlack;
    private Transform _healthTransform;
    private Material _mat1;
    private Material _mat2;
    private Material _mat3;
    private Material _mat4;
    private TextMeshPro _tmp;
    private float _timeLeft;

    void Start()
    {
        _tmp = _textObj.GetComponent<TextMeshPro>();
        Color color = Color.white;
        color.a = 0;
        _tmp.color = color;
        S.LifeBars = this;
        _healthTransform = _healthObj.transform;
        Renderer rend1 = _bar1.GetComponent<Renderer>();
        Renderer rend2 = _bar2.GetComponent<Renderer>();
        Renderer rend3 = _healthObjChild.GetComponent<Renderer>();
        Renderer rend4 = _healthObjBlack.GetComponent<Renderer>();
        _mat1 = rend1.sharedMaterial;
        _mat2 = rend2.sharedMaterial;
        _mat3 = rend3.sharedMaterial;
        _mat4 = rend4.sharedMaterial;
        _mat1.SetFloat("_Alpha", 0);
        _mat2.SetFloat("_Alpha", 0);
        _mat3.SetFloat("_Alpha", 0);
        _mat4.SetFloat("_Alpha", 0);
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
                float alpha = Mathf.SmoothStep(0, 1, t);
                color.a = alpha;
                _tmp.color = color;
                _mat1.SetFloat("_Alpha", alpha);
                _mat2.SetFloat("_Alpha", alpha);
                _mat3.SetFloat("_Alpha", alpha);
                _mat4.SetFloat("_Alpha", alpha);
            }
        }
    }

    public void Show(string text, float lifePercent)
    {
        _timeLeft = 3f;
        _tmp.text = text;
        Color color = Color.white;
        _tmp.color = color;
        _mat1.SetFloat("_Alpha", 1);
        _mat2.SetFloat("_Alpha", 1);
        _mat3.SetFloat("_Alpha", 1);
        _mat4.SetFloat("_Alpha", 1);
        _healthTransform.localScale = new Vector3(lifePercent, 1, 1);
    }
}