using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class Dialog : DialogIO {

	private Dictionary<int, DialogElement> dialogMap;
	private List<GameObject> uiDialogElements;
	private List<GameObject> uiChoiceElements;
	private DialogElement activeDialogElement;
	private GameObject player;
	public KeyCode pressButton = KeyCode.E;
	private Camera mainCamera;
	public string name;
	public DialogData dialogData;
	private bool inDialog = false;
	private bool inDialogRange = false;
	private bool newDialogElement = false;
	public float dialogDistance = 2.0f;
	static float closestDialogDistance = 999.0f;
	static GameObject bestOption;
	public Text npcName;
	public Text npcText;
	public EventSystem eventSystem;
	public Text playerText;
	public Text playerName;
	public Text dialogOptions;
	public GameObject dialogElementPrefab;
	public GameObject dialogList;
	public GameObject answerList;
	private int currentId;
	private bool startedDialog = false;
	//public Canvas dialogCanvas;
	private float distance;
	// Use this for initialization
	void Start () {
		uiDialogElements = new List<GameObject>();
		uiChoiceElements = new List<GameObject>();
		mainCamera = Camera.main;
		player = GameObject.FindWithTag ("Player") as GameObject;
		dialogList = GameObject.Find ("DialogList") as GameObject;
		answerList = GameObject.Find ("AnswerPanel") as GameObject;
		player.GetComponent<DialogEventHandler>().RegisterForEvents(this);
		eventSystem = (GameObject.Find ("EventSystem") as GameObject).GetComponent<EventSystem>();
		//dialogElementPrefab = Resources.Load ("Prefabs/DialogElement") as GameObject;
		/*npcName = GameObject.Find ("NPCName") as Text;
		npcText = GameObject.Find ("NPCText") as Text;
		playerText = GameObject.Find ("PlayerText") as Text;
		playerName = GameObject.Find ("PlayerName") as Text;
		dialogOptions = GameObject.Find ("DialogOptions") as Text;*/

	}

	void OnGUI() {

		/*if (inDialog && startedDialog)
		{
			Image dialogImage = Instantiate (Resources.Load ("Prefabs/DialogElement", typeof(Image))) as Image;
			dialogImage.transform.SetParent (dialogList.transform, false);
			Text[] texts = dialogImage.GetComponentsInChildren<Text>();
			texts[0].text = dialogData.characterName + ":";
			texts[1].text = dialogData.dialogElement[0].text;
			startedDialog = false;
			//playerName.text = "You:";

		}*/

		if (inDialogRange && !inDialog)
		{
			GUI.Box (new Rect ((float)(Screen.width*0.5-100),(float)(Screen.height*0.5-20),200,20), "Press E to talk.");
		}
	}

	// Update is called once per frame
	void Update () {
		if (player != null && !inDialog)
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
						CreateDialogMap();
					}
					activeDialogElement = dialogMap[dialogData.startsWith];
					inDialog = true;
					newDialogElement = true;
					//inDialog = true;
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
		else if (inDialog)
		{
			if(newDialogElement)
			{
				Debug.Log ("type: "+activeDialogElement.type);
				if(activeDialogElement.type == "text")
				{
					Image dialogImage = Instantiate (Resources.Load ("Prefabs/DialogElement", typeof(Image))) as Image;
					dialogImage.transform.SetParent (dialogList.transform, false);
					uiDialogElements.Add (dialogImage.gameObject);
					Text[] texts = dialogImage.GetComponentsInChildren<Text>();
					texts[0].text = dialogData.characterName + ":";
					texts[1].text = activeDialogElement.text;
				}
				else if (activeDialogElement.type == "choice")
				{
					Image dialogImage = Instantiate (Resources.Load ("Prefabs/DialogElement", typeof(Image))) as Image;
					dialogImage.transform.SetParent (dialogList.transform, false);
					uiDialogElements.Add (dialogImage.gameObject);
					Text[] texts = dialogImage.GetComponentsInChildren<Text>();
					texts[0].text = dialogData.characterName + ":";
					texts[1].text = activeDialogElement.text;
					int answerCnt = 0;
					foreach (DialogAnswer answer in activeDialogElement.dialogAnswers)
					{
						Text answerText = Instantiate (Resources.Load ("Prefabs/DialogOption", typeof(Text))) as Text;
						answerText.transform.SetParent (answerList.transform, false);
						uiChoiceElements.Add (answerText.gameObject);
						answerText.text = answerCnt+1 + ". "+answer.text;
						answerText.GetComponent<AnswerNumber>().number = answerCnt;
						answerText.GetComponent<Button>().onClick.AddListener(() => { SendAnswer(answerText.GetComponent<AnswerNumber>().number); });
						answerCnt++;
					}
				}
				newDialogElement = false;
			}
			else
			{
				if (Input.GetKeyUp (KeyCode.Return) && activeDialogElement.type == "text")
				{
					if(activeDialogElement.leadsTo == 0)
					{
						EndDialog ();
						return;
					}

					activeDialogElement = dialogMap[activeDialogElement.leadsTo];
					newDialogElement = true;
				}
			}
		}
	}

	void CreateDialogMap()
	{
		if(dialogData != null)
		{
			dialogMap = new Dictionary<int, DialogElement>();
			for (int i = 0; i < dialogData.dialogElement.Length; i++)
			{
				dialogMap.Add (dialogData.dialogElement[i].id, dialogData.dialogElement[i]);
			}
		}
	}

	void EndDialog()
	{
		inDialog = false;
		CleanUpDialog();
		player.GetComponent<DisplayWindows>().HideDialogWindow();
	}

	void CleanUpDialog()
	{
		uiDialogElements.ForEach (child => child.SetActive(false));
		uiDialogElements.Clear();
		uiChoiceElements.ForEach (child => child.SetActive(false));
		uiChoiceElements.Clear ();
		//uiDialogElements.Clear();
	}

	public void SendAnswer(int answer)
	{
		uiChoiceElements.ForEach (child => child.SetActive(false));
		uiChoiceElements.Clear ();
		if(activeDialogElement.dialogAnswers[answer].leadsTo == 0)
		{
			EndDialog ();
		}
		else
		{
			Image dialogImage = Instantiate (Resources.Load ("Prefabs/DialogElement", typeof(Image))) as Image;
			dialogImage.transform.SetParent (dialogList.transform, false);
			uiDialogElements.Add (dialogImage.gameObject);
			Text[] texts = dialogImage.GetComponentsInChildren<Text>();
			texts[0].text = "You:";
			texts[1].text = activeDialogElement.dialogAnswers[answer].text;

			activeDialogElement = dialogMap[activeDialogElement.dialogAnswers[answer].leadsTo];
			newDialogElement = true;
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
