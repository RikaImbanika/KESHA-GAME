using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Subwoofer : MonoBehaviour
{
    public Collider _collider;
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

    public void OnPlayerTouch(Vector3 center)
    {
        PlayerMovement pm = S.Pm;
        Vector3 direction = S.Camera.transform.position - center;
        direction = new Vector3(direction.x, direction.y, direction.z).normalized;
        direction *= 100f;
        direction += new Vector3(0, 3, 0);
        pm.Push(direction, false, 0.25f);
    }
}
