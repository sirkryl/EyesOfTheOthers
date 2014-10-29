using UnityEngine;
using System.Collections;

public class Weapon : Equipable
{
	public int damage = 0;
		// Use this for initialization
	protected void Start ()
	{
		base.Start ();
		type += " (Weapon)";
		effect = damage + " Damage";
	}

	public override void HandleEquip()
	{
		//equip weapon
	}
}

