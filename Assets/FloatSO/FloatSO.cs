using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FloatSO : ScriptableObject
{
	public int myVar;

	public int MyProperty
	{
		get { return myVar; }
		set { myVar = value; }
	}

}
