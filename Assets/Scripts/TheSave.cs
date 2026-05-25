using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class TheSave : MonoBehaviour
{
    [SerializeField] private List<string> stringKeys = new List<string>();
    [SerializeField] private List<string?> stringValues = new List<string?>();

    [SerializeField] private List<string> boolKeys = new List<string>();
    [SerializeField] private List<bool?> boolValues = new List<bool?>();

    [SerializeField] private List<string> floatKeys = new List<string>();
    [SerializeField] private List<float?> floatValues = new List<float?>();

    [SerializeField] private List<string> intKeys = new List<string>();
    [SerializeField] private List<int?> intValues = new List<int?>();

    [SerializeField] private List<string> vector3Keys = new List<string>();
    [SerializeField] private List<Vector3?> vector3Values = new List<Vector3?>();

    [SerializeField] private List<string> listStringKeys = new List<string>();
    [SerializeField] private List<List<string>?> listStringValues = new List<List<string>?>();

    [SerializeField] private List<string> listFloatKeys = new List<string>();
    [SerializeField] private List<List<float>?> listFloatValues = new List<List<float>?>();

    [SerializeField] private List<string> listIntKeys = new List<string>();
    [SerializeField] private List<List<int>?> listIntValues = new List<List<int>?>();

    [SerializeField] private List<string> listBoolKeys = new List<string>();
    [SerializeField] private List<List<bool>?> listBoolValues = new List<List<bool>?>();

    [SerializeField] private List<string> quaternionKeys = new List<string>();
    [SerializeField] private List<Quaternion?> quaternionValues = new List<Quaternion?>();

    [SerializeField] private List<string> byteKeys = new List<string>();
    [SerializeField] private List<byte?> byteValues = new List<byte?>();

    [SerializeField] private List<string> longKeys = new List<string>();
    [SerializeField] private List<long?> longValues = new List<long?>();

    public void RemoveBool(string key) => RemoveGeneric(key, boolKeys, boolValues);
    public void RemoveString(string key) => RemoveGeneric(key, stringKeys, stringValues);
    public void RemoveFloat(string key) => RemoveGeneric(key, floatKeys, floatValues);
    public void RemoveInt(string key) => RemoveGeneric(key, intKeys, intValues);
    public void RemoveVector3(string key) => RemoveGeneric(key, vector3Keys, vector3Values);
    public void RemoveListString(string key) => RemoveGeneric(key, listStringKeys, listStringValues);
    public void RemoveListFloat(string key) => RemoveGeneric(key, listFloatKeys, listFloatValues);
    public void RemoveListInt(string key) => RemoveGeneric(key, listIntKeys, listIntValues);
    public void RemoveQuaternion(string key) => RemoveGeneric(key, quaternionKeys, quaternionValues);
    public void RemoveByte(string key) => RemoveGeneric(key, byteKeys, byteValues);
    public void RemoveLong(string key) => RemoveGeneric(key, longKeys, longValues);
    public void RemoveListBool(string key) => RemoveGeneric(key, listBoolKeys, listBoolValues);
  
    private void RemoveGeneric<T>(string key, List<string> keys, List<T> values)
    {
        int index = keys.IndexOf(key);
        if (index == -1) return;

        keys.RemoveAt(index);
        values.RemoveAt(index);
    }

    public void SaveBool(string key, bool? value) => SaveGeneric(key, value, boolKeys, boolValues);
    public void SaveString(string key, string? value) => SaveGeneric(key, value, stringKeys, stringValues);
    public void SaveFloat(string key, float? value) => SaveGeneric(key, value, floatKeys, floatValues);
    public void SaveInt(string key, int? value) => SaveGeneric(key, value, intKeys, intValues);
    public void SaveVector3(string key, Vector3? value) => SaveGeneric(key, value, vector3Keys, vector3Values);
    public void SaveListString(string key, List<string>? value) => SaveGeneric(key, value, listStringKeys, listStringValues);
    public void SaveListFloat(string key, List<float>? value) => SaveGeneric(key, value, listFloatKeys, listFloatValues);
    public void SaveListInt(string key, List<int>? value) => SaveGeneric(key, value, listIntKeys, listIntValues);
    public void SaveQuaternion(string key, Quaternion? value) => SaveGeneric(key, value, quaternionKeys, quaternionValues);
    public void SaveByte(string key, byte? value) => SaveGeneric(key, value, byteKeys, byteValues);
    public void SaveLong(string key, long? value) => SaveGeneric(key, value, longKeys, longValues);
    public void SaveListBool(string key, List<bool>? value) => SaveGeneric(key, value, listBoolKeys, listBoolValues);
    private void SaveGeneric<T>(string key, T value, List<string> keys, List<T> values)
    {
        int index = keys.IndexOf(key);
        if (index == -1)
        {
            keys.Add(key);
            values.Add(value);
        }
        else
        {
            values[index] = value;
        }
    }

#nullable enable
    public bool? LoadBool(string key) => LoadGeneric(key, boolKeys, boolValues);
    public string? LoadString(string key) => LoadGeneric(key, stringKeys, stringValues);
    public float? LoadFloat(string key) => LoadGeneric(key, floatKeys, floatValues);
    public int? LoadInt(string key) => LoadGeneric(key, intKeys, intValues);
    public Vector3? LoadVector3(string key) => LoadGeneric(key, vector3Keys, vector3Values);
    public List<string>? LoadListString(string key) => LoadGeneric(key, listStringKeys, listStringValues);
    public List<float>? LoadListFloat(string key) => LoadGeneric(key, listFloatKeys, listFloatValues);
    public List<int>? LoadListInt(string key) => LoadGeneric(key, listIntKeys, listIntValues);
    public Quaternion? LoadQuaternion(string key) => LoadGeneric(key, quaternionKeys, quaternionValues);
    public byte? LoadByte(string key) => LoadGeneric(key, byteKeys, byteValues);
    public long? LoadLong(string key) => LoadGeneric(key, longKeys, longValues);
    public List<bool>? LoadListBool(string key) => LoadGeneric(key, listBoolKeys, listBoolValues);
    private string? LoadGeneric(string key, List<string> keys, List<string?> values)
    {
        int index = keys.IndexOf(key);
        if (index == -1) return null;
        return values[index];
    }

    private T? LoadGeneric<T>(string key, List<string> keys, List<T?> values) where T : struct
    {
        int index = keys.IndexOf(key);
        if (index == -1) return null;
        return values[index];
    }

    private List<T>? LoadGeneric<T>(string key, List<string> keys, List<List<T>?> values)
    {
        int index = keys.IndexOf(key);
        if (index == -1) return new List<T>();
        return values[index];
    }


    public void AddToList<T>(string listKey, T value)
    {
        switch (value)
        {
            case string strVal:
                UpdateList(listKey, strVal, LoadListString, SaveListString);
                break;

            case int intVal:
                UpdateList(listKey, intVal, LoadListInt, SaveListInt);
                break;

            case float floatVal:
                UpdateList(listKey, floatVal, LoadListFloat, SaveListFloat);
                break;

            case bool boolVal:
                UpdateList(listKey, boolVal, LoadListBool, SaveListBool);
                break;

            default:
                throw new NotSupportedException($"Type {typeof(T)} not supported");
        }
    }

    public void RemoveFromList<T>(string listKey, T value)
    {
        switch (value)
        {
            case string strVal:
                UpdateList(listKey, strVal, LoadListString, SaveListString, true);
                break;

            case int intVal:
                UpdateList(listKey, intVal, LoadListInt, SaveListInt, true);
                break;

            case float floatVal:
                UpdateList(listKey, floatVal, LoadListFloat, SaveListFloat, true);
                break;

            case bool boolVal:
                UpdateList(listKey, boolVal, LoadListBool, SaveListBool, true);
                break;

            default:
                throw new NotSupportedException($"Type {typeof(T)} not supported");
        }
    }

    private void UpdateList<T>(
        string listKey,
        T value,
        Func<string, List<T>> loadFunc,
        Action<string, List<T>> saveFunc,
        bool remove = false)
    {
        var list = loadFunc(listKey) ?? new List<T>();

        if (remove)
        {
            if (list.Remove(value))
                saveFunc(listKey, list);
        }
        else
        {
            if (!list.Contains(value))
            {
                list.Add(value);
                saveFunc(listKey, list);
            }
        }
    }

    public TheSave DeepCopy()
    {
        return new TheSave
        {
            stringKeys = new List<string>(stringKeys),
            stringValues = new List<string?>(stringValues),
            boolKeys = new List<string>(boolKeys),
            boolValues = new List<bool?>(boolValues),
            floatKeys = new List<string>(floatKeys),
            floatValues = new List<float?>(floatValues),
            intKeys = new List<string>(intKeys),
            intValues = new List<int?>(intValues),
            vector3Keys = new List<string>(vector3Keys),
            vector3Values = new List<Vector3?>(vector3Values),
            quaternionKeys = new List<string>(quaternionKeys),
            quaternionValues = new List<Quaternion?>(quaternionValues),
            byteKeys = new List<string>(byteKeys),
            byteValues = new List<byte?>(byteValues),
            longKeys = new List<string>(longKeys),
            longValues = new List<long?>(longValues),

            listStringKeys = new List<string>(listStringKeys),
            listStringValues = listStringValues
                .Select(list => list != null ? new List<string>(list) : null)
                .ToList(),

            listFloatKeys = new List<string>(listFloatKeys),
            listFloatValues = listFloatValues
                .Select(list => list != null ? new List<float>(list) : null)
                .ToList(),

            listIntKeys = new List<string>(listIntKeys),
            listIntValues = listIntValues
                .Select(list => list != null ? new List<int>(list) : null)
                .ToList(),

            listBoolKeys = new List<string>(listBoolKeys),
            listBoolValues = listBoolValues
                    .Select(list => list != null ? new List<bool>(list) : null)
                    .ToList()
        };
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("> Strings:");

        if (stringKeys.Count > 0)
            for (int i = 0; i < stringKeys.Count; i++)
                sb.AppendLine($"  {stringKeys[i]}: {stringValues[i] ?? "null"}");

        sb.AppendLine("> Bools:");

        if (boolKeys.Count > 0)
            for (int i = 0; i < boolKeys.Count; i++)
                sb.AppendLine($"  {boolKeys[i]}: {boolValues[i].ToString() ?? "null"}");

        sb.AppendLine("> Floats:");

        if (floatKeys.Count > 0)
            for (int i = 0; i < floatKeys.Count; i++)
                sb.AppendLine($"  {floatKeys[i]}: {floatValues[i].ToString() ?? "null"}");

        sb.AppendLine("> Ints:");

        if (intKeys.Count > 0)
            for (int i = 0; i < intKeys.Count; i++)
                sb.AppendLine($"  {intKeys[i]}: {intValues[i].ToString() ?? "null"}");

        sb.AppendLine("> Vector3:");

        if (vector3Keys.Count > 0)
            for (int i = 0; i < vector3Keys.Count; i++)
                sb.AppendLine($"  {vector3Keys[i]}: {vector3Values[i]?.ToString() ?? "null"}");

        sb.AppendLine("> ListStrings:");
        AppendListSection(sb, listStringKeys, listStringValues);

        sb.AppendLine("> ListFloats:");
        AppendListSection(sb, listFloatKeys, listFloatValues);

        sb.AppendLine("> ListInts:");
        AppendListSection(sb, listIntKeys, listIntValues);

        sb.AppendLine("> ListBools:");
        AppendListSection(sb, listBoolKeys, listBoolValues);

        sb.AppendLine("> Quaternions:");

        if (quaternionKeys.Count > 0)
            for (int i = 0; i < quaternionKeys.Count; i++)
                sb.AppendLine($"  {quaternionKeys[i]}: {quaternionValues[i]?.ToString() ?? "null"}");

        sb.AppendLine("> Bytes:");

        if (byteKeys.Count > 0)
            for (int i = 0; i < byteKeys.Count; i++)
                sb.AppendLine($"  {byteKeys[i]}: {byteValues[i]?.ToString() ?? "null"}");

        sb.AppendLine("> Longs:");

        if (longKeys.Count > 0)
            for (int i = 0; i < longKeys.Count; i++)
                sb.AppendLine($"  {longKeys[i]}: {longValues[i]?.ToString() ?? "null"}");

        return sb.ToString();
    }

    private void AppendListSection<TKey, TValue>(StringBuilder sb, List<TKey> keys, List<List<TValue>> values)
    {
        if (keys.Count > 0)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                var list = values != null && i < values.Count ? values[i] : null;
                sb.AppendLine($"  {keys[i]}: {(list != null ? FormatList(list) : "null")}");
            }
        }
    }

    private string FormatList<T>(List<T> list)
    {
        if (list == null) return "null";
        if (list.Count == 0) return "[]";
        return "[" + string.Join(", ", list) + "]";
    }
}