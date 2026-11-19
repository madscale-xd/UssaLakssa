using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using MyGrid.Code;
using System;
public class EnemyMovement : MonoBehaviour //(OVERWORLD) Enemy only moves when player does their move
{
    private TileController currentTile;
    private bool isMoving;
    [SerializeField] private float moveDuration = 0.33f;
    [SerializeField] private LeaderMovement leader;

    void Start()
    {
        List<TileController> tiles = GridManager.Instance.Tiles;
        currentTile = tiles[UnityEngine.Random.Range(0,tiles.Count)];
        transform.position = currentTile.transform.position + Vector3.up * 1.25f;
    }

    void Update()
    {
    }

    void TryMoveEnemy()
    {
        if(isMoving) return;
        TileController newTile = ChooseTile();
        float tileDiff = currentTile.transform.position.y - newTile.transform.position.y;
        isMoving = true;
        currentTile = newTile;
        StartCoroutine(MoveToTile(moveDuration, tileDiff));
    }

    public TileController ChooseTile()
    {
        TileController enemyUp = currentTile.GetNeighbour(Direction.Up);
        TileController enemyDown = currentTile.GetNeighbour(Direction.Down);
        TileController enemyLeft = currentTile.GetNeighbour(Direction.Left);
        TileController enemyRight = currentTile.GetNeighbour(Direction.Right);
        List<TileController> enemyValidTiles = new List<TileController>{enemyUp, enemyDown, enemyLeft, enemyRight};
        for (int i = enemyValidTiles.Count-1; i >= 0; i--)
        {
            if(enemyValidTiles[i] == null ||Mathf.Abs(currentTile.transform.position.y - enemyValidTiles[i].transform.position.y) > 1)
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
        Vector3 newPosition = currentTile.transform.position;
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
    }
    void OnEnable()
    {
        leader.LeaderMoved += TryMoveEnemy;
    }

    void OnDisable()
    {
        leader.LeaderMoved -= TryMoveEnemy;
    }
}