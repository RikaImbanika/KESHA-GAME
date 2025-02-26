using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Initialiser : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(InitializeCoroutine());
    }

    private IEnumerator InitializeCoroutine()
    {
        yield return WaitForCondition(() => Camera.main != null, "Waiting for camera");
        S.Camera = Camera.main;

        yield return WaitForObjectWithTag("Player", obj => S.PObj = obj);
        S.Ph = S.PObj.transform.parent?.gameObject;
        S.Ps = null;
        yield return StartCoroutine(GetComponentSafe<PlayerStorage>(S.Ph, playerStorage => S.Ps = playerStorage));

        yield return WaitForObjectWithTag("Canvas", obj => S.CanvasObj = obj);
        S.Canvas = null;
        yield return StartCoroutine(GetComponentSafe<Canvas>(S.CanvasObj, canvas => S.Canvas = canvas));

        yield return WaitForObjectWithTag("FPS", obj => S.FpsObj = obj);
        yield return StartCoroutine(GetComponentSafe<TextMeshProUGUI>(S.FpsObj, fpsObj => S.FpsTMP = fpsObj));

        yield return WaitForObjectWithTag("Sparkle", obj => S.Sparkle = obj);
        yield return WaitForObjectWithTag("RedSparkle", obj => S.RedSparkle = obj);
        yield return WaitForObjectWithTag("Spot", obj => S.Spot = obj);
    }

    private static IEnumerator WaitForObjectWithTag(string tag, System.Action<GameObject> setter)
    {
        GameObject result = null;
        int attempts = 0;

        while (result == null)
        {
            result = GameObject.FindGameObjectWithTag(tag);
            if (result != null) break;

            Debug.LogWarning($"Obj with tag {tag} not found. Try x {attempts + 1}.");
            yield return new WaitForSeconds(0.15f);
            attempts++;
        }

        setter?.Invoke(result);
    }

    private static IEnumerator WaitForCondition(System.Func<bool> condition, string message)
    {
        float timer = 0;
        while (!condition())
        {
            timer += Time.deltaTime;
            Debug.LogWarning($"{message} ({timer:F1}s)");
            yield return null;
        }
    }

    private static IEnumerator GetComponentSafe<T>(GameObject target, Action<T> callback) where T : Component
    {
        while (target == null)
        {
            Debug.LogError("Target GameObject is null!");
            callback?.Invoke(default);
            yield return new WaitForSeconds(0.1f);
        }

        T component = null;
        while (component == null)
        {
            component = target.GetComponent<T>();
            if (component != null) break;
            Debug.LogWarning($"Waiting for component {typeof(T)} on {target.name}...");
            yield return new WaitForSeconds(0.1f);
        }

        callback?.Invoke(component);
    }
}
