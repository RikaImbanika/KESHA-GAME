using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamp : MonoBehaviour
{
	public Door _door;
	string _id;

    public void Start()
    {
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
        S.SM.Save(S.ID(_id, "destroyed"), true);
        Debug.Log($"Unlocked and saved. id = {_id}");
        _door.Unlock();
    }
}
