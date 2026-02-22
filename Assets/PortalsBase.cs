using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalsBase : MonoBehaviour
{
    private Dictionary<string, Portal> _portals;

    public Dictionary<string, Portal> Portals
    {
        get
        {
            return _portals;
        }
        set
        {
            _portals = value;
        }
    }

    void Start()
    {
        _portals = new Dictionary<string, Portal>();
        S.PortalsBase = this;
    }
}