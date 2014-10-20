using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class GUIManager : MonoBehaviour {


	//private static GUIManager instance = null;
	public static GUIManager SharedInstance { get; private set; }

	public Canvas inventoryCanvas;
	public Canvas debugCanvas;
	public Canvas characterCanvas;

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

	public Canvas overlayCanvas;
	public List<Canvas> allCanvas;
	public GameObject maskItemInfo;
	public GameObject player;
	public GameObject interactionOverlay;
	public Canvas dialogCanvas;
	public bool alternativeDialogCanvas = false;
	public bool loadAndSaveStuff = false;
	private UnitySampleAssets.Characters.FirstPerson.FirstPersonController playerMouseLook;
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

		camera = Camera.main;
		StateManager.SharedInstance.OnStateChange += HandleOnStateChange;
		PlayerManager.SharedInstance.OnPlayerAttributeChange += HandleOnPlayerAttributeChange;
		maskItemInfo.GetComponent<Image>().enabled = false;
		playerMouseLook = player.GetComponent<UnitySampleAssets.Characters.FirstPerson.FirstPersonController>();
		allCanvas.Add (inventoryCanvas);
		allCanvas.Add (characterCanvas);
		allCanvas.Add (overlayCanvas);
		//cameraMouseLook = camera.GetComponent<MouseLook>();
		MainComponentManager.CreateInstance ();
		PlayerManager.SharedInstance.Load ();
		StateManager.SharedInstance.SetGameState(GameState.Free);

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
		//GlobalState.gameState.StartState();
	}
	// Update is called once per frame
	void Update () {
		timeValue.text = ((int)DayNightCycleManager.SharedInstance.timeOfDay).ToString("00") + ":" + ((int)((DayNightCycleManager.SharedInstance.timeOfDay*60)%60)).ToString ("00");
		if (Input.GetKeyUp("f10"))
		{
			debugCanvas.enabled = !debugCanvas.enabled;

		}
		else if (Input.GetKeyUp("i"))
		{
			DeactivateAllOtherWindows(inventoryCanvas);

			maskItemInfo.GetComponent<Image>().enabled = false;
			//imagePanel.GetComponent<Mask>().enabled = true;

			inventoryCanvas.GetComponent<Canvas>().enabled = !inventoryCanvas.GetComponent<Canvas>().enabled;
			inventoryCanvas.GetComponent<GraphicRaycaster>().enabled = !inventoryCanvas.GetComponent<GraphicRaycaster>().enabled;

			if(inventoryCanvas.enabled)
				StateManager.SharedInstance.SetGameState(GameState.Interface);
			else
				StateManager.SharedInstance.SetGameState(GameState.Free);
		}
		else if (Input.GetKeyUp("c"))
		{
			//Debug.Log ("apple: "+GlobalVariableManager.SharedInstance.GetGlobalVariable("tookApple"));
			//StateManager.SharedInstance.SetGameState(GameState.Interface);
			DeactivateAllOtherWindows(characterCanvas);
			characterCanvas.GetComponent<Canvas>().enabled = !characterCanvas.GetComponent<Canvas>().enabled;
			characterCanvas.GetComponent<GraphicRaycaster>().enabled = !characterCanvas.GetComponent<GraphicRaycaster>().enabled;
			if(characterCanvas.enabled)
				StateManager.SharedInstance.SetGameState(GameState.Interface);
			else
				StateManager.SharedInstance.SetGameState(GameState.Free);
		}
		else if (Input.GetKeyUp ("f5"))
		{
			Debug.Log ("save variables");
			GlobalVariableManager.SharedInstance.SaveVariables ();
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

	public void DeactiveAllWindows()
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
	
	void HandleOnStateChange ()
	{
		if (StateManager.SharedInstance.gameState == GameState.Dialog)
		{
			DeactiveAllWindows();
			dialogCanvas.GetComponent<Canvas>().enabled = true;
			dialogCanvas.GetComponent<GraphicRaycaster>().enabled = true;
			
			playerMouseLook.enabled = false;
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
			if(!overlayCanvas.GetComponent<Canvas>().enabled)
			{
				overlayCanvas.GetComponent<Canvas>().enabled = true;
				overlayCanvas.GetComponent<GraphicRaycaster>().enabled = true;
			}
		}
		else if (StateManager.SharedInstance.gameState == GameState.Interface)
		{
			playerMouseLook.enabled = false;
		}
	}

}
