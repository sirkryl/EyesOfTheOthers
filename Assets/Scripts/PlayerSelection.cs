using UnityEngine;
using System.Collections;

public class PlayerSelection : MonoBehaviour {
	

	// Update is called once per frame
	void Update () {
		if (StateManager.SharedInstance.gameState == GameState.Free)
		{
			//raycast for dialog objects
			Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
			RaycastHit hit;// = new RaycastHit();
			Debug.DrawRay (ray.origin, ray.direction * 2, Color.green);
			if(Physics.Raycast(ray, out hit, 2))
			{
				Selectable selectedObject = hit.transform.gameObject.GetComponent<Selectable>();
				if(selectedObject != null)
				{
					if (Input.GetKeyDown (KeyCode.E))
					{
						selectedObject.HandleSelection();
						GUIManager.SharedInstance.HideInteractionOverlay();
					}
					GUIManager.SharedInstance.ShowInteractionOverlay(selectedObject.highlight);
				}
			}
			else
			{
				GUIManager.SharedInstance.HideInteractionOverlay();
			}
		}

	}
}
