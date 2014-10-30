using UnityEngine;
using System.Collections;

public class PlayerSelection : MonoBehaviour {

	//private ThrowableItem handItem;

	// Update is called once per frame
	void Update () {
		if (StateManager.SharedInstance.gameState == GameState.Free)
		{
			//raycast for dialog objects
			Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
			RaycastHit hit;// = new RaycastHit();
			Debug.DrawRay (ray.origin, ray.direction * 2, Color.green);
			if(Physics.Raycast(ray, out hit, 2))
			{
				Selectable selectedObject = hit.transform.gameObject.GetComponent<Selectable>();
				if(selectedObject != null)
				{
					if (Input.GetKeyDown (KeyCode.E))
					{
						selectedObject.HandleSelection();
						GUIManager.SharedInstance.HideInteractionOverlay();
					}
					if(selectedObject is ThrowableItem)
					{
						if (Input.GetKeyDown (KeyCode.F))
						{
							((ThrowableItem)selectedObject).HandlePickUp();
							PlayerManager.SharedInstance.handItem = (ThrowableItem)selectedObject;
						}
					}
					//Consumable consumable = hit.transform.gameObject.GetComponent<Consumable>();
					if(selectedObject is Consumable)
					{
						if (Input.GetKeyDown (KeyCode.Z))
						{
							((Consumable)selectedObject).HandleConsume();
							//PlayerManager.SharedInstance.handItem = (ThrowableItem)selectedObject;
						}
					}
					GUIManager.SharedInstance.ShowInteractionOverlay(selectedObject.highlight);
				}
			}
			else
			{
				GUIManager.SharedInstance.HideInteractionOverlay();

				if(PlayerManager.SharedInstance.handItem != null)
				{
					if (PlayerManager.SharedInstance.handItem is ThrowableItem)
					{
						if (Input.GetMouseButtonUp(0))
						{
							((ThrowableItem)PlayerManager.SharedInstance.handItem).HandleThrow();
							PlayerManager.SharedInstance.handItem = null;
						}
						else if (Input.GetMouseButtonUp(1) || Input.GetKeyDown(KeyCode.F))
						{
							((ThrowableItem)PlayerManager.SharedInstance.handItem).HandleDrop ();
							PlayerManager.SharedInstance.handItem = null;
						}
						else if (Input.GetKeyDown(KeyCode.E))
						{
							gameObject.GetComponentInChildren<Inventory>().AddItem(PlayerManager.SharedInstance.handItem);
							PlayerManager.SharedInstance.handItem.gameObject.SetActive(false);
							PlayerManager.SharedInstance.handItem = null;
						}
					}
					else if (PlayerManager.SharedInstance.handItem is Equipable)
					{
						if (Input.GetMouseButton(0) && PlayerManager.SharedInstance.handItem is Weapon)
						{
							//PlayerManager.SharedInstance.handItem.gameObject.SetActive(false);
							((Weapon)PlayerManager.SharedInstance.handItem).HandleAttack ();
						}
						if (Input.GetMouseButtonUp(1))
						{
							PlayerManager.SharedInstance.handItem.gameObject.SetActive(false);
							PlayerManager.SharedInstance.handItem = null;
						}
					}
				}

			}
		}

	}
}
