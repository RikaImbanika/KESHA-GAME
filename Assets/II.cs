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
            prefabName: "Balls/PurpleBall 0",
            visibleName: "Purple ball",
            throwable: true
            ));

        _base.Add("BlueBall", new ItemInfo(
            spriteName: "CoinTex",
            prefabName: "BlueBall",
            visibleName: "Blue ball",
            throwable: true
            ));

        _base.Add("FirstAidKit", new ItemInfo(
            spriteName: "FirstAidKit",
            prefabName: "FirstAidKit",
            visibleName: "First aid kit",
            throwable: false
            ));

        _base.Add("Gun", new ItemInfo(
            spriteName: "Gun",
            prefabName: "Gun",
            visibleName: "Gun",
            throwable: false
            ));

        _base.Add("Apple", new ItemInfo(
            spriteName: "Apple",
            prefabName: "Apple",
            visibleName: "Apple",
            throwable: false
            ));

        _base.Add("Cucumber", new ItemInfo(
            spriteName: "Cucumber",
            prefabName: "Cucumber",
            visibleName: "Cucumber",
            throwable: false
            ));

        _base.Add("Frerard1", new ItemInfo(
            spriteName: "Frerard1",
            prefabName: "Frerard1",
            visibleName: "Picture part 1",
            throwable: false
            ));

        _base.Add("Frerard2", new ItemInfo(
            spriteName: "Frerard2",
            prefabName: "Frerard2",
            visibleName: "Picture part 2",
            throwable: false
            ));

        _base.Add("Frerard3", new ItemInfo(
            spriteName: "Frerard3",
            prefabName: "Frerard3",
            visibleName: "Picture part 3",
            throwable: false
            ));

        _base.Add("Frerard4", new ItemInfo(
            spriteName: "Frerard4",
            prefabName: "Frerard4",
            visibleName: "Picture part 4",
            throwable: false
            ));

        _base.Add("Frerard5", new ItemInfo(
            spriteName: "Frerard5",
            prefabName: "Frerard5",
            visibleName: "Picture part 5",
            throwable: false
            ));

        _base.Add("Frerard6", new ItemInfo(
            spriteName: "Frerard6",
            prefabName: "Frerard6",
            visibleName: "Picture part 6",
            throwable: false
            ));

        _base.Add("BlueKey", new ItemInfo(
            spriteName: "BlueKey",
            prefabName: "BlueKey",
            visibleName: "Blue key",
            throwable: false
            ));

        _base.Add("PurpleKey", new ItemInfo(
            spriteName: "PurpleKey",
            prefabName: "PurpleKey",
            visibleName: "Purple key",
            throwable: false
            ));

        _base.Add("RedKey", new ItemInfo(
            spriteName: "RedKey",
            prefabName: "RedKey",
            visibleName: "Red key",
            throwable: false
            ));

        _base.Add("GreenKey", new ItemInfo(
            spriteName: "GreenKey",
            prefabName: "GreenKey",
            visibleName: "Green key",
            throwable: false
            ));

        _base.Add("RedCrystal", new ItemInfo(
            spriteName: "RedCrystal",
            prefabName: "RedCrystal",
            visibleName: "Red crystal",
            throwable: false
            ));
    }

    public ItemInfo Get(string name)
	{
        if (!_base.ContainsKey(name))
            _base.Add(name, new ItemInfo(
            spriteName: name,
            prefabName: name,
            visibleName: name,
            throwable: false
            ));

        return _base[name];
	}
}
