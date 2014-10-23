using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GenerateTerrain : MonoBehaviour
{
	private Terrain[][] tiles;
	private object[] blockPrefabs;
	public bool noGenerationPlease = false;
	public bool pathFinderScan = true;
	public bool block1inMiddle = true;

	public int rows = 3;
	public int cols = 3;

	public bool smoothBorders = true;
	public bool twoIterations = true;
	public int smoothDistance = 30;
	public bool smoothLeft = true;
	public bool smoothRight = true;
	public bool smoothTop = true;
	public bool smoothBottom = true;

	public bool setNeighbors = false;

	private GameObject gameObjectParent;


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
			if(pathFinderScan)
				AstarPath.active.Scan ();
		}
		return;
	}

	private void GenerateTerrainBlocks ()
	{
		if(rows % 2 != 1) rows++;
		if(cols % 2 != 1) cols++;
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
				if (block1inMiddle && col == 0 && row == 0) 
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
		{
			SmoothBorders (tiles);
			if(twoIterations)
				SmoothBorders (tiles);
		}

		UpdateColliders (tiles);
		if(setNeighbors)
			SetNeighbors (tiles);
		for (int row = 0; row < rows; row++)
		{
			for (int col = 0; col < cols; col++)
			{
				//tiles[row][col].terrainData.RefreshPrototypes();
				tiles[row][col].Flush();
			}
		}
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
				//declarations
				int ownHeightMapWidth = tiles [row] [col].terrainData.heightmapWidth;
				int ownHeightMapHeight = tiles [row] [col].terrainData.heightmapHeight;
				float[,] ownHeightMap;

				int ownAlphaMapWidth = tiles [row] [col].terrainData.alphamapWidth;
				int ownAlphaMapHeight = tiles [row] [col].terrainData.alphamapHeight;
				float[,,] ownAlphaMap;

				float[,,] rightAlphaMap;
				float[,] rightHeightMap;
				float[,] bottomHeightMap; 
				float[,,] bottomAlphaMap;

				//LEFT TO RIGHT
				if (col != cols - 1) 
				{
					#region texture merging
					//get missing textures for left terrain smoothing
					List<int> changeTextures = new List<int> ();
					List<int> doubleTextures = new List<int> ();
					Dictionary<int,int> combinedTextureMap = new Dictionary<int,int> ();
					float[,,] combinedAlphaMaps = new float[1,1,1];
					if (smoothLeft)
					{
						AddMissingTextures(tiles[row][col],tiles[row][col+1],out changeTextures, out doubleTextures, out combinedTextureMap, out combinedAlphaMaps);
					}
					//end get missing textures

					ownHeightMap = tiles [row] [col].terrainData.GetHeights (0, 0, ownHeightMapWidth, ownHeightMapHeight);


					int rightHeightMapWidth = tiles [row] [col + 1].terrainData.heightmapWidth;
					int rightHeightMapHeight = tiles [row] [col + 1].terrainData.heightmapHeight;
					rightHeightMap = tiles [row] [col + 1].terrainData.GetHeights (0, 0, rightHeightMapWidth, rightHeightMapHeight);

					int rightAlphaMapWidth = tiles [row] [col + 1].terrainData.alphamapWidth;
					int rightAlphaMapHeight = tiles [row] [col + 1].terrainData.alphamapHeight;

					//get missing textures for right terrain smoothing
					List<int> rightChangeTextures = new List<int> ();
					List<int> rightDoubleTextures = new List<int> ();
					Dictionary<int,int> rightCombinedTextureMap = new Dictionary<int,int> ();
					float[,,] rightCombinedAlphaMaps = new float[1,1,1];
					if (smoothRight)
					{
						AddMissingTextures(tiles[row][col+1],tiles[row][col],out rightChangeTextures, out rightDoubleTextures, out rightCombinedTextureMap, out rightCombinedAlphaMaps);
					}
					//end get missing textures
					#endregion
					ownAlphaMap = tiles [row] [col].terrainData.GetAlphamaps (0, 0, ownAlphaMapWidth, ownAlphaMapHeight);
					rightAlphaMap = tiles [row] [col + 1].terrainData.GetAlphamaps (0, 0, rightAlphaMapWidth, rightAlphaMapHeight);

					SmoothTerrainHeightMapsX(tiles[row][col], tiles[row][col+1]);

					#region alpha map smoothing
					if (smoothLeft || smoothRight)
					{
						int xCnt = 1;
						for (int x = tiles[row][col].terrainData.alphamapWidth - smoothDistance; x < tiles[row][col].terrainData.alphamapWidth; x++) 
						{
							for (int y = 0; y < tiles[row][col].terrainData.alphamapHeight; y++) 
							{
								if(smoothLeft)
								{
									for (int i = 0; i < combinedAlphaMaps.GetLength (2); i++) 
									{
										if (changeTextures.Contains (i)) 
										{
											if(!smoothRight)
												combinedAlphaMaps [x, y, i] = (rightAlphaMap [smoothDistance - xCnt, y, combinedTextureMap [i]] * ((float)xCnt / (float)smoothDistance));// * (xCnt/30.0f);
											else
											combinedAlphaMaps [x, y, i] = (rightAlphaMap [smoothDistance - xCnt, y, combinedTextureMap [i]] * (((float)xCnt/2.0f) / (float)smoothDistance));// * (xCnt/30.0f);
										} 
										else if (doubleTextures.Contains (i)) 
										{
											if(!smoothRight)
												combinedAlphaMaps [x, y, i] = ((rightAlphaMap [smoothDistance - xCnt, y, combinedTextureMap [i]] * ((float)xCnt / (float)smoothDistance)) + (ownAlphaMap [x, y, i] * (((float)smoothDistance - (float)xCnt) / (float)smoothDistance)));
											else
												combinedAlphaMaps [x, y, i] = ((rightAlphaMap [smoothDistance - xCnt, y, combinedTextureMap [i]] * (((float)xCnt/2.0f) / (float)smoothDistance)) + (ownAlphaMap [x, y, i] * (((float)smoothDistance - (float)xCnt/2.0f) / (float)smoothDistance)));

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
												rightCombinedAlphaMaps [smoothDistance-xCnt, y, i] = (ownAlphaMap[x,y,rightCombinedTextureMap[i]] * (((float)xCnt) / (float)smoothDistance));
											else
												rightCombinedAlphaMaps [smoothDistance-xCnt, y, i] = (ownAlphaMap[x,y,rightCombinedTextureMap[i]] * (((float)xCnt/2.0f) / (float)smoothDistance));
										}
										else if (rightDoubleTextures.Contains (i))
										{
											if(!smoothLeft)
												rightCombinedAlphaMaps [smoothDistance-xCnt, y, i] = ((ownAlphaMap[x,y,rightCombinedTextureMap[i]] * (((float)xCnt) / (float)smoothDistance)) + (rightAlphaMap [smoothDistance-xCnt, y, i] * (((float)smoothDistance-(float)xCnt) / (float)smoothDistance)));
											else
												rightCombinedAlphaMaps [smoothDistance-xCnt, y, i] = ((ownAlphaMap[x,y,rightCombinedTextureMap[i]] * (((float)xCnt/2.0f) / (float)smoothDistance)) + (rightAlphaMap [smoothDistance-xCnt, y, i] * (((float)smoothDistance-(float)xCnt/2.0f) / (float)smoothDistance)));

										}
										else
										{
											if(!smoothLeft)
												rightCombinedAlphaMaps [smoothDistance-xCnt, y, i] = (rightAlphaMap[smoothDistance-xCnt, y, i] * (((float)smoothDistance - (float)xCnt) / (float)smoothDistance));
											else
												rightCombinedAlphaMaps [smoothDistance-xCnt, y, i] = (rightAlphaMap[smoothDistance-xCnt, y, i] * (((float)smoothDistance - (float)xCnt/2.0f) / (float)smoothDistance));

										}
									}
								}
							}
							xCnt++;
						}

						if(smoothLeft)
						{
							tiles [row] [col].terrainData.SetAlphamaps (0, 0, combinedAlphaMaps);
							tiles [row] [col].terrainData.RefreshPrototypes();
							//tiles [row] [col].Flush ();
						}
						if(smoothRight)
						{
							tiles [row] [col + 1].terrainData.SetAlphamaps (0, 0, rightCombinedAlphaMaps);
							tiles [row] [col + 1].terrainData.RefreshPrototypes();
							//tiles [row] [col + 1].Flush ();
						}
					}
					#endregion

				}

				//TOP TO BOTTOM
				if (row != rows - 1) 
				{
					#region texture merging
					List<int> changeTextures = new List<int> ();
					List<int> doubleTextures = new List<int> ();
					Dictionary<int,int> combinedTextureMap = new Dictionary<int,int> ();
					float[,,] combinedAlphaMaps = new float[1,1,1];
					int index;

					if (smoothTop)
					{
						AddMissingTextures(tiles[row][col],tiles[row+1][col],out changeTextures, out doubleTextures, out combinedTextureMap, out combinedAlphaMaps);
					}

					ownHeightMap = tiles [row] [col].terrainData.GetHeights (0, 0, ownHeightMapWidth, ownHeightMapHeight);
					ownAlphaMap = tiles [row] [col].terrainData.GetAlphamaps (0, 0, ownAlphaMapWidth, ownAlphaMapHeight);
					int bottomHeightMapWidth = tiles [row+1] [col].terrainData.heightmapWidth;
					int bottomHeightMapHeight = tiles [row+1] [col].terrainData.heightmapHeight;
					bottomHeightMap = tiles [row+1] [col].terrainData.GetHeights (0, 0, bottomHeightMapWidth, bottomHeightMapHeight);
					
					int bottomAlphaMapWidth = tiles [row+1] [col].terrainData.alphamapWidth;
					int bottomAlphaMapHeight = tiles [row+1] [col].terrainData.alphamapHeight;
					bottomAlphaMap = tiles [row+1] [col].terrainData.GetAlphamaps (0, 0, bottomAlphaMapWidth, bottomAlphaMapHeight);


					List<int> bottomChangeTextures = new List<int> ();
					List<int> bottomDoubleTextures = new List<int> ();
					Dictionary<int,int> bottomCombinedTextureMap = new Dictionary<int,int> ();
					float[,,] bottomCombinedAlphaMaps = new float[1,1,1];

					if (smoothBottom)
					{
						AddMissingTextures(tiles[row+1][col],tiles[row][col],out bottomChangeTextures, out bottomDoubleTextures, out bottomCombinedTextureMap, out bottomCombinedAlphaMaps);
					}
					#endregion

					SmoothTerrainHeightMapsY(tiles[row][col],tiles[row+1][col]);

					#region alpha map smoothing
					if(smoothTop || smoothBottom)
					{
						int yCnt = 1;
						for (int y = ownAlphaMapHeight-smoothDistance; y < ownAlphaMapHeight; y++) 
						{
							for (int x = 0; x < ownAlphaMapWidth; x++) 
							{
								if(smoothTop)
								{
									for (int i = 0; i < combinedAlphaMaps.GetLength (2); i++) 
									{
										if (changeTextures.Contains (i)) 
										{
											if(!smoothBottom)
												combinedAlphaMaps [x, y, i] = (bottomAlphaMap [x,smoothDistance - yCnt, combinedTextureMap [i]] * (((float)yCnt) / (float)smoothDistance));// * (xCnt/30.0f);
											else
												combinedAlphaMaps [x, y, i] = (bottomAlphaMap [x,smoothDistance - yCnt, combinedTextureMap [i]] * (((float)yCnt/2.0f) / (float)smoothDistance));// * (xCnt/30.0f);
										} 
										else if (doubleTextures.Contains (i)) 
										{
											if(!smoothBottom)
												combinedAlphaMaps [x, y, i] = ((bottomAlphaMap [x, smoothDistance - yCnt, combinedTextureMap [i]] * (((float)yCnt) / (float)smoothDistance)) + (ownAlphaMap [x, y, i] * (((float)smoothDistance - (float)yCnt) / (float)smoothDistance)));
											else
												combinedAlphaMaps [x, y, i] = ((bottomAlphaMap [x, smoothDistance - yCnt, combinedTextureMap [i]] * (((float)yCnt/2.0f) / (float)smoothDistance)) + (ownAlphaMap [x, y, i] * (((float)smoothDistance - (float)yCnt/2.0f) / (float)smoothDistance)));
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
												bottomCombinedAlphaMaps [x,smoothDistance-yCnt, i] = (ownAlphaMap[x,y,bottomCombinedTextureMap[i]] * (((float)yCnt) / (float)smoothDistance));
											else
												bottomCombinedAlphaMaps [x,smoothDistance-yCnt, i] = (ownAlphaMap[x,y,bottomCombinedTextureMap[i]] * (((float)yCnt/2.0f) / (float)smoothDistance));
										}
										else if (bottomDoubleTextures.Contains (i))
										{
											if(!smoothTop)
												bottomCombinedAlphaMaps [x,smoothDistance-yCnt, i] = ((ownAlphaMap[x,y,bottomCombinedTextureMap[i]] * (((float)yCnt) / (float)smoothDistance)) + (bottomAlphaMap [x,smoothDistance-yCnt, i] * (((float)smoothDistance-(float)yCnt) / (float)smoothDistance)));
											else
												bottomCombinedAlphaMaps [x,smoothDistance-yCnt, i] = ((ownAlphaMap[x,y,bottomCombinedTextureMap[i]] * (((float)yCnt/2.0f) / (float)smoothDistance)) + (bottomAlphaMap [x,smoothDistance-yCnt, i] * (((float)smoothDistance-(float)yCnt/2.0f) / (float)smoothDistance)));
										}
										else
										{
											if(!smoothTop)
												bottomCombinedAlphaMaps [x,smoothDistance-yCnt, i] = (bottomAlphaMap[x, smoothDistance-yCnt, i] * (((float)smoothDistance - (float)yCnt) / (float)smoothDistance));
											else
												bottomCombinedAlphaMaps [x,smoothDistance-yCnt, i] = (bottomAlphaMap[x, smoothDistance-yCnt, i] * (((float)smoothDistance - (float)yCnt/2.0f) / (float)smoothDistance));
										}
									}
								}
							}
							yCnt++;
						}
						if(smoothBottom)
						{
							tiles [row+1] [col].terrainData.SetAlphamaps (0, 0, bottomCombinedAlphaMaps);
							tiles [row+1] [col].terrainData.RefreshPrototypes();
							//tiles [row+1] [col].Flush ();
						}
						if(smoothTop)
						{
							tiles [row] [col].terrainData.SetAlphamaps (0, 0, combinedAlphaMaps);
							tiles [row] [col].terrainData.RefreshPrototypes();
							//tiles [row] [col].Flush ();
						}
					}
					#endregion
				}
			}
		}
	}
	/*void SmoothAlphaMapsX(Terrain terrain1,Terrain terrain2,float[,,] combinedAlphaMaps, List<int> changeTextures, List<int> doubleTextures, bool smoothOther)
	{
		//float[,,] rightAlphaMap 
		float[,,] rightAlphaMap = terrain2.terrainData.GetAlphamaps(0,0,terrain2.terrainData.alphamapWidth,terrain2.terrainData.alphamapHeight);
		float[,,] ownAlphaMap = terrain1.terrainData.GetAlphamaps(0,0,terrain1.terrainData.alphamapWidth,terrain1.terrainData.alphamapHeight);
		int xCnt = 1;
		for (int x = terrain1.terrainData.alphamapWidth - smoothDistance; x < terrain1.terrainData.alphamapWidth; x++) 
		{
			for (int y = 0; y < terrain1.terrainData.alphamapHeight; y++) 
			{
					for (int i = 0; i < combinedAlphaMaps.GetLength (2); i++) 
					{
						if (changeTextures.Contains (i)) 
						{
							if(!smoothRight)
								combinedAlphaMaps [x, y, i] = (rightAlphaMap [smoothDistance - xCnt, y, combinedTextureMap [i]] * ((float)xCnt / (float)smoothDistance));// * (xCnt/30.0f);
							else
								combinedAlphaMaps [x, y, i] = (rightAlphaMap [smoothDistance - xCnt, y, combinedTextureMap [i]] * (((float)xCnt/2.0f) / (float)smoothDistance));// * (xCnt/30.0f);
						} 
						else if (doubleTextures.Contains (i)) 
						{
							if(!smoothRight)
								combinedAlphaMaps [x, y, i] = ((rightAlphaMap [smoothDistance - xCnt, y, combinedTextureMap [i]] * ((float)xCnt / (float)smoothDistance)) + (ownAlphaMap [x, y, i] * (((float)smoothDistance - (float)xCnt) / (float)smoothDistance)));
							else
								combinedAlphaMaps [x, y, i] = ((rightAlphaMap [smoothDistance - xCnt, y, combinedTextureMap [i]] * (((float)xCnt/2.0f) / (float)smoothDistance)) + (ownAlphaMap [x, y, i] * (((float)smoothDistance - (float)xCnt/2.0f) / (float)smoothDistance)));
							
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
			xCnt++;
		}
		
		terrain1.terrainData.SetAlphamaps (0, 0, combinedAlphaMaps);

	}*/
	void SmoothTerrainHeightMapsX(Terrain terrain1, Terrain terrain2)
	{
		int terrain1HeightMapWidth = terrain1.terrainData.heightmapWidth;
		int terrain1HeightMapHeight = terrain1.terrainData.heightmapHeight;
		float[,] terrain1HeightMap = terrain1.terrainData.GetHeights (0, 0, terrain1HeightMapWidth, terrain1HeightMapHeight);

		int terrain2HeightMapWidth = terrain2.terrainData.heightmapWidth;
		int terrain2HeightMapHeight = terrain2.terrainData.heightmapHeight;
		float[,] terrain2HeightMap = terrain2.terrainData.GetHeights (0, 0, terrain2HeightMapWidth, terrain2HeightMapHeight);

		float[] heightDifference = new float[terrain1HeightMapWidth];
		for (int y = 0; y < terrain1HeightMapHeight; y++) 
		{
			heightDifference [y] = terrain1HeightMap [terrain1HeightMapWidth - 1, y] - terrain2HeightMap [0, y];
		}

		int xCnt = 1;
		for (int x = terrain1HeightMapWidth - smoothDistance; x < terrain1HeightMapWidth; x++) 
		{
			for (int y = 0; y < terrain1HeightMapHeight; y++) 
			{
				//if(xCnt == 1)
					//Debug.Log ("x: "+x+", xCnt:"+xCnt);
				terrain1HeightMap [x, y] = terrain1HeightMap [x, y] - (heightDifference [y] / 2.0f) * (float)(((float)xCnt / (float)smoothDistance));
				terrain2HeightMap [smoothDistance - xCnt, y] = terrain2HeightMap [smoothDistance - xCnt, y] + (heightDifference [y] / 2.0f) * (float)((((float)xCnt) / (float)smoothDistance));
			
			}
			xCnt++;
		}
		//Debug.Log (terrain2HeightMap[0,10]+" vs "+terrain1HeightMap[terrain1HeightMapWidth-1,10]);
		terrain1.terrainData.SetHeights (0, 0, terrain1HeightMap);
		terrain2.terrainData.SetHeights (0, 0, terrain2HeightMap);
	//	terrain1.Flush ();
	//	terrain2.Flush ();
	}

	void SmoothTerrainHeightMapsY(Terrain terrain1, Terrain terrain2)
	{
		int terrain1HeightMapWidth = terrain1.terrainData.heightmapWidth;
		int terrain1HeightMapHeight = terrain1.terrainData.heightmapHeight;
		float[,] terrain1HeightMap = terrain1.terrainData.GetHeights (0, 0, terrain1HeightMapWidth, terrain1HeightMapHeight);
		
		int terrain2HeightMapWidth = terrain2.terrainData.heightmapWidth;
		int terrain2HeightMapHeight = terrain2.terrainData.heightmapHeight;
		float[,] terrain2HeightMap = terrain2.terrainData.GetHeights (0, 0, terrain2HeightMapWidth, terrain2HeightMapHeight);


		float[] heightDifference = new float[terrain1HeightMapHeight];
		for (int x = 0; x < terrain1HeightMapWidth; x++) 
		{
			heightDifference [x] = terrain1HeightMap [x, terrain1HeightMapHeight - 1] - terrain2HeightMap [x, 0];
		}
		
		int yCnt = 1;
		for (int y = terrain1HeightMapHeight - smoothDistance; y < terrain1HeightMapHeight; y++) 
		{
			for (int x = 0; x < terrain1HeightMapWidth; x++) 
			{
				terrain1HeightMap [x, y] = terrain1HeightMap [x, y] - (heightDifference [x] / 2.0f) * (float)(((float)yCnt / (float)smoothDistance));
				terrain2HeightMap [x, smoothDistance - yCnt] = terrain2HeightMap [x, smoothDistance - yCnt] + (heightDifference [x] / 2.0f) * (float)((((float)yCnt) / (float)smoothDistance));
			}
			yCnt++;
		}
		terrain1.terrainData.SetHeights (0, 0, terrain1HeightMap);
		terrain2.terrainData.SetHeights (0, 0, terrain2HeightMap);
	//	terrain1.Flush ();
	//	terrain2.Flush ();
	}

	void AddMissingTextures(Terrain targetTerrain, Terrain sourceTerrain, out List<int> changeTextures, out List<int> doubleTextures, out Dictionary<int,int> combinedTextureMap, out float[,,] combinedAlphaMaps)
	{
		SplatPrototype[] targetTextures = targetTerrain.terrainData.splatPrototypes;
		SplatPrototype[] sourceTextures = sourceTerrain.terrainData.splatPrototypes;
		List<SplatPrototype> newTextures = new List<SplatPrototype> ();
		combinedTextureMap = new Dictionary<int,int>();
		doubleTextures = new List<int>();
		changeTextures = new List<int>();
		combinedAlphaMaps = new float[1,1,1];
		Dictionary<int,float[,,]> sourceTextureMap = new Dictionary<int,float[,,]> ();
		SplatPrototype[] combinedTextures = new SplatPrototype[1];
		int targetAlphaMapHeight = targetTerrain.terrainData.alphamapHeight;
		int targetAlphaMapWidth = targetTerrain.terrainData.alphamapWidth;
		float[,,] targetAlphaMap = targetTerrain.terrainData.GetAlphamaps (0, 0, targetAlphaMapWidth, targetAlphaMapHeight);

		//Dictionary<int,float> ownTextureMap = new Dictionary<int,float> ();
		int texturesToBeAdded = 0;
		int index;
		for (int i = 0; i < sourceTextures.Length; i++) 
		{
			bool alreadyHasIt = false;
			for (int j = 0; j < targetTextures.Length; j++) 
			{
				if (targetTextures [j].texture == sourceTextures [i].texture) 
				{
					alreadyHasIt = true;
					combinedTextureMap.Add (j, i);
					doubleTextures.Add (j);
				}
			}
			
			if (!alreadyHasIt) 
			{
				combinedTextureMap.Add (targetTextures.Length + texturesToBeAdded, i);
				newTextures.Add (sourceTextures [i]);
				texturesToBeAdded++;
			}
		}
		combinedTextures = new SplatPrototype[targetTextures.Length + newTextures.Count];
		combinedAlphaMaps = new float[targetAlphaMapWidth, targetAlphaMapHeight, targetTextures.Length + newTextures.Count];
		
		for (int i = 0; i < targetTextures.Length; i++) 
		{
			combinedTextures [i] = targetTextures [i];
			for (int x = 0; x < targetAlphaMapWidth; x++) 
				for (int y = 0; y < targetAlphaMapHeight; y++) 
					combinedAlphaMaps [x, y, i] = targetAlphaMap [x, y, i];
		}
		
		index = targetTextures.Length;
		foreach (SplatPrototype texture in newTextures) 
		{
			changeTextures.Add (index);
			combinedTextures [index] = texture;
			index++;
		}
		
		targetTerrain.terrainData.splatPrototypes = combinedTextures;
		//tiles [row] [col+1].terrainData.splatPrototypes = combinedTextures;
		targetTerrain.terrainData.SetAlphamaps (0, 0, combinedAlphaMaps);
		targetTerrain.terrainData.RefreshPrototypes ();
		//targetTerrain.Flush ();
		//sourceTerrain.Flush ();
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
		newTerrainData.treePrototypes = prefabTerrainData.treePrototypes;
		newTerrainData.treeInstances = prefabTerrainData.treeInstances;

		terrain.terrainData.RefreshPrototypes();
		for (int i=0; i < prefabTerrainData.detailPrototypes.Length; i++) {
				newTerrainData.SetDetailLayer (0, 0, i, prefabTerrainData.GetDetailLayer (0, 0, prefabTerrainData.detailWidth, prefabTerrainData.detailHeight, i));
		}
		terrain.terrainData = newTerrainData;
		//terrain.terrainData.RefreshPrototypes();
		//terrain.Flush ();
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