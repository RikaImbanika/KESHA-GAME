using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class II : object
{
    static Dictionary<string, ItemInfo> _base = new Dictionary<string, ItemInfo>();

    public static void Init()
    {
        _base.Add("PurpleBall", new ItemInfo(
            spriteName: "PurpleBall",
            prefabName: "PurpleBall"
            ));

        _base.Add("BlueBall", new ItemInfo(
            spriteName: "BlueBall",
            prefabName: "BlueBall"
            ));
    }

	public static ItemInfo Get(string name)
	{
		Debug.Log($"name {name}");
		return _base[name];
	}
}
