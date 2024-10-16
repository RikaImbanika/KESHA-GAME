using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrongWay : MonoBehaviour
{
	private void OnTriggerEnter(Collider collider)
	{
        if (collider.gameObject.tag == "Player")
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
            Transform canvasTransform = canvas.transform;
            Transform goTransform = canvasTransform.Find("WrongWayLabel");
            GameObject label = goTransform.gameObject;
            label.SetActive(true);

            StartCoroutine(HideAfterDelay(label, 1f));
        }        
    }

    private IEnumerator HideAfterDelay(GameObject label, float delay)
    {
        yield return new WaitForSeconds(delay);

        label.SetActive(false);
    }
}
