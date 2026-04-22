using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Lighters : MonoBehaviour
{
    public float[] _lightersSizes;
    public string[] _lightersColors;
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
            _lightersSizes = new float[]
            {
                1f,
                0.7f,
                0.5f,
                0.35f
            };

            _lightersColors = new string[]
            {
                "Yellow",
                "Red",
                "Blue",
                "Purple",
                "Green",
                "RainbowSlow",
                "RainbowFast",
                "Zombella",
                "Bakalavrus"
            };

            _colorN = new Dictionary<string, byte>();

            _materials = new Dictionary<string, Material>();

            for (byte a = 0; a < _lightersColors.Count(); a++)
            {
                string colName = _lightersColors[a];
                _colorN.Add(colName, a);

                if (colName != "Zombella" && colName != "Bakalavrus")
                    _materials.Add(colName, Materials.Get($"Sparkles/Normal/Sparkle{colName}"));
                else
                    _materials.Add(colName, Materials.Get($"FlyingEnemies/{colName}"));
            }

            S.Lighters = this;

            yield return null;
        }
    }
}
