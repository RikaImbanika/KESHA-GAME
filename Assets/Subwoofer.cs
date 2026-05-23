using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subwoofer : MonoBehaviour
{
    Animator _ani;
    string _sceneName;
    float _timeLeft;
    int prevR;

    void Start()
    {
        _sceneName = gameObject.scene.name;
        _ani = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if (S.Ps._currentSceneName == _sceneName)
        {
            _timeLeft -= Time.deltaTime;

            if (_timeLeft <= 0)
            {
                int r = S.RND.Next(6) + 1;

                while (r == prevR)
                    r = S.RND.Next(6) + 1;

                prevR = r;

                _ani.SetTrigger($"Tr{r}");
                _timeLeft = 1;
            }
        }
    }
}
