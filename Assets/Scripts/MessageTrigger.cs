// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageTrigger : MonoBehaviour
{
    public string _message; //Will be problems with langueges
    public Color _color;
    public float _delay;
    public string _saveId;
    bool _said;

    void Start()
    {
        _said = S.SM.LoadBool(_saveId) ?? false;
    }

    void OnTriggerEnter(Collider collider)
    {
        if ((collider.gameObject.tag == "Player") && !_said)
        {
            _said = true;
            S.SM.Save(_saveId, true);
            StartCoroutine(SayAsync());
        }

        IEnumerator SayAsync()
        {
            yield return new WaitForSeconds(_delay);

            S.Console.AddMessage(_message, _color);
        }
    }
}
