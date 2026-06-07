// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomsDeepEnter : MonoBehaviour
{
	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			S.SM.Save($"Was in MR", true);
			S.MM.EnterMushrooms();
		}
	}
}