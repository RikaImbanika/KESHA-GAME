// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fireflies : MonoBehaviour
{
    public float[] _firefliesSizes;
    public string[] _firefliesColors;
    public string[] _zombieFirefliesColors;
    public Dictionary<string, bool> _canMirror;
    public Dictionary<string, Material> _materials;
    private Dictionary<string, byte> _colorN;

    public Dictionary<string, byte> ColorN
    {
        get
        {
            return _colorN;
        }
        set
        {
            _colorN = value;
        }
    }

    void Start()
    {
        StartCoroutine(Yep());

        IEnumerator Yep()
        {
            _firefliesSizes = new float[]
            {
                1f,
                0.7f,
                0.5f,
                0.35f
            };

            _firefliesColors = new string[]
            {
                "Yellow",
                "Red",
                "Blue",
                "Purple",
                "Green",
                "RainbowSlow",
                "RainbowFast",
                "Zombella",
                "Baka",
                "FatZombella",
                "FatBaka",
                "Illuminaty",
                "Kuplinov1",
                "Kuplinov2",
                "Boykisser1",
                "Boykisser2"
            };

            _zombieFirefliesColors = new string[]
            {
                "Zombella",
                "Baka",
                "FatZombella",
                "FatBaka",
                "Illuminaty",
                "Kuplinov1",
                "Kuplinov2",
                "Boykisser1",
                "Boykisser2"
            };

            _canMirror = new Dictionary<string, bool>();
            _canMirror.Add("Zombella", true);
            _canMirror.Add("Baka", true);
            _canMirror.Add("FatZombella", true);
            _canMirror.Add("FatBaka", true);
            _canMirror.Add("Illuminaty", true);
            _canMirror.Add("Kuplinov1", false);
            _canMirror.Add("Kuplinov2", false);
            _canMirror.Add("Boykisser1", true);
            _canMirror.Add("Boykisser2", true);

            _colorN = new Dictionary<string, byte>();

            _materials = new Dictionary<string, Material>();

            for (byte a = 0; a < _firefliesColors.Count(); a++)
            {
                string colName = _firefliesColors[a];
                _colorN.Add(colName, a);

                if (!IsZombieFirefly(colName))
                    _materials.Add(colName, Materials.Get($"Sparkles/Normal/Sparkle{colName}"));
                else
                    _materials.Add(colName, Materials.Get($"FlyingEnemies/{colName}"));
            }

            S.Fireflies = this;

            yield return null;
        }
    }

    public bool IsZombieFirefly(string color)
    {
        return _zombieFirefliesColors.Contains(color);
    }
}
