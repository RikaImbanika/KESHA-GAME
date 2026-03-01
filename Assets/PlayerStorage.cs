using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStorage : MonoBehaviour
{
    public string _currentSceneName;
    public float _health;
    public Image _healthImage;
    public GameObject _onDiePanel;
    public Vector3 _camPos;
    public Vector3 _prevCamPos;

    void Start()
    {
        _health = 100f; /////////
        _camPos = new Vector3(0, 0, 0);
        _prevCamPos = new Vector3(0, 0, 0);
    }

    public void Damage(float amount)
    {
        if (_health > 0)
        {
            _health -= amount;
            S.AudioManager.Play("damage", 1);
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
        _healthImage.fillAmount = _health / 100f;
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
        _healthImage.fillAmount = _health / 100f;
    }
}