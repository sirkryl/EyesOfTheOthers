using UnityEngine;
using System.Collections;

public abstract class Selectable : MonoBehaviour {

	public string id_string;
	public string name;
	public string highlight { get; protected set; }
	public abstract void HandleSelection();
}