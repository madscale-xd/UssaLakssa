using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using MyGrid.Code;
using System;

public class TileSharing : MonoBehaviour
{
    private TileController currentTile;
    public TileController CurrentTile => currentTile;   //getter
    private TileSharing[] TileSharers;
    void Start()
    {
        List<TileController> tiles = GridManager.Instance.Tiles;
        currentTile = tiles[UnityEngine.Random.Range(0,tiles.Count)];
        transform.position = currentTile.transform.position + Vector3.up * 1.25f;
        FindTileSharers();
    }
    private void FindTileSharers()
    {
        TileSharers = FindObjectsByType<TileSharing>(FindObjectsSortMode.None);
    }
    private void TryTileShare()
    {
        foreach(TileSharing unit in TileSharers)
        {
            if(unit.currentTile == currentTile)
            {
                
            }
        }
    }
    public void UpdateTile(TileController newTile)
    {
        currentTile = newTile;
    }
}