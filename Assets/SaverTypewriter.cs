using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaverTypewriter : MonoBehaviour
{
	public void Save()
	{
		S.SM.SaveCurrentToLast();

		Transform goTransform = S.Canvas.transform.Find("Game saved");
		GameObject go = goTransform.gameObject;
		go.SetActive(true);

		StartCoroutine(HideAfterDelay(go, 1f));
	}

	private IEnumerator HideAfterDelay(GameObject go2, float delay)
	{
		yield return new WaitForSeconds(delay);
		go2.SetActive(false);
	}
}
