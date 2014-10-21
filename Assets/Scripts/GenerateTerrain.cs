using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GenerateTerrain : MonoBehaviour {

	private Terrain[][] tiles;
	private object[] blockPrefabs;
	private Vector3[] positions;
	private float[,,,] heightMaps;
	private int random;
	public bool debug = true;
	private Dictionary<Terrain,float[,,]> alphaMapReset;
	private Dictionary<Terrain,SplatPrototype[]> textureReset;
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

		alphaMapReset = new Dictionary<Terrain, float[,,]>();
		textureReset = new Dictionary<Terrain, SplatPrototype[]>();
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
				{
					tiles[rowCount][colCount] = 
						((GameObject)Instantiate((GameObject)(blockPrefabs[0]), new Vector3(row*300, 0, col*300), Quaternion.identity)).GetComponent<Terrain>();

				}
				else
					tiles[rowCount][colCount] = 
						((GameObject)Instantiate((GameObject)(blockPrefabs[Random.Range (0,blockPrefabs.Length)]), new Vector3(row*300, 0, col*300), Quaternion.identity)).GetComponent<Terrain>();
				tiles[rowCount][colCount].terrainData = (TerrainData) Object.Instantiate (tiles[rowCount][colCount].terrainData);
				tiles[rowCount][colCount].transform.parent = gameObjectParent.transform;
				float[,] heightMap = tiles[rowCount][colCount].terrainData.GetHeights (0,0,513,513);
				for(int i=0; i<=20;i++)
					for(int j=0; j <=20; j++)
						heightMap[i,j] = 1.0f;
				//tiles[rowCount][colCount].terrainData.SetHeights (0,0,heightMap);
				cnt++;
				colCount++;
			}
			rowCount++;
		}
		heightMaps = new float[rows,cols,513, 513];
		for (int row = 0; row < rows; row++)
		{
			for (int col = 0; col < cols; col++)
			{
				//heightMaps[
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

		for (int row = 0; row < rows; row++)
		{
			for (int col = 0; col < cols; col++)
			{
				int heightWidth = tiles[row][col].terrainData.heightmapWidth;
				int heightHeight = tiles[row][col].terrainData.heightmapHeight;
				float[,] ownHeightMap = tiles[row][col].terrainData.GetHeights (0,0,heightWidth,heightHeight);
				SplatPrototype[] ownTextures = tiles[row][col].terrainData.splatPrototypes;
				textureReset.Add(tiles[row][col],(SplatPrototype[])ownTextures.Clone ());
				int oAlphaWidth = tiles[row][col].terrainData.alphamapWidth;
				int oAlphaHeight = tiles[row][col].terrainData.alphamapHeight;
				float[,,] ownAlphaMap = tiles[row][col].terrainData.GetAlphamaps(0,0,oAlphaWidth,oAlphaHeight);
				alphaMapReset.Add (tiles[row][col],(float[,,])ownAlphaMap.Clone ());
				int[,] rightDetailMap;
				float[,,] rightAlphaMap;
				float[,] rightHeightMap;
				float[,] bottomHeightMap; 
				if(col != cols-1)
				{
					rightHeightMap = tiles[row][col+1].terrainData.GetHeights (0,0,heightWidth,heightHeight);

					int alphaWidth = tiles[row][col+1].terrainData.alphamapWidth;
					int alphaHeight = tiles[row][col+1].terrainData.alphamapHeight;
					SplatPrototype[] rightTextures = tiles[row][col+1].terrainData.splatPrototypes;

					List<int> changeTextures = new List<int>();
					List<SplatPrototype> newTextures = new List<SplatPrototype>();
					Dictionary<int,int> combinedTextureMap = new Dictionary<int,int>();

					int added = 0;
					for(int i = 0; i < rightTextures.Length; i++)
					{
						bool alreadyHasIt = false;
						for (int j = 0; j < ownTextures.Length; j++)
						{
							if(ownTextures[j].texture == rightTextures[i].texture)
							{
								alreadyHasIt = true;
								combinedTextureMap.Add(j,i);
								changeTextures.Add (j);
							}
						}
						//if(alreadyHasIt)
						//	Debug.Log ("already has texture #"+i);

						if(!alreadyHasIt)
						{
							combinedTextureMap.Add(ownTextures.Length+added,i);
							newTextures.Add (rightTextures[i]);
							added++;
						}
					}
					SplatPrototype[] combinedTextures = new SplatPrototype[ownTextures.Length+newTextures.Count];
					float[,,] newAlphamaps = new float[oAlphaWidth, oAlphaHeight, ownTextures.Length+newTextures.Count];
					for (int i = 0; i < ownTextures.Length; i++)
					{
						combinedTextures[i] = ownTextures[i];
						for (int x = 0; x < oAlphaWidth; x++)
						{
							for (int y = 0; y < oAlphaHeight; y++)
							{
								newAlphamaps[x,y,i] = ownAlphaMap[x,y,i];
							}
						}
					}
					int index = ownTextures.Length;
					foreach (SplatPrototype texture in newTextures)
					{

						changeTextures.Add (index);
						combinedTextures[index] = texture;
						for (int x = 0; x < oAlphaWidth; x++)
						{
							for (int y = 0; y < oAlphaHeight; y++)
							{
								newAlphamaps[x,y,index] = 0.0f;
							}
						}
						index++;
					}

					//Debug.Log ("texture Count: "+combinedTextures.Length);
					//Debug.Log ("alphamaps Count: "+newAlphamaps.GetLength (2));

					tiles[row][col].terrainData.splatPrototypes = combinedTextures;
					tiles[row][col].terrainData.SetAlphamaps(0,0,newAlphamaps);
					tiles[row][col].terrainData.RefreshPrototypes();
					//Debug.Log ("alphamaps length in tD: "+tiles[row][col].terrainData.GetAlphamaps(0,0,alphaWidth, alphaHeight).GetLength (2));
					//Debug.Log ("alphamaps width: "+tiles[row][col].terrainData.alphamapWidth+", height: "+tiles[row][col].terrainData.alphamapHeight);
					//tiles[row][col].terrainData;
					//Debug.Log ("width: "+alphaWidth+", height: "+alphaHeight);
					rightAlphaMap = tiles[row][col+1].terrainData.GetAlphamaps(0,0,alphaWidth,alphaHeight);
					//int detailWidth = tiles[row][col+1].terrainData.detailWidth;
					//int detailHeight = tiles[row][col+1].terrainData.detailHeight;
					//rightAlphaMap = tiles[row[col+1].terrainData.
					//rightDetailMap = tiles[row][col+1].terrainData.GetDetailLayer (0,0,detailWidth, detailHeight,6);
					int xCnt = 1;
					int yCnt = 0;
					float[] difference = new float[tiles[row][col].terrainData.heightmapHeight];
					for (int y = 0; y < tiles[row][col].terrainData.heightmapHeight; y++)
					{
						difference[y] = ownHeightMap[tiles[row][col].terrainData.heightmapWidth-1,y]-rightHeightMap[0,y];
					}
					for (int x = tiles[row][col].terrainData.heightmapWidth-30; x < tiles[row][col].terrainData.heightmapWidth; x++)
					{
						for (int y = 0; y < tiles[row][col].terrainData.heightmapHeight; y++)
						{
							ownHeightMap[x,y] = ownHeightMap[x,y] - (difference[y]/2)*(float)((xCnt/30.0f));///((20-xCnt)/20);//rightHeightMap[20-xCnt,y];
							rightHeightMap[30-xCnt,y] = rightHeightMap[30-xCnt,y] + (difference[y]/2)*(float)(((xCnt)/30.0f));
							if(x < alphaWidth && y < alphaHeight)
							{
								for(int i = 0; i < ownAlphaMap.GetLength (2); i++)
								{
									if(changeTextures.Contains (i))
									{
										newAlphamaps[x,y,i] = rightAlphaMap[x,y,combinedTextureMap[i]] * (xCnt/30.0f);
									}
									else
										newAlphamaps[x,y,i] = ((30-xCnt)/30.0f);
								}
								//newAlphamaps[x,y,0] = ((30-xCnt)/30.0f);
								//rightAlphaMap[x,y,1] = 0.0f;
								//rightAlphaMap[x,y,2] = 0.0f;
								//newAlphamaps[x,y,3] = (xCnt/30.0f);
							}
							//rightDetailMap[30-xCnt,y] = 16;
							yCnt++;
						}
						xCnt++;
					}
					//tiles[row][col].terrainData.SetAlphamaps(0,0,ownAlphaMap);
					//tiles[row][col].terrainData.SetAlphamaps(0,0,newAlphamaps);
					tiles[row][col+1].terrainData.SetHeights (0,0,rightHeightMap);
					//tiles[row][col+1].terrainData.SetDetailLayer (0,0,0,rightDetailMap);
					tiles[row][col].terrainData.SetHeights (0,0,ownHeightMap);
				}
				//tiles[row][col].SetNeighbors (null, null, tiles[row][col+1],tiles[row+1][col+1]);
				//if(col != 0)
				//	leftHeightMap = tiles[row][col-1].terrainData.GetHeights(0,0,513,513);
				if(row != rows-1)
				{
					bottomHeightMap = tiles[row+1][col].terrainData.GetHeights(0,0,513,513);
					int xCnt = 0;
					int yCnt = 1;
					float[] difference = new float[tiles[row][col].terrainData.heightmapHeight];
					for (int x = 0; x < tiles[row][col].terrainData.heightmapWidth; x++)
					{
						difference[x] = ownHeightMap[x,tiles[row][col].terrainData.heightmapHeight-1]-bottomHeightMap[x,0];
					}
					for (int y = tiles[row][col].terrainData.heightmapHeight-30; y < tiles[row][col].terrainData.heightmapHeight; y++)
					{
						for (int x = 0; x < tiles[row][col].terrainData.heightmapWidth; x++)
						{
							//bottomHeightMap[x,512] = 1.0f;
							//ownHeightMap[x,0] = 1.0f;
							ownHeightMap[x,y] = ownHeightMap[x,y] - (difference[x]/2)*(float)((yCnt/30.0f));///((20-xCnt)/20);//rightHeightMap[20-xCnt,y];
							bottomHeightMap[x,30-yCnt] = bottomHeightMap[x,30-yCnt] + (difference[x]/2)*(float)(((yCnt)/30.0f));
							xCnt++;
						}
						yCnt++;
					}
					tiles[row+1][col].terrainData.SetHeights (0,0,bottomHeightMap);
					tiles[row][col].terrainData.SetHeights (0,0,ownHeightMap);
				}

				//if(row != 0)
				//	topHeightMap = tiles[row-1][col].terrainData.GetHeights(0,0,513,513);
			}
		}

		for (int row = 0; row < rows; row++)
		{
			for (int col = 0; col < cols; col++)
			{
				tiles[row][col].GetComponent<TerrainCollider>().terrainData = tiles[row][col].terrainData;
			}
		}
	}

	void OnApplicationQuit() {
		//Debug.Log ("quit");
		foreach (KeyValuePair<Terrain, SplatPrototype[]> entry in textureReset)
		{
			entry.Key.terrainData.splatPrototypes =  entry.Value;
			entry.Key.terrainData.RefreshPrototypes();
		}
		foreach (KeyValuePair<Terrain, float[,,]> entry in alphaMapReset)
		{
			entry.Key.terrainData.SetAlphamaps(0,0,entry.Value);
		}

	}
}