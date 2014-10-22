using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GenerateTerrain : MonoBehaviour
{
	private Terrain[][] tiles;
	private object[] blockPrefabs;

	public bool debug = true;

	public int rows = 3;
	public int cols = 3;

	public bool smoothBorders = true;
	public int smoothDistance = 30;
	public bool smoothLeft = true;
	public bool smoothRight = true;
	public bool smoothTop = true;
	public bool smoothBottom = true;


	private GameObject gameObjectParent;
	public bool noGenerationPlease = false;

	void Awake ()
	{
		if (!noGenerationPlease) 
		{
			if (GameObject.Find ("block1") != null) 
			{
				Destroy (GameObject.Find ("block1"));
			}
		}
	}

	// Use this for initialization
	void Start ()
	{
		gameObjectParent = GameObject.FindWithTag ("Gameplay");

		blockPrefabs = Resources.LoadAll ("Prefabs/Blocks", typeof(GameObject));

		if (!noGenerationPlease) 
		{
			GenerateTerrainBlocks ();
			AstarPath.active.Scan ();
		}
		return;
	}

	private void GenerateTerrainBlocks ()
	{
		tiles = new Terrain[rows][];
		int rowBegin = -rows / 2;
		int colBegin = -cols / 2;
		int rowCount = 0;

		for (int row = rowBegin; row <= rows/2; row++) 
		{
			tiles [rowCount] = new Terrain[cols];
			int colCount = 0;
			for (int col = colBegin; col <= cols/2; col++) 
			{
				if (debug && col == 0 && row == 0) 
				{
					tiles [rowCount] [colCount] = 
						((GameObject)Instantiate ((GameObject)(blockPrefabs [0]), new Vector3 (row * 300, 0, col * 300), Quaternion.identity)).GetComponent<Terrain> ();
				} 
				else
					tiles [rowCount] [colCount] = 
						((GameObject)Instantiate ((GameObject)(blockPrefabs [Random.Range (0, blockPrefabs.Length)]), new Vector3 (row * 300, 0, col * 300), Quaternion.identity)).GetComponent<Terrain> ();

				CreateNewTerrainData (tiles [rowCount] [colCount]);

				tiles [rowCount] [colCount].transform.parent = gameObjectParent.transform;

				colCount++;
			}
			rowCount++;
		}

		if(smoothBorders)
			SmoothBorders (tiles);

		UpdateColliders (tiles);
		SetNeighbors (tiles);
	}

	void UpdateColliders (Terrain[][] tiles)
	{
		for (int row = 0; row < rows; row++) 
		{
			for (int col = 0; col < cols; col++) 
			{
				tiles [row] [col].GetComponent<TerrainCollider> ().terrainData = tiles [row] [col].terrainData;
			}
		}
	}

	void SmoothBorders (Terrain[][] tiles)
	{
		for (int row = 0; row < rows; row++) 
		{
			for (int col = 0; col < cols; col++) 
			{
				int ownHeightMapWidth = tiles [row] [col].terrainData.heightmapWidth;
				int ownHeightMapHeight = tiles [row] [col].terrainData.heightmapHeight;
				float[,] ownHeightMap;// = tiles [row] [col].terrainData.GetHeights (0, 0, ownHeightMapWidth, ownHeightMapHeight);

				int ownAlphaMapWidth = tiles [row] [col].terrainData.alphamapWidth;
				int ownAlphaMapHeight = tiles [row] [col].terrainData.alphamapHeight;
				float[,,] ownAlphaMap;// = tiles [row] [col].terrainData.GetAlphamaps (0, 0, ownAlphaMapWidth, ownAlphaMapHeight);

				SplatPrototype[] ownTextures;// = tiles [row] [col].terrainData.splatPrototypes;
				//int[,] rightDetailMap;
				float[,,] rightAlphaMap;
				float[,] rightHeightMap;
				float[,] bottomHeightMap; 
				float[,,] bottomAlphaMap;

				//LEFT TO RIGHT
				if (col != cols - 1) 
				{
					ownTextures = tiles [row] [col].terrainData.splatPrototypes;
					ownHeightMap = tiles [row] [col].terrainData.GetHeights (0, 0, ownHeightMapWidth, ownHeightMapHeight);
					ownAlphaMap = tiles [row] [col].terrainData.GetAlphamaps (0, 0, ownAlphaMapWidth, ownAlphaMapHeight);

					int rightHeightMapWidth = tiles [row] [col + 1].terrainData.heightmapWidth;
					int rightHeightMapHeight = tiles [row] [col + 1].terrainData.heightmapHeight;
					rightHeightMap = tiles [row] [col + 1].terrainData.GetHeights (0, 0, rightHeightMapWidth, rightHeightMapHeight);

					int rightAlphaMapWidth = tiles [row] [col + 1].terrainData.alphamapWidth;
					int rightAlphaMapHeight = tiles [row] [col + 1].terrainData.alphamapHeight;
					rightAlphaMap = tiles [row] [col + 1].terrainData.GetAlphamaps (0, 0, rightAlphaMapWidth, rightAlphaMapHeight);

					SplatPrototype[] rightTextures = tiles [row] [col + 1].terrainData.splatPrototypes;

					List<int> changeTextures = new List<int> ();
					List<int> doubleTextures = new List<int> ();
					List<SplatPrototype> newTextures = new List<SplatPrototype> ();
					Dictionary<int,int> combinedTextureMap = new Dictionary<int,int> ();
					Dictionary<int,float[,,]> rightTextureMap = new Dictionary<int,float[,,]> ();
					float[,,] combinedAlphaMaps = new float[1,1,1];
					SplatPrototype[] combinedTextures = new SplatPrototype[1];
					//Dictionary<int,float> ownTextureMap = new Dictionary<int,float> ();
					int texturesToBeAdded = 0;
					int index;
					if (smoothLeft)
					{
						for (int i = 0; i < rightTextures.Length; i++) 
						{
							bool alreadyHasIt = false;
							for (int j = 0; j < ownTextures.Length; j++) 
							{
								if (ownTextures [j].texture == rightTextures [i].texture) 
								{
									alreadyHasIt = true;
									combinedTextureMap.Add (j, i);
									doubleTextures.Add (j);
								}
							}

							if (!alreadyHasIt) 
							{
								combinedTextureMap.Add (ownTextures.Length + texturesToBeAdded, i);
								newTextures.Add (rightTextures [i]);
								texturesToBeAdded++;
							}
						}
						combinedTextures = new SplatPrototype[ownTextures.Length + newTextures.Count];
						combinedAlphaMaps = new float[ownAlphaMapWidth, ownAlphaMapHeight, ownTextures.Length + newTextures.Count];

						for (int i = 0; i < ownTextures.Length; i++) 
						{
							combinedTextures [i] = ownTextures [i];
							for (int x = 0; x < ownAlphaMapWidth; x++) 
								for (int y = 0; y < ownAlphaMapHeight; y++) 
									combinedAlphaMaps [x, y, i] = ownAlphaMap [x, y, i];
						}

						index = ownTextures.Length;
						foreach (SplatPrototype texture in newTextures) 
						{
							changeTextures.Add (index);
							combinedTextures [index] = texture;
							index++;
						}

						tiles [row] [col].terrainData.splatPrototypes = combinedTextures;
						//tiles [row] [col+1].terrainData.splatPrototypes = combinedTextures;
						tiles [row] [col].terrainData.SetAlphamaps (0, 0, combinedAlphaMaps);
						tiles [row] [col].terrainData.RefreshPrototypes ();
					}
					List<int> rightChangeTextures = new List<int> ();
					List<int> rightDoubleTextures = new List<int> ();
					List<SplatPrototype> rightNewTextures = new List<SplatPrototype> ();
					Dictionary<int,int> rightCombinedTextureMap = new Dictionary<int,int> ();
					Dictionary<int,float[,,]> ownTextureMap = new Dictionary<int,float[,,]> ();
					SplatPrototype[] rightCombinedTextures = new SplatPrototype[1];
					float[,,] rightCombinedAlphaMaps = new float[1,1,1];
					if(smoothRight)
					{
						//begin right texture

						texturesToBeAdded = 0;
						
						for (int i = 0; i < ownTextures.Length; i++) 
						{
							bool alreadyHasIt = false;
							for (int j = 0; j < rightTextures.Length; j++) 
							{
								if (rightTextures [j].texture == ownTextures [i].texture) 
								{
									alreadyHasIt = true;
									rightCombinedTextureMap.Add (j, i);
									rightDoubleTextures.Add (j);
								}
							}
							
							if (!alreadyHasIt) 
							{
								rightCombinedTextureMap.Add (rightTextures.Length + texturesToBeAdded, i);
								rightNewTextures.Add (ownTextures [i]);
								texturesToBeAdded++;
							}
						}

						rightCombinedTextures = new SplatPrototype[rightTextures.Length + rightNewTextures.Count];
						rightCombinedAlphaMaps = new float[rightAlphaMapWidth, rightAlphaMapHeight, rightTextures.Length + rightNewTextures.Count];
						
						for (int i = 0; i < rightTextures.Length; i++) 
						{
							rightCombinedTextures [i] = rightTextures [i];
							for (int x = 0; x < rightAlphaMapWidth; x++) 
								for (int y = 0; y < rightAlphaMapHeight; y++) 
									rightCombinedAlphaMaps [x, y, i] = rightAlphaMap [x, y, i];
						}
						/*for (int i = 0; i < ownTextures.Length; i++) 
						{
							for (int x = 0; x < 2; x++)
							{
								for (int y = 0; y < rightAlphaMapHeight; y++)
								{
									rightCombinedAlphaMaps[x,y,i] = ownAlphaMap[ownAlphaMapWidth-x,y,i];
								}
							}
						}*/
						index = rightTextures.Length;
						foreach (SplatPrototype texture in rightNewTextures) 
						{
							rightChangeTextures.Add (index);
							rightCombinedTextures [index] = texture;
							index++;
						}
						
						tiles [row] [col+1].terrainData.splatPrototypes = rightCombinedTextures;
						//tiles [row] [col+1].terrainData.splatPrototypes = combinedTextures;
						tiles [row] [col+1].terrainData.SetAlphamaps (0, 0, rightCombinedAlphaMaps);
						tiles [row] [col+1].terrainData.RefreshPrototypes ();
					}
					//end right texture
					int xCnt = 1;
					float[] heightDifference = new float[tiles [row] [col].terrainData.heightmapHeight];
					for (int y = 0; y < tiles[row][col].terrainData.heightmapHeight; y++) 
					{
						heightDifference [y] = ownHeightMap [tiles [row] [col].terrainData.heightmapWidth - 1, y] - rightHeightMap [0, y];
					}
					for (int x = tiles[row][col].terrainData.heightmapWidth - smoothDistance; x < tiles[row][col].terrainData.heightmapWidth; x++) 
					{
						for (int y = 0; y < tiles[row][col].terrainData.heightmapHeight; y++) 
						{
							ownHeightMap [x, y] = ownHeightMap [x, y] - (heightDifference [y] / 2.0f) * (float)(((float)xCnt / (float)smoothDistance));
							rightHeightMap [smoothDistance - xCnt, y] = rightHeightMap [smoothDistance - xCnt, y] + (heightDifference [y] / 2.0f) * (float)((((float)xCnt) / (float)smoothDistance));
							if (x < rightAlphaMapWidth && y < rightAlphaMapHeight) 
							{
								if(smoothLeft)
								{
									for (int i = 0; i < combinedAlphaMaps.GetLength (2); i++) 
									{
										if (changeTextures.Contains (i)) 
										{
											if(!smoothRight)
												combinedAlphaMaps [x, y, i] = (rightAlphaMap [smoothDistance - xCnt-1, y, combinedTextureMap [i]] * ((float)xCnt / (float)smoothDistance));// * (xCnt/30.0f);
											else
											combinedAlphaMaps [x, y, i] = (rightAlphaMap [smoothDistance - xCnt-1, y, combinedTextureMap [i]] * (((float)xCnt/2.0f) / (float)smoothDistance));// * (xCnt/30.0f);
										} 
										else if (doubleTextures.Contains (i)) 
										{
											if(!smoothRight)
												combinedAlphaMaps [x, y, i] = ((rightAlphaMap [smoothDistance - xCnt-1, y, combinedTextureMap [i]] * ((float)xCnt / (float)smoothDistance)) + (ownAlphaMap [x, y, i] * (((float)smoothDistance - (float)xCnt) / (float)smoothDistance)));
											else
												combinedAlphaMaps [x, y, i] = ((rightAlphaMap [smoothDistance - xCnt-1, y, combinedTextureMap [i]] * (((float)xCnt/2.0f) / (float)smoothDistance)) + (ownAlphaMap [x, y, i] * (((float)smoothDistance - (float)xCnt/2.0f) / (float)smoothDistance)));

										} 
										else
										{
											if(!smoothRight)
												combinedAlphaMaps [x, y, i] = (ownAlphaMap [x, y, i] * ((((float)smoothDistance - (float)xCnt) / (float)smoothDistance)));
											else
												combinedAlphaMaps [x, y, i] = (ownAlphaMap [x, y, i] * ((((float)smoothDistance - (float)xCnt/2.0f) / (float)smoothDistance)));
										}
									}
								}
								if(smoothRight)
								{
									for ( int i = 0; i < rightCombinedAlphaMaps.GetLength (2); i++)
									{
										if(rightChangeTextures.Contains (i))
										{
											if(!smoothLeft)
												rightCombinedAlphaMaps [smoothDistance-xCnt-1, y, i] = (ownAlphaMap[x,y,rightCombinedTextureMap[i]] * (((float)xCnt) / (float)smoothDistance));
											else
												rightCombinedAlphaMaps [smoothDistance-xCnt-1, y, i] = (ownAlphaMap[x,y,rightCombinedTextureMap[i]] * (((float)xCnt/2.0f) / (float)smoothDistance));
										}
										else if (rightDoubleTextures.Contains (i))
										{
											if(!smoothLeft)
												rightCombinedAlphaMaps [smoothDistance-xCnt-1, y, i] = ((ownAlphaMap[x,y,rightCombinedTextureMap[i]] * (((float)xCnt) / (float)smoothDistance)) + (rightAlphaMap [smoothDistance-xCnt-1, y, i] * (((float)smoothDistance-(float)xCnt) / (float)smoothDistance)));
											else
												rightCombinedAlphaMaps [smoothDistance-xCnt-1, y, i] = ((ownAlphaMap[x,y,rightCombinedTextureMap[i]] * (((float)xCnt/2.0f) / (float)smoothDistance)) + (rightAlphaMap [smoothDistance-xCnt-1, y, i] * (((float)smoothDistance-(float)xCnt/2.0f) / (float)smoothDistance)));

										}
										else
										{
											if(!smoothLeft)
												rightCombinedAlphaMaps [smoothDistance-xCnt-1, y, i] = (rightAlphaMap[smoothDistance-xCnt-1, y, i] * (((float)smoothDistance - (float)xCnt) / (float)smoothDistance));
											else
												rightCombinedAlphaMaps [smoothDistance-xCnt-1, y, i] = (rightAlphaMap[smoothDistance-xCnt-1, y, i] * (((float)smoothDistance - (float)xCnt/2.0f) / (float)smoothDistance));

										}
									}
								}
								//Debug.Log ("x: "+x);
							}
						}
						//Debug.Log ("x: "+xCnt);
						xCnt++;
					}

					if(smoothLeft)
						tiles [row] [col].terrainData.SetAlphamaps (0, 0, combinedAlphaMaps);
					if(smoothRight)
						tiles [row] [col + 1].terrainData.SetAlphamaps (0, 0, rightCombinedAlphaMaps);
					tiles [row] [col + 1].terrainData.SetHeights (0, 0, rightHeightMap);
					//tiles[row][col+1].terrainData.SetDetailLayer (0,0,0,rightDetailMap);
					tiles [row] [col].terrainData.SetHeights (0, 0, ownHeightMap);

				}

				//TOP TO BOTTOM
				if (row != rows - 1) 
				{
					ownTextures = tiles [row] [col].terrainData.splatPrototypes;
					ownHeightMap = tiles [row] [col].terrainData.GetHeights (0, 0, ownHeightMapWidth, ownHeightMapHeight);
					ownAlphaMap = tiles [row] [col].terrainData.GetAlphamaps (0, 0, ownAlphaMapWidth, ownAlphaMapHeight);
					int bottomHeightMapWidth = tiles [row+1] [col].terrainData.heightmapWidth;
					int bottomHeightMapHeight = tiles [row+1] [col].terrainData.heightmapHeight;
					bottomHeightMap = tiles [row+1] [col].terrainData.GetHeights (0, 0, bottomHeightMapWidth, bottomHeightMapHeight);
					
					int bottomAlphaMapWidth = tiles [row+1] [col].terrainData.alphamapWidth;
					int bottomAlphaMapHeight = tiles [row+1] [col].terrainData.alphamapHeight;
					bottomAlphaMap = tiles [row+1] [col].terrainData.GetAlphamaps (0, 0, bottomAlphaMapWidth, bottomAlphaMapHeight);
					
					SplatPrototype[] bottomTextures = tiles [row+1] [col].terrainData.splatPrototypes;
					
					List<int> changeTextures = new List<int> ();
					List<int> doubleTextures = new List<int> ();
					List<SplatPrototype> newTextures = new List<SplatPrototype> ();
					Dictionary<int,int> combinedTextureMap = new Dictionary<int,int> ();
					Dictionary<int,float[,,]> bottomTextureMap = new Dictionary<int,float[,,]> ();
					int texturesToBeAdded = 0;
					SplatPrototype[] combinedTextures = new SplatPrototype[1];
					float[,,] combinedAlphaMaps = new float[1,1,1];
					int index;

					if(smoothTop)
					{
						for (int i = 0; i < bottomTextures.Length; i++) 
						{
							bool alreadyHasIt = false;
							for (int j = 0; j < ownTextures.Length; j++) 
							{
								if (ownTextures [j].texture == bottomTextures [i].texture) 
								{
									alreadyHasIt = true;
									combinedTextureMap.Add (j, i);
									doubleTextures.Add (j);
								}
							}
							
							if (!alreadyHasIt) 
							{
								combinedTextureMap.Add (ownTextures.Length + texturesToBeAdded, i);
								newTextures.Add (bottomTextures [i]);
								texturesToBeAdded++;
							}
						}
						combinedTextures = new SplatPrototype[ownTextures.Length + newTextures.Count];
						combinedAlphaMaps = new float[ownAlphaMapWidth, ownAlphaMapHeight, ownTextures.Length + newTextures.Count];
						
						for (int i = 0; i < ownTextures.Length; i++) 
						{
							combinedTextures [i] = ownTextures [i];
							for (int x = 0; x < ownAlphaMapWidth; x++) 
								for (int y = 0; y < ownAlphaMapHeight; y++) 
									combinedAlphaMaps [x, y, i] = ownAlphaMap [x, y, i];
						}
						
						index = ownTextures.Length;
						foreach (SplatPrototype texture in newTextures) 
						{
							changeTextures.Add (index);
							combinedTextures [index] = texture;
							index++;
						}
						float cum2 = 0.0f;
						for (int i = 0; i < combinedAlphaMaps.GetLength(2); i++) {
							cum2 += combinedAlphaMaps [tiles [row] [col].terrainData.heightmapWidth - 20, 10, i];
						}
						Debug.Log ("cum2: " + cum2);
						tiles [row] [col].terrainData.splatPrototypes = combinedTextures;
						//tiles [row] [col+1].terrainData.splatPrototypes = combinedTextures;
						tiles [row] [col].terrainData.SetAlphamaps (0, 0, combinedAlphaMaps);
						tiles [row] [col].terrainData.RefreshPrototypes ();
					}
					//bottom

					List<int> bottomChangeTextures = new List<int> ();
					List<int> bottomDoubleTextures = new List<int> ();
					List<SplatPrototype> bottomNewTextures = new List<SplatPrototype> ();
					Dictionary<int,int> bottomCombinedTextureMap = new Dictionary<int,int> ();
					Dictionary<int,float[,,]> ownTextureMap = new Dictionary<int,float[,,]> ();
					texturesToBeAdded = 0;
					SplatPrototype[] bottomCombinedTextures = new SplatPrototype[1];
					float[,,] bottomCombinedAlphaMaps = new float[1,1,1];
					if(smoothBottom)
					{
						for (int i = 0; i < ownTextures.Length; i++) 
						{
							bool alreadyHasIt = false;
							for (int j = 0; j < bottomTextures.Length; j++) 
							{
								if (bottomTextures [j].texture == ownTextures [i].texture) 
								{
									alreadyHasIt = true;
									bottomCombinedTextureMap.Add (j, i);
									bottomDoubleTextures.Add (j);
								}
							}
							
							if (!alreadyHasIt) 
							{
								bottomCombinedTextureMap.Add (bottomTextures.Length + texturesToBeAdded, i);
								bottomNewTextures.Add (ownTextures [i]);
								texturesToBeAdded++;
							}
						}
						
						bottomCombinedTextures = new SplatPrototype[bottomTextures.Length + bottomNewTextures.Count];
						bottomCombinedAlphaMaps = new float[bottomAlphaMapWidth, bottomAlphaMapHeight, bottomTextures.Length + bottomNewTextures.Count];
						
						for (int i = 0; i < bottomTextures.Length; i++) 
						{
							bottomCombinedTextures [i] = bottomTextures [i];
							for (int x = 0; x < bottomAlphaMapWidth; x++) 
								for (int y = 0; y < bottomAlphaMapHeight; y++) 
									bottomCombinedAlphaMaps [x, y, i] = bottomAlphaMap [x, y, i];
						}
						
						index = bottomTextures.Length;
						foreach (SplatPrototype texture in bottomNewTextures) 
						{
							bottomChangeTextures.Add (index);
							bottomCombinedTextures [index] = texture;
							index++;
						}
						
						tiles [row+1] [col].terrainData.splatPrototypes = bottomCombinedTextures;
						//tiles [row] [col+1].terrainData.splatPrototypes = combinedTextures;
						tiles [row+1] [col].terrainData.SetAlphamaps (0, 0, bottomCombinedAlphaMaps);
						tiles [row+1] [col].terrainData.RefreshPrototypes ();
					}
					//end bottom


					//bottomHeightMap = tiles [row + 1] [col].terrainData.GetHeights (0, 0, 513, 513);
					int yCnt = 1;
					float[] difference = new float[tiles [row] [col].terrainData.heightmapHeight];
					for (int x = 0; x < tiles[row][col].terrainData.heightmapWidth; x++) {
							difference [x] = ownHeightMap [x, tiles [row] [col].terrainData.heightmapHeight - 1] - bottomHeightMap [x, 0];
					}
					for (int y = tiles[row][col].terrainData.heightmapHeight-smoothDistance; y < tiles[row][col].terrainData.heightmapHeight; y++) 
					{
						for (int x = 0; x < tiles[row][col].terrainData.heightmapWidth; x++) 
						{
							ownHeightMap [x, y] = ownHeightMap [x, y] - (difference [x] / 2) * (float)((yCnt / (float)smoothDistance));///((20-xCnt)/20);//rightHeightMap[20-xCnt,y];
							bottomHeightMap [x, smoothDistance - yCnt] = bottomHeightMap [x, smoothDistance - yCnt] + (difference [x] / 2) * (float)(((yCnt) / (float)smoothDistance));
							if (x < bottomAlphaMapWidth && y < bottomAlphaMapHeight) 
							{
								if(smoothTop)
								{
									for (int i = 0; i < combinedAlphaMaps.GetLength (2); i++) 
									{
										if (changeTextures.Contains (i)) 
										{
											if(!smoothBottom)
												combinedAlphaMaps [x, y, i] = (bottomAlphaMap [x,smoothDistance - yCnt-1, combinedTextureMap [i]] * (((float)yCnt) / (float)smoothDistance));// * (xCnt/30.0f);
											else
												combinedAlphaMaps [x, y, i] = (bottomAlphaMap [x,smoothDistance - yCnt-1, combinedTextureMap [i]] * (((float)yCnt/2.0f) / (float)smoothDistance));// * (xCnt/30.0f);
										} 
										else if (doubleTextures.Contains (i)) 
										{
											if(!smoothBottom)
												combinedAlphaMaps [x, y, i] = ((bottomAlphaMap [x, smoothDistance - yCnt-1, combinedTextureMap [i]] * (((float)yCnt) / (float)smoothDistance)) + (ownAlphaMap [x, y, i] * (((float)smoothDistance - (float)yCnt) / (float)smoothDistance)));
											else
												combinedAlphaMaps [x, y, i] = ((bottomAlphaMap [x, smoothDistance - yCnt-1, combinedTextureMap [i]] * (((float)yCnt/2.0f) / (float)smoothDistance)) + (ownAlphaMap [x, y, i] * (((float)smoothDistance - (float)yCnt/2.0f) / (float)smoothDistance)));
										} 
										else
										{
											if(!smoothBottom)
												combinedAlphaMaps [x, y, i] = (ownAlphaMap [x, y, i] * ((((float)smoothDistance - (float)yCnt) / (float)smoothDistance)));
											else
												combinedAlphaMaps [x, y, i] = (ownAlphaMap [x, y, i] * ((((float)smoothDistance - (float)yCnt/2.0f) / (float)smoothDistance)));
										}
									}
								}
								if(smoothBottom)
								{
									for ( int i = 0; i < bottomCombinedAlphaMaps.GetLength (2); i++)
									{
										if(bottomChangeTextures.Contains (i))
										{
											if(!smoothTop)
												bottomCombinedAlphaMaps [x,smoothDistance-yCnt-1, i] = (ownAlphaMap[x,y,bottomCombinedTextureMap[i]] * (((float)yCnt) / (float)smoothDistance));
											else
												bottomCombinedAlphaMaps [x,smoothDistance-yCnt-1, i] = (ownAlphaMap[x,y,bottomCombinedTextureMap[i]] * (((float)yCnt/2.0f) / (float)smoothDistance));
										}
										else if (bottomDoubleTextures.Contains (i))
										{
											if(!smoothTop)
												bottomCombinedAlphaMaps [x,smoothDistance-yCnt-1, i] = ((ownAlphaMap[x,y,bottomCombinedTextureMap[i]] * (((float)yCnt) / (float)smoothDistance)) + (bottomAlphaMap [x,smoothDistance-yCnt-1, i] * (((float)smoothDistance-(float)yCnt) / (float)smoothDistance)));
											else
												bottomCombinedAlphaMaps [x,smoothDistance-yCnt-1, i] = ((ownAlphaMap[x,y,bottomCombinedTextureMap[i]] * (((float)yCnt/2.0f) / (float)smoothDistance)) + (bottomAlphaMap [x,smoothDistance-yCnt-1, i] * (((float)smoothDistance-(float)yCnt/2.0f) / (float)smoothDistance)));
										}
										else
										{
											if(!smoothTop)
												bottomCombinedAlphaMaps [x,smoothDistance-yCnt-1, i] = (bottomAlphaMap[x, smoothDistance-yCnt-1, i] * (((float)smoothDistance - (float)yCnt) / (float)smoothDistance));
											else
												bottomCombinedAlphaMaps [x,smoothDistance-yCnt-1, i] = (bottomAlphaMap[x, smoothDistance-yCnt-1, i] * (((float)smoothDistance - (float)yCnt/2.0f) / (float)smoothDistance));
										}
									}
								}
							}
						}
						yCnt++;
					}
					if(smoothBottom)
						tiles [row+1] [col].terrainData.SetAlphamaps (0, 0, bottomCombinedAlphaMaps);
					if(smoothTop)
						tiles [row] [col].terrainData.SetAlphamaps (0, 0, combinedAlphaMaps);

					tiles [row + 1] [col].terrainData.SetHeights (0, 0, bottomHeightMap);
					tiles [row] [col].terrainData.SetHeights (0, 0, ownHeightMap);
				}
			}
		}
	}

	void CreateNewTerrainData (Terrain terrain)
	{
		TerrainData prefabTerrainData = (TerrainData)Object.Instantiate (terrain.terrainData);
		TerrainData newTerrainData = new TerrainData ();

		newTerrainData.heightmapResolution = prefabTerrainData.heightmapResolution;
		newTerrainData.size = prefabTerrainData.size;
		newTerrainData.baseMapResolution = prefabTerrainData.baseMapResolution;
		newTerrainData.SetDetailResolution (prefabTerrainData.detailResolution, 8);
		newTerrainData.SetHeights (0, 0, prefabTerrainData.GetHeights (0, 0, 513, 513));
		newTerrainData.splatPrototypes = prefabTerrainData.splatPrototypes;
		newTerrainData.SetAlphamaps (0, 0, prefabTerrainData.GetAlphamaps (0, 0, prefabTerrainData.alphamapWidth, prefabTerrainData.alphamapHeight));
		newTerrainData.detailPrototypes = prefabTerrainData.detailPrototypes;
		newTerrainData.treeInstances = prefabTerrainData.treeInstances;
		newTerrainData.treePrototypes = prefabTerrainData.treePrototypes;

		for (int i=0; i < prefabTerrainData.detailPrototypes.Length; i++) {
				newTerrainData.SetDetailLayer (0, 0, i, prefabTerrainData.GetDetailLayer (0, 0, prefabTerrainData.detailWidth, prefabTerrainData.detailHeight, i));
		}
		terrain.terrainData = newTerrainData;
	}

	void SetNeighbors (Terrain[][] tiles)
	{
		for (int row = 0; row < rows; row++) 
		{
			for (int col = 0; col < cols; col++) 
			{
				Terrain leftNeighbor = null;
				Terrain rightNeighbor = null;
				Terrain topNeighbor = null;
				Terrain bottomNeighbor = null;
				if (col != cols - 1)
						rightNeighbor = tiles [row] [col + 1];
				if (col != 0)
						leftNeighbor = tiles [row] [col - 1];
				if (row != rows - 1)
						bottomNeighbor = tiles [row + 1] [col];
				if (row != 0)
						topNeighbor = tiles [row - 1] [col];

				tiles [row] [col].SetNeighbors (leftNeighbor, topNeighbor, rightNeighbor, bottomNeighbor);
			}
		}
	}
}