using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Dialog : DialogIO {

	public static bool isAnythingActive = false;

	private Dictionary<int, DialogElement> dialogMap;
	private Dictionary<string, int> localVariables;

	private List<GameObject> uiDialogElements;
	private List<GameObject> uiChoiceElements;
	private List<Text> answerOptions;

	public string name;
	private DialogElement activeDialogElement;
	private DialogData dialogData;

	private GameObject player;
	private Camera mainCamera;
	private GameObject dialogElementPrefab;
	private GameObject dialogList;
	private GameObject answerList;
	public GameObject interactionOverlay;
	private GameObject sceneManager;
	private GameObject dialogText;
	//private WindowManager windowManager;

	private bool inDialog = false;
	public bool isActiveObject = false;
	private bool newDialogElement = false;

	// Use this for initialization
	void Start () {
		uiDialogElements = new List<GameObject>();
		uiChoiceElements = new List<GameObject>();
		answerOptions = new List<Text>();
		localVariables = new Dictionary<string, int>();
		mainCamera = Camera.main;
		player = GameObject.FindWithTag ("Player") as GameObject;
		dialogList = GameObject.Find ("DialogList") as GameObject;
		answerList = GameObject.Find ("AnswerPanel") as GameObject;
		sceneManager = GameObject.FindWithTag("SceneManager") as GameObject;
		dialogText = GameObject.FindWithTag ("DialogText") as GameObject;
		//windowManager = sceneManager.GetComponent<WindowManager>();
		interactionOverlay = GameObject.Find ("InteractionOverlay") as GameObject;
		//player.GetComponent<DialogEventHandler>().RegisterForEvents(this);
		//eventSystem = (GameObject.Find ("EventSystem") as GameObject).GetComponent<EventSystem>();
		//dialogElementPrefab = Resources.Load ("Prefabs/DialogElement") as GameObject;
		/*npcName = GameObject.Find ("NPCName") as Text;
		npcText = GameObject.Find ("NPCText") as Text;
		playerText = GameObject.Find ("PlayerText") as Text;
		playerName = GameObject.Find ("PlayerName") as Text;
		dialogOptions = GameObject.Find ("DialogOptions") as Text;*/

	}
	/*
	void OnGUI() {

		if (isActiveObject && !inDialog)
		{
			//GUI.Box (new Rect ((float)(Screen.width*0.5-100),(float)(Screen.height*0.5-20),200,20), "Press E to talk.");
		}
		else if(!isAnythingActive)// || !inDialogRange)
		{
			//windowManager.HideInteractionOverlay();
		}
	}*/

	// Update is called once per frame
	void Update () {
		//Debug.Log (name + " is inDialog: "+inDialog+", or in Dialog range: "+inDialogRange);
		if (player != null && !inDialog)
		{
			if (Input.GetKeyDown (KeyCode.E) && isActiveObject)// && inDialogRange)
			{
				//if (bestOption == gameObject)
				if (dialogData == null)
				{
					dialogData = Load (name);
					CreateDialogMap();
					CreateLocalVariableMap();
				}

				activeDialogElement = dialogMap[dialogData.startsWith];
				inDialog = true;
				StateManager.SharedInstance.SetGameState(GameState.Dialog);
				newDialogElement = true;
				//sceneManager.GetComponent<DisplayWindows>().ShowDialogWindow();

			}
		}
		else if (inDialog)
		{
			if(newDialogElement)
			{
				if(activeDialogElement.type == "text")
				{

					if(WindowManager.SharedInstance.alternativeDialogCanvas)
					{
						dialogText.GetComponent<Text>().enabled = true;
						dialogText.GetComponent<Text>().text = activeDialogElement.text;
					}	

					#region Legacy GUI
					else
					{
						Image dialogImage = Instantiate (Resources.Load ("Prefabs/DialogElement", typeof(Image))) as Image;
						dialogImage.transform.SetParent (dialogList.transform, false);
						uiDialogElements.Add (dialogImage.gameObject);
						Text[] texts = dialogImage.GetComponentsInChildren<Text>();
						texts[0].text = dialogData.characterName + ":";
						texts[1].text = activeDialogElement.text;
					}
					#endregion
				}
				else if (activeDialogElement.type == "choice")
				{
					if(WindowManager.SharedInstance.alternativeDialogCanvas)
					{
						//dialogText.GetComponent<Text>().enabled = false;
						dialogList.GetComponent<Image>().enabled = false;
						GameObject.FindWithTag ("ChoiceText").GetComponent<Text>().enabled = true;
						GameObject.FindWithTag ("ChoiceText").GetComponent<Text>().text = activeDialogElement.text;
						//dialogText.GetComponent<Text>().text = activeDialogElement.text;
					}

					#region Legacy GUI	
					else
					{
						Image dialogImage = Instantiate (Resources.Load ("Prefabs/DialogElement", typeof(Image))) as Image;
						dialogImage.transform.SetParent (dialogList.transform, false);
						uiDialogElements.Add (dialogImage.gameObject);
						Text[] texts = dialogImage.GetComponentsInChildren<Text>();
						texts[0].text = dialogData.characterName + ":";
						texts[1].text = activeDialogElement.text;
					}
					#endregion

					if(WindowManager.SharedInstance.alternativeDialogCanvas)
					{
						GameObject.FindWithTag("AnswerList").GetComponent<Image>().enabled = true;
						answerOptions.ForEach (child => child.gameObject.GetComponent<Text>().enabled = false);
						while (answerOptions.Count < activeDialogElement.dialogAnswers.Length)
						{

							Text answerText = Instantiate (Resources.Load ("Prefabs/DialogOption_new", typeof(Text))) as Text;
							answerText.transform.SetParent (GameObject.FindWithTag("AnswerList").transform, false);
							answerOptions.Add (answerText);
						}
					}

					int answerCnt = 0;
					foreach (DialogAnswer answer in activeDialogElement.dialogAnswers)
					{
						//Debug.Log ("size "+answerOptions.Count);
						//Debug.Log ("answerCnt "+answerCnt);
						if(WindowManager.SharedInstance.alternativeDialogCanvas)
						{
							Text answerText = answerOptions[answerCnt];
							answerText.gameObject.GetComponent<Text>().enabled = true;
							answerText.text = answerCnt+1 + ". "+answer.text;
							answerText.GetComponent<Button>().onClick.RemoveAllListeners();
							answerText.GetComponent<AnswerNumber>().number = answerCnt;
							answerText.GetComponent<Button>().onClick.AddListener(() => { SendAnswer(answerText.GetComponent<AnswerNumber>().number); });
						}

						#region Legacy GUI	
						else
						{
							Text answerText = Instantiate (Resources.Load ("Prefabs/DialogOption", typeof(Text))) as Text;
							answerText.transform.SetParent (answerList.transform, false);
							uiChoiceElements.Add (answerText.gameObject);
							answerText.text = answerCnt+1 + ". "+answer.text;
							answerText.GetComponent<AnswerNumber>().number = answerCnt;
							answerText.GetComponent<Button>().onClick.AddListener(() => { SendAnswer(answerText.GetComponent<AnswerNumber>().number); });
						}
						#endregion

						answerCnt++;
					}
				}
				else if (activeDialogElement.type == "increaseValue")
				{
					//Debug.Log ("increaseValue");
					if(activeDialogElement.vartype == "global")
						GlobalVariableManager.SharedInstance.SetGlobalVariable (activeDialogElement.variable, 
						                                                        GlobalVariableManager.SharedInstance.GetGlobalVariable(activeDialogElement.variable)+1);
					else 
						localVariables[activeDialogElement.variable]++;
					/*if (localVariables[activeDialogElement.variable].type == "int")
					{
						int value = int.Parse (localVariables[activeDialogElement.variable].value);
						value++;
						string strValue = value.ToString();
						localVariables[activeDialogElement.variable].value = strValue;
					}*/
					if(activeDialogElement.leadsTo == 0)
					{
						EndDialog ();
						return;
					}
					activeDialogElement = dialogMap[activeDialogElement.leadsTo];
					newDialogElement = true;
				}
				else if (activeDialogElement.type == "switch")
				{
					foreach(DialogCase dialogCase in activeDialogElement.dialogCases)
					{

						bool pass = false;
						int value = 0;
						if(dialogCase.variable != null)
						{
							if (dialogCase.vartype == "global")
								value = GlobalVariableManager.SharedInstance.GetGlobalVariable(dialogCase.variable);
							else
								value = localVariables[dialogCase.variable];
							if(dialogCase.type == "equal")
							{
								if(value == dialogCase.value)
									pass = true;
							}
							else if(dialogCase.type == "greaterThan")
							{
								if(value > dialogCase.value)
									pass = true;
							}
							else if(dialogCase.type == "lessThan")
							{
								if(value < dialogCase.value)
									pass = true;
							}
						}
						if(dialogCase.type == "default")
							pass = true;

						if(dialogCase.item != null)
						{
							if (player.GetComponentInChildren<Inventory>().GotItem (dialogCase.item))
							{
								pass = true;
							}
						}
						//Debug.Log ("pass: "+pass);
						if (pass)
						{
							if (dialogCase.variable != null)
							{
									if(dialogCase.reset)
									{
										if (dialogCase.vartype == "global")
											GlobalVariableManager.SharedInstance.SetGlobalVariable(dialogCase.variable,value+1);
										else
											localVariables[dialogCase.variable] = 0;
									}
							}
							if(dialogCase.leadsTo == 0)
							{
								EndDialog ();
								return;
							}
							activeDialogElement = dialogMap[dialogCase.leadsTo];
							newDialogElement = true;
							return;
						}
					}
				}
				newDialogElement = false;
			}
			else
			{
				if ((Input.GetKeyUp (KeyCode.Return) || Input.GetMouseButtonUp(0)) && activeDialogElement.type == "text")
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
				if(dialogData.dialogElement[i].type != "variable")
					dialogMap.Add (dialogData.dialogElement[i].id, dialogData.dialogElement[i]);
			}
		}
	}

	void CreateLocalVariableMap()
	{
		if(dialogData != null)
		{
			localVariables = new Dictionary<string, int>();
			for (int i = 0; i < dialogData.dialogElement.Length; i++)
			{
				if(dialogData.dialogElement[i].type == "variable")
					localVariables.Add (dialogData.dialogElement[i].name, dialogData.dialogElement[i].value);
			}
		}
	}

	void EndDialog()
	{
		inDialog = false;
		StateManager.SharedInstance.SetGameState(GameState.Free);
		CleanUpDialog();
		//sceneManager.GetComponent<DisplayWindows>().HideDialogWindow();
	}

	void CleanUpDialog()
	{
		uiDialogElements.ForEach (child => child.SetActive(false));
		uiDialogElements.Clear();
		uiChoiceElements.ForEach (child => child.SetActive(false));
		uiChoiceElements.Clear ();
		if(WindowManager.SharedInstance.alternativeDialogCanvas)
		{
			dialogText.GetComponent<Text>().text = "";
			answerOptions.ForEach (child => child.gameObject.GetComponent<Text>().enabled = false);
			GameObject.FindWithTag("AnswerList").GetComponent<Image>().enabled = false;
			answerOptions.ForEach (child => Destroy(child.gameObject));
			answerOptions.Clear ();
		}
		//answerOptions.Clear ();
		//uiDialogElements.Clear();
	}

	public void SendAnswer(int answer)
	{
		if(!WindowManager.SharedInstance.alternativeDialogCanvas)
		{
			uiChoiceElements.ForEach (child => child.SetActive(false));
			uiChoiceElements.Clear ();
		}
		if(activeDialogElement.dialogAnswers[answer].leadsTo == null
		   || activeDialogElement.dialogAnswers[answer].leadsTo == 0)
		{
			EndDialog ();
		}
		else
		{
			if(!WindowManager.SharedInstance.alternativeDialogCanvas)
			{
				Image dialogImage = Instantiate (Resources.Load ("Prefabs/DialogElement", typeof(Image))) as Image;
				dialogImage.transform.SetParent (dialogList.transform, false);
				uiDialogElements.Add (dialogImage.gameObject);
				Text[] texts = dialogImage.GetComponentsInChildren<Text>();
				texts[0].text = "You:";
				texts[1].text = activeDialogElement.dialogAnswers[answer].text;
			}
			else
			{
				GameObject.FindWithTag("AnswerList").GetComponent<Image>().enabled = false;
				dialogList.GetComponent<Image>().enabled = true;
				dialogText.GetComponent<Text>().enabled = true;
			}
			//dialogText.SetActive (true);
			activeDialogElement = dialogMap[activeDialogElement.dialogAnswers[answer].leadsTo];
			newDialogElement = true;
		}
	}
}
