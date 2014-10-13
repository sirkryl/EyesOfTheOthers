using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class DisplayWindows : MonoBehaviour {

	public Canvas inventoryCanvas;
	public Canvas debugCanvas;
	public Canvas characterCanvas;
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
			DeactivateOtherWindows("i");
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
		else if (Input.GetKeyUp("c"))
		{
			DeactivateOtherWindows("c");
			characterCanvas.GetComponent<Canvas>().enabled = !characterCanvas.GetComponent<Canvas>().enabled;
			characterCanvas.GetComponent<GraphicRaycaster>().enabled = !characterCanvas.GetComponent<GraphicRaycaster>().enabled;
			if(characterCanvas.enabled)
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

	void DeactivateOtherWindows(string hotkey)
	{
		if (hotkey != "c")
		{
			characterCanvas.GetComponent<Canvas>().enabled = false;
			characterCanvas.GetComponent<GraphicRaycaster>().enabled = false;
		}
		if (hotkey != "i")
		{
			inventoryCanvas.GetComponent<Canvas>().enabled = false;
			inventoryCanvas.GetComponent<GraphicRaycaster>().enabled = false;
		}
	}
}
