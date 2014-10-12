using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class DisplayWindows : MonoBehaviour {

	public Canvas inventoryCanvas;
	public Canvas debugCanvas;
	public GameObject infoPanel;
	public GameObject player;
	public GameObject camera;

	private MouseLook playerMouseLook;
	private MouseLook cameraMouseLook;
	
	// Use this for initialization
	void Start () {
		infoPanel.GetComponent<Mask>().enabled = true;
		playerMouseLook = player.GetComponent<MouseLook>();
		cameraMouseLook = camera.GetComponent<MouseLook>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp("f10"))
		{
			debugCanvas.enabled = !debugCanvas.enabled;
			if(debugCanvas.enabled)
			{
				playerMouseLook.enabled = false;
				cameraMouseLook.enabled = false;
			}
			else
			{
				playerMouseLook.enabled = true;
				cameraMouseLook.enabled = true;
			}

		}
		else if (Input.GetKeyUp("i"))
		{
			inventoryCanvas.GetComponent<Canvas>().enabled = !inventoryCanvas.GetComponent<Canvas>().enabled;
			inventoryCanvas.GetComponent<GraphicRaycaster>().enabled = !inventoryCanvas.GetComponent<GraphicRaycaster>().enabled;
			if(inventoryCanvas.enabled)
			{
				playerMouseLook.enabled = false;
				cameraMouseLook.enabled = false;
			}
			else
			{
				playerMouseLook.enabled = true;
				cameraMouseLook.enabled = true;
			}
		}
	}
}
