using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stamp : MonoBehaviour
{
	public Door _door;
    string _id;
    string _sceneName;
    public GameObject _go;
    public GameObject _blueFlames;
    bool _alreadyUnlocked;

    public void Start()
    {
        _go = gameObject;

        _sceneName = SceneManager.GetSceneByBuildIndex(gameObject.scene.buildIndex).name;

        _id = "" + transform.position.x + transform.position.y + transform.position.z;

        OldSave s = new OldSave();

        bool destroyed = S.SM.LoadBool(S.ID(_id, "destroyed")) ?? false;
        _door._locked = !destroyed;

        if (destroyed)
        {
            Destroy(gameObject);

            Debug.Log($"Stamp Opened. id = {_id}");
        }
        else
        {
            Debug.Log($"Stamp Not Opened. id = {_id}");

            StartCoroutine(SetParent());

            IEnumerator SetParent()
            {
                while (S.Loader.Roots[_sceneName] == null)
                    yield return new WaitForSeconds(0.25f);

                _blueFlames = GameObject.Instantiate(Prefabs.Get("BlueFlames"), S.Loader.Roots[_sceneName]);

                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 20f))
                {
                    Vector3 point = hit.point;
                    _blueFlames.transform.position = point;
                    _blueFlames.transform.rotation = Quaternion.LookRotation(transform.forward);
                }
            }
        }
    }

    public void Unlock()
    {
        if (!_alreadyUnlocked)
        {
            _alreadyUnlocked = true;
            S.AudioManager.Play("stampSound", 1);
            S.SM.Save(S.ID(_id, "destroyed"), true);
            Debug.Log($"STAMP UNLOCKED AND SAVED!!! id = {_id}");

            float count = _blueFlames.transform.childCount;

            for (int i = 0; i < count; i++)
            {
                GameObject child = _blueFlames.transform.GetChild(i).gameObject;
                child.AddComponent<BlueFlame>();
            }

            _door.Unlock();
        }
    }
}