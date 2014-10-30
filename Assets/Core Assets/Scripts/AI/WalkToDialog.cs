using UnityEngine;
using System.Collections;
//using Pathfinding;
public class WalkToDialog : MonoBehaviour {

	private AIPath pathFinder;
	public bool enabled = false;
	// Use this for initialization
	void Start () {
		pathFinder = GetComponent<AIPath>();
		//seeker = GetComponent<Seeker>();
		//seeker.pathCallback += OnPathComplete;
	}

	void OnDisable()
	{
		//seeker.pathCallback -= OnPathComplete;
	}
	// Update is called once per frame
	void Update () {
		//seeker.pathCallback += OnPathComplete;
	}

	public void OnTargetReached ()
	{
		if(enabled)
		{
			GetComponent<Dialog>().HandleSelection();
		}

		pathFinder.canSearch = false;
		//pathFinder.character.Move(gameObject.transform.position, false, false, Camera.main.transform.position);

		Destroy(this);

	}
}
