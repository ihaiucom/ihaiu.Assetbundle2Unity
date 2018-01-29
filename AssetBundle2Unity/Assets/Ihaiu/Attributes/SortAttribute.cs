using UnityEngine;
using System.Collections;
using System;

public class SortAttribute : Attribute
{
	public readonly int val = 0;

	
	public SortAttribute(int val)
	{
		this.val = val;
	}
}
