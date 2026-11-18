using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using MyGrid.Code;

public class LeaderMovement : MonoBehaviour
{
    private TileController currentTile;
    private bool isMoving;
    [SerializeField] private float moveDuration = 0.33f;

    void Start()
    {
        List<TileController> tiles = GridManager.Instance.Tiles;
        currentTile = tiles[Random.Range(0,tiles.Count)];
        transform.position = currentTile.transform.position + Vector3.up * 1.25f;
    }

    void Update()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame && !isMoving)
        {
            TryMove(currentTile.GetNeighbour(Direction.Up));
        }

        if (Keyboard.current.sKey.wasPressedThisFrame && !isMoving)
        {
            TryMove(currentTile.GetNeighbour(Direction.Down));
        }

        if (Keyboard.current.dKey.wasPressedThisFrame && !isMoving)
        {
            TryMove(currentTile.GetNeighbour(Direction.Right));
        }

        if (Keyboard.current.aKey.wasPressedThisFrame && !isMoving)
        {
            TryMove(currentTile.GetNeighbour(Direction.Left));
        }
    }

    void TryMove(TileController newTile)
    {
        if (newTile == null)
        {
            return;
        }
        isMoving = true;
        currentTile = newTile;
        StartCoroutine(MoveToTile(moveDuration));
    }

    private IEnumerator MoveToTile(float duration)
    {
        Vector3 startPosition = transform.position;
        Vector3 newPosition = currentTile.transform.position;
        newPosition.y = transform.position.y;
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
}
