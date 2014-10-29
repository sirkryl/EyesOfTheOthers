using UnityEngine;
using System.Collections;

public abstract class Consumable : ThrowableItem
{
	public int amount = 0;
	public abstract void HandleConsume();

	protected void Start()
	{
		base.Start ();
		type = "Consumable";
		effect = "+"+amount+" ";
	}
}

