using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class II : MonoBehaviour
{
    Dictionary<string, ItemInfo> _base;

    public void Start()
    {
        _base = new Dictionary<string, ItemInfo>();

        _base.Add("PurpleBall", new ItemInfo(
            spriteName: "PurpleBall",
            prefabName: "PurpleBall",
            throwable: true
            ));

        _base.Add("BlueBall", new ItemInfo(
            spriteName: "BlueBall",
            prefabName: "BlueBall",
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
