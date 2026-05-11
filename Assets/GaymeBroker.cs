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
        S.SceneSelector.OkayBroIAmStartingDoingThisFuckingShitBro();

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
