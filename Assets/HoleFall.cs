using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleFall : MonoBehaviour
{
    string _sceneName;
    bool _inside;
    float _startTime;
    bool _secondTime;
    bool _sayed;
    int _layerMask;
    Vector3 _pos;

    void Start()
    {
        _sceneName = gameObject.scene.name;
        _pos = transform.position;
        _layerMask = 1 << LayerMask.NameToLayer("Player");
    }

    void Enter()
    {
        if (!_inside)
        {
            _inside = true;
            _startTime = Time.time;
            _sayed = false;
        }
    }

    void Exit()
    {
        if (_inside)
        {
            _inside = false;
            _sayed = false;
        }
    }

    void Update()
    {
        if (_sceneName == S.PS._currentSceneName)
        {
            RaycastHit hit;
            if (Physics.Raycast(_pos, Vector3.up, out hit, 4f, _layerMask))
            {
                Enter();
            }
            else
                Exit();
        }

        if (_inside)
        {
            float d = Time.time - _startTime;

            if (d > 1f && !_sayed)
                SayIt();
            if (d > 2f)
                Push();
        }
    }

    void SayIt()
    {
        float pitch = 1.25f + (float)S.RND.NextDouble() * 0.1f;

        if (_secondTime)
            S.AM.Play("wrong2", pitch);
        else
            S.AM.Play("ohNo", pitch);

        _secondTime = !_secondTime;
        _sayed = true;
    }

    void Push()
    {
        S.PlayerMovement.Push(transform.up * 100f);
        S.AM.Play("throw");
        _inside = false;
        _sayed = false;

        StartCoroutine(Push2());
    }

    private IEnumerator Push2()
    {
        yield return new WaitForSeconds(0.15f);

        S.AM.Play("throw");
        S.PlayerMovement.Push(transform.forward * 200f, true);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 4f);
    }
#endif
}
