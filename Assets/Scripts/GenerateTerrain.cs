using UnityEngine;
using System.Collections;

public class GenerateTerrain : MonoBehaviour {

	private Terrain[][] tiles;
	private object[] blockPrefabs;
	private Vector3[] positions;
	private int random;
	public bool debug = true;
	public int rows = 3;
	public int cols = 3;
	private Transform block1Fab;
	private GameObject gameObjectParent;
	public bool noGenerationPlease = false;
	void Awake () {
		if(!noGenerationPlease)
		{
			if (GameObject.Find ("block1") != null)
			{
				Destroy(GameObject.Find ("block1"));
			}
		}
	}
	// Use this for initialization
	void Start () {
		StateManager.SharedInstance.SetGameState(GameState.Free);
		gameObjectParent = GameObject.FindWithTag("Gameplay");

		blockPrefabs = Resources.LoadAll ("Prefabs/Blocks", typeof(GameObject));

		//block1Fab = Resources.Load ("Prefabs/World/block1", typeof(Transform)) as Transform;
		//uncomment the following two lines to use other system again

		if(!noGenerationPlease)
		{
			UseRowsCols ();
			AstarPath.active.Scan();
		}
		return;

		/*positions = new Vector3[8];
		positions [0] = new Vector3 (0, 1, 0);
		positions [1] = new Vector3 (0, 1, 300);
		positions [2] = new Vector3 (0, 1, 600);
		positions [3] = new Vector3 (300, 1, 600);
		positions [4] = new Vector3 (600, 1, 600);
		positions [5] = new Vector3 (600, 1, 300);
		positions [6] = new Vector3 (600, 1, 0);
		positions [7] = new Vector3 (300, 1, 0);
		tiles = new Terrain[8];
		for (int i=0; i<8; i++){
			//tiles[i] = (GameObject) GameObject.Instantiate(Resources.Load("block1"));
			//tiles[i] = Instantiate(blockPrefabs[Random.Range (0,blockPrefabs.Length)]) as Transform;
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
		//}

		/*Vector3[] positions_random = shuffle(positions);
		for (int i=0; i<8; i++) {
			tiles[i].position = positions_random[i];
		}*/
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
		tiles = new Terrain[rows][];
		int cnt = 0;
		int rowBegin = -rows/2;
		int colBegin = -cols/2;
		int rowCount = 0;

		for (int row = rowBegin; row <= rows/2; row++)
		{
			tiles[rowCount] = new Terrain[cols];
			int colCount = 0;
			for (int col = colBegin; col <= cols/2; col++)
			{
				if(debug && col == 0 && row == 0)
					tiles[rowCount][colCount] = 
						((GameObject)Instantiate((GameObject)(blockPrefabs[0]), new Vector3(row*300, 0, col*300), Quaternion.identity)).GetComponent<Terrain>();
				else
					tiles[rowCount][colCount] = 
						((GameObject)Instantiate((GameObject)(blockPrefabs[Random.Range (0,blockPrefabs.Length)]), new Vector3(row*300, 0, col*300), Quaternion.identity)).GetComponent<Terrain>();
				tiles[rowCount][colCount].transform.parent = gameObjectParent.transform;
				cnt++;
				colCount++;
			}
			rowCount++;
		}

		for (int row = 0; row < rows; row++)
		{
			for (int col = 0; col < cols; col++)
			{
				Terrain leftNeighbor = null;
				Terrain rightNeighbor = null;
				Terrain topNeighbor = null;
				Terrain bottomNeighbor = null;
				if(col != cols-1)
					rightNeighbor = tiles[row][col+1];
					//tiles[row][col].SetNeighbors (null, null, tiles[row][col+1],tiles[row+1][col+1]);
				if(col != 0)
					leftNeighbor = tiles[row][col-1];
				if(row != rows-1)
					bottomNeighbor = tiles[row+1][col];
				if(row != 0)
					topNeighbor = tiles[row-1][col];

				tiles[row][col].SetNeighbors (leftNeighbor, topNeighbor, rightNeighbor, bottomNeighbor);
			}
		}
	}
}