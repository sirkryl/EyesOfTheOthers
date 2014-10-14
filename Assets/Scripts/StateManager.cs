using UnityEngine;
using System.Collections;

public class StateManager : MonoBehaviour
{
	private float timeOfDay;

	private static StateManager instance = null;
	public static StateManager SharedInstance {
			get {
				if (instance == null) {
				instance = MainComponentManager.AddMainComponent<StateManager> ();

				}
				return instance;
			}
		}

	public float GetTimeOfDay()
	{
		return timeOfDay;
	}

	public void SetTimeOfDay(float value)
	{
		timeOfDay = value;
	}
	
	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{

	}
}

