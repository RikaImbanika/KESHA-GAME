using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushrooms : MonoBehaviour
{
    public List<string> _allRoomsList;
    public Dictionary<string, float> _lightersProbabilities;

    void Start()
    {
        StartCoroutine(Yep());

        IEnumerator Yep()
        {
            _lightersProbabilities = new Dictionary<string, float>();

            S.Mushrooms = this;

            FillAllRoomsList();

            yield return new WaitForSeconds(1);

            SummonLighters();

            yield return null;
        }
    }

    void Update()
    {

    }

    void SummonLighters()
    {
        List<float> probs = new List<float>
        {
            100f,
            70f,
            50f,
            25f,
            10f,
            5f,
            1f,
            0f
        };

        List<float> counts = new List<float>
        {
            5f,
            6f,
            7f,
            5f,
            8f,
            24f,
            30f,
            15f
        };

        float[] array = new float[100];

        int j = 0;
        int m = 0;
        for (int i = 0; i < 100; i++)
        {
            array[i] = probs[j];
            m++;
            if (m > counts[j])
            {
                j++;
                m = 0;
            }
        }

        for (int i = 0; i < _allRoomsList.Count; i++)
        {
            int d = S.RND.Next(100);
            _lightersProbabilities.Add(_allRoomsList[i], array[d]);
        }

        //

        _lightersProbabilities["MR 2"] = 10f;
        _lightersProbabilities["MR 3"] = 20f;
        _lightersProbabilities["MR 4"] = 30f;
    }

    void FillAllRoomsList()
    {
        _allRoomsList = new List<string>();

        _allRoomsList.Add("MR 1");
        _allRoomsList.Add("MR 2");
        _allRoomsList.Add("MR 3");
        _allRoomsList.Add("MR 4");
    }
}