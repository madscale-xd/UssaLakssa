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

    void Update()
    {
    }
    private void FindTileSharers()
    {
        TileSharers = FindObjectsByType<TileSharing>(FindObjectsSortMode.None);
    }
    public void TryTileShare()
    {
        Vector3 tileCenter = currentTile.transform.position;
        float radius = 0.25f;
        List<TileSharing> sharers = new List<TileSharing>();
        foreach(TileSharing unit in TileSharers)
        {
            if(unit.currentTile == currentTile)
            {
               sharers.Add(unit);
            }
            Debug.Log("Tile: " + currentTile.name + " | Sharers: " + sharers.Count);
        }
        if(sharers.Count > 1)
        {
            int sharerCount = 0;
            foreach(TileSharing unit in sharers)
            {
                float angle = (360f / sharers.Count) * sharerCount;
                float angleRad = angle * Mathf.Deg2Rad;

                float sharedX = tileCenter.x + radius * Mathf.Cos(angleRad);
                float sharedZ = tileCenter.z + radius * Mathf.Sin(angleRad);

                Vector3 formationPosition = new Vector3(sharedX, unit.transform.position.y, sharedZ);

                unit.StartCoroutine(unit.RepositionUnit(0.25f, formationPosition));
                sharerCount++;
                Debug.Log(unit.name + " -> " + formationPosition);
            }
        }
    }
    public void UpdateTile(TileController newTile)
    {
        currentTile = newTile;
    }
    private IEnumerator RepositionUnit(float duration, Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            float progress = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}