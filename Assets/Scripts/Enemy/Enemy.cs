using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using MyGrid.Code;
using System;
public class Enemy : MonoBehaviour //(OVERWORLD) Enemy only moves when player does their move
{
    private TileSharing tileSharer;
    private bool isMoving;
    [SerializeField] private float moveDuration = 0.33f;
    private LeaderMovement leader;

    void Start()
    {
        tileSharer = GetComponent<TileSharing>();
        leader = FindFirstObjectByType<LeaderMovement>();
        leader.LeaderMoved += TryMoveEnemy;
    }

    void Update()
    {
    }

    void TryMoveEnemy()
    {
        if(isMoving) return;
        TileController newTile = ChooseTile();
        float tileDiff = tileSharer.CurrentTile.transform.position.y - newTile.transform.position.y;
        isMoving = true;
        tileSharer.UpdateTile(newTile);
        StartCoroutine(MoveToTile(moveDuration, tileDiff));
    }

    public TileController ChooseTile()
    {
        TileController enemyUp = tileSharer.CurrentTile.GetNeighbour(Direction.Up);
        TileController enemyDown = tileSharer.CurrentTile.GetNeighbour(Direction.Down);
        TileController enemyLeft = tileSharer.CurrentTile.GetNeighbour(Direction.Left);
        TileController enemyRight = tileSharer.CurrentTile.GetNeighbour(Direction.Right);
        List<TileController> enemyValidTiles = new List<TileController>{enemyUp, enemyDown, enemyLeft, enemyRight};
        for (int i = enemyValidTiles.Count-1; i >= 0; i--)
        {
            if(enemyValidTiles[i] == null ||Mathf.Abs(tileSharer.CurrentTile.transform.position.y - enemyValidTiles[i].transform.position.y) > 1)
            {
                enemyValidTiles.RemoveAt(i);
            }
        }
        TileController chosenTile = enemyValidTiles[UnityEngine.Random.Range(0, enemyValidTiles.Count)];
        return chosenTile;
    }

    private IEnumerator MoveToTile(float duration, float tileDiff)
    {
        Vector3 startPosition = transform.position;
        Vector3 newPosition = tileSharer.CurrentTile.transform.position;
        newPosition.y = transform.position.y - tileDiff;
        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            float progress = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, newPosition, progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = newPosition;
        isMoving = false;
        tileSharer.TryTileShare();
    }

    void OnDisable()
    {
        leader.LeaderMoved -= TryMoveEnemy;
    }
}