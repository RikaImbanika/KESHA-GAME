using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class OnStartPanel : MonoBehaviour
{
	public TextMeshProUGUI _text;
	public TextMeshProUGUI _text2;
	public TextMeshProUGUI _text3;
	public Image _p1;
	public Image _p2;

	float K
	{
		get
		{
			return Time.deltaTime * 0.4f;
		}
	}

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
				alfa = Mathf.Min(alfa + 1.8f * K, 1f);

				UPD();

				yield return null;
			}

			while (alfa > 0)
			{
				alfa = Mathf.Max(alfa - 0.9f * K, 0f);

				UPD();

				yield return null;
			}

			alfa = 1;

			while (alfa > 0.5f)
			{
				alfa = Mathf.Max(alfa - 0.9f * K, 0.5f);

				UPD2();

				yield return null;
			}

			while (alfa > 0)
			{
				alfa = Mathf.Max(alfa - 3f * K, 0f);

				UPD2();

				yield return null;
			}

			S.Console.AddMessage("Rika: One day I go to visit my granny...", Color.magenta);

			gameObject.SetActive(false);

			void UPD()
			{
				Color nc = new Color(tclr.r, tclr.g, tclr.b, alfa);
				Color ncBlack = new Color(0, 0, 0, Mathf.Pow(alfa, 0.25f));

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