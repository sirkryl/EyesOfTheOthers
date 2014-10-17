using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using System.Collections.Generic;
public class Inventory : MonoBehaviour {

	//will likely change to something like an ItemData[]
	public Transform[] contents;

	private ArrayList items;
	public Image itemList;
	public Image itemLabelFab;
	public GameObject maskItemInfo;
	private Dictionary<string, Item> itemMap;
	private Dictionary<string, Image> itemUIMap;
	public Text itemName; 
	//public Image itemImage;
	public Text itemType;
	public Text itemEffect;
	public Text itemDescription;
	// Use this for initialization
	void Start () {
		items = new ArrayList();
		itemMap = new Dictionary<string, Item>();
		itemUIMap = new Dictionary<string, Image>();
	}
	
	// Update is called once per frame
	void Update () {
	}

	/*void OnGUI() {
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
	}*/

	public void AddItem(Item item)
	{
		if (itemMap.ContainsKey(item.name))
		{
			itemMap[item.name].stackSize += item.stackSize;
		}
		else		
			itemMap.Add (item.name, item);

		if (itemUIMap.ContainsKey (item.name))
		{
			itemUIMap[item.name].GetComponentInChildren<Text>().text = item.name + " (" + itemMap[item.name].stackSize + ")";
			itemUIMap[item.name].GetComponent<Item>().stackSize = itemMap[item.name].stackSize;
		}
		else
		{
			Image itemListElement = Instantiate (itemLabelFab, new Vector3(0,-10-((items.Count-1)*18),0), Quaternion.identity) as Image;
			itemListElement.transform.SetParent (itemList.transform, false);
			itemListElement.GetComponentInChildren<Text>().enabled = true;
			itemListElement.GetComponentInChildren<Text>().text = item.name + " (" + item.stackSize + ")";
			itemListElement.GetComponent<Button>().onClick.AddListener(() => { GUIShowItemInfo(itemListElement.GetComponent<Item>()); });
			itemListElement.GetComponent<Item>().name = item.name;
			itemListElement.GetComponent<Item>().type = item.type;
			itemListElement.GetComponent<Item>().description = item.description;
			itemListElement.GetComponent<Item>().icon = item.icon;
			itemListElement.GetComponent<Item>().stackSize = item.stackSize;
			itemUIMap.Add (item.name, itemListElement);
		}

		//Debug.Log ("itemList height: "+itemList.rectTransform.sizeDelta.x+" oder "+itemList.rectTransform.sizeDelta.y);
	}

	public void RemoveItem()
	{
		//remove item
	}

	public bool GotItem(string name)
	{
		return itemMap.ContainsKey (name);
	}

	public void GUIShowItemInfo(Item item)
	{
		maskItemInfo.GetComponent<Image>().enabled = true;
		//imagePanel.GetComponent<Mask>().enabled = false;
		itemName.text = item.name;
		//itemImage.sprite = Sprite.
		itemType.text = item.type;
		itemEffect.text = "No effect.";
		itemDescription.text = item.description;


		GameObject.FindWithTag ("ItemImage").GetComponent<Image>().sprite = item.icon;
	}
}
