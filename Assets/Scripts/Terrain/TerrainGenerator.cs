using UnityEngine;
using MyGrid.Code;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
    [Header ("Terrain Info")]
    [SerializeField] int mapX = 10;
    [SerializeField] int mapZ = 10;
    [SerializeField] int maxElevation = 4;
    [SerializeField] float variance;
    [SerializeField] float elevationBias;
    [SerializeField] private TileSetting tileSetting;
    int [,] elevationMap;   //two-dimensional array for containing mapX and mapZ coordinates
                            //to assign elevation on them
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        elevationMap = new int[mapX, mapZ];
        for (int i = 0; i < mapX; i++)
        {
            for(int j = 0; j < mapZ; j++) // < makes it inclusive to mapZ
            {
                float noise = Mathf.PerlinNoise(i*variance, j*variance);    //higher number = higher variation
                noise = Mathf.Pow(noise, elevationBias);     //implementing elevation bias, higher num = higher bias
                int elevation = Mathf.FloorToInt(noise * (maxElevation+1)); //distribution
                elevationMap[i,j] = elevation;
            }
        }
        GridCreator creator = new GridCreator();
        creator.Create(GridType.Rectangle, AxisType.XZ, tileSetting, new Vector2(mapX, mapZ), false);
        //Creating a map with the values from mapX, mapZ to match it with elevation after.
        List<TileController> tiles = GridManager.Instance.Tiles;
        foreach(TileController tile in tiles)
        {
            int elevationY = elevationMap[tile.coordinate.x, tile.coordinate.y]; //get assigned elevation
            tile.transform.position = new Vector3(tile.transform.position.x, 
            elevationY, tile.transform.position.z); //actually assign it, modifying Y only
            for (int i = elevationY-1; i >= 0; i--)    
            {
                Instantiate(tileSetting.rectangle.prefab, new Vector3(tile.transform.position.x, 
                i, tile.transform.position.z), Quaternion.identity);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
