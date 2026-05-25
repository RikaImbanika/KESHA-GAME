using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushrooms : MonoBehaviour
{
    public List<string> _allRoomsList;
    public Dictionary<string, float> _firefliesProbabilities;

    void Start()
    {
        StartCoroutine(Yep());

        IEnumerator Yep()
        {
            _firefliesProbabilities = new Dictionary<string, float>();

            S.Mushrooms = this;

            FillAllRoomsList();

            yield return new WaitForSeconds(1);

            SummonFireflies();

            yield return null;
        }
    }

    void Update()
    {

    }

    void SummonFireflies()
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
            10f,
            20f,
            10f,
            8f,
            8f,
            30f,
            10f,
            4f
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
            _firefliesProbabilities.Add(_allRoomsList[i], array[d]);
        }

        /////////////////////////////

        _firefliesProbabilities["MR 2"] = 20f;
        _firefliesProbabilities["MR 3"] = 30f;
        _firefliesProbabilities["MR 4"] = 40f;
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