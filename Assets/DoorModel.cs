using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorModel : MonoBehaviour
{
    public Vector3 _coordinates;
    public Vector3 _right;
    public string _nextSceneName;
    public int _nextDoorNumber;

    public DoorModel()
    {
        _coordinates = new Vector3(0, 0, 0);
        _right = new Vector3(0, 5, 0);
    }

    public DoorModel(float x, float y, float z, Vector3 right)
    {
        _coordinates = new Vector3(x, y, z);
        _right = right;
    }
}
