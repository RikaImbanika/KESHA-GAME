using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[System.Serializable]
public class Save
{
    public string _name;
    public Vector3 _position;
    public Quaternion _rotation;
    public float _health;
    public float _value;
    public bool _bool;
    public bool _hidden;
    public bool _destroyed;
    public bool _pressed;
    public bool _opened;
    public bool _locked;
    public bool _cheated;
    public int _count;
    public string _scene;
    public bool _unnatural;
    public List<string> _strings;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        if (!string.IsNullOrEmpty(_name))
            sb.AppendLine("_name: " + _name);

        if (_position != Vector3.zero)
            sb.AppendLine("_position: " + _position);

        if (_rotation != Quaternion.identity)
            sb.AppendLine("_rotation: " + _rotation);

        if (_health != 0)
            sb.AppendLine("_health: " + _health);

        if (_value != 0)
            sb.AppendLine("_value: " + _value);

        if (_bool != false)
            sb.AppendLine("_bool: " + _bool);

        if (_hidden != false)
            sb.AppendLine("_hidden: " + _hidden);

        if (_destroyed != false)
            sb.AppendLine("_destroyed: " + _destroyed);

        if (_pressed != false)
            sb.AppendLine("_pressed: " + _pressed);

        if (_opened != false)
            sb.AppendLine("_opened: " + _opened);

        if (_locked != false)
            sb.AppendLine("_locked: " + _locked);

        if (_cheated != false)
            sb.AppendLine("_cheated: " + _cheated);

        if (_count != 0)
            sb.AppendLine("_count: " + _count);

        if (!string.IsNullOrEmpty(_scene))
            sb.AppendLine("_scene: " + _scene);

        if (_unnatural != false)
            sb.AppendLine("_unnatural: " + _unnatural);

        if (_strings != null)
            if (_strings.Count > 0)
            {
                sb.AppendLine("_strings:");
                foreach (string s in _strings)
                {
                    sb.AppendLine("  " + s);
                }
            }

        return sb.ToString();
    }

    public Save()
    {
    }

    public Save(bool value)
    {
        _bool = value;
    }

    public Save(float value)
    {
        _value = value;
    }

    public Save(Vector3 value)
    {
        _position = value;
    }
}
