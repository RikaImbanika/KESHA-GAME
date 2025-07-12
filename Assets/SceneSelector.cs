using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour
{
    List<string> _literallyScenes;
    Vector3 _p1;
    Vector3 _p2;
    bool _done;

    public void OkayBroIAmStartingDoingThisFuckingShitBro()
    {
        S.Ph.transform.position = new Vector3(3.22f, -165.3f, -2.87f);

        if (_done)
        {
            Debug.Log("Oh no bro, I will not. I already done this shit. Sorry, goodbye, good luck bro.");
            return;
        }

        StartCoroutine(IEshit());

        IEnumerator IEshit()
        {
            while (S.Teleporter == null)
            {
                Debug.Log("Oh no, sorre bro, I am waiting for S.Teleporter.");
                yield return new WaitForSeconds(0.05f);
            }

            S.Teleporter.ImportantStaticShitToDo("Start");

            _literallyScenes = new List<string>();

            _p1 = new Vector3(-61.2000008f, -169.089996f, 61.7400017f);
            _p2 = new Vector3(67.9000015f, -150.399994f, 61.7400017f);

            var shit = S.Loader._map.Keys;

            for (int i = 0; i < S.Loader._map.Count; i++)
            {
                if (shit.ElementAt(i) != "Start")
                    _literallyScenes.Add(shit.ElementAt(i));
            }

            float x = _p1.x + 1;
            float y = _p1.y + 1;
            float z = 61.7400017f;

            for (int i = 0; i < S.Loader._map.Count; i++)
            {
                int count = _literallyScenes[i].Length;

                for (int j = 0; j < count; j++)
                {
                    string symbol = $"{_literallyScenes[i][j]}";

                    if (char.IsLower(symbol[0]))
                        symbol = $"_{symbol}";

                    if (symbol == " ")
                        symbol = "_";

                    string matName = $"Materials/Symbols/{symbol}";

                    GameObject prefab = Resources.Load<GameObject>($"Prefabs/LETTER");
                    Material material = Resources.Load<Material>(matName);
                    GameObject obj = Instantiate(prefab, new Vector3(x, y, z), new Quaternion(0, 180, 0, 0));
                    obj.GetComponent<Renderer>().material = material;
                    obj.AddComponent(typeof(IsSceneName));
                    IsSceneName isSceneName = obj.GetComponent<IsSceneName>();
                    isSceneName._sceneName = _literallyScenes[i];

                    x += 1;
                }

                x += 1;

                if (x > _p2.x)
                {
                    x = _p1.x;
                    y += 1;
                }
            }

            _done = true;
        }
    }

    void Start()
    {
        S.SceneSelector = this;
    }
}