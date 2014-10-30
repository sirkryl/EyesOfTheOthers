using UnityEngine;
using System.Collections;

public class Gun : Weapon
{

	// Use this for initialization
	protected void Start ()
	{
		base.Start ();
	}

	// Update is called once per frame
	protected void Update ()
	{
		if (shotDelayTimer > 0)
			shotDelayTimer -= Time.deltaTime;
	}

	public override void HandleAttack()
	{
		if (shotDelayTimer <= 0)
		{
			Instantiate(Resources.Load ("Prefabs/Bullets/bullet"), GameObject.FindWithTag("PlayerHand").transform.position, GameObject.FindWithTag("PlayerHand").transform.rotation);
			shotDelayTimer = shotDelay;
		}
	}
}

