using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugFunctions : MonoBehaviour {

	Text spawnText;
	// Use this for initialization
	void Start () {
		spawnText = GameObject.Find ("SpawnText").GetComponent<Text>();
	}

	
	// Update is called once per frame
	void Update () {

	}

	public void SpawnObject(string objectName)
	{

		GameObject spawnObject = Resources.Load("Prefabs/Items/"+objectName, typeof(GameObject)) as GameObject;

		if(spawnObject == null)
			spawnObject = Resources.Load("Prefabs/Characters/"+objectName, typeof(GameObject)) as GameObject;

		if(spawnObject != null)
		{
			Instantiate (spawnObject, Camera.main.ViewportToWorldPoint(new Vector3(0.5f,0.5f,0.5f)), Quaternion.identity);
			//if (spawnText != null)
			//	spawnText.text = "";
		}

	}

	public void SpawnObject()
	{
		if(spawnText != null)
			SpawnObject(spawnText.text);
	}
}
