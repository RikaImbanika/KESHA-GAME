using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEngine.UI;
using TMPro;

public class OnDiePanel : MonoBehaviour
{
	public TextMeshProUGUI text;

	void Start()
	{
	}

	void OnEnable()
	{
		Image img = GetComponent<Image>();

		StartCoroutine(WaitLoad());

		IEnumerator WaitLoad()
		{
			Color clr = img.color;
			Color tclr = text.color;

			gameObject.SetActive(true);
			text.color = new Color(tclr.r, tclr.g, tclr.b, 0);

			float alfa = 0;

			while (alfa < 1)
			{
				alfa += 0.03f;

				text.color = new Color(tclr.r, tclr.g, tclr.b, alfa);

				yield return new WaitForSeconds(0.02f);
			}

			while (alfa > 0)
			{
				alfa -= 0.015f;

				text.color = new Color(tclr.r, tclr.g, tclr.b, alfa);

				yield return new WaitForSeconds(0.02f);
			}

			alfa = 1;

			gameObject.SetActive(false);

			yield return null;
		}
	}
}
