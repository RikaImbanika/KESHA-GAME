using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighters : MonoBehaviour
{
    public float[] _lightersSizes;
    public string[] _lightersColors;
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
                "Blue"
            };

            _colorN = new Dictionary<string, byte>();
            _colorN.Add("Yellow", 0);
            _colorN.Add("Red", 1);
            _colorN.Add("Blue", 2);

            S.Lighters = this;

            yield return null;
        }
    }
}
