using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
class PlayerData
{
	public int hunger;
	public float experience;
}

public class PlayerManager : MonoBehaviour
{
	private int health = 100;
	private int hunger = 100;
	private int thirst = 100;
	private int sleep = 100;
	private float timer = 0.5f;
	//private WindowManager windowManager;
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
		hungerText = GameObject.Find ("HungerText");
		//windowManager = GameObject.Find ("SceneManager").GetComponent<WindowManager>();
	}

	// Update is called once per frame
	void Update ()
	{
		timer -= Time.deltaTime;
		if (timer <= 0 && hunger > 0)
		{
			hunger--;
			if (hunger <= 60 && hunger > 30)
				hungerText.GetComponent<Text>().color = Color.yellow;
			if (hunger <= 30)
				hungerText.GetComponent<Text>().color = Color.red;
			hungerText.GetComponent<Text>().text = "Hunger: "+hunger+"%";
			timer = 2.0f;
		}
		else if (hunger <= 0)
		{
			PlayerDead();
		}
	}

	public void Save()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

		PlayerData data = new PlayerData();
		data.hunger = hunger;

		bf.Serialize (file, data);
		file.Close ();
	}

	public void Load()
	{
		if(File.Exists (Application.persistentDataPath + "/playerInfo.dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			PlayerData data = bf.Deserialize(file) as PlayerData;
			file.Close ();
			//hunger = data.hunger;
		}
	}

	void PlayerDead()
	{
		WindowManager.SharedInstance.ShowInteractionOverlay("You starved to death.");
		Debug.Log ("player dead");
	}
}

