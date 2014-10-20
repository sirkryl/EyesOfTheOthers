using UnityEngine;
using System.Collections;

public abstract class Equipable : Item
{
	
	// Use this for initialization
	protected void Start ()
	{
		base.Start ();
		type = "Equipable";
	}

	public abstract void HandleEquip();
}

