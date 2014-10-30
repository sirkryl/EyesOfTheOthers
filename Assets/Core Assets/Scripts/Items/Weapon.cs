using UnityEngine;
using System.Collections;

public abstract class Weapon : Equipable
{
	public int damage = 0;
	public float shotDelay = 0.5f;
	protected float shotDelayTimer;
		// Use this for initialization
	protected void Start ()
	{
		base.Start ();
		type += " (Weapon)";
		effect = damage + " Damage";
		shotDelayTimer = shotDelay;
	}

	public override void HandleEquip()
	{
		gameObject.transform.parent = GameObject.FindWithTag("PlayerHand").transform;
		gameObject.transform.localPosition = new Vector3(0,0,0);
		gameObject.transform.rotation = GameObject.FindWithTag("PlayerHand").transform.rotation;
		gameObject.rigidbody.isKinematic = true;
		gameObject.collider.enabled = false;
	}

	public abstract void HandleAttack();
}

