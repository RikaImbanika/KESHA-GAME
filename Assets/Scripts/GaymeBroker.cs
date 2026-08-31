// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaymeBroker : MonoBehaviour
{
    string[] _sounds = {
        "Oh No",
        "Wrong",
        "Wrong 2" };

    int[] _order = new int[]
    {
            0, 1, 2, 0, 2, 1, 2, 0, 1
    };

    int _num = 0;

    void Start()
    {
        S.GaymeBroker = this;
    }

    public void OhNoTeso()
    {
        Transform canvasTransform = S.CanvasObj.transform;
        Transform goTransform = canvasTransform.Find("GameBrokenLabel");
        GameObject label = goTransform.gameObject;
        label.SetActive(true);

        float pitch = 1.25f + (float)S.RND.NextDouble() * 0.1f;

        S.AudioManager.Play(GetAudio(), pitch);

        S.Ph.transform.position = new Vector3(78.0599976f, -169.330002f, -84.0100021f);
        S.PlayerCamScript.xRotation = 180;
        S.PlayerCamScript.yRotation = 0;

        S.Loader.ImportantStaticShitToDo("Start");

        S.SceneSelector.PlaceButtons();

        StartCoroutine(HideAfterDelay(label, 3f));
    }

    private string GetAudio()
    {
        string res = _sounds[_order[_num]];
        _num++;
        if (_num >= _order.Length)
            _num = 0;
        return res;
    }

    private IEnumerator HideAfterDelay(GameObject label, float delay)
    {
        yield return new WaitForSeconds(delay);

        label.SetActive(false);
    }
}
