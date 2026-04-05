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

        S.PortalObj1 = Prefabs.Get("Portals/Portal 1");

        S.SnakeBalls = new Dictionary<string, List<GameObject>>();
        S.SnakeBalls.Add("Classic", new List<GameObject>());
        S.SnakeBalls.Add("Ice", new List<GameObject>());
        S.SnakeBalls.Add("Nature", new List<GameObject>());
        S.SnakeBalls.Add("Silent", new List<GameObject>());

        S.SnakeBalls["Classic"].Add(Prefabs.Get("Balls/RedBall"));
        S.SnakeBalls["Classic"].Add(Prefabs.Get("Balls/BlueBall"));
        S.SnakeBalls["Classic"].Add(Prefabs.Get("Balls/PurpleBall"));
        S.SnakeBalls["Classic"].Add(Prefabs.Get("Balls/YellowBall"));
        S.SnakeBalls["Classic"].Add(Prefabs.Get("Balls/GreenBall"));
        S.SnakeBalls["Classic"].Add(Prefabs.Get("Balls/WhiteBall"));
        S.SnakeBalls["Classic"].Add(Prefabs.Get("Balls/CyanBall"));

        S.SnakeBalls["Ice"].Add(Prefabs.Get("Balls/CyanBall"));
        S.SnakeBalls["Ice"].Add(Prefabs.Get("Balls/IceBall 1"));
        S.SnakeBalls["Ice"].Add(Prefabs.Get("Balls/IceBall 2"));
        S.SnakeBalls["Ice"].Add(Prefabs.Get("Balls/IceBall 3"));
        S.SnakeBalls["Ice"].Add(Prefabs.Get("Balls/IceBall 4"));
        S.SnakeBalls["Ice"].Add(Prefabs.Get("Balls/IceBall 5"));
        S.SnakeBalls["Ice"].Add(Prefabs.Get("Balls/IceBall 6"));

        S.SnakeBalls["Nature"].Add(Prefabs.Get("Balls/GreenBall"));
        S.SnakeBalls["Nature"].Add(Prefabs.Get("Balls/NatureBall 1"));
        S.SnakeBalls["Nature"].Add(Prefabs.Get("Balls/NatureBall 2"));
        S.SnakeBalls["Nature"].Add(Prefabs.Get("Balls/NatureBall 3"));
        S.SnakeBalls["Nature"].Add(Prefabs.Get("Balls/NatureBall 4"));
        S.SnakeBalls["Nature"].Add(Prefabs.Get("Balls/NatureBall 5"));
        S.SnakeBalls["Nature"].Add(Prefabs.Get("Balls/NatureBall 6"));
        S.SnakeBalls["Nature"].Add(Prefabs.Get("Balls/NatureBall 7"));
        S.SnakeBalls["Nature"].Add(Prefabs.Get("Balls/NatureBall 8"));

        S.SnakeBalls["Silent"].Add(Prefabs.Get("Balls/SilentBall 1"));
        S.SnakeBalls["Silent"].Add(Prefabs.Get("Balls/SilentBall 2"));
        S.SnakeBalls["Silent"].Add(Prefabs.Get("Balls/SilentBall 3"));
        S.SnakeBalls["Silent"].Add(Prefabs.Get("Balls/SilentBall 4"));
        S.SnakeBalls["Silent"].Add(Prefabs.Get("Balls/SilentBall 5"));
        S.SnakeBalls["Silent"].Add(Prefabs.Get("Balls/SilentBall 6"));

        yield return WaitForCondition(() => Camera.main != null, "Waiting for camera");
        S.Camera = Camera.main;

        S.Camera.usePhysicalProperties = true;
        S.Camera.gateFit = Camera.GateFitMode.None;
        S.Camera.fieldOfView = 76f;

        yield return WaitForObjectWithTag("Player", obj => S.PObj = obj);
        S.Ph = S.PObj.transform.parent?.gameObject;
        S.Pm = S.Ph.GetComponent<PlayerMovement>();

        GameObject fp = Instantiate(Prefabs.Get("FakePlayer"));
        S.FakePlayerScene = "Start";
        S.FakePlayer = fp.transform;
        S.FakePlayerCamera = fp.transform.GetChild(0);

        yield return StartCoroutine(GetComponentSafe<PlayerStorage>(S.Ph, playerStorage => S.Ps = playerStorage));

        yield return WaitForObjectWithTag("Canvas", obj => S.CanvasObj = obj);

        yield return StartCoroutine(GetComponentSafe<Canvas>(S.CanvasObj, canvas => S.Canvas = canvas));

        yield return WaitForObjectWithTag("FPS", obj => S.FpsObj = obj);
        yield return StartCoroutine(GetComponentSafe<TextMeshProUGUI>(S.FpsObj, fpsObj => S.FpsTMP = fpsObj));

        yield return WaitForObjectWithTag("Spot", obj => S.Spot = obj);
        S.RedLaser = Prefabs.Get("Lasers/RedLaser");
        S.BlueLaser = Prefabs.Get("Lasers/BlueLaser");
        S.GreenLaser = Prefabs.Get("Lasers/GreenLaser");
        S.PurpleLaser = Prefabs.Get("Lasers/PurpleLaser");

        S.RedPoint = Prefabs.Get("Sparkles/Points/RedPoint");
        S.BluePoint = Prefabs.Get("Sparkles/Points/BluePoint");
        S.GreenPoint = Prefabs.Get("Sparkles/Points/GreenPoint");
        S.PurplePoint = Prefabs.Get("Sparkles/Points/PurplePoint");

        S.RedHitPoint = Prefabs.Get("Sparkles/HitPoints/HitPointRed");
        S.BlueHitPoint = Prefabs.Get("Sparkles/HitPoints/HitPointBlue");
        S.GreenHitPoint = Prefabs.Get("Sparkles/HitPoints/HitPointGreen");
        S.PurpleHitPoint = Prefabs.Get("Sparkles/HitPoints/HitPointPurple");

        S.BlueRay = Prefabs.Get("Lasers/GunLaser");

        S.RedSparkle = Prefabs.Get("Sparkles/Normal/RedSparkle");
        S.BlueSparkle = Prefabs.Get("Sparkles/Normal/BlueSparkle");
        S.GreenSparkle = Prefabs.Get("Sparkles/Normal/GreenSparkle");
        S.PurpleSparkle = Prefabs.Get("Sparkles/Normal/PurpleSparkle");

        S.PlayerSparkle = Prefabs.Get("PlayerSparkle");

        S.RedHeavySparkle = Prefabs.Get("Sparkles/HeavySparkles/HeavySparkleRed");
        S.BlueHeavySparkle = Prefabs.Get("Sparkles/HeavySparkles/HeavySparkleBlue");
        S.GreenHeavySparkle = Prefabs.Get("Sparkles/HeavySparkles/HeavySparkleGreen");
        S.PurpleHeavySparkle = Prefabs.Get("Sparkles/HeavySparkles/HeavySparklePurple");

        S.FireballRed = Prefabs.Get("Fireballs/FireballRed");
        S.FireballBlue = Prefabs.Get("Fireballs/FireballBlue");
        S.FireballGreen = Prefabs.Get("Fireballs/FireballGreen");
        S.FireballPurple = Prefabs.Get("Fireballs/FireballPurple");

        S.Loot = Prefabs.Get("Loot");

        S.Shot = Prefabs.GetAudioSource("Shot");
        S.Caboom = Prefabs.GetAudioSource("Caboom");
        S.SnakeBody = Prefabs.Get("SnakeBody");
        S.LighterObj = Prefabs.Get("Lighter");

        S.Snakes = new List<GameObject>();
        for (int i = 1; i <= 4; i++)
            S.Snakes.Add(Prefabs.Get($"SNAKIE {i}"));

        S.Zombie = Prefabs.Get("FirstZombie");
        S.Bakalavr = Prefabs.Get("Bakalavr");
        S.Musculus = Prefabs.Get("Musculus");
        S.Ghost = Prefabs.Get("Ghost");
        S.Spider = Prefabs.Get("Spider");
        S.Stamp = Prefabs.Get("Stamp");
        S.Arrow = Prefabs.Get("Arrow");
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