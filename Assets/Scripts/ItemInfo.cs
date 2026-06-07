// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
	public string _name;
	public string _spriteName;
	public string _prefabName;
	public string _visibleName;
	public Color _inventoryNameColor;
	public Color _lookNameColor;
	public string _pickUpA;
	public float _throwRotX;
	public float _throwRotY;
	public float _throwRotZ;
	public bool _throwable;

	public ItemInfo(Color inventoryNameColor, Color lookNameColor, string spriteName = "Error", string prefabName = "PurpleBall", string visibleName = "Item of cringe", string pickUpA = "Pick Up",
		float throwRotX = 0, float throwRotY = 0, float throwRotZ = 0, bool throwable = true)
	{
		_inventoryNameColor = inventoryNameColor;
		_lookNameColor = lookNameColor;
		_spriteName = spriteName;
		_prefabName = prefabName;
		_visibleName = visibleName;
		_pickUpA = pickUpA;
		_throwRotX = throwRotX;
		_throwRotY = throwRotY;
		_throwRotZ = throwRotZ;
		_throwable = throwable;
	}
}
