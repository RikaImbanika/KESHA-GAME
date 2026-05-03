using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleShower : MonoBehaviour
{
    public GameObject _bathrooms;
    public GameObject _backrooms;
    public GameObject _mushrooms;

    private TextMeshPro _bathroomsTmp;
    private TextMeshPro _backroomsTmp;
    private TextMeshPro _mushroomsTmp;

    private TextMeshPro _tmp;
    private GameObject _obj;

    private float _dur;
    private float _delay;

    private string _whatToShow;
    private float _timeLeft;
    private Vector3 _targetScale;

    void Start()
    {
        _delay = 2f;
        _dur = 7f;

        _bathrooms.SetActive(true);
        _mushrooms.SetActive(true);
        _backrooms.SetActive(true);

        _bathroomsTmp = _bathrooms.GetComponent<TextMeshPro>();
        _backroomsTmp = _backrooms.GetComponent<TextMeshPro>();
        _mushroomsTmp = _mushrooms.GetComponent<TextMeshPro>();

        Color trans = Color.white;
        trans.a = 0;

        _bathroomsTmp.color = trans;
        _backroomsTmp.color = trans;
        _mushroomsTmp.color = trans;
    }

    void Show(string what)
    {
        if (_whatToShow != what)
        {
            if (_timeLeft > 0f)
            {
                _obj.transform.localScale = _targetScale;
            }

            _whatToShow = what;
            _timeLeft = _dur + _delay;

            _tmp = _backroomsTmp;
            _obj = _backrooms;
            _targetScale = _obj.transform.localScale;

            if (what == "Bathrooms")
            {
                _tmp = _bathroomsTmp;
                _obj = _bathrooms;
            }
            else if (what == "Mushrooms")
            {
                _tmp = _mushroomsTmp;
                _obj = _mushrooms;
            }

            Color clr = Color.white;
            clr.a = 0;
            _tmp.color = clr;
        }
    }

    void Update()
    {
        string sn = S.PS._currentSceneName;

        if (sn.Contains("BR"))
            Show("Backrooms");
        else if (sn.Contains("MR"))
        {
            if (sn != "MR 1")
                Show("Mushrooms");
        }
        else if (sn.Contains("TL"))
        {
            if (sn != "TL 0" && sn != "TL 1")
                Show("Bathrooms");
            else if (sn == "TL 1" && S.Camera.transform.position.x < -162)
                Show("Bathrooms");
        }

        if (_timeLeft > 0)
        {
            //Just math. Unexplainable.

            _timeLeft = Mathf.Max(_timeLeft - Time.deltaTime, 0);
            float x0 = Math.Max(_dur - _timeLeft, 0);
            float x1 = (x0) / _dur;
            x1 = Mathf.Max(x1, 0);
            float scaleCoef = 0.7f + 0.3f * Mathf.Pow(x1, 0.5f);

            if (x0 <= 0)
                scaleCoef = 0f;

            _obj.transform.localScale = _targetScale * scaleCoef;
            Color clr = Color.white;
            float x = (x0 - 4f) / 3f;
            float x2 = 1 - (x0 / _dur * 1.25f);
            float x3 = Mathf.Max(x, x2);
            clr.a = Mathf.SmoothStep(0, 1, 1 - x3);
            _tmp.color = clr;
        }
    }
}
