using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class II : MonoBehaviour
{
    //* Item Info

    Dictionary<string, ItemInfo> _base;

    public void Start()
    {
        S.II = this;

        _base = new Dictionary<string, ItemInfo>();

        _base.Add("PurpleBall", new ItemInfo(
            spriteName: "CoinTex",
            prefabName: "PurpleBall",
            throwable: true
            ));

        _base.Add("BlueBall", new ItemInfo(
            spriteName: "CoinTex",
            prefabName: "BlueBall",
            throwable: true
            ));

        _base.Add("FirstAidKit", new ItemInfo(
            spriteName: "FirstAidKit",
            prefabName: "FirstAidKit",
            throwable: true
            ));
    }

    public ItemInfo Get(string name)
	{
        if (!_base.ContainsKey(name))
            _base.Add(name, new ItemInfo(
            spriteName: name,
            prefabName: name,
            throwable: false
            ));

        return _base[name];
	}
}
