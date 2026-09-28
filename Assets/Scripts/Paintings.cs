// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paintings : MonoBehaviour
{
    public string[] _names;
    public bool[] _canMirror;
    public float[] _probabilities;
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
            "GetEarth",
            "Kuplinov",
            "BrokenVase",
            "DeepReader"
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
            false,
            false,
            true,
            true
        };

        _probabilities = new float[]
        {
            100,
            100,
            100,
            100,
            80,
            100,
            100,
            100,
            34,
            100,
            100
        };

        _phrases = new (string, bool)[]
        {
            ("Memes.", true),
            ("Why are you not a pony?", true),
            ("Deep fried.", true),
            ("Nonsense.", true),
            ("I hate entropy.", true),
            ("Oh no!", true),
            ("This is picture.", true),
            ("Triangles... Triangles everywhere!", true),
            ("Second replicator.", true),
            ("Guys, stop dying!", true),
            ("Everything.", true),
            ("More.", true),
            ("New content!", true),
            ("Pathetic.", true),
            ("Holy cow!", true),
            ("Welcome to hell!", true),
            ("Impossible...", true),
            ("You can do it!", true),
            ("Hello.", true),
            ("Die, stupied zombella!", true),
            ("O.M.G.", true),
            ("Ready, set, fish.", true),
            ("What?", true),
            ("Revolutionise gaming again!", true),
            ("I'm real!", true),
            ("Art.", true),
            ("Remember?", true),
            ("Saga about zombela.", true),
            ("Ah, yes, quality content!", true),
            ("Perfection.", true),
            ("Ideal.", true),
            ("Art of trash.", true),
            ("In begining was nothing. But then...", true),
            ("Again.", true),
            ("Order.", true),
            ("Entropy should die.", true),
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
