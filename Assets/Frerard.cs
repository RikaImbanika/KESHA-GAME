using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Frerard : MonoBehaviour
{
    public string _audioName;
    AudioManager _audioManager;
    public bool[] _values;
    public bool _activated;
    bool _finished;
    float _startY;
    public float _finalDeltaY;
    public float _deltaY;

    void Start()
    {
        _values = new bool[6];
        GameObject go = GameObject.FindGameObjectWithTag("AudioManager");
        _audioManager = go.GetComponent<AudioManager>();
    }

    public void Set(int number, bool value)
    {
        _values[number] = value;

        for (int i = 0; i < 6; i++)
            if (!_values[i])
                return;

        PlayAnimation();
    }

    void PlayAnimation()
    {
        StartCoroutine(WaitLoad());

        IEnumerator WaitLoad()
        {
            _audioManager.Play(_audioName, 1);
            yield return new WaitForSeconds(3);
            _startY = transform.position.y;
            _activated = true;
            yield return null;
        }
    }

    void Update()
    {
        if (!_finished && _activated)
        {
            if (transform.position.y > _startY + _finalDeltaY)
            {
                transform.position = transform.position + new Vector3(0, _deltaY * Time.deltaTime * 60, 0);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, _startY + _finalDeltaY, transform.position.z);

                Thread.Sleep(500);

                _finished = true;
            }
        }
    }
}
