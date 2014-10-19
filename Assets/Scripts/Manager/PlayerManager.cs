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
	public int health;
	//public float experience;
}

public class PlayerManager : MonoBehaviour
{
	public PlayerData playerData;

	private float timer = 0.5f;
	//private GUIManager windowManager;
	public ThrowableItem handItem;
	private GameObject hungerText;
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
	// Use this for initialization
	void Start ()
	{
		playerData = new PlayerData();
		playerData.health = 100;
		playerData.hunger = 100;
		playerData.thirst = 100;
		playerData.fatigue = 100;
		hungerText = GameObject.Find ("HungerText");
		//windowManager = GameObject.Find ("SceneManager").GetComponent<GUIManager>();
	}

	// Update is called once per frame
	void Update ()
	{
		playerData.hunger = Mathf.Min (playerData.hunger, 100);
		timer -= Time.deltaTime;
		if (timer <= 0 && playerData.hunger > 0)
		{
			playerData.hunger--;
			if (playerData.hunger <= 60 && playerData.hunger > 30)
				hungerText.GetComponent<Text>().color = Color.yellow;
			if (playerData.hunger <= 30)
				hungerText.GetComponent<Text>().color = Color.red;
			hungerText.GetComponent<Text>().text = "Hunger: "+playerData.hunger+"%";
			timer = 2.0f;
		}
		else if (playerData.hunger <= 0)
		{
			PlayerDead();
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

