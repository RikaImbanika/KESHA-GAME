// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinishedMessages : MonoBehaviour
{
    string _saveId;
    bool _said;

    void Start()
    {
        _saveId = "GameFinishedMessages";
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
            yield return new WaitForSeconds(0.1f);
            S.Console.AddMessage("Congratulations!", Color.cyan);
            yield return new WaitForSeconds(2.5f);
            S.Console.AddMessage("You have finished current version of Kesha Game!", Color.cyan);
            yield return new WaitForSeconds(2.5f);
            S.Console.AddMessage($"Developing still in progress. Current version is {S.GameVersionPlacer._version}.", Color.cyan);
            yield return new WaitForSeconds(2.5f);
            S.Console.AddMessage($"You can suggest anything to vk.com/RikaImbanika", Color.cyan);
            yield return new WaitForSeconds(2.5f);
            S.Console.AddMessage($"Game has console and many sometimes funny commands.", Color.cyan);
            yield return new WaitForSeconds(2.5f);
            S.Console.AddMessage($"You can teleport, summon enemies, change fog, get infinite items.", Color.cyan);
            yield return new WaitForSeconds(2.5f);
            S.Console.AddMessage($"GitHub: GitHub.com/RikaImbanika/KESHA-GAME", Color.cyan);
            yield return new WaitForSeconds(3f);
            S.Console.AddMessage($"Thanks for playing. Pls return when I add anything new.", Color.cyan);
        }
    }
}