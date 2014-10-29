using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable] 
class VariableStorage {
	public Hashtable globalVariables;
}

/*[System.Serializable] 
class VariableStorage {
	public DictionaryOfStringAndInt<string, int> storedVariables;
}*/

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

	private VariableStorage variableStorage;

	void Awake()
	{
		//variableContainer = new VariableContainer();
		//variableContainer.globalVariables = new Dictionary<string, int>();
		LoadVariables();
	}

	void LoadVariables()
	{
		if(File.Exists (Application.persistentDataPath + "/variableInfo.dat") && GUIManager.SharedInstance.loadAndSaveStuff)
		{
			//Debug.Log ("Path: "+Application.persistentDataPath);
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/variableInfo.dat", FileMode.Open);
			variableStorage = bf.Deserialize(file) as VariableStorage;
			file.Close ();
		}
		else
		{
			variableStorage = new VariableStorage();
			variableStorage.globalVariables = new Hashtable();
		}
			//containerData.storedVariables = new SerializableDictionary<string, int>();
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(VariableData));
		FileStream readFileStream = new FileStream("Assets/Core Assets/Resources/variables.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
		VariableData data = (VariableData)xmlSerializer.Deserialize(readFileStream);
		readFileStream.Close();
			
		foreach (Variable var in data.variables)
		{
			if(!variableStorage.globalVariables.ContainsKey(var.name))
			{
				variableStorage.globalVariables.Add (var.name, (int)var.value);
			}
		}
	}

	public void SaveVariables()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/variableInfo.dat");
		
		//VariableStorage data = new VariableStorage();
		//data.storedVariables = globalVariables;

		//Debug.Log ("file: "+file+", data: "+data);
		bf.Serialize (file, variableStorage);
		file.Close ();
	}

	public int GetGlobalVariable(string name)
	{
		if (variableStorage.globalVariables.ContainsKey (name))
			return (int)variableStorage.globalVariables[name];
		else return -1;
	}
	
	public bool SetGlobalVariable(string name, int value)
	{
		if (variableStorage.globalVariables.ContainsKey (name))
			variableStorage.globalVariables[name] = value;
		else
			return false;
		return true;
	}
}

