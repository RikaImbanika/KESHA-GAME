using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamp : MonoBehaviour
{
	public Door _door;
	string _id;
    public GameObject _go;

    public void Start()
    {
        _go = gameObject;
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
            Debug.Log($"Stamp Not Opened. id = {_id}");
    }

	public void Unlock()
	{
        S.AudioManager.Play("arfa", 1);
        S.SM.Save(S.ID(_id, "destroyed"), true);
        Debug.Log($"STAMP UNLOCKED AND SAVED!!! id = {_id}");
        _door.Unlock();
    }
}
