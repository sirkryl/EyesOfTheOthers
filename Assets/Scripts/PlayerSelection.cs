using UnityEngine;
using System.Collections;

public class PlayerSelection : MonoBehaviour {

	private Dialog activeObject;
	private GameObject sceneManager;
	private Inventory inventory;

	//Ray ray;
	// Use this for initialization
	void Start () {
		sceneManager = GameObject.FindWithTag ("SceneManager");
		inventory = GameObject.FindWithTag ("Inventory").GetComponent<Inventory>();
		//Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
	}
	
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
				//sceneManager.GetComponent<DisplayWindows>().HideInteractionOverlay();
				//Debug.Log (
				if(activeObject != hit.transform.gameObject && activeObject != null)
				{
					GUIManager.SharedInstance.HideInteractionOverlay();
					activeObject.isActiveObject = false;
					Dialog.isAnythingActive = false;
				}
				if(hit.transform.gameObject.GetComponent<Dialog>() != null)
				{
					//Debug.Log ("hit "+hit.transform.gameObject.GetComponent<Dialog>().name);
					activeObject = hit.transform.gameObject.GetComponent<Dialog>();
					hit.transform.gameObject.GetComponent<Dialog>().isActiveObject = true;
					Dialog.isAnythingActive = true;
					GUIManager.SharedInstance.ShowInteractionOverlay("Press 'E' to talk.");
				}
				if(hit.transform.gameObject.GetComponent<Item>() != null)
				{
					if (hit.transform.gameObject.GetComponent<Item>().pickUp)
					{
						//Debug.Log ("hit "+hit.transform.gameObject.GetComponent<Dialog>().name);
						GUIManager.SharedInstance.ShowInteractionOverlay("Press 'E' to take "+hit.transform.gameObject.GetComponent<Item>().name+".");
						if (Input.GetKeyDown (KeyCode.E))
						{
							if (hit.transform.gameObject.GetComponent<Item>().name == "Apple")
								GlobalVariableManager.SharedInstance.SetGlobalVariable("tookApple",GlobalVariableManager.SharedInstance.GetGlobalVariable("tookApple")+1);
							inventory.AddItem (hit.transform.gameObject.GetComponent<Item>());
							hit.transform.gameObject.SetActive (false);
							GUIManager.SharedInstance.HideInteractionOverlay();
						}
					}
				}
				else
				{
					//sceneManager.GetComponent<DisplayWindows>().HideInteractionOverlay();
				}

				//else Debug.Log ("didnt it "+name);
			}
			else
			{
				GUIManager.SharedInstance.HideInteractionOverlay();
				if(activeObject != null && activeObject.GetComponent<Dialog>() != null)
				{
					//sceneManager.GetComponent<DisplayWindows>().HideInteractionOverlay();
					Dialog.isAnythingActive = false;
					activeObject.isActiveObject = false;
				}
			}
		}

	}
}
