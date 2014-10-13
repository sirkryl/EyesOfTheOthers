using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Dialog : MonoBehaviour {
	private GameObject player;
	public KeyCode pressButton = KeyCode.E;
	private Camera mainCamera;
	private bool inDialogRange = false;
	public float dialogDistance = 2.0f;
	static float closestDialogDistance = 999.0f;
	static GameObject bestOption;
	//public Canvas dialogCanvas;
	private float distance;
	// Use this for initialization
	void Start () {
		mainCamera = Camera.main;
		player = GameObject.FindWithTag ("Player");

	}

	void OnGUI() {
		
		if (inDialogRange)
		{
			GUI.Box (new Rect ((float)(Screen.width*0.5-100),(float)(Screen.height*0.5-20),200,20), "Press E to talk.");
		}
	}

	// Update is called once per frame
	void Update () {
		if (player != null)
		{
			distance = Vector3.Distance (player.transform.position, transform.position);
			Vector3 viewPortCoords = mainCamera.WorldToViewportPoint(transform.position);
			if (distance <= dialogDistance 
			    && viewPortCoords.x >= 0 && viewPortCoords.x <= 1 
			    && viewPortCoords.y >= 0 && viewPortCoords.y <= 1)
			{
				if (distance <= closestDialogDistance)
				{
					closestDialogDistance = distance;
					bestOption = gameObject;
				}
				inDialogRange = true;
			}
			else inDialogRange = false;
			
			if (Input.GetKeyDown (pressButton) && inDialogRange)
			{
				if (bestOption == gameObject)
				{
					player.GetComponent<DisplayWindows>().ShowDialogWindow();
					//player.GetComponent<MouseLook>().enabled = false;;
					//mainCamera.GetComponent<MouseLook>().enabled = false;
					//dialogCanvas.GetComponent<Canvas>().enabled = true;
					//dialogCanvas.GetComponent<GraphicRaycaster>().enabled = true;
					inDialogRange = false;
					//gameObject.SetActive (false);
					closestDialogDistance = 999.0f;
				}
			}
		}
	}

	/*public void HideDialogCanvas()
	{
		dialogCanvas.GetComponent<Canvas>().enabled = false;
		dialogCanvas.GetComponent<GraphicRaycaster>().enabled = false;
		player.GetComponent<MouseLook>().enabled = true;;
		mainCamera.GetComponent<MouseLook>().enabled = true;
	}*/
}
