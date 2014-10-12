using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Inventory : MonoBehaviour {

	//will likely change to something like an ItemData[]
	public Transform[] contents;
	private bool justPickedUp = false;
	private string newItemName;
	private float timer = 3;
	private ArrayList items;
	public GameObject itemList;
	public Transform itemLabelFab;
	public GameObject infoPanel;
	public Text itemName; 
	public Image itemImage;
	public Text itemType;
	public Text itemEffect;
	public Text itemDescription;
	// Use this for initialization
	void Start () {
		items = new ArrayList();
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
		items.Add (item);
		justPickedUp = true;
		/* buggy
		Transform itemLabel = Instantiate (itemLabelFab) as Transform;
		itemLabel.parent = itemList.transform;*/

		//temporary measure
		if (items.Count <= 4)
		{
			string labelName = "Item" + items.Count + "Label";
			GameObject label = GameObject.Find (labelName);
			label.GetComponent<Text>().enabled = true;
			label.GetComponent<Button>().enabled = true;
			label.GetComponent<Text>().text = item.name;
			label.GetComponent<Item>().name = item.name;
			label.GetComponent<Item>().type = item.type;
			label.GetComponent<Item>().description = item.description;
			label.GetComponent<Item>().icon = item.icon;
		}
		//label.GComponent<Item>() = item;
		//itemLabel.text = item.name;
		//addItem
	}

	public void RemoveItem()
	{
		//remove item
	}

	public void GUIShowItemInfo(Item item)
	{
		infoPanel.GetComponent<Mask>().enabled = false;
		itemName.text = item.name;
		//itemImage.sprite = Sprite.
		itemType.text = item.type;
		itemEffect.text = "No effect.";
		itemDescription.text = item.description;
	}
}
