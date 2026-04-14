using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEngine.UI;
using TMPro;

public class OnStartPanel : MonoBehaviour
{
	public TextMeshProUGUI _text;
	public TextMeshProUGUI _text2;
	public TextMeshProUGUI _text3;
	public Image _p1;
	public Image _p2;

	void Start()
	{
		Image img = GetComponent<Image>();

		StartCoroutine(WaitLoad());

		IEnumerator WaitLoad()
		{
			Color tclr = new Color(230, 0, 255, 0);

			gameObject.SetActive(true);

			float alfa = 0;

			UPD();

			while (alfa < 1)
			{
				alfa += 0.03f;

				UPD();

				yield return new WaitForSeconds(0.02f);
			}

			while (alfa > 0)
			{
				alfa -= 0.015f;

				UPD();

				yield return new WaitForSeconds(0.02f);
			}

			alfa = 1;

			while (alfa > 0.5f)
			{
				alfa -= 0.005f;

				UPD2();

				yield return new WaitForSeconds(0.02f);
			}

			while (alfa > 0)
			{
				alfa -= 0.025f;

				UPD2();

				yield return new WaitForSeconds(0.02f);
			}

			gameObject.SetActive(false);

			void UPD()
			{
				Color nc = new Color(tclr.r, tclr.g, tclr.b, alfa);
				Color ncBlack = new Color(0, 0, 0, alfa);

				_text.color = nc;
				_text2.color = ncBlack;
				_text3.color = ncBlack;
				_p1.color = nc;
				_p2.color = nc;
			}

			void UPD2()
			{
				Color nc = new Color(0, 0, 0, alfa);

				img.color = nc;
			}

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

			return false;
		}
	}
}
