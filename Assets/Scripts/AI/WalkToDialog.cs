using UnityEngine;
using System.Collections;
//using Pathfinding;
public class WalkToDialog : MonoBehaviour {

	private AIPath pathFinder;
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
		GetComponent<Dialog>().HandleSelection();
		pathFinder.canMove = false;
		pathFinder.canSearch = false;
		Destroy(this);
	}
}
