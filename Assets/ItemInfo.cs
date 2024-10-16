using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
	public string _spriteName;
	public string _prefabName;
	public string _pickUpA;
	public float _throwRotX;
	public float _throwRotY;
	public float _throwRotZ;
	public bool _throwable;

	public ItemInfo(string spriteName = "Error", string prefabName = "PurpleBall", string pickUpA = "pickUp",
		float throwRotX = 0, float throwRotY = 0, float throwRotZ = 0, bool throwable = true)
	{
		_spriteName = spriteName;
		_prefabName = prefabName;
		_pickUpA = pickUpA;
		_throwRotX = throwRotX;
		_throwRotY = throwRotY;
		_throwRotZ = throwRotZ;
		_throwable = throwable;
	}
}
