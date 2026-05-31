using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SceneRoot : MonoBehaviour
{
    private Bounds sceneBounds;
    private bool _boundsCalculated;

    void Start()
    {
        string sceneName = gameObject.scene.name;
        StartCoroutine(LateStart());

        IEnumerator LateStart()
        {
            while (S.Loader == null)
                yield return new WaitForSeconds(0.1f);

            while (S.Loader.Roots == null)
                yield return new WaitForSeconds(0.1f);

            sceneBounds = CalculateSceneBounds(gameObject.scene);
            _boundsCalculated = true;

            if (!S.Loader.Roots.ContainsKey(sceneName))
            {
                S.Loader.Roots.Add(sceneName, transform);
                S.Loader.SceneRoots.Add(sceneName, this);
            }
            else
            {
                S.Loader.Roots[sceneName] = transform;
                S.Loader.SceneRoots[sceneName] = this;
            }
        }
    }

    public Vector3 GetRandomPoint()
    {
        if (!_boundsCalculated || sceneBounds.size == Vector3.zero)
        {
            S.Console.AddMessage($"SceneRoot: no bounds!", Color.red);
            return Vector3.zero;
        }

        const int maxAttempts = 1000; // So much for scenes like Income
        const float sampleDistance = 100f;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomPoint = new Vector3(
                Random.Range(sceneBounds.min.x, sceneBounds.max.x),
                Random.Range(sceneBounds.min.y, sceneBounds.max.y),
                Random.Range(sceneBounds.min.z, sceneBounds.max.z)
            );

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, sampleDistance, NavMesh.AllAreas))
                return hit.position;
        }

        S.Console.AddMessage($"SceneRoot: can't find NavMesh point after {maxAttempts} attempts  in {gameObject.scene.name}", Color.red);
        return Vector3.zero;
    }

    Bounds CalculateSceneBounds(Scene scene)
    {
        Bounds bounds = new Bounds();
        bool first = true;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>())
            {
                if (first)
                {
                    bounds = renderer.bounds;
                    first = false;
                }
                else
                    bounds.Encapsulate(renderer.bounds);
            }
        }

        if (first)
            S.Console.AddMessage($"Scene {scene.name} has no Renderer, can't find bounds!", Color.red);

        return bounds;
    }
}