using UnityEngine;
using UnityEngine.UI;
using System.Collections;
//using System.Collections.Generic;
public class Inventory : MonoBehaviour {

	//will likely change to something like an ItemData[]
	public Transform[] contents;
	private bool justPickedUp = false;
	private string newItemName;
	private float timer = 3;
	private ArrayList items;
	public Image itemList;
	public Image itemLabelFab;
	public GameObject infoPanel;
	public GameObject imagePanel;
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

		Image itemListElement = Instantiate (itemLabelFab, new Vector3(0,-10-((items.Count-1)*18),0), Quaternion.identity) as Image;
		itemListElement.transform.SetParent (itemList.transform, false);
		itemListElement.GetComponentInChildren<Text>().enabled = true;
		itemListElement.GetComponentInChildren<Text>().text = item.name + " (1)";

		itemListElement.GetComponent<Button>().onClick.AddListener(() => { GUIShowItemInfo(itemListElement.GetComponent<Item>()); });
		itemListElement.GetComponent<Item>().name = item.name;
		itemListElement.GetComponent<Item>().type = item.type;
		itemListElement.GetComponent<Item>().description = item.description;
		itemListElement.GetComponent<Item>().icon = item.icon;
		//Debug.Log ("itemList height: "+itemList.rectTransform.sizeDelta.x+" oder "+itemList.rectTransform.sizeDelta.y);
	}

	public void RemoveItem()
	{
		//remove item
	}

	public void GUIShowItemInfo(Item item)
	{
		infoPanel.GetComponent<Mask>().enabled = false;
		imagePanel.GetComponent<Mask>().enabled = false;
		itemName.text = item.name;
		//itemImage.sprite = Sprite.
		itemType.text = item.type;
		itemEffect.text = "No effect.";
		itemDescription.text = item.description;
	}
}
