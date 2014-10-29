using UnityEngine;
using System.Collections;
using Pathfinding;
public class CustomAIPath : MonoBehaviour {
	public Transform targetPosition;

	private Seeker seeker;
	private CharacterController controller;

	public Path path;

	public float speed = 10;
	public float nextWaypointDistance = 3;

	private int currentWaypoint = 0;
	
	// Use this for initialization
	void Start () {
		Seeker seeker = GetComponent<Seeker>();
		controller = GetComponent<CharacterController>();
		seeker.StartPath (transform.position, targetPosition.position, OnPathComplete);
	}
	
	// Update is called once per frame
	public void OnPathComplete (Path p)
	{
		Debug.Log ("Yay");
		if (!p.error) {
			path = p;
			currentWaypoint = 0;
		}
	}

	public void FixedUpdate() {
		if (path == null)
			return;

		if (currentWaypoint >= path.vectorPath.Count)
		{
			Debug.Log ("End of path");
			return;
		}

		Vector3 dir = (path.vectorPath[currentWaypoint]-transform.position).normalized;
		dir *= speed * Time.fixedDeltaTime;
		controller.SimpleMove (dir);

		if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) 
		{
			currentWaypoint++;
			return;
		}
	}
}
