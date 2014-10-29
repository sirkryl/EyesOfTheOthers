using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class GUIManager : MonoBehaviour {


	//private static GUIManager instance = null;
	public static GUIManager SharedInstance { get; private set; }

	private bool debugMode = false;

	#region canvas
	public Canvas inventoryCanvas;
	public Canvas debugCanvas;
	public Canvas characterCanvas;
	public Canvas overlayCanvas;
	public Canvas logCanvas;
	private List<Canvas> allCanvas;
	#endregion
	

	#region inventory ui elements
	private Inventory playerInventory;

	private Text itemName; 
	private Image itemImage;
	private Text itemType;
	private Text itemEffect;
	private Text itemDescription;
	private Text itemWeight;

	private Image itemList;
	private Image itemLabelFab;
	private GameObject inventoryEmptyFab;
	private GameObject inventoryEmptyLabel;

	private GameObject handButton;
	private GameObject useButton;
	private GameObject maskItemInfo;

	private Dictionary<string, Image> itemUIMap;
	#endregion
	

	#region character ui elements
	private Text hungerText;
	private Text hungerValue;
	private Text thirstText;
	private Text thirstValue;
	private Text fatigueText;
	private Text fatigueValue;
	private Text coldnessText;
	private Text coldnessValue;
	private Text temperatureText;
	private Text temperatureValue;
	private Text stressText;
	private Text stressValue;
	private Text psycheText;
	private Text psycheValue;
	private Slider psycheBar;
	private Image psycheFill;
	private Text timeValue;
	#endregion

	private Text debugTimeText;
	
	public GameObject interactionOverlay;
	public Canvas dialogCanvas;
	public bool alternativeDialogCanvas = false;
	public bool loadAndSaveStuff = false;
	private UnitySampleAssets.Characters.FirstPerson.FirstPersonController playerController;
	private Camera camera;

	void Awake()
	{
		if(SharedInstance != null && SharedInstance != this)
			Destroy(gameObject);

		SharedInstance = this;
		DontDestroyOnLoad(gameObject);

		allCanvas = new List<Canvas>();
	}

	// Use this for initialization
	void Start () {

		MainComponentManager.CreateInstance ();
		StateManager.SharedInstance.SetGameState(GameState.Free);
		camera = Camera.main;
		playerInventory = GameObject.FindWithTag ("Player").GetComponentInChildren<Inventory>();
		playerInventory.OnItemAdded += HandleOnItemAdded;
		playerInventory.OnItemRemoved += HandleOnItemRemoved;

		StateManager.SharedInstance.OnStateChange += HandleOnStateChange;
		PlayerManager.SharedInstance.OnPlayerAttributeChange += HandleOnPlayerAttributeChange;

		if (inventoryCanvas == null)
			inventoryCanvas = GameObject.Find ("InventoryCanvas").GetComponent<Canvas>();
		if (overlayCanvas == null)
			overlayCanvas = GameObject.Find ("OverlayCanvas").GetComponent<Canvas>();
		if (characterCanvas == null)
			characterCanvas = GameObject.Find ("CharacterCanvas").GetComponent<Canvas>();
		if (dialogCanvas == null)
			dialogCanvas = GameObject.Find ("DialogCanvas").GetComponent<Canvas>();
		if (logCanvas == null)
			logCanvas = GameObject.Find ("LogCanvas").GetComponent<Canvas>();

		playerController = GameObject.FindWithTag ("Player").GetComponent<UnitySampleAssets.Characters.FirstPerson.FirstPersonController>();
		allCanvas.Add (inventoryCanvas);
		allCanvas.Add (characterCanvas);
		allCanvas.Add (overlayCanvas);
		allCanvas.Add (logCanvas);
		//cameraMouseLook = camera.GetComponent<MouseLook>();
		debugTimeText = GameObject.Find ("DebugTimeText").GetComponent<Text>();

		#region character ui
		hungerText = GameObject.Find ("HungerText").GetComponent<Text>();
		hungerValue = GameObject.Find ("HungerValue").GetComponent<Text>();
		thirstText = GameObject.Find ("ThirstText").GetComponent<Text>();
		thirstValue = GameObject.Find ("ThirstValue").GetComponent<Text>();
		fatigueText = GameObject.Find ("FatigueText").GetComponent<Text>();
		fatigueValue = GameObject.Find ("FatigueValue").GetComponent<Text>();
		coldnessText = GameObject.Find ("ColdnessText").GetComponent<Text>();
		coldnessValue = GameObject.Find ("ColdnessValue").GetComponent<Text>();
		temperatureText = GameObject.Find ("TemperatureText").GetComponent<Text>();
		temperatureValue = GameObject.Find ("TemperatureValue").GetComponent<Text>();
		stressText = GameObject.Find ("StressText").GetComponent<Text>();
		stressValue = GameObject.Find ("StressValue").GetComponent<Text>();
		psycheText = GameObject.Find ("PsycheText").GetComponent<Text>();
		psycheValue = GameObject.Find ("PsycheValue").GetComponent<Text>();
		psycheBar = GameObject.Find ("PsycheBar").GetComponent<Slider>();
		psycheFill = GameObject.Find ("PsycheFill").GetComponent<Image>();
		timeValue = GameObject.Find ("TimeValue").GetComponent<Text>();
		#endregion

		#region inventory ui
		itemList = GameObject.Find ("ItemList").GetComponent<Image>();
		itemUIMap = new Dictionary<string, Image>();
		inventoryEmptyFab = Resources.Load("Prefabs/GUI/itemListEmpty",typeof(GameObject)) as GameObject;
		itemLabelFab = Resources.Load ("Prefabs/GUI/itemListElement", typeof(Image)) as Image;
		inventoryEmptyLabel = Instantiate(inventoryEmptyFab) as GameObject;
		inventoryEmptyLabel.transform.SetParent (itemList.transform, false);
		maskItemInfo = GameObject.Find ("MaskPanel");

		maskItemInfo.GetComponent<Image>().enabled = false;
		itemName = GameObject.Find ("ItemName").GetComponent<Text>();
		itemImage = GameObject.Find ("ItemImage").GetComponent<Image>();
		itemType = GameObject.Find ("ItemType").GetComponent<Text>();
		itemEffect = GameObject.Find ("ItemEffect").GetComponent<Text>();
		itemDescription = GameObject.Find ("ItemDescription").GetComponent<Text>();
		itemWeight = GameObject.Find ("ItemWeight").GetComponent<Text>();

		handButton = GameObject.Find ("HandButton");
		useButton = GameObject.Find ("UseButton");
		#endregion
	}

	// Update is called once per frame
	void Update () {

		//update time in GUI
		string timeString = ((int)DayNightCycleManager.SharedInstance.timeOfDay).ToString("00") + ":" + ((int)((DayNightCycleManager.SharedInstance.timeOfDay*60)%60)).ToString ("00");
		timeValue.text = timeString;
		if(debugTimeText != null)
			debugTimeText.text = timeString;

		if (Input.GetKeyUp("f10"))
		{
			ToggleCanvas(debugCanvas);
			debugMode = !debugMode;
		}
		if(!debugMode)
		{
			if (Input.GetKeyUp("i"))
			{
				ToggleCanvas(inventoryCanvas);
				maskItemInfo.GetComponent<Image>().enabled = false;
			}
			else if (Input.GetKeyUp("c"))
				ToggleCanvas(characterCanvas);
			else if (Input.GetKeyUp ("l"))
				ToggleCanvas(logCanvas);
			else if (Input.GetKeyUp ("f5"))
			{
				Debug.Log ("save variables");
				GlobalVariableManager.SharedInstance.SaveVariables ();
			}
		}

	}

	void ToggleCanvas(Canvas canvas)
	{
		DeactivateAllOtherCanvas(canvas);
		
		canvas.GetComponent<Canvas>().enabled = !canvas.GetComponent<Canvas>().enabled;
		canvas.GetComponent<GraphicRaycaster>().enabled = !canvas.GetComponent<GraphicRaycaster>().enabled;
		
		if(canvas.enabled)
			StateManager.SharedInstance.SetGameState(GameState.Interface);
		else
			StateManager.SharedInstance.SetGameState(GameState.Free);
	}

	void DeactivateAllOtherCanvas(Canvas canvas)
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

	public void DeactiveAllCanvas()
	{
		foreach (Canvas child in allCanvas)
		{
			child.GetComponent<Canvas>().enabled = false;
			child.GetComponent<GraphicRaycaster>().enabled = false;
		}
		StateManager.SharedInstance.SetGameState(GameState.Free);
	}

	public void ShowInteractionOverlay(string text)
	{

		if(interactionOverlay.GetComponent<Image>().enabled == false 
		   || text != interactionOverlay.GetComponentInChildren<Text>().text)
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

	#region character GUI

	void HandleOnPlayerAttributeChange ()
	{
		if (PlayerManager.SharedInstance.playerData.hunger <= 60 && PlayerManager.SharedInstance.playerData.hunger > 30)
		{
			hungerText.color = Color.yellow;
			hungerValue.color = Color.yellow;
		}
		if (PlayerManager.SharedInstance.playerData.hunger <= 30)
		{
			hungerText.color = Color.red;
			hungerValue.color = Color.red;
		}
		hungerValue.text = PlayerManager.SharedInstance.playerData.hunger+" %";
		
		if (PlayerManager.SharedInstance.playerData.fatigue <= 60 && PlayerManager.SharedInstance.playerData.fatigue > 30)
		{
			fatigueText.color = Color.yellow;
			fatigueValue.color = Color.yellow;
		}
		if (PlayerManager.SharedInstance.playerData.fatigue <= 30)
		{
			fatigueText.color = Color.red;
			fatigueValue.color = Color.red;
		}
		fatigueValue.text = PlayerManager.SharedInstance.playerData.fatigue+" %";
		
		if (PlayerManager.SharedInstance.playerData.thirst <= 60 && PlayerManager.SharedInstance.playerData.thirst > 30)
		{
			thirstText.color = Color.yellow;
			thirstValue.color = Color.yellow;
		}
		if (PlayerManager.SharedInstance.playerData.thirst <= 30)
		{
			thirstText.color = Color.red;
			thirstValue.color = Color.red;
		}
		thirstValue.text = PlayerManager.SharedInstance.playerData.thirst+" %";
		
		temperatureValue.text = PlayerManager.SharedInstance.temperature+"\u00b0c";
		
		if (PlayerManager.SharedInstance.temperature < 20)
		{
			PlayerManager.SharedInstance.playerData.coldness-=10;
			if (PlayerManager.SharedInstance.playerData.coldness <= 60 && PlayerManager.SharedInstance.playerData.coldness > 30)
			{
				coldnessText.color = Color.yellow;
				coldnessValue.color = Color.yellow;
			}
			if (PlayerManager.SharedInstance.playerData.coldness <= 30)
			{
				coldnessText.color = Color.red;
				coldnessValue.color = Color.red;
			}
			coldnessValue.text = PlayerManager.SharedInstance.playerData.coldness+" %";
		}
		
		if (PlayerManager.SharedInstance.playerData.psyche <= 60 && PlayerManager.SharedInstance.playerData.psyche > 30)
		{
			psycheValue.text = "Average";
			psycheFill.color = Color.yellow;
		}
		if (PlayerManager.SharedInstance.playerData.psyche <= 30)
		{
			psycheValue.text = "Insane";
			psycheFill.color = Color.red;
		}
		psycheBar.value = (float)((float)PlayerManager.SharedInstance.playerData.psyche/100.0f);
		
		if (PlayerManager.SharedInstance.playerData.coldness <= 60 && PlayerManager.SharedInstance.playerData.coldness > 30)
		{
			coldnessText.color = Color.yellow;
			coldnessValue.color = Color.yellow;
		}
		if (PlayerManager.SharedInstance.playerData.coldness <= 30)
		{
			coldnessText.color = Color.red;
			coldnessValue.color = Color.red;
		}
		coldnessValue.text = PlayerManager.SharedInstance.playerData.coldness+" %";
	}

	#endregion

	#region inventory GUI

	void HandleOnStateChange ()
	{
		if (StateManager.SharedInstance.gameState == GameState.Dialog)
		{
			DeactiveAllCanvas();
			dialogCanvas.GetComponent<Canvas>().enabled = true;
			dialogCanvas.GetComponent<GraphicRaycaster>().enabled = true;
			
			playerController.enabled = false;
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
			playerController.enabled = true;
			if(!overlayCanvas.GetComponent<Canvas>().enabled)
			{
				overlayCanvas.GetComponent<Canvas>().enabled = true;
				overlayCanvas.GetComponent<GraphicRaycaster>().enabled = true;
			}
		}
		else if (StateManager.SharedInstance.gameState == GameState.Interface)
		{
			playerController.enabled = false;
		}
	}

	void HandleOnItemAdded (Item item)
	{
		if (itemUIMap.ContainsKey (item.id_string))
		{
			itemUIMap[item.id_string].GetComponentInChildren<Text>().text = item.name + " (" + item.stackSize + ")";
			//itemUIMap[item.id_string].GetComponent<Item>().stackSize = itemMap[item.id_string].stackSize;
		}
		else
		{
			if (itemUIMap.Count == 0)
				Destroy(inventoryEmptyLabel);
			//Image itemListElement = Instantiate (itemLabelFab, new Vector3(0,-10-((items.Count-1)*18),0), Quaternion.identity) as Image;
			Image itemListElement = Instantiate (itemLabelFab, new Vector3(0,0,0), Quaternion.identity) as Image;
			itemListElement.transform.SetParent (itemList.transform, false);
			itemListElement.GetComponentInChildren<Text>().enabled = true;
			itemListElement.GetComponentInChildren<Text>().text = item.name + " (" + item.stackSize + ")";
			//itemListElement.GetComponent<Button>().onClick.AddListener(() => { GUIShowItemInfo(itemListElement.GetComponent<Item>()); });
			itemListElement.GetComponent<Button>().onClick.AddListener(() => { ShowItemInfo(playerInventory.GetItem (item.id_string)); });
			
			//itemListElement.GetComponent<Item>().name = item.id_string;
			itemUIMap.Add (item.id_string, itemListElement);
		}
	}
	
	void HandleOnItemRemoved (Item item)
	{
		int stackSize = playerInventory.GetStackSize (item);
		if (stackSize > 0 && itemUIMap.ContainsKey (item.id_string))
		{
			itemUIMap[item.id_string].GetComponentInChildren<Text>().text = item.name + " (" + stackSize+ ")";
		}
		else
		{
			Image itemListElement = itemUIMap[item.id_string];
			itemUIMap.Remove (item.id_string);
			Destroy(itemListElement.gameObject);
			if(itemUIMap.Count == 0)
			{
				inventoryEmptyFab = Resources.Load ("Prefabs/GUI/itemListEmpty",typeof(GameObject)) as GameObject;
				inventoryEmptyLabel = Instantiate(inventoryEmptyFab) as GameObject;
				inventoryEmptyLabel.transform.SetParent (itemList.transform, false);
			}
			handButton.GetComponent<Button>().interactable = false;
			useButton.GetComponent<Button>().interactable = false;
			handButton.GetComponentInChildren<Text>().enabled = false;
			useButton.GetComponentInChildren<Text>().enabled = false;
			//GameObject.Find ("HandButton").GetComponent<Mask>().enabled = true;
			//GameObject.Find ("HandButton").GetComponent<Image>().enabled = false;
			//GameObject.Find ("UseButton").GetComponent<Mask>().enabled = true;
			//GameObject.Find ("UseButton").GetComponent<Image>().enabled = false;
			maskItemInfo.GetComponent<Image>().enabled = false;
		}
	}
	
	void ShowItemInfo(Item item)
	{
		maskItemInfo.GetComponent<Image>().enabled = true;
		//imagePanel.GetComponent<Mask>().enabled = false;
		itemName.text = item.name;
		//itemImage.sprite = Sprite.
		itemType.text = item.GetItemType();
		itemEffect.text = item.GetItemEffect ();
		itemDescription.text = item.description;
		handButton.GetComponent<Button>().onClick.RemoveAllListeners();
		useButton.GetComponent<Button>().onClick.RemoveAllListeners();
		useButton.GetComponent<Button>().interactable = true;
		useButton.GetComponentInChildren<Text>().enabled = true;
		if(item is ThrowableItem)
		{
			handButton.GetComponent<Button>().interactable = true;
			handButton.GetComponentInChildren<Text>().enabled = true;
			//GameObject.Find ("HandButton").GetComponent<Image>().enabled = true;
			handButton.GetComponent<Button>().onClick.AddListener(() => { playerInventory.SelectItem(item); });
		}
		else
		{
			handButton.GetComponent<Button>().interactable = false;
			handButton.GetComponentInChildren<Text>().enabled = false;
			//GameObject.Find ("HandButton").GetComponent<Image>().enabled = false;
		}
		
		if(item is Consumable)
		{
			useButton.GetComponentInChildren<Text>().text = "Consume";
			useButton.GetComponent<Button>().onClick.AddListener(() => { playerInventory.ConsumeItem(item); });
		}
		else if(item is Equipable)
		{
			useButton.GetComponentInChildren<Text>().text = "Equip";
			useButton.GetComponent<Button>().onClick.AddListener(() => { playerInventory.EquipItem(item); });
		}
		itemImage.sprite = item.icon;
	}

	#endregion
}
