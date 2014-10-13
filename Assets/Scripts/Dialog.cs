using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class Dialog : DialogIO {
	private GameObject player;
	public KeyCode pressButton = KeyCode.E;
	private Camera mainCamera;
	public string name;
	public DialogData dialogData;
	private bool inDialog = false;
	private bool inDialogRange = false;
	public float dialogDistance = 2.0f;
	static float closestDialogDistance = 999.0f;
	static GameObject bestOption;
	public Text npcName;
	public Text npcText;
	public Text playerText;
	public Text playerName;
	public Text dialogOptions;
	public GameObject dialogElementPrefab;
	public GameObject dialogList;
	private int currentId;
	private bool startedDialog = false;
	//public Canvas dialogCanvas;
	private float distance;
	// Use this for initialization
	void Start () {
		mainCamera = Camera.main;
		player = GameObject.FindWithTag ("Player") as GameObject;
		dialogList = GameObject.Find ("DialogList") as GameObject;
		//dialogElementPrefab = Resources.Load ("Prefabs/DialogElement") as GameObject;
		/*npcName = GameObject.Find ("NPCName") as Text;
		npcText = GameObject.Find ("NPCText") as Text;
		playerText = GameObject.Find ("PlayerText") as Text;
		playerName = GameObject.Find ("PlayerName") as Text;
		dialogOptions = GameObject.Find ("DialogOptions") as Text;*/

	}

	void OnGUI() {

		if (inDialog && startedDialog)
		{
			Image dialogImage = Instantiate (Resources.Load ("Prefabs/DialogElement", typeof(Image))) as Image;
			dialogImage.transform.SetParent (dialogList.transform, false);
			Text[] texts = dialogImage.GetComponentsInChildren<Text>();
			texts[0].text = dialogData.characterName + ":";
			texts[1].text = dialogData.dialogText[0].text;
			startedDialog = false;
			//playerName.text = "You:";

		}

		if (inDialogRange && !inDialog)
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
					if (dialogData == null)
					{
						dialogData = Load (name);

					}
					inDialog = true;
					startedDialog = true;
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
