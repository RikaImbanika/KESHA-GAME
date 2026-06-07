// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    private List<string> _types;
    
    public List<string> Types
    {
        get
        {
            return _types;
        }
        set
        {
            _types = value;
        }
    }

    public byte TypeN(string name)
    {
        return (byte)_types.IndexOf(name);
    }

    void Start()
    {
        StartCoroutine(Yep());

        IEnumerator Yep()
        {
            _types = new List<string>();
            _types.Add("Zombella");
            _types.Add("Baka");
            _types.Add("Musculus");
            _types.Add("Ghost");
            _types.Add("Spider");

            S.Enemies = this;

            yield return null;
        }
    }

    public EnemyParams GetEnemyParams(string name)
    {
        EnemyParams ep = new EnemyParams();

        if (name == "zombella")
        {
            ep._screamerX = 0;
            ep._screamerY = -3.9f;
            ep._screamerZ = 0.60f;
            ep._screamerSounds = new string[] { "Screamer 1", "Screamer 2", "Screamer 3", "Screamer 4", "Screamer 5", "Screamer 6", "Screamer 7" };
        }
        else if (name == "baka")
        {
            ep._screamerX = 0;
            ep._screamerY = -4.2f;
            ep._screamerZ = 0.65f;
            ep._screamerSounds = new string[] { "Screamer 1", "Screamer 2", "Screamer 3", "Screamer 4", "Screamer 5", "Screamer 6", "Screamer 7" };
        }
        else if (name == "musculus")
        {
            ep._screamerX = 0;
            ep._screamerY = -4.6f;
            ep._screamerZ = 2.7f;
            ep._screamerSounds = new string[] { "Screamer 1", "Screamer 2", "Screamer 3", "Screamer 4", "Screamer 5", "Screamer 6", "Screamer 7" };
        }
        else if (name == "ghost")
        {
            ep._screamerX = 0;
            ep._screamerY = -3;
            ep._screamerZ = 1.5f;
            ep._screamerSounds = new string[] { "Screamer 1", "Screamer 2", "Screamer 3", "Screamer 4", "Screamer 5", "Screamer 6", "Screamer 7" };
        }

        return ep;
    }
}
