using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class II : MonoBehaviour
{
    //* Item Info

    Dictionary<string, ItemInfo> _base;
    Dictionary<string, string> _aliases;

    public void Start()
    {
        StartCoroutine(LateStart());

        IEnumerator LateStart()
        {
            while (S.TextProcessor == null)
                yield return new WaitForSeconds(0.25f);

            _base = new Dictionary<string, ItemInfo>();

            _base.Add("PurpleBall", new ItemInfo(
             inventoryNameColor: Color.white,
             lookNameColor: Color.white,
             spriteName: "Balls/PurpleBall",
             prefabName: "Balls/PurpleBall",
             visibleName: "Purple ball",
             throwable: true
            ));

            _base.Add("BlueBall", new ItemInfo(
                inventoryNameColor: Color.white,
                lookNameColor: Color.white,
                spriteName: "Balls/BlueBall",
                prefabName: "Balls/BlueBall",
                visibleName: "Blue ball",
                throwable: true
            ));

            _base.Add("CyanBall", new ItemInfo(
            inventoryNameColor: Color.white,
            lookNameColor: Color.white,
            spriteName: "Balls/CyanBall",
            prefabName: "Balls/CyanBall",
            visibleName: "Cyan ball",
            throwable: true
            ));

            _base.Add("YellowBall", new ItemInfo(
            inventoryNameColor: Color.yellow,
            lookNameColor: Color.yellow,
            spriteName: "Balls/YellowBall",
            prefabName: "Balls/YellowBall",
            visibleName: "Yellow ball",
            throwable: true
            ));

            _base.Add("RedBall", new ItemInfo(
            inventoryNameColor: Color.red,
            lookNameColor: Color.red,
            spriteName: "Balls/RedBall",
            prefabName: "Balls/RedBall",
            visibleName: "Red ball",
            throwable: true
            ));

            _base.Add("WhiteBall", new ItemInfo(
            inventoryNameColor: Color.white,
            lookNameColor: Color.white,
            spriteName: "Balls/WhiteBall",
            prefabName: "Balls/WhiteBall",
            visibleName: "White ball",
            throwable: true
            ));

            _base.Add("NatureBall1", new ItemInfo(
            inventoryNameColor: Color.green,
            lookNameColor: Color.green,
            spriteName: "Balls/NatureBall 1",
            prefabName: "Balls/NatureBall 1",
            visibleName: "Nature ball",
            throwable: true
            ));

            _base.Add("NatureBall2", new ItemInfo(
            inventoryNameColor: Color.green,
            lookNameColor: Color.green,
            spriteName: "Balls/NatureBall 2",
            prefabName: "Balls/NatureBall 2",
            visibleName: "Nature ball",
            throwable: true
            ));

            _base.Add("NatureBall3", new ItemInfo(
            inventoryNameColor: Color.green,
            lookNameColor: Color.green,
            spriteName: "Balls/NatureBall 3",
            prefabName: "Balls/NatureBall 3",
            visibleName: "Nature ball",
            throwable: true
            ));

            _base.Add("NatureBall4", new ItemInfo(
            inventoryNameColor: Color.green,
            lookNameColor: Color.green,
            spriteName: "Balls/NatureBall 4",
            prefabName: "Balls/NatureBall 4",
            visibleName: "Nature ball",
            throwable: true
            ));

            _base.Add("NatureBall5", new ItemInfo(
            inventoryNameColor: Color.green,
            lookNameColor: Color.green,
            spriteName: "Balls/NatureBall 5",
            prefabName: "Balls/NatureBall 5",
            visibleName: "Nature ball",
            throwable: true
            ));

            _base.Add("NatureBall6", new ItemInfo(
            inventoryNameColor: Color.green,
            lookNameColor: Color.green,
            spriteName: "Balls/NatureBall 6",
            prefabName: "Balls/NatureBall 6",
            visibleName: "Nature ball",
            throwable: true
            ));

            _base.Add("NatureBall7", new ItemInfo(
            inventoryNameColor: Color.green,
            lookNameColor: Color.green,
            spriteName: "Balls/NatureBall 7",
            prefabName: "Balls/NatureBall 7",
            visibleName: "Nature ball",
            throwable: true
            ));

            _base.Add("NatureBall8", new ItemInfo(
            inventoryNameColor: Color.green,
            lookNameColor: Color.green,
            spriteName: "Balls/NatureBall 8",
            prefabName: "Balls/NatureBall 8",
            visibleName: "Nature ball",
            throwable: true
            ));

            _base.Add("IceBall1", new ItemInfo(
            inventoryNameColor: Color.cyan,
            lookNameColor: Color.cyan,
            spriteName: "Balls/IceBall 1",
            prefabName: "Balls/IceBall 1",
            visibleName: "Ice ball",
            throwable: true
            ));

            _base.Add("IceBall2", new ItemInfo(
            inventoryNameColor: Color.cyan,
            lookNameColor: Color.cyan,
            spriteName: "Balls/IceBall 2",
            prefabName: "Balls/IceBall 2",
            visibleName: "Ice ball",
            throwable: true
            ));

            _base.Add("IceBall3", new ItemInfo(
            inventoryNameColor: Color.cyan,
            lookNameColor: Color.cyan,
            spriteName: "Balls/IceBall 3",
            prefabName: "Balls/IceBall 3",
            visibleName: "Ice ball",
            throwable: true
            ));

            _base.Add("IceBall4", new ItemInfo(
            inventoryNameColor: Color.cyan,
            lookNameColor: Color.cyan,
            spriteName: "Balls/IceBall 4",
            prefabName: "Balls/IceBall 4",
            visibleName: "Ice ball",
            throwable: true
            ));

            _base.Add("IceBall5", new ItemInfo(
            inventoryNameColor: Color.cyan,
            lookNameColor: Color.cyan,
            spriteName: "Balls/IceBall 5",
            prefabName: "Balls/IceBall 5",
            visibleName: "Ice ball",
            throwable: true
            ));

            _base.Add("IceBall6", new ItemInfo(
            inventoryNameColor: Color.cyan,
            lookNameColor: Color.cyan,
            spriteName: "Balls/IceBall 6",
            prefabName: "Balls/IceBall 6",
            visibleName: "Ice ball",
            throwable: true
            ));

            _base.Add("SilentBall1", new ItemInfo(
            inventoryNameColor: new Color(0.4f, 0.16f, 1f),
            lookNameColor: new Color(0.4f, 0.16f, 1f),
            spriteName: "Balls/SilentBall 1",
            prefabName: "Balls/SilentBall 1",
            visibleName: "Silent ball",
            throwable: true
            ));

            _base.Add("SilentBall2", new ItemInfo(
            inventoryNameColor: new Color(0.4f, 0.16f, 1f),
            lookNameColor: new Color(0.4f, 0.16f, 1f),
            spriteName: "Balls/SilentBall 2",
            prefabName: "Balls/SilentBall 2",
            visibleName: "Silent ball",
            throwable: true
            ));

            _base.Add("SilentBall3", new ItemInfo(
            inventoryNameColor: new Color(0.4f, 0.16f, 1f),
            lookNameColor: new Color(0.4f, 0.16f, 1f),
            spriteName: "Balls/SilentBall 3",
            prefabName: "Balls/SilentBall 3",
            visibleName: "Silent ball",
            throwable: true
            ));

            _base.Add("SilentBall4", new ItemInfo(
            inventoryNameColor: new Color(0.4f, 0.16f, 1f),
            lookNameColor: new Color(0.4f, 0.16f, 1f),
            spriteName: "Balls/SilentBall 4",
            prefabName: "Balls/SilentBall 4",
            visibleName: "Silent ball",
            throwable: true
            ));

            _base.Add("SilentBall5", new ItemInfo(
            inventoryNameColor: new Color(0.4f, 0.16f, 1f),
            lookNameColor: new Color(0.4f, 0.16f, 1f),
            spriteName: "Balls/SilentBall 5",
            prefabName: "Balls/SilentBall 5",
            visibleName: "Silent ball",
            throwable: true
            ));

            _base.Add("SilentBall6", new ItemInfo(
            inventoryNameColor: new Color(0.4f, 0.16f, 1f),
            lookNameColor: new Color(0.4f, 0.16f, 1f),
            spriteName: "Balls/SilentBall 6",
            prefabName: "Balls/SilentBall 6",
            visibleName: "Silent ball",
            throwable: true
            ));

            _base.Add("FirstAidKit", new ItemInfo(
                inventoryNameColor: new Color(0.066f, 1f, 0.12f),
                lookNameColor: new Color(0.099f, 1f, 0.18f),
                spriteName: "FirstAidKit",
                prefabName: "FirstAidKit",
                visibleName: "First aid kit",
                throwable: false
            ));

            _base.Add("Gun", new ItemInfo(
                inventoryNameColor: Color.white,
                lookNameColor: Color.white,
                spriteName: "Gun",
                prefabName: "Gun",
                visibleName: "Gun",
                throwable: false
            ));

            _base.Add("Ammo", new ItemInfo(
                inventoryNameColor: Color.white,
                lookNameColor: Color.white,
                spriteName: "Ammo",
                prefabName: "Ammo",
                visibleName: "Ammo",
                throwable: false
            ));

            _base.Add("Apple", new ItemInfo(
                inventoryNameColor: Color.white,
                lookNameColor: Color.white,
                spriteName: "Apple",
                prefabName: "Apple",
                visibleName: "Apple",
                throwable: false
            ));

            _base.Add("Cucumber", new ItemInfo(
                inventoryNameColor: Color.white,
                lookNameColor: Color.white,
                spriteName: "Cucumber",
                prefabName: "Cucumber",
                visibleName: "Cucumber",
                throwable: false
            ));

            _base.Add("Frerard1", new ItemInfo(
                inventoryNameColor: new Color(0.4f, 0.16f, 1f),
                lookNameColor: new Color(0.4f, 0.16f, 1f),
                spriteName: "Frerard1",
                prefabName: "Frerard1",
                visibleName: "Picture part",
                throwable: false
            ));

            _base.Add("Frerard2", new ItemInfo(
                inventoryNameColor: new Color(0.4f, 0.16f, 1f),
                lookNameColor: new Color(0.4f, 0.16f, 1f),
                spriteName: "Frerard2",
                prefabName: "Frerard2",
                visibleName: "Picture part",
                throwable: false
            ));

            _base.Add("Frerard3", new ItemInfo(
                inventoryNameColor: new Color(0.4f, 0.16f, 1f),
                lookNameColor: new Color(0.4f, 0.16f, 1f),
                spriteName: "Frerard3",
                prefabName: "Frerard3",
                visibleName: "Picture part",
                throwable: false
            ));

            _base.Add("Frerard4", new ItemInfo(
                inventoryNameColor: new Color(0.4f, 0.16f, 1f),
                lookNameColor: new Color(0.4f, 0.16f, 1f),
                spriteName: "Frerard4",
                prefabName: "Frerard4",
                visibleName: "Picture part",
                throwable: false
            ));

            _base.Add("Frerard5", new ItemInfo(
                inventoryNameColor: new Color(0.4f, 0.16f, 1f),
                lookNameColor: new Color(0.4f, 0.16f, 1f),
                spriteName: "Frerard5",
                prefabName: "Frerard5",
                visibleName: "Picture part",
                throwable: false
            ));

            _base.Add("Frerard6", new ItemInfo(
                inventoryNameColor: new Color(0.4f, 0.16f, 1f),
                lookNameColor: new Color(0.4f, 0.16f, 1f),
                spriteName: "Frerard6",
                prefabName: "Frerard6",
                visibleName: "Picture part",
                throwable: false
            ));

            _base.Add("BlueKey", new ItemInfo(
                inventoryNameColor: Color.white,
                lookNameColor: Color.white,
                spriteName: "BlueKey",
                prefabName: "BlueKey",
                visibleName: "Blue key",
                throwable: false
            ));

            _base.Add("PurpleKey", new ItemInfo(
                inventoryNameColor: Color.white,
                lookNameColor: Color.white,
                spriteName: "PurpleKey",
                prefabName: "PurpleKey",
                visibleName: "Purple key",
                throwable: false
            ));

            _base.Add("RedKey", new ItemInfo(
                inventoryNameColor: Color.white,
                lookNameColor: Color.white,
                spriteName: "RedKey",
                prefabName: "RedKey",
                visibleName: "Red key",
                throwable: false
            ));

            _base.Add("GreenKey", new ItemInfo(
                inventoryNameColor: new Color(0.066f, 1f, 0.12f),
                lookNameColor: new Color(0.099f, 1f, 0.18f),
                spriteName: "GreenKey",
                prefabName: "GreenKey",
                visibleName: "Green key",
                throwable: false
            ));

            _base.Add("Crystal", new ItemInfo(
                inventoryNameColor: Color.magenta,
                lookNameColor: Color.magenta,
                spriteName: "Crystal",
                prefabName: "Crystal",
                visibleName: "Crystal",
                throwable: false
            ));

            _base.Add("RedCrystal", new ItemInfo(
                inventoryNameColor: Color.red,
                lookNameColor: Color.red,
                spriteName: "RedCrystal",
                prefabName: "RedCrystal",
                visibleName: "Red crystal",
                throwable: false
            ));

            _base.Add("Earth", new ItemInfo(
                inventoryNameColor: Color.white,
                lookNameColor: Color.white,
                spriteName: "Earth",
                prefabName: "Earth",
                visibleName: "Earth",
                throwable: true
            ));

            _base.Add("Moon", new ItemInfo(
                inventoryNameColor: Color.white,
                lookNameColor: Color.white,
                spriteName: "Moon",
                prefabName: "Moon",
                visibleName: "Moon",
                throwable: true
            ));

            _base.Add("Mars", new ItemInfo(
                inventoryNameColor: Color.white,
                lookNameColor: Color.white,
                spriteName: "Mars",
                prefabName: "Mars",
                visibleName: "Mars",
                throwable: true
            ));

            FillAliases();

            S.II = this;
        }
    }
    
    void FillAliases()
    {
        _aliases = new Dictionary<string, string>();

        foreach (string name in _base.Keys)
        {
            _base[name]._name = name;

            string a1 = name.ToLower();
            string a2 = S.TextProcessor.ToSnakeCase(name);
            string a3 = S.TextProcessor.ToPascalSnakeCase(name);

            if (!_aliases.ContainsKey(a1))
                _aliases.Add(a1, name);

            if (!_aliases.ContainsKey(a2))
                _aliases.Add(a2, name);

            if (!_aliases.ContainsKey(a3))
                _aliases.Add(a3, name);
        }
    }

    public ItemInfo Get(string name)
    {
        if (!_base.ContainsKey(name))
        {
            if (!_aliases.ContainsKey(name))
            {
                string spriteName = "Zombella";
                if (S.RND.Next(2) == 0)
                    spriteName = "Baka";

                _base.Add(name, new ItemInfo(
                inventoryNameColor: Color.white,
                lookNameColor: Color.white,
                spriteName: spriteName,
                prefabName: name,
                visibleName: name,
                throwable: false
                ));
                _base[name]._name = name;
            }
            else
                return _base[_aliases[name]];
        }

        return _base[name];
    }

    public string[] Names
    {
        get
        {
            return _base.Keys.ToArray<string>();
        }
    }
}
