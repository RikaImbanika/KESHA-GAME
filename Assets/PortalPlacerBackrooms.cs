using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalPlacerBackrooms : MonoBehaviour
{
    string _id;
    string _sceneName;
    string _otherSceneName;
    string _secondPortalId;
    Vector3 _secondPortalPosition;
    Quaternion _secondPortalRotation;
    private MeshRenderer _unityEditorMeshRenderer;
    private MeshFilter _unityEditorMeshFilter;


    public void Start()
    {
        StartCoroutine(Wait());

        IEnumerator Wait()
        {
            _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;

            if (!S.Backrooms._portalsSystem.ContainsKey(_sceneName))
                Destroy(gameObject);

            _id = S.ID(_sceneName, S.ID(transform.position.x, S.ID(transform.position.y, transform.position.z)));

            while (true)
            {
                while (S.Backrooms._portalsSystem == null)
                    yield return new WaitForSeconds(0.3f);

                while (S.Backrooms._portalsSystem.Count <= 0)
                    yield return new WaitForSeconds(0.3f);

                if (!S.Backrooms._portals.ContainsKey(_sceneName))
                {
                    S.Backrooms._portals.Add(_sceneName, new Dictionary<string, (Vector3, Quaternion)>());

                    string rotId = S.ID(_id, "rot");
                    Quaternion? rot = S.SM.LoadQuaternion(rotId);
                    if (rot == null)
                    {
                        float ang = S.RND.Next(360);
                        transform.Rotate(0, ang, 0);
                        S.SaveManager.Save(rotId, transform.rotation);
                    }
                    else
                        transform.rotation = (Quaternion)rot;

                    yield return new WaitForSeconds(0.15f + (float)S.RND.NextDouble() * 0.15f);

                    S.Backrooms._portals[_sceneName].Add(_id, (transform.position, transform.rotation));

                    S.Backrooms.UpdateInitPortalsInRoom(_sceneName);

                    yield return new WaitForSeconds(1.15f);

                    TryLoad();
                }
                else
                    TryLoad();
            }
        }
    }

    IEnumerator TryLoad()
    {
        Dictionary<string, (string, string)> di = S.Backrooms._portalsSystem[_sceneName];
        if (di.ContainsKey(_id))
        {
            //IT SELECTED US!

            (string, string) touple = di[_id];

            _otherSceneName = touple.Item1;
            _secondPortalId = touple.Item2;

            if (S.Backrooms._portals[_otherSceneName].ContainsKey(_secondPortalId))
            {
                (Vector3, Quaternion) tuple2 = S.Backrooms._portals[_otherSceneName][_secondPortalId];
                _secondPortalPosition = tuple2.Item1;
                _secondPortalRotation = tuple2.Item2;

                Instantiate();
            }
        }
        else
            Destroy(gameObject);

        yield return null;
    }

    public void Update()
    {
        if (string.IsNullOrEmpty(_secondPortalId))
            return;

        //It is waiting while other portal will put it params
        if (S.Backrooms._portals[_otherSceneName].ContainsKey(_secondPortalId))
        {
            (Vector3, Quaternion) tuple2 = S.Backrooms._portals[_otherSceneName][_secondPortalId];
            _secondPortalPosition = tuple2.Item1;
            _secondPortalRotation = tuple2.Item2;

            Instantiate();
        }
    }

    public void Instantiate()
    {
        Portal portal = gameObject.AddComponent<Portal>();
        portal._otherSceneName = _otherSceneName;
        portal._secondPortalPosition = _secondPortalPosition;
        portal._secondPortalRotation = _secondPortalRotation;

        Destroy(this);
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
