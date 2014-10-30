
using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float lifeTime = 5.0f;
	public float speed;
	// Use this for initialization
	void Start () {
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		rigidbody.AddForce (ray.direction * speed);
	}
	
	// Update is called once per frame
	void Update () {
		lifeTime -= Time.deltaTime;
		if(lifeTime <= 0)
			Destroy(gameObject);
	}
}
