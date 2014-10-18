using UnityEngine;
using System.Collections;

public class GenerateTerrain : MonoBehaviour {

	private Transform[] tiles;
	private Vector3[] positions;
	private int random;
	public int rows = 3;
	public int cols = 3;
	private Transform block1Fab;
	private GameObject gameObjectParent;

	void Awake () {
		if (GameObject.Find ("block1") != null)
		{
			Destroy(GameObject.Find ("block1"));
		}
	}
	// Use this for initialization
	void Start () {
		StateManager.SharedInstance.SetGameState(GameState.Free);
		gameObjectParent = GameObject.FindWithTag("Gameplay");
		
		block1Fab = Resources.Load ("Prefabs/World/block1", typeof(Transform)) as Transform;
		//uncomment the following two lines to use other system again
		UseRowsCols ();
		return;

		positions = new Vector3[8];
		positions [0] = new Vector3 (0, 1, 0);
		positions [1] = new Vector3 (0, 1, 300);
		positions [2] = new Vector3 (0, 1, 600);
		positions [3] = new Vector3 (300, 1, 600);
		positions [4] = new Vector3 (600, 1, 600);
		positions [5] = new Vector3 (600, 1, 300);
		positions [6] = new Vector3 (600, 1, 0);
		positions [7] = new Vector3 (300, 1, 0);
		tiles = new Transform[8];
		for (int i=0; i<8; i++){
			//tiles[i] = (GameObject) GameObject.Instantiate(Resources.Load("block1"));
			tiles[i] = Instantiate(block1Fab) as Transform;
			/*switch (i) {
			case 0: cube.renderer.material.color = Color.red; break;
			case 1: cube.renderer.material.color = Color.blue; break;
			case 2: cube.renderer.material.color = Color.green; break;
			case 3: cube.renderer.material.color = Color.yellow; break;
			case 4: cube.renderer.material.color = Color.black; break;
			case 5: cube.renderer.material.color = Color.white; break;
			case 6: cube.renderer.material.color = Color.cyan; break;
			case 7: cube.renderer.material.color = Color.magenta; break;
			}*/
			/*tiles[i].localRotation = Quaternion.identity;
			random = Random.Range(0,4);
			switch (random){
			case 0: break;
			case 1: tiles[i].Rotate(new Vector3(0,90,0)); break;
			case 2: tiles[i].Rotate(new Vector3(0,180,0)); break;
			case 3: tiles[i].Rotate(new Vector3(0,270,0)); break;
			}*/
		}

		Vector3[] positions_random = shuffle(positions);
		for (int i=0; i<8; i++) {
			tiles[i].position = positions_random[i];
		}
	}

	private Vector3[] shuffle(Vector3[] array){
		
		int m = array.Length;
		Vector3 v;
		int i = 0;
		
		while (m!=0) {
			i = Random.Range(0,m--);
			v=array[m];
			array[m] = array[i];
			array[i] = v;
		}
		return array;
	}

	private void UseRowsCols()
	{
		tiles = new Transform[rows*cols];
		int cnt = 0;
		int rowBegin = -rows/2;
		int colBegin = -cols/2;
		for (int row = rowBegin; row <= rows/2; row++)
		{
			for (int col = colBegin; col <= cols/2; col++)
			{
				Debug.Log ("row: "+row+", col: "+col);
				//-300, 1, -300
				// 0, 1, 0
				//300, 1, 300
				tiles[cnt] = Instantiate(block1Fab, new Vector3(row*300, 0, col*300), Quaternion.identity) as Transform;
				tiles[cnt].parent = gameObjectParent.transform;
				cnt++;
			}
		}
	}
}