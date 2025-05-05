using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Linq;

public class Frerard : MonoBehaviour
{
    public string _audioName;
    public List<bool> _values;
    public bool _activated;
    bool _finished;
    float _startY;
    public float _finalDeltaY;
    public float _deltaY;

    void Start()
    {
        _values = S.SM.LoadListBool("FrerardList");
        if (_values.Count != 6)
            _values = Enumerable.Repeat(false, 6).ToList();

        if (S.SM.LoadBool("FrerardFinished") ?? false)
            transform.position += new Vector3(0, _finalDeltaY, 0);
    }

    public void Set(int number, bool value)
    {
        Debug.Log($"FRERARD NUMBER {number} - {value}; valuesLen = {_values.Count}");

        _values[number] = value;

        S.SM.Save("FrerardList", _values);

        for (int i = 0; i < 6; i++)
            if (!_values[i])
                return;

        Finish();
    }

    void Finish()
    {
        StartCoroutine(WaitLoad());

        IEnumerator WaitLoad()
        {
            S.SM.Save("FreardFinished", true);

            S.AM.Play(_audioName, 1);
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
