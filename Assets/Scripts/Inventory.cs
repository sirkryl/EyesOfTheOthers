using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using System.Collections.Generic;
public delegate void OnInventoryChangeHandler(Item item);

[System.Serializable] 
class ItemStorage {
	public Hashtable playerInventory;
}

public class Inventory : MonoBehaviour {

	//will likely change to something like an ItemData[]
	public event OnInventoryChangeHandler OnItemAdded;
	public event OnInventoryChangeHandler OnItemRemoved;

	private Dictionary<string, Item> itemMap;

	void Awake ()
	{
		itemMap = new Dictionary<string, Item>();
	}
	// Use this for initialization
	void Start () {
	}


	public void AddItem(Item item)
	{
		if (itemMap.ContainsKey(item.id_string))
		{
			itemMap[item.id_string].stackSize += item.stackSize;
		}
		else	
		{
			itemMap.Add (item.id_string, item);

		}
		if(OnItemAdded!=null) {
			OnItemAdded(itemMap[item.id_string]);
		}
		//HandleOnItemAdded


		//Debug.Log ("itemList height: "+itemList.rectTransform.sizeDelta.x+" oder "+itemList.rectTransform.sizeDelta.y);
	}

	public void RemoveItem(Item item)
	{
		if (itemMap[item.id_string].stackSize > 1)
		{
			itemMap[item.id_string].stackSize -= 1;
			//itemUIMap[item.id_string].GetComponent<Item>().stackSize = itemMap[item.id_string].stackSize;
		}
		else
		{
			itemMap.Remove (item.id_string);
			
		}
		if(OnItemRemoved!=null) {
			OnItemRemoved(item);
		}
		
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

	public void EquipItem(Item item)
	{
		((Equipable)item).HandleEquip ();
	}

	public Item GetItem(string name)
	{
		if(itemMap.ContainsKey (name))
			return itemMap[name];
		return null;
	}

	public int GetStackSize(Item item)
	{
		if (itemMap.ContainsKey (item.id_string))
			return itemMap[item.id_string].stackSize;
		else
			return -1;
	}

	public bool GotItem(string name)
	{
		return itemMap.ContainsKey (name);
	}
}
