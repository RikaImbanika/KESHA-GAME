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
        S.RND = new System.Random();

        S.SnakeBallMaterials = new List<Material>
        {
            Materials.Get("Balls/BlueBall"),
            Materials.Get("Balls/PurpleBall"),
            Materials.Get("Balls/RedBall"),
            Materials.Get("Balls/YellowBall"),
            Materials.Get("Balls/LimeBall"),
            Materials.Get("Balls/CyanBall"),
            Materials.Get("Balls/WhiteBall")
        };

        yield return WaitForCondition(() => Camera.main != null, "Waiting for camera");
        S.Camera = Camera.main;

        S.Camera.usePhysicalProperties = true;
        S.Camera.gateFit = Camera.GateFitMode.None;
        S.Camera.fieldOfView = 75f;

        yield return WaitForObjectWithTag("Player", obj => S.PObj = obj);
        S.Ph = S.PObj.transform.parent?.gameObject;
        S.Pm = S.Ph.GetComponent<PlayerMovement>();

        yield return StartCoroutine(GetComponentSafe<PlayerStorage>(S.Ph, playerStorage => S.Ps = playerStorage));

        yield return WaitForObjectWithTag("Canvas", obj => S.CanvasObj = obj);
        
        yield return StartCoroutine(GetComponentSafe<Canvas>(S.CanvasObj, canvas => S.Canvas = canvas));

        yield return WaitForObjectWithTag("FPS", obj => S.FpsObj = obj);
        yield return StartCoroutine(GetComponentSafe<TextMeshProUGUI>(S.FpsObj, fpsObj => S.FpsTMP = fpsObj));

        yield return WaitForObjectWithTag("Spot", obj => S.Spot = obj);
        S.RedLaser = Prefabs.Get("RedLaser");
        S.BlueLaser = Prefabs.Get("BlueLaser");
        S.RedPoint = Prefabs.Get("RedPoint");
        S.RedHitPoint = Prefabs.Get("RedHitPoint");
        S.BlueHitPoint = Prefabs.Get("BlueHitPoint");
        S.BlueRay = Prefabs.Get("BlueRay");
        S.RedSparkle = Prefabs.Get("RedSparkle");
        S.BlueSparkle = Prefabs.Get("BlueSparkle");
        S.RedOldSparkle = Prefabs.Get("RedOldSparkle");
        S.BlueOldSparkle = Prefabs.Get("BlueOldSparkle");
        S.Loot = Prefabs.Get("Loot");
        S.EnemyBullet = Prefabs.Get("EnemyBullet");
        S.Shot = Prefabs.GetAudioSource("Shot");
        S.Caboom = Prefabs.GetAudioSource("Caboom");
        S.SnakeBody = Prefabs.Get("SnakeBody");
        S.LighterObj = Prefabs.Get("Lighter");
        S.Snakie1 = Prefabs.Get("SNAKIE 1");
        S.Snakie2 = Prefabs.Get("SNAKIE 2");
        S.Snakie3 = Prefabs.Get("SNAKIE 3");
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