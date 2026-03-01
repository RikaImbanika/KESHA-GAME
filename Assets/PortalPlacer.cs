using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalPlacer : MonoBehaviour
{
    string _id;
    string _sceneName;
    string _secondSceneName;
    Portal _portal;
    private MeshRenderer _unityEditorMeshRenderer;
    private MeshFilter _unityEditorMeshFilter;

    public void Start()
    {
        StartCoroutine(LateStart());

        IEnumerator LateStart()
        {
            _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;
            _id = S.ID(gameObject);
            string idSecondPortId = S.ID(_id, "npid");
            string idRotation = S.ID(_id, "rot");
            string idNextSceneName = S.ID(_id, "ncn");

            string secondPortId = S.SM.LoadString(idSecondPortId);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 20f))
                transform.position = hit.point;

            if (!string.IsNullOrEmpty(secondPortId))
            {
                if (secondPortId == "none")
                    Destroy(gameObject);
                else
                    StartCoroutine(Load());

                yield return null;
            }
            else
                StartCoroutine(Instantiate());

            IEnumerator Load()
            {
                _portal = gameObject.AddComponent<Portal>();
                S.PortalsBase.AddPortal(_sceneName, _id, _portal);

                _portal._secondPortalId = secondPortId;
                _portal._secondSceneName = S.SM.LoadString(idNextSceneName);
                _portal._id = _id;
                _portal._sceneName = _sceneName;

                Debug.LogError($"Loading portal... ({_sceneName} {_id} - {_portal._secondSceneName} {secondPortId})");

                transform.rotation = S.SM.LoadQuaternion(idRotation) ?? Quaternion.identity;

                Destroy(this);

                yield break;
            }

            IEnumerator Instantiate()
            {
                while (S.PortalsBase.Connections == null)
                    yield return new WaitForSeconds(0.3f);

                yield return new WaitForSeconds(S.RND.Next(1000) * 0.0003f);

                _secondSceneName = S.SM.LoadString(idNextSceneName);
                if (string.IsNullOrEmpty(_secondSceneName))
                {
                    _secondSceneName = S.PortalsBase.TakeConnection(_sceneName);
                    if (string.IsNullOrEmpty(_secondSceneName))
                    {
                        Debug.LogError("No connection, destroying.");

                        S.SM.Save(idSecondPortId, "none");
                        Destroy(gameObject);
                        yield break;
                    }

                    Debug.LogError($"Instantiating... (Connection TAKEN {_sceneName} - {_secondSceneName})");
                }
                else
                    Debug.LogError($"Instantiating... (Connection LOADED {_sceneName} - {_secondSceneName})");

                S.SM.Save(idNextSceneName, _secondSceneName);

                InitRotation();
                _portal = gameObject.AddComponent<Portal>();
                _portal._sceneName = _sceneName;
                _portal._secondSceneName = _secondSceneName;
                _portal._id = _id;

                S.PortalsBase.AddPortal(_sceneName, _id, _portal);
                S.PortalsBase.AddFreePort(_sceneName, _id);

                //This stroke will wait for it
                var enumerator = S.PortalsBase.TakeFreePort(_portal._sceneName, _portal._id, _secondSceneName);
                yield return enumerator;
                _portal._secondPortalId = (string)enumerator.Current;

                S.SM.Save(idSecondPortId, _portal._secondPortalId);
                S.SM.Save(idRotation, transform.rotation);
            }
        }
    }

    private void InitRotation()
    {
        float ang = S.RND.Next(360);
        transform.Rotate(0, ang, 0);
    }

#if UNITY_EDITOR
    private void EnsureMeshFilter()
    {
        if (GetComponent<MeshRenderer>() == null)
        {
            _unityEditorMeshRenderer = gameObject.AddComponent<MeshRenderer>();
            _unityEditorMeshRenderer.sharedMaterial = Materials.GetInEditor("DUMMY_MAGENTA");
        }

        if (GetComponent<MeshFilter>() == null)
        {
            _unityEditorMeshFilter = gameObject.AddComponent<MeshFilter>();

            // Создаем увеличенный куб
            Mesh scaledCube = CreateScaledCube(2f);
            _unityEditorMeshFilter.sharedMesh = scaledCube;
        }
    }

    private Mesh CreateScaledCube(float scale)
    {
        // Получаем стандартный куб
        Mesh originalCube = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
        Mesh scaledMesh = Instantiate(originalCube);

        // Масштабируем вершины
        Vector3[] vertices = scaledMesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = vertices[i] * scale;
        }
        scaledMesh.vertices = vertices;

        // Обновляем меш
        scaledMesh.RecalculateNormals();
        scaledMesh.RecalculateBounds();

        return scaledMesh;
    }

    void OnDrawGizmos()
    {
        EnsureMeshFilter();
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, new Vector3(2, 2, 2));
    }
#endif
}
