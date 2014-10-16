using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour {

	public KeyCode pressButton = KeyCode.E;
	public float pickUpDistance = 2.0f;
	private GameObject player;
	private Camera mainCamera;
	private Item item;
	private bool inPickUpRange = false;
	private float distance;
	private bool pickedUp = false;
	static GameObject bestOption;
	static Inventory inventory;
	static float closestPickUpDistance = 999.0f;
	private GameObject sceneManager;
	// Use this for initialization
	void Start () {
		sceneManager = GameObject.FindWithTag ("SceneManager");
		mainCamera = Camera.main;
		player = GameObject.FindWithTag ("Player");
		item = (GetComponent<Item>());
		GameObject invObject = GameObject.FindWithTag ("Inventory");
		inventory = invObject.GetComponent<Inventory>();
	}

	void OnGUI() {

		if (inPickUpRange)
		{
			//sceneManager.GetComponent<DisplayWindows>().ShowInteractionOverlay("Press 'E' to take "+item.name);
			//GUI.Box (new Rect ((float)(Screen.width*0.5-100),(float)(Screen.height*0.5-20),200,20), "Press E to pick up item.");
		}
		else if (pickedUp)
		{
			//not working because object is getting destroyed at pickup
			//sceneManager.GetComponent<DisplayWindows>().HideInteractionOverlay();
			//GUI.Box (new Rect ((float)(Screen.width*0.5-100),(float)(Screen.height*0.5-20),200,20), "Picked up " + item.name + "!");
		}
	}

	// Update is called once per frame
	void Update () {
		/*if (player != null)
		{
			distance = Vector3.Distance (player.transform.position, transform.position);
			Vector3 viewPortCoords = mainCamera.WorldToViewportPoint(transform.position);
			if (distance <= pickUpDistance 
			    && viewPortCoords.x >= 0 && viewPortCoords.x <= 1 
			    && viewPortCoords.y >= 0 && viewPortCoords.y <= 1)
			{
				if (distance <= closestPickUpDistance)
				{
					closestPickUpDistance = distance;
					bestOption = gameObject;
				}
				inPickUpRange = true;
			}
			else inPickUpRange = false;

			if (Input.GetKeyDown (pressButton) && inPickUpRange)
			{
				if (bestOption == gameObject)
				{
					inventory.AddItem (item);
					pickedUp = true;
					inPickUpRange = false;
					//sceneManager.GetComponent<DisplayWindows>().HideInteractionOverlay();
					gameObject.SetActive (false);
					closestPickUpDistance = 999.0f;
				}
			}
		}*/
	}

}
