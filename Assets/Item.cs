using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	//very likely that something like an ItemData struct with attributes will be better to avoid multiple active models
	public Texture2D icon;
	public string name;
	public bool stackable = false;
	//probably with an extra script for consumables
	public bool consumable = false;
	public int maxStackSize = 10;
	private GameObject player;
	static Inventory inventory;
	// Use this for initialization
	void Start () {
		GameObject invObject = GameObject.FindWithTag ("Inventory");
		inventory = invObject.GetComponent<Inventory>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Use()
	{
		//use item
	}

	public void PickUp()
	{
		if(stackable)
		{
			//do stuff
		}
		inventory.AddItem (this);
		//add to players inventory

	}

	public void Drop()
	{
		//drop item
	}

	public void Place()
	{
		//place item
	}
}
