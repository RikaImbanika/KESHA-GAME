using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    private Quaternion[] _rotations;
    private int _index;

    void Start()
    {
        _rotations = new Quaternion[150];

        StartCoroutine(Yep());

        IEnumerator Yep()
        {
            while (S.RND == null)
                yield return new WaitForSeconds(0.15f);

            for (int i = 0; i < 150; i++)
            {
                _rotations[i] = Quaternion.Euler(S.RND.Next(-180, 180), S.RND.Next(-180, 180), S.RND.Next(-180, 180));
            }

            S.RandRot = this;
        }
    }

    public Quaternion Get()
    {
        _index++;
        if (_index >= 150)
            _index = 0;

        return _rotations[_index];
    }
}
