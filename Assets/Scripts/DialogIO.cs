using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;
public class DialogIO : MonoBehaviour {

	//public static Dictionary<string, DialogData> dialogMap;

	[XmlRoot("dialog")]
	public class DialogData {
		[XmlAttribute]
		public string characterName;
		[XmlAttribute]
		public int startsWith;
		//[XmlElement("variable")]
		//public DialogVariable[] dialogVariable;
		[XmlElement("element")]
		public DialogElement[] dialogElement;
		//[XmlElement("text")]
		//public DialogText[] dialogText;
		//[XmlElement("choice")]
		//public DialogChoice[] dialogChoice;
	}

	public class DialogElement {
		[XmlAttribute]
		public int id;
		[XmlAttribute]
		public int leadsTo;
		[XmlAttribute]
		public string variable;
		[XmlText]
		public string text;
		[XmlAttribute]
		public string type;
		[XmlAttribute]
		public string name;
		[XmlAttribute]
		public int value;
		[XmlAttribute]
		public string vartype = "local";
		[XmlElement("answer")]
		public DialogAnswer[] dialogAnswers;
		[XmlElement("case")]
		public DialogCase[] dialogCases;
	}

	public class DialogCase {
		[XmlAttribute]
		public string variable;
		[XmlAttribute]
		public string item;
		[XmlAttribute]
		public string type;
		[XmlAttribute]
		public int value;
		[XmlAttribute]
		public int leadsTo;
		[XmlAttribute]
		public bool reset;
		[XmlAttribute]
		public string vartype = "local";
	}
	/*public class DialogText {
		[XmlAttribute]
		public int id;
		[XmlAttribute]
		public int leadsTo;
		[XmlText]
		public string text;
	}

	public class DialogChoice {
		[XmlAttribute]
		public int id;
		[XmlText]
		public string text;
		[XmlElement("answer")]
		public DialogAnswer[] dialogAnswers;
	}*/

	public class DialogAnswer {
		[XmlAttribute]
		public int id;
		[XmlAttribute]
		public int leadsTo;
		[XmlText]
		public string text;
	}

	/*public class DialogVariable {
		[XmlAttribute]
		public string type;
		[XmlAttribute]
		public string name;
		[XmlAttribute]
		public string value;
	}*/

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		/*Debug.Log ("Name:" +data.characterName);
		Debug.Log ("Text size: "+data.dialogText.Length);
		for(int i = 0; i < data.dialogText.Length; i++)
		{
			Debug.Log ("Text ids: "+data.dialogText[i].id);
			Debug.Log ("Text content: "+data.dialogText[i].text);
		}
		Debug.Log ("Choice size: "+data.dialogChoice.Length);
		for(int i = 0; i < data.dialogChoice.Length; i++)
		{
			Debug.Log ("Choice ids: "+data.dialogChoice[i].id);
			Debug.Log ("Choice size: "+data.dialogChoice[i].dialogAnswers.Length);
			for (int j = 0; j < data.dialogChoice[i].dialogAnswers.Length; j++)
			{
				Debug.Log ("Answer ids: "+data.dialogChoice[i].dialogAnswers[j].id);
			}
		}*/
	}

	public DialogData Load(string charName)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(DialogData));
		FileStream readFileStream = new FileStream("Assets/Resources/Dialog/mechanic.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
		DialogData data = (DialogData)xmlSerializer.Deserialize(readFileStream);
		readFileStream.Close();
		foreach (DialogElement element in data.dialogElement)
		{
			//int index = element.text.IndexOf(System.Environment.NewLine);
			if (element.text == null)
				continue;
			element.text = element.text.Replace("\r", "").Replace("\n", "");
			if (element.dialogAnswers != null)
			{
				foreach (DialogAnswer answer in element.dialogAnswers)
				{
					//index = answer.text.IndexOf(System.Environment.NewLine);
					answer.text = answer.text.Replace("\r", "").Replace("\n", "");
				}
			}
		}
		return data;
		//Debug.Log ("Count: "+dialogMap.Count);
		//dialogMap.Add (charName, data);
	}
}
