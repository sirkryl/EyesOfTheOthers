﻿using UnityEngine;
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
	private GameObject inventoryEmptyFab;
	private GameObject inventoryEmptyLabel;
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
		inventoryEmptyFab = Resources.Load ("Prefabs/GUI/itemListEmpty",typeof(GameObject)) as GameObject;
		inventoryEmptyLabel = Instantiate(inventoryEmptyFab) as GameObject;
		inventoryEmptyLabel.transform.SetParent (itemList.transform, false);
		GameObject.Find ("HandButton").GetComponent<Button>().interactable = false;
		GameObject.Find ("UseButton").GetComponent<Button>().interactable = false;
		GameObject.Find ("HandButton").GetComponentInChildren<Text>().enabled = false;
		GameObject.Find ("UseButton").GetComponentInChildren<Text>().enabled = false;
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
		if (itemMap.ContainsKey(item.id_string))
		{
			itemMap[item.id_string].stackSize += item.stackSize;
		}
		else	
		{
			itemMap.Add (item.id_string, item);
			Destroy(inventoryEmptyLabel);
		}

		if (itemUIMap.ContainsKey (item.id_string))
		{
			itemUIMap[item.id_string].GetComponentInChildren<Text>().text = item.name + " (" + itemMap[item.id_string].stackSize + ")";
			//itemUIMap[item.id_string].GetComponent<Item>().stackSize = itemMap[item.id_string].stackSize;
		}
		else
		{
			Image itemListElement = Instantiate (itemLabelFab, new Vector3(0,-10-((items.Count-1)*18),0), Quaternion.identity) as Image;
			itemListElement.transform.SetParent (itemList.transform, false);
			itemListElement.GetComponentInChildren<Text>().enabled = true;
			itemListElement.GetComponentInChildren<Text>().text = item.name + " (" + item.stackSize + ")";
			//itemListElement.GetComponent<Button>().onClick.AddListener(() => { GUIShowItemInfo(itemListElement.GetComponent<Item>()); });
			itemListElement.GetComponent<Button>().onClick.AddListener(() => { GUIShowItemInfo(itemMap[item.id_string]); });

			//itemListElement.GetComponent<Item>().name = item.id_string;
			itemUIMap.Add (item.id_string, itemListElement);
		}

		//Debug.Log ("itemList height: "+itemList.rectTransform.sizeDelta.x+" oder "+itemList.rectTransform.sizeDelta.y);
	}

	public void SelectItem(Item item)
	{
		if(item != null && PlayerManager.SharedInstance.handItem == null)
		{
			GameObject itemInHand = Instantiate(Resources.Load ("Prefabs/Items/"+item.id_string, typeof(GameObject))) as GameObject;
			//itemInHand.AddComponent<item.GetType ()>();
			//itemInHand.GetComponent<ThrowableItem>().name = item.id_string;
			//itemInHand.GetComponent<ThrowableItem>().type = item.GetItemType();
			//itemInHand.GetComponent<ThrowableItem>().description = item.description;
			//itemInHand.GetComponent<ThrowableItem>().icon = item.icon;
			//itemInHand.GetComponent<ThrowableItem>().stackSize = 1;
			PlayerManager.SharedInstance.handItem = itemInHand.GetComponent<ThrowableItem>();
			PlayerManager.SharedInstance.handItem.HandlePickUp();
			RemoveItem(item);
			//GUIManager.SharedInstance.DeactiveAllWindows();
		}
	}

	public void ConsumeItem(Item item)
	{
		if(item != null)
		{
			((Consumable)item).HandleConsume();
			RemoveItem(item);
		}
	}


	public void RemoveItem(Item item)
	{
		if (itemMap[item.id_string].stackSize > 1)
		{
			itemMap[item.id_string].stackSize -= 1;
			itemUIMap[item.id_string].GetComponentInChildren<Text>().text = item.name + " (" + itemMap[item.id_string].stackSize + ")";
			//itemUIMap[item.id_string].GetComponent<Item>().stackSize = itemMap[item.id_string].stackSize;
		}
		else
		{
			itemMap.Remove (item.id_string);
			Image itemListElement = itemUIMap[item.id_string];
			itemUIMap.Remove (item.id_string);
			Destroy(itemListElement.gameObject);
			if(itemMap.Count == 0)
			{
				inventoryEmptyFab = Resources.Load ("Prefabs/GUI/itemListEmpty",typeof(GameObject)) as GameObject;
				inventoryEmptyLabel = Instantiate(inventoryEmptyFab) as GameObject;
				inventoryEmptyLabel.transform.SetParent (itemList.transform, false);
			}
			GameObject.Find ("HandButton").GetComponent<Button>().interactable = false;
			GameObject.Find ("UseButton").GetComponent<Button>().interactable = false;
			GameObject.Find ("HandButton").GetComponentInChildren<Text>().enabled = false;
			GameObject.Find ("UseButton").GetComponentInChildren<Text>().enabled = false;
			//GameObject.Find ("HandButton").GetComponent<Mask>().enabled = true;
			//GameObject.Find ("HandButton").GetComponent<Image>().enabled = false;
			//GameObject.Find ("UseButton").GetComponent<Mask>().enabled = true;
			//GameObject.Find ("UseButton").GetComponent<Image>().enabled = false;
			maskItemInfo.GetComponent<Image>().enabled = false;
		}

	}

	public void EquipItem(Item item)
	{
		((Equipable)item).HandleEquip ();
	}

	public bool GotItem(string name)
	{
		return itemMap.ContainsKey (name);
	}

	public void GUIShowItemInfo(Item item)
	{
		maskItemInfo.GetComponent<Image>().enabled = true;
		//imagePanel.GetComponent<Mask>().enabled = false;
		itemName.text = itemMap[item.id_string].name;
		//itemImage.sprite = Sprite.
		itemType.text = itemMap[item.id_string].GetItemType();
		itemEffect.text = itemMap[item.id_string].GetItemEffect ();
		itemDescription.text = itemMap[item.id_string].description;
		GameObject.Find ("HandButton").GetComponent<Button>().onClick.RemoveAllListeners();
		GameObject.Find ("UseButton").GetComponent<Button>().onClick.RemoveAllListeners();
		GameObject.Find ("UseButton").GetComponent<Button>().interactable = true;
		GameObject.Find ("UseButton").GetComponentInChildren<Text>().enabled = true;
		if(itemMap[item.id_string] is ThrowableItem)
		{
			GameObject.Find ("HandButton").GetComponent<Button>().interactable = true;
			GameObject.Find ("HandButton").GetComponentInChildren<Text>().enabled = true;
			//GameObject.Find ("HandButton").GetComponent<Image>().enabled = true;
			GameObject.Find ("HandButton").GetComponent<Button>().onClick.AddListener(() => { SelectItem(itemMap[item.id_string]); });
		}
		else
		{
			GameObject.Find ("HandButton").GetComponent<Button>().interactable = false;
			GameObject.Find ("HandButton").GetComponentInChildren<Text>().enabled = false;
			//GameObject.Find ("HandButton").GetComponent<Image>().enabled = false;
		}

		if(itemMap[item.id_string] is Consumable)
		{
			GameObject.Find ("UseButton").GetComponentInChildren<Text>().text = "Consume";
			GameObject.Find ("UseButton").GetComponent<Button>().onClick.AddListener(() => { ConsumeItem(itemMap[item.id_string]); });
		}
		else if(itemMap[item.id_string] is Equipable)
		{
			GameObject.Find ("UseButton").GetComponentInChildren<Text>().text = "Equip";
			GameObject.Find ("UseButton").GetComponent<Button>().onClick.AddListener(() => { EquipItem(itemMap[item.id_string]); });
		}
		GameObject.FindWithTag ("ItemImage").GetComponent<Image>().sprite = itemMap[item.id_string].icon;
	}
}
