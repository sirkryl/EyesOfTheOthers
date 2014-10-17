using UnityEngine;
using System.Collections;

public class Item : Selectable {

	//very likely that something like an ItemData struct with attributes will be better to avoid multiple active models
	public Sprite icon;
	public string type;
	public int stackSize = 1;
	public string description;	
	//public bool pickUp = true;
	public bool stackable = false;
	//probably with an extra script for consumables
	public bool consumable = false;
	public int maxStackSize = 10;

	private Inventory inventory;

	// Use this for initialization
	void Start () {
		highlight = "Press 'E' to take "+name+".";
		inventory = GameObject.FindWithTag ("Inventory").GetComponent<Inventory>();

		icon = Resources.Load<Sprite>("Textures/"+name);
		if (icon == null)
		{
			icon = Resources.Load<Sprite>("Textures/Apple");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void HandleSelection () {
			if (name == "Apple")
				GlobalVariableManager.SharedInstance.SetGlobalVariable("tookApple",GlobalVariableManager.SharedInstance.GetGlobalVariable("tookApple")+1);
			
			inventory.AddItem (this);
			gameObject.SetActive (false);
	}

	public void Use()
	{
		//use item
	}

	public void PickUp()
	{
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
