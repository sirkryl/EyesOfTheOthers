using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Dialog : Selectable {

	public static bool isAnythingActive = false;
	private Dictionary<int, DialogIO.DialogElement> dialogMap;
	private Dictionary<string, int> localVariables;

	//only for legacy UI
	private List<GameObject> uiDialogElements;
	private List<GameObject> uiChoiceElements;


	private List<Text> answerOptions;

	private DialogIO dialogIO;

	private DialogIO.DialogElement activeDialogElement;
	private DialogIO.DialogData dialogData;
	
	private GameObject dialogElementPrefab;
	private GameObject dialogList;
	private GameObject answerList;
	private GameObject dialogText;
	private GameObject logList;
	private Inventory inventory;

	private bool inDialog = false;
	private bool newDialogElement = false;


	void Awake () {
		//legacy UI
		uiDialogElements = new List<GameObject>();
		uiChoiceElements = new List<GameObject>();


		answerOptions = new List<Text>();
		localVariables = new Dictionary<string, int>();
		
		highlight = "Press 'E' to talk.";
		dialogIO = new DialogIO();
	}
	// Use this for initialization
	void Start () {
		logList = GameObject.Find ("LogList");
		inventory = GameObject.FindWithTag ("Inventory").GetComponent<Inventory>();
		dialogList = GameObject.Find ("DialogList") as GameObject;
		answerList = GameObject.Find ("AnswerPanel") as GameObject;
		dialogText = GameObject.FindWithTag ("DialogText") as GameObject;


	}

	public override void HandleSelection() {
		if (dialogData == null)
		{
			WalkToDialog wtd = GetComponent<WalkToDialog>();
			if(wtd != null)
				Destroy(wtd);
			dialogData = dialogIO.Load (id_string);
			if (dialogData == null)
			{
				return;
			}
			CreateDialogMap();
			CreateLocalVariableMap();
		}
		if(dialogData.startsWith != null)
		{
			if(dialogMap.ContainsKey (dialogData.startsWith))
			{
				activeDialogElement = dialogMap[dialogData.startsWith];
				inDialog = true;
				StateManager.SharedInstance.SetGameState(GameState.Dialog);
				newDialogElement = true;
			}
			else
			{
				Debug.Log ("[Dialog ERROR] the startsWith-value of the root element is not a valid id of another element.");
				EndDialog ();
				return;
			}
		}
		else
		{
			Debug.Log ("[Dialog ERROR] Root element has no startsWith-value");
			EndDialog ();
			return;
		}
	}

	// Update is called once per frame
	void Update () {
		if(newDialogElement)
		{
			if(!HandleDialog())
			{
				Debug.Log ("[Dialog ERROR] Error in Dialog.cs HandleDialog(). Aborting dialog.");
				EndDialog ();
				return;
			}
		}
		else if(activeDialogElement != null)
		{
			if ((Input.GetKeyUp (KeyCode.Return) || Input.GetMouseButtonUp(0)) && activeDialogElement.type == "text")
			{
				if(activeDialogElement.leadsTo == 0 || activeDialogElement.leadsTo == null)
				{
					EndDialog ();
					return;
				}
				
				if(dialogMap.ContainsKey (activeDialogElement.leadsTo))
				{
					activeDialogElement = dialogMap[activeDialogElement.leadsTo];
					newDialogElement = true;
				}
				else
				{
					Debug.Log ("[Dialog ERROR] choice-Element with id = "+activeDialogElement.id+"'s leadTo-value is not a valid id of another element.");
					return;
				}
			}
		}
	}

	bool HandleDialog()
	{
		if(activeDialogElement.type == "text")
		{
			
			if(GUIManager.SharedInstance.alternativeDialogCanvas)
			{
				dialogText.GetComponent<Text>().enabled = true;
				if(activeDialogElement.text != null)
					dialogText.GetComponent<Text>().text = activeDialogElement.text;
				else
				{
					Debug.Log ("[Dialog ERROR] Element (id = " + activeDialogElement.id + " has no text to display");
					return false;
				}
			}	
			
			//for the log
			Image dialogImage = Instantiate (Resources.Load ("Prefabs/GUI/DialogElement", typeof(Image))) as Image;
			dialogImage.transform.SetParent (logList.transform, false);
			uiDialogElements.Add (dialogImage.gameObject);
			Text[] texts = dialogImage.GetComponentsInChildren<Text>();
			texts[0].text = dialogData.characterName + ":";
			texts[1].text = activeDialogElement.text;
			//actual dialog
		}
		else if (activeDialogElement.type == "choice")
		{
			if(GUIManager.SharedInstance.alternativeDialogCanvas)
			{
				if (activeDialogElement.dialogAnswers == null)
				{
					Debug.Log ("[Dialog ERROR] Element (id="+activeDialogElement.id+", type=choice) has no answer-Elements.");
					return false;
				}
				//dialogText.GetComponent<Text>().enabled = false;
				dialogList.GetComponent<Image>().enabled = false;
				GameObject.FindWithTag ("ChoiceText").GetComponent<Text>().enabled = true;
				if (activeDialogElement.text != null)
					GameObject.FindWithTag ("ChoiceText").GetComponent<Text>().text = activeDialogElement.text;
				//dialogText.GetComponent<Text>().text = activeDialogElement.text;
			}

			//for the log
			Image dialogImage = Instantiate (Resources.Load ("Prefabs/GUI/DialogElement", typeof(Image))) as Image;
			dialogImage.transform.SetParent (logList.transform, false);
			uiDialogElements.Add (dialogImage.gameObject);
			Text[] texts = dialogImage.GetComponentsInChildren<Text>();
			texts[0].text = dialogData.characterName + ":";
			texts[1].text = activeDialogElement.text;

			//actual dialog
			if(GUIManager.SharedInstance.alternativeDialogCanvas)
			{
				GameObject.FindWithTag("AnswerList").GetComponent<Image>().enabled = true;
				answerOptions.ForEach (child => child.gameObject.GetComponent<Text>().enabled = false);
				while (answerOptions.Count < activeDialogElement.dialogAnswers.Length)
				{
					Text answerText = Instantiate (Resources.Load ("Prefabs/GUI/DialogOption", typeof(Text))) as Text;
					answerText.transform.SetParent (GameObject.FindWithTag("AnswerList").transform, false);
					answerOptions.Add (answerText);
				}
			}
			
			int answerCnt = 0;
			int answerTxtCnt = 0;
			foreach (DialogIO.DialogAnswer answer in activeDialogElement.dialogAnswers)
			{
				//Debug.Log ("size "+answerOptions.Count);
				//Debug.Log ("answerCnt "+answerCnt);
				if(GUIManager.SharedInstance.alternativeDialogCanvas)
				{
					Text answerText = answerOptions[answerCnt];
					answerText.gameObject.GetComponent<Text>().enabled = true;
					if(answer.text == null)
					{
						answerCnt++;
						continue;
					}
					answerText.text = answerCnt+1 + ". "+answer.text;
					answerText.GetComponent<Button>().onClick.RemoveAllListeners();
					answerText.GetComponent<AnswerNumber>().number = answerCnt;
					answerText.GetComponent<Button>().onClick.AddListener(() => { SendAnswer(answerText.GetComponent<AnswerNumber>().number); });
				}

				answerTxtCnt++;
				answerCnt++;
			}
			if(answerTxtCnt == 0)
			{
				Debug.Log ("[Dialog ERROR] Element (id="+activeDialogElement.id+") has no answer-elements with text values.");
				return false;
			}
			else if(answerTxtCnt < answerCnt)
			{
				Debug.Log ("[Dialog WARNING] Some answers from element (id="+activeDialogElement.id+") could not be displayed because of missing text values.");
			}
		}
		else if (activeDialogElement.type == "increaseValue")
		{
			//Debug.Log ("increaseValue");
			if(activeDialogElement.variable == null)
			{
				Debug.Log ("[Dialog ERROR] No variable attribute for element (id="+activeDialogElement.id+").");
				return false;
			}
			if(activeDialogElement.vartype == "global")
			{
				if(!GlobalVariableManager.SharedInstance.SetGlobalVariable (activeDialogElement.variable, 
				                                                        GlobalVariableManager.SharedInstance.GetGlobalVariable(activeDialogElement.variable)+1))
				{
					Debug.Log ("[Dialog ERROR] Could not find global variable "+activeDialogElement.variable+" for element (id="+activeDialogElement.id+").");
					return false;
				}
			}
			else if(localVariables.ContainsKey (activeDialogElement.variable))
			{
				localVariables[activeDialogElement.variable]++;
			}
			else
			{
				Debug.Log ("[Dialog ERROR] Could not find local variable "+activeDialogElement.variable+" for element (id="+activeDialogElement.id+").");
				return false;
			}
			/*if (localVariables[activeDialogElement.variable].type == "int")
					{
						int value = int.Parse (localVariables[activeDialogElement.variable].value);
						value++;
						string strValue = value.ToString();
						localVariables[activeDialogElement.variable].value = strValue;
					}*/
			if(activeDialogElement.leadsTo == 0 || activeDialogElement.leadsTo == null)
			{
				EndDialog ();
				return true;
			}

			if (dialogMap.ContainsKey (activeDialogElement.leadsTo))
			{
				activeDialogElement = dialogMap[activeDialogElement.leadsTo];
				newDialogElement = true;
			}
			else
			{
				Debug.Log ("[Dialog ERROR] Element with id = "+activeDialogElement.id+"'s leadTo-value is not a valid id of another element.");
				return false;
			}
		}
		else if (activeDialogElement.type == "switch")
		{
			if(activeDialogElement.dialogCases.Length < 1)
			{
				Debug.Log ("[Dialog ERROR] There are no case elements for element with id="+activeDialogElement.id+".");
				return false;
			}
			bool pass = false;
			foreach(DialogIO.DialogCase dialogCase in activeDialogElement.dialogCases)
			{
				if (dialogCase.value == null)
				{
					Debug.Log ("[Dialog WARNING] No value attribute for "+dialogCase.variable+", value=0 is assumed for choice of element with id="+activeDialogElement.id+".");
					dialogCase.value = 0;
				}
				int value = 0;
				if(dialogCase.variable != null)
				{
					if (dialogCase.vartype == "global")
					{
						value = GlobalVariableManager.SharedInstance.GetGlobalVariable(dialogCase.variable);
						if (value == -1)
						{
							Debug.Log ("[Dialog WARNING] Couldn't find global variable "+dialogCase.variable+" for a case of element with id="+activeDialogElement.id+".");
							continue;
						}
					}
					else
					{
						if (localVariables.ContainsKey (dialogCase.variable))
							value = localVariables[dialogCase.variable];
						else
						{
							Debug.Log ("[Dialog WARNING] Couldn't find local variable "+dialogCase.variable+" for a case of element with id="+activeDialogElement.id+".");
							continue;
						}
					}
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
					if(dialogCase.type == "inHand")
					{
						if (PlayerManager.SharedInstance.handItem != null)
						{
							if (PlayerManager.SharedInstance.handItem.id_string == dialogCase.item)
							{
								pass = true;
								if (dialogCase.remove)
								{
									PlayerManager.SharedInstance.handItem.gameObject.SetActive(false);
									PlayerManager.SharedInstance.handItem = null;
								}
							}
						}
					}
					else if (inventory.GotItem (dialogCase.item))
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
						return true;
					}
					if(dialogMap.ContainsKey (dialogCase.leadsTo))
					{
						activeDialogElement = dialogMap[dialogCase.leadsTo];
						newDialogElement = true;
						return true;
					}
					else
					{
						Debug.Log ("[Dialog ERROR] choice-Element with id = "+activeDialogElement.id+"'s leadTo-value is not a valid id of another element.");
						return false;
					}
				}
			}
			if(!pass)
			{
				Debug.Log ("[Dialog ERROR] No 'case' element was passed in element with id="+activeDialogElement.id+", consider using a 'default' case element");
				return false;
			}
		}
		else
		{
			Debug.Log ("[Dialog ERROR] At element with id = "+activeDialogElement.id+": '"+activeDialogElement.type+"' is not a valid type.");
			return false;
		}
		newDialogElement = false;
		return true;
	}

	void CreateDialogMap()
	{
		if(dialogData != null)
		{
			dialogMap = new Dictionary<int, DialogIO.DialogElement>();
			for (int i = 0; i < dialogData.dialogElement.Length; i++)
			{
				if(dialogData.dialogElement[i].type != "variable")
				{
					if(dialogData.dialogElement[i].id != null)
						dialogMap.Add (dialogData.dialogElement[i].id, dialogData.dialogElement[i]);
					else
						Debug.Log ("[Dialog ERROR] There is a non-variable root element without an ID.");
				}
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
				{
					if(dialogData.dialogElement[i].name != null)
						localVariables.Add (dialogData.dialogElement[i].name, dialogData.dialogElement[i].value);
					else
					{
						Debug.Log ("[Dialog WARNING] Can't create local variable without name-attribute.");
					}
				}
			}
		}
	}

	void EndDialog()
	{
		inDialog = false;
		newDialogElement = false;
		activeDialogElement = null;
		CleanUpDialog();
		//GameObject.FindWithTag("AnswerList").GetComponent<Image>().enabled = false;
		dialogText.GetComponent<Text>().text = "";
		StateManager.SharedInstance.SetGameState(GameState.Free);
	}

	void CleanUpDialog()
	{
		dialogText.GetComponent<Text>().text = "";
		answerOptions.ForEach (child => child.gameObject.GetComponent<Text>().enabled = false);
		GameObject.FindWithTag("AnswerList").GetComponent<Image>().enabled = false;
		answerOptions.ForEach (child => Destroy(child.gameObject));
		answerOptions.Clear ();
	}

	public void SendAnswer(int answer)
	{
		if(!GUIManager.SharedInstance.alternativeDialogCanvas)
		{
			uiChoiceElements.ForEach (child => child.SetActive(false));
			uiChoiceElements.Clear ();
		}
		if(activeDialogElement.dialogAnswers[answer].leadsTo == null
		   || activeDialogElement.dialogAnswers[answer].leadsTo == 0)
		{
			dialogList.GetComponent<Image>().enabled = true;
			dialogText.GetComponent<Text>().enabled = true;
			EndDialog();
		}
		else
		{
			//for the log
			Image dialogImage = Instantiate (Resources.Load ("Prefabs/GUI/DialogElement", typeof(Image))) as Image;
			dialogImage.transform.SetParent (logList.transform, false);
			uiDialogElements.Add (dialogImage.gameObject);
			Text[] texts = dialogImage.GetComponentsInChildren<Text>();
			texts[0].text = "You:";
			texts[1].text = activeDialogElement.dialogAnswers[answer].text;

			//actual dialog
			GameObject.FindWithTag("AnswerList").GetComponent<Image>().enabled = false;
			answerOptions.ForEach (child => Destroy(child.gameObject));
			answerOptions.Clear ();
			dialogList.GetComponent<Image>().enabled = true;
			dialogText.GetComponent<Text>().enabled = true;

			if (dialogMap.ContainsKey (activeDialogElement.dialogAnswers[answer].leadsTo))
			{
				activeDialogElement = dialogMap[activeDialogElement.dialogAnswers[answer].leadsTo];
				newDialogElement = true;
			}
			else
			{
				Debug.Log ("[Dialog ERROR] Element with id = "+activeDialogElement.id+"'s leadTo-value is not a valid id of another element.");
				EndDialog();
			}
		}
	}
}
