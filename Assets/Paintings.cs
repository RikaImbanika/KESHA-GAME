using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paintings : MonoBehaviour
{
    public string[] _names;

    void Start()
    {
        _names = new string[]
        {
            "YouAreVase",
            "Remember",
            "Tokyo",
            "Paris",
            "SoManyVases",
            "Palms"
        };

        S.Paintings = this;
    }
}
