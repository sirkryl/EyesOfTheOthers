using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class PlayerData
{
	public int hunger;
	public int thirst;
	public int fatigue;
	public int coldness;
	public int stress;
	public int psyche;
	//public int health;
	//public float experience;
}

public class PlayerManager : MonoBehaviour
{
	public PlayerData playerData;

	private int temperature = 24;
	private float hungerTimer = 3.0f;
	private float fatigueTimer = 5.0f;
	private float thirstTimer = 2.0f;
	private float temperatureTimer = 20.0f;
	//private GUIManager windowManager;
	public ThrowableItem handItem;
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
	private bool night = false;
	//private int 
	private static PlayerManager instance = null;
	public static PlayerManager SharedInstance {
		get {
			if (instance == null) {
				instance = MainComponentManager.AddMainComponent<PlayerManager> ();
			}
			return instance;
		}
	}

	void Awake ()
	{
		playerData = new PlayerData();
		
		playerData.hunger = 100;
		playerData.thirst = 100;
		playerData.fatigue = 100;
		playerData.coldness = 100;
		playerData.stress = 100;
		playerData.psyche = 70;
	}
	// Use this for initialization
	void Start ()
	{
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
		//windowManager = GameObject.Find ("SceneManager").GetComponent<GUIManager>();
	}

	// Update is called once per frame
	void Update ()
	{
		playerData.hunger = Mathf.Min (playerData.hunger, 100);
		playerData.thirst = Mathf.Min (playerData.thirst, 100);
		playerData.fatigue = Mathf.Min (playerData.fatigue, 100);
		playerData.coldness = Mathf.Min (playerData.coldness, 100);
		playerData.stress = Mathf.Min (playerData.stress, 100);
		playerData.psyche = Mathf.Min (playerData.psyche, 100);
		hungerTimer -= Time.deltaTime;
		fatigueTimer -= Time.deltaTime;
		thirstTimer -= Time.deltaTime;
		temperatureTimer -= Time.deltaTime;
		if (hungerTimer <= 0 && playerData.hunger > 0)
		{
			playerData.hunger--;
			if (playerData.hunger <= 60 && playerData.hunger > 30)
			{
				hungerText.color = Color.yellow;
				hungerValue.color = Color.yellow;
			}
			if (playerData.hunger <= 30)
			{
				hungerText.color = Color.red;
				hungerValue.color = Color.red;
			}
			hungerValue.text = playerData.hunger+" %";
			hungerTimer = 3.0f;
		}
		else if (playerData.hunger <= 0)
		{
			PlayerDead();
		}
		if (fatigueTimer <= 0 && playerData.fatigue > 0)
		{
			playerData.fatigue--;
			if (playerData.fatigue <= 60 && playerData.fatigue > 30)
			{
				fatigueText.color = Color.yellow;
				fatigueValue.color = Color.yellow;
			}
			if (playerData.fatigue <= 30)
			{
				fatigueText.color = Color.red;
				fatigueValue.color = Color.red;
			}
			fatigueValue.text = playerData.fatigue+" %";
			fatigueTimer = 5.0f;
		}
		if (thirstTimer <= 0 && playerData.thirst > 0)
		{
			playerData.thirst--;
			if (playerData.thirst <= 60 && playerData.thirst > 30)
			{
				thirstText.color = Color.yellow;
				thirstValue.color = Color.yellow;
			}
			if (playerData.thirst <= 30)
			{
				thirstText.color = Color.red;
				thirstValue.color = Color.red;
			}
			thirstValue.text = playerData.thirst+" %";
			thirstTimer = 2.0f;
		}
		if (temperatureTimer <= 0)
		{
			int random = Random.Range (0,2);
			if (random == 0)
				temperature++;
			else
				temperature--;
			temperatureValue.text = temperature+"\u00b0c";

			if (temperature < 20)
			{
				playerData.coldness-=10;
				if (playerData.coldness <= 60 && playerData.coldness > 30)
				{
					coldnessText.color = Color.yellow;
					coldnessValue.color = Color.yellow;
				}
				if (playerData.coldness <= 30)
				{
					coldnessText.color = Color.red;
					coldnessValue.color = Color.red;
				}
				coldnessValue.text = playerData.coldness+" %";
			}
			temperatureTimer = 15.0f;
		}
		if((GlobalVariableManager.SharedInstance.GetGlobalVariable("timeOfDay") > 20 
		   || GlobalVariableManager.SharedInstance.GetGlobalVariable("timeOfDay") < 5) && !night)
		{
			playerData.psyche -= 30;
			if (playerData.psyche <= 60 && playerData.psyche > 30)
			{
				psycheValue.text = "Average";
				psycheFill.color = Color.yellow;
			}
			if (playerData.psyche <= 30)
			{
				psycheValue.text = "Insane";
				psycheFill.color = Color.red;
			}
			psycheBar.value = (float)((float)playerData.psyche/100.0f);
			playerData.coldness -= 30;
			if (playerData.coldness <= 60 && playerData.coldness > 30)
			{
				coldnessText.color = Color.yellow;
				coldnessValue.color = Color.yellow;
			}
			if (playerData.coldness <= 30)
			{
				coldnessText.color = Color.red;
				coldnessValue.color = Color.red;
			}
			coldnessValue.text = playerData.coldness+" %";
			night = true;
		}
		else if((GlobalVariableManager.SharedInstance.GetGlobalVariable("timeOfDay") <= 20 
		         && GlobalVariableManager.SharedInstance.GetGlobalVariable("timeOfDay") >= 5) && night)
		{
			playerData.psyche += 30;
			if (playerData.psyche > 60)
			{
				psycheValue.text = "Alright";
				psycheFill.color = Color.green;
			}
			if (playerData.psyche <= 60 && playerData.psyche > 30)
			{
				psycheValue.text = "Average";
				psycheFill.color = Color.yellow;
			}
			if (playerData.psyche <= 30)
			{
				psycheValue.text = "Insane";
				psycheFill.color = Color.red;
			}
			psycheBar.value = (float)((float)playerData.psyche/100.0f);
			playerData.coldness += 30;
			if (playerData.coldness <= 60 && playerData.coldness > 30)
			{
				coldnessText.color = Color.yellow;
				coldnessValue.color = Color.yellow;
			}
			if (playerData.coldness <= 30)
			{
				coldnessText.color = Color.red;
				coldnessValue.color = Color.red;
			}
			coldnessValue.text = playerData.coldness+" %";
			night = false;
		}
	}

	public void Save()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

		bf.Serialize (file, playerData);
		file.Close ();
	}

	public void Load()
	{
		if(File.Exists (Application.persistentDataPath + "/playerInfo.dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			playerData = bf.Deserialize(file) as PlayerData;
			file.Close ();
			//hunger = data.hunger;
		}
	}

	void PlayerDead()
	{
		GUIManager.SharedInstance.ShowInteractionOverlay("You starved to death.");
	}
}

