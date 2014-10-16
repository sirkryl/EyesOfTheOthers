using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public enum GameState { NullState, Free, Dialog, Interface }
public delegate void OnStateChangeHandler();
public class StateManager : MonoBehaviour
{

	public event OnStateChangeHandler OnStateChange;
	public GameState gameState { get; private set; }

	private float timeOfDay;
	//private string gameState = "free";
	private static StateManager instance = null;
	public static StateManager SharedInstance {
			get {
				if (instance == null) {
				instance = MainComponentManager.AddMainComponent<StateManager> ();
				//LoadVariables();
				}
				return instance;
			}
		}



	public void SetGameState(GameState gameState) {
		this.gameState = gameState;
		if(OnStateChange!=null) {
			OnStateChange();
		}
	}


	


	/*public string GetGameState()
	{
		return gameState;
	}

	public void SetGameState(string state)
	{
		gameState = state;
	}*/


	/*public float GetTimeOfDay()
	{
		return timeOfDay;
	}*/

	/*public void SetTimeOfDay(float value)
	{
		timeOfDay = value;
	}*/
	
	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{

	}
}

