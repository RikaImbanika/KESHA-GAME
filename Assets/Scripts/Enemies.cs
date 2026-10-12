// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    private List<Zombie> _playerFollowers;

    private float _timer;

    private int _killsCounter;

    private List<string> _types;

    private List<string> _flatCeilingScenes;

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

    public bool FlatCeiling(string sceneName)
    {
        return _flatCeilingScenes.Contains(sceneName);
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

            _playerFollowers = new List<Zombie>();

            _flatCeilingScenes = new List<string>();
            _flatCeilingScenes.Add("Corridor");
            _flatCeilingScenes.Add("TL 0");
            _flatCeilingScenes.Add("BR 1");
            _flatCeilingScenes.Add("BR 1R");
            _flatCeilingScenes.Add("BR 2");
            _flatCeilingScenes.Add("BR 2R");
            _flatCeilingScenes.Add("BR 3");
            _flatCeilingScenes.Add("BR 3R");
            _flatCeilingScenes.Add("BR 4");
            _flatCeilingScenes.Add("BR 4R");
            _flatCeilingScenes.Add("BR 5");
            _flatCeilingScenes.Add("BR 6");
            _flatCeilingScenes.Add("BR 6R");
            _flatCeilingScenes.Add("BR 7");
            _flatCeilingScenes.Add("BR 7R");
            _flatCeilingScenes.Add("BR 8");
            _flatCeilingScenes.Add("MR 1");
            _flatCeilingScenes.Add("MR 2");
            _flatCeilingScenes.Add("MR 3");
            _flatCeilingScenes.Add("MR 4");

            S.Enemies = this;

            yield return null;
        }
    }

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer > 1f)
        {
            for (int i = 0; i < _playerFollowers.Count; i++)
            {
                if (_playerFollowers[i] == null)
                    Remove();
                else if (!_playerFollowers[i]._followPlayer)
                    Remove();
                else if (_playerFollowers[i]._health <= 0)
                    Remove();

                void Remove()
                {
                    _playerFollowers.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    public void FollowPlayer(Zombie zombie)
    {
        if (!_playerFollowers.Contains(zombie))
            _playerFollowers.Add(zombie);
    }

    public void UnfollowPlayer(Zombie zombie)
    {
        if (_playerFollowers.Contains(zombie))
            _playerFollowers.Remove(zombie);
    }

    public void SomebodyDies()
    {
        _killsCounter++;

        if (_playerFollowers.Count > 0)
        {
            if (_killsCounter > 7)
            {
                _killsCounter = 0;
                Zombie who = _playerFollowers.Last();
                string name = who._visibleName;
                S.Console.AddMessage($"{name}: Guys, stop dying!", new Color(1f, 0.2f, 0f));
            }
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
