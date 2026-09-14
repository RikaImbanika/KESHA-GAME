// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStorage : MonoBehaviour
{
    public string _currentSceneName;
    public float _health;
    public Transform _healthTransform;
    public Material _healthMat;
    public GameObject _onDiePanel;
    public Vector3 _camPos;
    public Vector3 _prevCamPos;

    void Start()
    {
        _health = 100f; ///////// Load...
        _camPos = new Vector3(0, 0, 0);
        _prevCamPos = new Vector3(0, 0, 0);
        VisualiseHealth();
    }

    public void Damage(float amount)
    {
        if (_health > 0)
        {
            _health -= amount;
            S.AM.Play("Damage", 1);
            S.SM.Save("health", _health);

            if (_health <= 0f)
            {
                _onDiePanel.SetActive(true);
                S.SM.LoadLastSave();
            }
        }
        else
        {
            Debug.LogError("Player is already dead!");
        }
        VisualiseHealth();
    }

    public void VisualiseHealth()
    {
        _healthTransform.localScale = new Vector3(_health / 100f, 1, 1);

        Color orange = new Color(1f, 0.5f, 0f);

        if (_health >= 45f)
            _healthMat.color = Color.green;
        else if (_health >= 30f)
        {
            float t = Mathf.InverseLerp(30f, 45f, _health);
            _healthMat.color = Color.Lerp(Color.yellow, Color.green, t);
        }
        else if (_health >= 22f)
        {
            float t = Mathf.InverseLerp(22f, 30f, _health);
            _healthMat.color = Color.Lerp(orange, Color.yellow, t);
        }
        else if (_health >= 12f)
        {
            float t = Mathf.InverseLerp(12f, 22f, _health);
            _healthMat.color = Color.Lerp(Color.red, orange, t);
        }
        else
            _healthMat.color = Color.red;
    }

    bool SceneCurrentlyLoaded(string sceneName_no_extention)
    {
        for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName_no_extention)
            {
                if (scene.isLoaded)
                    return true;
                else
                    return false;
            }
        }

        return false;
    }

    public void Heal(float amount)
    {
        _health += amount;
        if (_health > 100f)
            _health = 100f;
        VisualiseHealth();
    }
}