using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamp : MonoBehaviour
{
	public Door _door;
	AllFather _allFather;
	string _id;

	public void Start()
	{
        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();
        _id = "" + transform.position.x + transform.position.y + transform.position.z;

        Save s = new Save();

        if (_allFather.Contains(_id))
        {
            s = _allFather.Load(_id);

            _door._locked = s._locked;

            if (s._destroyed)
            {
                Destroy(gameObject);

                Debug.Log($"Opened. id = {_id}");
            }
            else
                Debug.Log($"Not Opened. id = {_id}");
        }
        else
            Debug.Log($"Not Opened. id = {_id}");
    }

	public void Unlock()
	{
        Save s = new Save();
        if (_allFather.Contains(_id))
            s = _allFather.Load(_id);

        s._locked = false;
        s._destroyed = true;

        Debug.Log($"Saved. id = {_id}");

        _allFather.Save(_id, s);

        _door.Unlock();
    }
}
