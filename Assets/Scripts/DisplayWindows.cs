using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class DisplayWindows : MonoBehaviour {
	
	public Canvas debugCanvas;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp("f10"))
		{
			debugCanvas.enabled = !debugCanvas.enabled;     
		}
	}
}
