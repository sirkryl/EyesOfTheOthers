using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
	private int health = 100;
	private int hunger = 100;
	private int thirst = 100;
	private int sleep = 100;
	private float timer = 2.0f;
	private WindowManager windowManager;
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
		windowManager = GameObject.Find ("SceneManager").GetComponent<WindowManager>();
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

	void PlayerDead()
	{
		windowManager.ShowInteractionOverlay("You starved to death.");
		Debug.Log ("player dead");
	}
}

