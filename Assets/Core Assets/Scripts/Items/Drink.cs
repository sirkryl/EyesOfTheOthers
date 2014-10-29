using UnityEngine;
using System.Collections;

public class Drink : Consumable
{
	public override void HandleConsume()
	{
		PlayerManager.SharedInstance.playerData.thirst+=amount;
		//GameObject.FindWithTag ("Inventory").GetComponent<Inventory>().RemoveItem (this);
		gameObject.SetActive (false);
	}

	protected void Start()
	{
		base.Start();
		type += " (Drink)";
		effect += " Thirst";
	}
}

