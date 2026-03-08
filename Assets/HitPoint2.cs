using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoint2 : MonoBehaviour
{
    private float _timeLeft;
    private Vector3 _scale;
    public Texture2D _texture;
    public float _MaxScale;
    public float _duration;
    Material _mat;

    void Start()
    {
        _timeLeft = _duration;
        _scale = transform.localScale;
        transform.rotation = S.RandRot.Get();

        Shader shader = Shader.Find("Custom/FadeOverTime");

        _mat = new Material(shader);
        _mat.SetTexture("_MainTex", _texture);

        foreach (Transform child in transform)
        {
            Renderer rend = child.GetComponent<Renderer>();
            rend.sharedMaterials = new Material[] { _mat };
        }
    }

    void Update()
    {
        _timeLeft -= Time.deltaTime;

        if (_timeLeft < 0)
            Destroy(gameObject);

        float c0 = (_duration - _timeLeft) / _duration;
        float c = c0 * _MaxScale;
        transform.localScale = new Vector3(_scale.x * c, _scale.y * c, _scale.z * c);

        float alpha = MathF.Min(MathF.Pow(1 - c0, 3f), 1f);
        _mat.SetFloat("_Alpha", alpha);
    }
}