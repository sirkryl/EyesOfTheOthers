using UnityEngine;
using System.Collections;

public class Food : Consumable
{
	public override void HandleConsume()
	{
		PlayerManager.SharedInstance.playerData.hunger+=amount;
		//GameObject.FindWithTag ("Inventory").GetComponent<Inventory>().RemoveItem (this);
		gameObject.SetActive (false);
	}

	protected void Start()
	{
		base.Start();
		type += " (Food)";
		effect += " Hunger";
	}
}

