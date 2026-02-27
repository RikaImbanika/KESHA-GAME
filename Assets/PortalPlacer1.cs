using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalPlacer1 : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LateStart());

        IEnumerator LateStart()
        {
            Portal _portal = gameObject.AddComponent<Portal>();
            //_portal.gameObject.transform.rotation;
            _portal._sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
            _portal._otherSceneName = "Corridor";

            while (S.PortalsBase == null)
                yield return new WaitForSeconds(0.025f);

            S.PortalsBase.Portals["IncomePortal"] = _portal;

            while (!S.PortalsBase.Portals.ContainsKey("CorridorPortal"))
                yield return new WaitForSeconds(0.033f);

            Transform spt = S.PortalsBase.Portals["CorridorPortal"].transform;
            _portal._secondPortalPosition = spt.position;
            _portal._secondPortalRotation = spt.rotation;
        }
    }
}