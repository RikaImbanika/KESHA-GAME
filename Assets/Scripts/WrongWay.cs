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
    Transform _ict;
    int _num = 0;

    Coroutine _hideCoroutine;

    void Start()
    {
        _sceneName = gameObject.scene.name;
        _signs = new List<GameObject>();

        _wrongWays = new string[]
        {
            "Oh No",
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

        StartCoroutine(AsyncStart());
    }

    private IEnumerator AsyncStart()
    {
        while (S.AllFather == null || S.Loader == null)
            yield return new WaitForSeconds(0.2f);

        while (S.Loader.Roots == null)
            yield return new WaitForSeconds(0.2f);

        while (!S.Loader.Roots.ContainsKey(_sceneName))
            yield return new WaitForSeconds(0.2f);

        while (S.Loader.Roots[_sceneName] == null)
            yield return new WaitForSeconds(0.2f);

        S.AllFather.Shuffle(_signsOrder);

        _signsHolder = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _signsHolder.name = "WrongWayHolder";
        _signsHolder.transform.position += new Vector3(0, -20, 0);
        Transform root = S.Loader.Roots[_sceneName];
        _signsHolder.transform.SetParent(root, true);

        while (S.Intercam == null)
            yield return new WaitForSeconds(0.2f);

        _ict = S.Intercam.transform;
    }

    private string GetAudio()
    {
        string res = _wrongWays[_order[_num]];
        _num++;
        if (_num >= _order.Length)
            _num = 0;
        return res;
    }

    private int ReserveSignIndex()
    {
        int idx = _signsOrder[_signCounter];
        _signCounter++;
        if (_signCounter >= _signsOrder.Length)
        {
            _signCounter = 0;
            S.AllFather.Shuffle(_signsOrder);
        }
        return idx;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag != "Player") return;
        if (_signsHolder == null || _ict == null) return;

        int signIndex = ReserveSignIndex();
        int globalIndex = _globalSignCounter++;

        Transform goTransform = S.CanvasObj.transform.Find("WrongWayLabel");
        GameObject label = goTransform.gameObject;

        _signsHolder.SetActive(true);
        label.SetActive(true);

        string audioName = GetAudio();
        float pitch = 1.25f + (float)S.RND.NextDouble() * 0.1f;
        S.AM.Play(audioName, pitch);

        StartCoroutine(CreateSign(signIndex, globalIndex));

        if (_hideCoroutine != null)
            StopCoroutine(_hideCoroutine);
        _hideCoroutine = StartCoroutine(HideDelayed(label));
    }

    private IEnumerator CreateSign(int signIndex, int globalIndex)
    {
        // 0.1 + 0.15, check.
        yield return new WaitForSeconds(0.25f);

        GameObject sign = Instantiate(S.InventoryPlane, _signsHolder.transform);

        sign.transform.position = _ict.position + _ict.forward * (30f - 0.001f * globalIndex);
        sign.transform.rotation = Quaternion.LookRotation(-_ict.forward);

        Vector3 targetScale = sign.transform.localScale * (0.20f + (float)S.RND.NextDouble() * 0.6f);
        sign.transform.localScale = targetScale;

        sign.transform.position += 29 * _ict.right * ((float)S.RND.NextDouble() - 0.5f);
        sign.transform.position += 14 * _ict.up * ((float)S.RND.NextDouble() - 0.5f);

        sign.transform.Rotate(0, 0, ((float)S.RND.NextDouble() - 0.5f) * 80f);

        Material mat = new Material(Shader.Find("Custom/AlphaUnlitSingleSideWithAlphaMultiplier"));
        mat.mainTexture = Resources.Load<Texture2D>($"Textures/Wrong Way/Wrong Way {signIndex}");
        sign.GetComponent<MeshRenderer>().material = mat;

        Vector3 startScale = targetScale * 4.0f;
        sign.transform.localScale = startScale;

        float animDuration = 0.1f;
        float animElapsed = 0f;
        bool soundPlayed = false;

        while (animElapsed < animDuration)
        {
            if (sign == null) yield break;

            float t = animElapsed / animDuration;
            sign.transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            animElapsed += Time.deltaTime;

            if (!soundPlayed && animElapsed >= (animDuration - 0.05f))
            {
                float pitch = UnityEngine.Random.Range(0.95f, 1.05f);
                S.AudioManager.Play("Kick Metal 1", pitch);
                soundPlayed = true;
            }

            yield return null;
        }
        sign.transform.localScale = targetScale;

        for (int i = 0; i < 35; i++)
            InstantiateParticle(sign.transform.position - _ict.forward * 0.0005f);

        _signs.Add(sign);
    }

    private IEnumerator HideDelayed(GameObject label)
    {
        // 1.0 + 0.35...
        yield return new WaitForSeconds(1.35f);

        label.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        _signsHolder.SetActive(false);
        _hideCoroutine = null;
    }

    public void InstantiateParticle(Vector3 position)
    {
        Quaternion rot = Quaternion.LookRotation(-_ict.forward);
        rot = Quaternion.Euler(rot.x, rot.y, UnityEngine.Random.Range(0f, 360f));
        Instantiate(S.WrongWayParticlePrefab, position, rot);
    }
}