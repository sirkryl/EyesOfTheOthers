using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogEventHandler : MonoBehaviour {

	private Dialog eventDialog;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SendAnswer(int answer)
	{
		eventDialog.SendAnswer(answer);
	}

	public void RegisterForEvents(Dialog dialog)
	{
		eventDialog = dialog;
	}

	public void Unregister()
	{
		eventDialog = null;
	}
}
