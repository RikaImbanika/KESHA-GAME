// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryClick : MonoBehaviour
{
	public Inventory inventory;
	public int id;

	public void OnClick()
	{
		inventory.SelectItem(id, inventory.selectedId == id || inventory.ultraSelectedId > -1);
	}
}