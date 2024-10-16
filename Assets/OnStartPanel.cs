using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEngine.UI;
using TMPro;

public class OnStartPanel : MonoBehaviour
{
	public TextMeshProUGUI text;

	void Start()
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

/*			while (!SceneCurrentlyLoaded("Income"))
				yield return new WaitForSeconds(0.2f);*/

			alfa = 1;

			while (alfa > 0.5f)
			{
				alfa -= 0.005f;

				img.color = new Color(clr.r, clr.g, clr.b, alfa);

				yield return new WaitForSeconds(0.02f);
			}

			while (alfa > 0)
			{
				alfa -= 0.025f;

				img.color = new Color(clr.r, clr.g, clr.b, alfa);

				yield return new WaitForSeconds(0.02f);
			}

			gameObject.SetActive(false);

			yield return null;
		}

		bool SceneCurrentlyLoaded(string sceneName_no_extention)
		{
			for (int i = 0; i < SceneManager.sceneCount; ++i)
			{
				Scene scene = SceneManager.GetSceneAt(i);
				if (scene.name == sceneName_no_extention)
				{
					if (scene.isLoaded)
						return true;
					else
						return false;
				}
			}

			return false;//scene not currently loaded in the $$anonymous$$erarchy
		}
	}
}
