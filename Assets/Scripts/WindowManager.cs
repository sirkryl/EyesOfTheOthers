﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class WindowManager : MonoBehaviour {


	//private static WindowManager instance = null;
	public static WindowManager SharedInstance { get; private set; }

	public Canvas inventoryCanvas;
	public Canvas debugCanvas;
	public Canvas characterCanvas;
	public Canvas overlayCanvas;
	public List<Canvas> allCanvas;
	public GameObject infoPanel;
	public GameObject imagePanel;
	public GameObject player;
	public GameObject camera;
	public GameObject interactionOverlay;
	public Canvas dialogCanvas;
	public bool alternativeDialogCanvas = false;
	private MouseLook playerMouseLook;
	private MouseLook cameraMouseLook;

	void Awake()
	{
		if(SharedInstance != null && SharedInstance != this)
			Destroy(gameObject);

		SharedInstance = this;
		DontDestroyOnLoad(gameObject);
	}

	// Use this for initialization
	void Start () {
		allCanvas = new List<Canvas>();
		StateManager.SharedInstance.OnStateChange += HandleOnStateChange;
		infoPanel.GetComponent<Mask>().enabled = true;
		imagePanel.GetComponent<Mask>().enabled = true;
		playerMouseLook = player.GetComponent<MouseLook>();
		allCanvas.Add (inventoryCanvas);
		allCanvas.Add (characterCanvas);
		allCanvas.Add (overlayCanvas);
		cameraMouseLook = camera.GetComponent<MouseLook>();
		MainComponentManager.CreateInstance ();
		PlayerManager.SharedInstance.Load ();
		//GlobalState.gameState.StartState();
	}

	void HandleOnStateChange ()
	{
		if (StateManager.SharedInstance.gameState == GameState.Dialog)
		{
			dialogCanvas.GetComponent<Canvas>().enabled = true;
			dialogCanvas.GetComponent<GraphicRaycaster>().enabled = true;
			playerMouseLook.enabled = false;
			cameraMouseLook.enabled = false;
			player.GetComponent<CharacterMotor>().enabled = false;
			if(overlayCanvas.GetComponent<Canvas>().enabled)
			{
				overlayCanvas.GetComponent<Canvas>().enabled = false;
				overlayCanvas.GetComponent<GraphicRaycaster>().enabled = false;
			}
		}
		else if (StateManager.SharedInstance.gameState == GameState.Free)
		{
			if (dialogCanvas.GetComponent<Canvas>().enabled)
			{
				dialogCanvas.GetComponent<Canvas>().enabled = false;
				dialogCanvas.GetComponent<GraphicRaycaster>().enabled = false;
			}
			playerMouseLook.enabled = true;
			cameraMouseLook.enabled = true;
			player.GetComponent<CharacterMotor>().enabled = true;
			if(!overlayCanvas.GetComponent<Canvas>().enabled)
			{
				overlayCanvas.GetComponent<Canvas>().enabled = true;
				overlayCanvas.GetComponent<GraphicRaycaster>().enabled = true;
			}
		}
		else if (StateManager.SharedInstance.gameState == GameState.Interface)
		{
			playerMouseLook.enabled = false;
			cameraMouseLook.enabled = false;
			//player.GetComponent<CharacterMotor>().enabled = false;

		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp("f10"))
		{
			debugCanvas.enabled = !debugCanvas.enabled;

		}
		else if (Input.GetKeyUp("i"))
		{
			//PlayerManager.SharedInstance.Save ();
			GlobalVariableManager.SharedInstance.SaveVariables ();
			DeactivateAllOtherWindows(inventoryCanvas);

			infoPanel.GetComponent<Mask>().enabled = true;
			imagePanel.GetComponent<Mask>().enabled = true;

			inventoryCanvas.GetComponent<Canvas>().enabled = !inventoryCanvas.GetComponent<Canvas>().enabled;
			inventoryCanvas.GetComponent<GraphicRaycaster>().enabled = !inventoryCanvas.GetComponent<GraphicRaycaster>().enabled;

			if(inventoryCanvas.enabled)
				StateManager.SharedInstance.SetGameState(GameState.Interface);
			else
				StateManager.SharedInstance.SetGameState(GameState.Free);
		}
		else if (Input.GetKeyUp("c"))
		{
			//Debug.Log ("tookApple: "+GlobalVariableManager.SharedInstance.GetGlobalVariable("tookApple"));
			//StateManager.SharedInstance.SetGameState(GameState.Interface);
			DeactivateAllOtherWindows(characterCanvas);
			characterCanvas.GetComponent<Canvas>().enabled = !characterCanvas.GetComponent<Canvas>().enabled;
			characterCanvas.GetComponent<GraphicRaycaster>().enabled = !characterCanvas.GetComponent<GraphicRaycaster>().enabled;
			if(characterCanvas.enabled)
				StateManager.SharedInstance.SetGameState(GameState.Interface);
			else
				StateManager.SharedInstance.SetGameState(GameState.Free);
		}
	}

	void DeactivateAllOtherWindows(Canvas canvas)
	{
		foreach (Canvas child in allCanvas)
		{
			if (child != canvas)
			{
				child.GetComponent<Canvas>().enabled = false;
				child.GetComponent<GraphicRaycaster>().enabled = false;
			}
		}
	}

	/*public void ShowDialogWindow()
	{
		playerMouseLook.enabled = false;
		cameraMouseLook.enabled = false;
		dialogCanvas.GetComponent<Canvas>().enabled = true;
		dialogCanvas.GetComponent<GraphicRaycaster>().enabled = true;
	}

	public void HideDialogWindow()
	{
		playerMouseLook.enabled = true;
		cameraMouseLook.enabled = true;
		dialogCanvas.GetComponent<Canvas>().enabled = false;
		dialogCanvas.GetComponent<GraphicRaycaster>().enabled = false;
	}*/

	public void ShowInteractionOverlay(string text)
	{
		//overlayCanvas.GetComponent<Canvas>().enabled = false;
		//overlayCanvas.GetComponent<GraphicRaycaster>().enabled = false;
		if(interactionOverlay.GetComponent<Image>().enabled == false)
		{
			interactionOverlay.GetComponent<Image>().enabled = true;
			interactionOverlay.GetComponentInChildren<Text>().text = text;
		}
	}
	public void HideInteractionOverlay()
	{
		//overlayCanvas.GetComponent<Canvas>().enabled = true;
		//overlayCanvas.GetComponent<GraphicRaycaster>().enabled = true;
		if(interactionOverlay.GetComponent<Image>().enabled == true)
			interactionOverlay.GetComponent<Image>().enabled = false;
	}

}
