// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrongWay : MonoBehaviour
{
    GameObject _signsHolder;
    List<GameObject> _signs;
    int[] _signsOrder;
    int _signCounter;
    int _globalSignCounter;
    string[] _wrongWays;
    int[] _order;
    string _sceneName;


    void Start()
    {
        _sceneName = gameObject.scene.name;        
        _signs = new List<GameObject>();

        _wrongWays = new string[] 
        {"Oh No", 
        "Wrong",
        "Wrong 2"
        };

        _order = new int[]
        {
            0, 1, 2, 0, 2, 1, 2, 0, 1
        };

        int signsCount = 27;

        _signsOrder = new int[signsCount];

        for (int i = 0; i < signsCount; i++)
            _signsOrder[i] = i + 1;

        S.AllFather.Shuffle(_signsOrder);

        _signsHolder = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _signsHolder.name = "WrongWayHolder";
        _signsHolder.transform.position += new Vector3(0, -20, 0);
        Transform root = S.Loader.Roots[_sceneName];
        _signsHolder.transform.SetParent(root, true);
    }

    int _num = 0;

    private string GetAudio()
    {
        string res = _wrongWays[_order[_num]];
        _num++;
        if (_num >= _order.Length)
            _num = 0;
        return res;
    }

    private void OnTriggerEnter(Collider collider)
	{
        if (collider.gameObject.tag == "Player")
        {
            Transform canvasTransform = S.CanvasObj.transform;
            Transform goTransform = canvasTransform.Find("WrongWayLabel");
            GameObject label = goTransform.gameObject;

            _signsHolder.SetActive(true);

            string audioName = GetAudio();

            float pitch = 1.25f + (float)S.RND.NextDouble() * 0.1f;
            S.AM.Play(audioName, pitch);

            StartCoroutine(AsyncPart(label));
        }
    }

    private IEnumerator AsyncPart(GameObject label)
    {
        yield return new WaitForSeconds(0.1f);

        label.SetActive(true);

        yield return new WaitForSeconds(0.15f);

        GameObject sign = Instantiate(S.InventoryPlane, _signsHolder.transform);

        Transform ict = S.Intercam.transform;
        sign.transform.position = ict.position + ict.forward * (30f - 0.001f * _globalSignCounter);
        sign.transform.rotation = Quaternion.LookRotation(-ict.forward);

        sign.transform.localScale *= 0.20f + (float)S.RND.NextDouble() * 0.6f;
        sign.transform.position += 29 * ict.right * ((float)S.RND.NextDouble() - 0.5f);
        sign.transform.position += 14 * ict.up * ((float)S.RND.NextDouble() - 0.5f);

        sign.transform.Rotate(0, 0, ((float)S.RND.NextDouble() - 0.5f) * 80f);

        Material mat = new Material(Shader.Find("Custom/AlphaUnlitSingleSideWithAlphaMultiplier"));
        mat.mainTexture = Resources.Load<Texture2D>($"Textures/Wrong Way/Wrong Way {_signsOrder[_signCounter]}");
        sign.GetComponent<MeshRenderer>().material = mat;
        _signs.Add(sign);

        float pitch = Random.Range(0.95f, 1.05f);
        S.AudioManager.Play("Kick Metal 1");

        _signCounter++;
        _globalSignCounter++;

        if (_signCounter >= _signsOrder.Length)
        {
            _signCounter = 0;
            S.AllFather.Shuffle(_signsOrder);
        }

        yield return new WaitForSeconds(1f);

        label.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        _signsHolder.SetActive(false);
    }
}
