using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class StateManager : MonoBehaviour
{
	private float timeOfDay;

	private static StateManager instance = null;
	public static StateManager SharedInstance {
			get {
				if (instance == null) {
				instance = MainComponentManager.AddMainComponent<StateManager> ();
				LoadVariables();
				}
				return instance;
			}
		}

	[XmlRoot("variables")]
	public class VariableData {
		[XmlElement("variable")]
		public Variable[] variables;
	}
	
	public class Variable {
		[XmlAttribute]
		public string name;
		[XmlAttribute]
		public int value;
	}
	
	private static Dictionary<string, int> globalVariables;
	
	static void LoadVariables()
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(VariableData));
		FileStream readFileStream = new FileStream("Assets/Resources/variables.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
		VariableData data = (VariableData)xmlSerializer.Deserialize(readFileStream);
		readFileStream.Close();
		globalVariables = new Dictionary<string, int>();
		foreach (Variable var in data.variables)
		{
			globalVariables.Add (var.name,var.value);
		}
		
	}
	
	public int GetGlobalVariable(string name)
	{
		//Debug.Log ("name: "+name);
		return globalVariables[name];
	}
	
	public void SetGlobalVariable(string name, int value)
	{
		globalVariables[name] = value;
	}


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

