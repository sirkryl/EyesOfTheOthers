using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class GlobalVariableManager : MonoBehaviour
{
	private static GlobalVariableManager instance = null;
	public static GlobalVariableManager SharedInstance {
		get {
			if (instance == null) {
				instance = MainComponentManager.AddMainComponent<GlobalVariableManager> ();
				//LoadVariables();
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
	[System.Serializable] 
	class VariableStorage {
		public SerializableDictionary<string, int> storedVariables;
	}
	
	private Dictionary<string, int> globalVariables;

	void Awake()
	{
		//variableContainer = new VariableContainer();
		//variableContainer.globalVariables = new Dictionary<string, int>();
		LoadVariables();
	}

	void LoadVariables()
	{
		//VariableStorage containerData = new VariableStorage();
		//containerData.storedVariables = new Dictionary<string, int>();
		/*if(File.Exists (Application.persistentDataPath + "/variableInfo2.dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/variableInfo2.dat", FileMode.Open);
			containerData = bf.Deserialize(file) as VariableStorage;
			file.Close ();
		}*/

		XmlSerializer xmlSerializer = new XmlSerializer(typeof(VariableData));
		FileStream readFileStream = new FileStream("Assets/Resources/variables.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
		VariableData data = (VariableData)xmlSerializer.Deserialize(readFileStream);
		readFileStream.Close();
		globalVariables = new SerializableDictionary<string, int>();
		foreach (Variable var in data.variables)
		{
			//if(containerData != null)
			//{
			//	if(containerData.storedVariables.ContainsKey (var.name))
			//	{
			//		globalVariables.Add (var.name, containerData.storedVariables[var.name]);
			//		continue;
			//	}
			//}
			globalVariables.Add (var.name,var.value);
		}
	}

	public void SaveVariables()
	{
		/*BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/variableInfo2.dat");
		
		VariableStorage data = new VariableStorage();
		data.storedVariables = globalVariables;

		//Debug.Log ("file: "+file+", data: "+data);
		bf.Serialize (file, data);
		file.Close ();*/
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
}

