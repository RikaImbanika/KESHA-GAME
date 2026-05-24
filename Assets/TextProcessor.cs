using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TextProcessor : MonoBehaviour
{
    void Start()
    {
        S.TextProcessor = this;
    }

    // "ThatString" to "that_string"
    public string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        var sb = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (char.IsUpper(c) && i > 0 && char.IsLower(input[i - 1]))
                sb.Append('_');
            sb.Append(char.ToLower(c));
        }
        return sb.ToString();
    }

    // "ThatString" to "That_String"
    public string ToPascalSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        var sb = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (char.IsUpper(c) && i > 0 && char.IsLower(input[i - 1]))
                sb.Append('_');
            sb.Append(c);
        }
        return sb.ToString();
    }

    public string ConvertCyrillicToLatinLayout(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        const string rusLower = "йцукенгшщзхъфывапролджэячсмитьбю";
        const string latLower = "qwertyuiop[]asdfghjkl;'zxcvbnm,.";

        string rusUpper = rusLower.ToUpper();
        string latUpper = latLower.ToUpper();

        char[] result = new char[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            int idx = rusLower.IndexOf(c);
            if (idx >= 0)
            {
                result[i] = latLower[idx];
            }
            else
            {
                idx = rusUpper.IndexOf(c);
                if (idx >= 0)
                    result[i] = latUpper[idx];
                else
                    result[i] = c;
            }
        }
        return new string(result);
    }
}
