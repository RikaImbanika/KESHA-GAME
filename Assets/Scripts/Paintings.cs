// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paintings : MonoBehaviour
{
    public string[] _names;
    public bool[] _canMirror;
    public (string, bool)[] _phrases;
    public List<string> _scenesTakePlainTextPainting;

    void Start()
    {
        _names = new string[]
        {
            "YouAreVase",
            "Remember",
            "Tokyo",
            "Paris",
            "SoManyVases",
            "Palms",
            "BeDifferent",
            "GetEarth"
        };

        _canMirror = new bool[]
        {
            false,
            false,
            true,
            true,
            false,
            true,
            false,
            false
        };

        _phrases = new (string, bool)[]
        {
            ("Nonsense.", true),
            ("I hate entropy.", true),
            ("Oh no!", true),
            ("This is picture.", true),
            ("Triangles... Triangles everywhere!", true),
            ("Memes.", true),
            ("Second replicator.", true),
            ("Guys, stop dying!", true),
            ("Deep fried.", true),
            ("Everything.", true),
            ("More.", true),
            ("Why are you not a pony?", true),
            ("New content.", true),
            ("Pathetic.", true),
            ("Holy cow!", true),
            ("Welcome to hell!", true),
            ("Impossible...", true),
            ("You can do it!", true)
        };

        _scenesTakePlainTextPainting = new List<string>();

        S.Paintings = this;
    }

    public int TryTakePhrase(string sceneName)
    {
        if (!_scenesTakePlainTextPainting.Contains(sceneName))
        {
            int index = S.RND.Next(_phrases.Length);

            int counter = 0;

            while (!_phrases[index].Item2 && counter < 300)
            {
                index = S.RND.Next(_phrases.Length);
                counter++;
            }

            if (counter >= 300)
                return -1;
            else
            {
                _phrases[index].Item2 = false;
                _scenesTakePlainTextPainting.Add(sceneName);
                return index;
            }
        }
        else
            return -1;
    }
}
