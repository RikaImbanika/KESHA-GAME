using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalPlacer2 : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LateStart());

        IEnumerator LateStart()
        {
            float ang = S.RND.Next(360);
            transform.Rotate(0, ang, 0);

            Portal _portal = gameObject.AddComponent<Portal>();
            _portal._sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
            string otn = "Income";
            _portal._secondSceneName = otn;
            _portal._id = S.ID(gameObject);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 20f))
                transform.position = hit.point;

            while (S.PortalsBase == null)
                yield return new WaitForSeconds(0.025f);

            S.PortalsBase.AddPortal(_portal._sceneName, _portal._id, _portal);
            S.PortalsBase.AddFreePort(_portal._sceneName, _portal._id);

            var enumerator = S.PortalsBase.TakeFreePort(_portal._sceneName, _portal._id, otn);
            yield return enumerator;
            _portal._secondPortalId = (string)enumerator.Current;
        }
    }
}