using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrongWay : MonoBehaviour
{
    string[] _wrongWays = { 
        "Oh No", 
        "Wrong",
        "Wrong 2"
         };

    int[] _order = new int[]
    {
        0, 1, 2, 0, 2, 1, 2, 0, 1
    };

    int _num = 0;

	private void OnTriggerEnter(Collider collider)
	{
        if (collider.gameObject.tag == "Player")
        {
            Transform canvasTransform = S.CanvasObj.transform;
            Transform goTransform = canvasTransform.Find("WrongWayLabel");
            GameObject label = goTransform.gameObject;
            label.SetActive(true);
            string audioName = GetAudio();

            float pitch = 1.25f + (float)S.RND.NextDouble() * 0.1f;
            S.AM.Play(audioName, pitch);

            StartCoroutine(HideAfterDelay(label, 1f));
        }        
    }

    private string GetAudio()
    {
        string res = _wrongWays[_order[_num]];
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
