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
	public event OnStateChangeHandler OnPlayerAttributeChange;
	public PlayerData playerData { get; private set; }

	public int temperature = 24;
	private float hungerTimer = 3.0f;
	private float fatigueTimer = 5.0f;
	private float thirstTimer = 2.0f;
	private float temperatureTimer = 20.0f;
	//private GUIManager windowManager;
	public Item handItem;
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
		DayNightCycleManager.SharedInstance.OnDayTimeChange += HandleOnDayTimeChange;
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
			hungerTimer = 3.0f;
			NotifyPlayerAttributeChanged();
		}
		else if (playerData.hunger <= 0)
		{
			//PlayerDead();
		}
		if (fatigueTimer <= 0 && playerData.fatigue > 0)
		{
			playerData.fatigue--;
			fatigueTimer = 5.0f;
			NotifyPlayerAttributeChanged();
		}
		if (thirstTimer <= 0 && playerData.thirst > 0)
		{
			playerData.thirst--;
			thirstTimer = 2.0f;
			NotifyPlayerAttributeChanged();
		}
		if (temperatureTimer <= 0)
		{
			int random = Random.Range (0,2);

			if (random == 0)
				temperature++;
			else
				temperature--;

			temperatureTimer = 15.0f;
			NotifyPlayerAttributeChanged();
		}
	}

	void HandleOnDayTimeChange()
	{
		if (DayNightCycleManager.SharedInstance.dayPhase == DayPhase.Day)
		{
			playerData.psyche += 30;
			playerData.coldness += 30;
			NotifyPlayerAttributeChanged();
		}
		else if (DayNightCycleManager.SharedInstance.dayPhase == DayPhase.Night)
		{
			playerData.psyche -= 30;
			playerData.coldness -= 30;
			NotifyPlayerAttributeChanged();
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

	void NotifyPlayerAttributeChanged()
	{
		if(OnPlayerAttributeChange!=null) {
			OnPlayerAttributeChange();
		}
	}

	void PlayerDead()
	{
		GUIManager.SharedInstance.ShowInteractionOverlay("You starved to death.");
	}
}

