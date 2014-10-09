using UnityEngine;
using System.Collections;

public class Inventory : MonoBehaviour {

	//will likely change to something like an ItemData[]
	public Transform[] contents;
	private bool justPickedUp = false;
	private string newItemName;
	private float timer = 3;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (justPickedUp)
		{

		}
	}

	void OnGUI() {
		if (justPickedUp)
		{
			timer -= Time.deltaTime;
			if (timer > 0)
				GUI.Box (new Rect ((float)(Screen.width*0.5-100),(float)(Screen.height*0.5-20),200,20), "Picked up " + newItemName + " !");
			else
			{
				justPickedUp = false;
				timer = 3;
			}
		}
	}


	public void AddItem(Item item)
	{
		newItemName = item.name;
		justPickedUp = true;
		//addItem
	}

	public void RemoveItem()
	{
		//remove item
	}
}
