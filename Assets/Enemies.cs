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
            _types.Add("Bakalavrus");
            _types.Add("Musculus");
            _types.Add("Ghost");
            _types.Add("Spider");

            S.Enemies = this;

            yield return null;
        }
    }

    
}
