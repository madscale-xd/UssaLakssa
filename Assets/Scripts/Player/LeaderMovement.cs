using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using MyGrid.Code;
using System;

public class LeaderMovement : MonoBehaviour
{
    public event Action LeaderMoved;
    private bool isMoving;
    [SerializeField] private float moveDuration = 0.33f;
    [SerializeField] private StepCounter stepCounter;
    private Companion currentLeader;
    private TileSharing tileSharer;

    void Start()
    {
        currentLeader = GetComponent<Companion>();
        tileSharer = GetComponent<TileSharing>();
    }

    void Update()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame && !isMoving)
        {
            TryMove(tileSharer.CurrentTile.GetNeighbour(Direction.Up));
        }

        if (Keyboard.current.sKey.wasPressedThisFrame && !isMoving)
        {
            TryMove(tileSharer.CurrentTile.GetNeighbour(Direction.Down));
        }

        if (Keyboard.current.dKey.wasPressedThisFrame && !isMoving)
        {
            TryMove(tileSharer.CurrentTile.GetNeighbour(Direction.Right));
        }

        if (Keyboard.current.aKey.wasPressedThisFrame && !isMoving)
        {
            TryMove(tileSharer.CurrentTile.GetNeighbour(Direction.Left));
        }
    }

    void TryMove(TileController newTile)
    {
        if (newTile == null) return;
        float tileDiff = tileSharer.CurrentTile.transform.position.y - newTile.transform.position.y;
        if (Mathf.Abs(tileDiff) > 1) return;
        if (tileDiff <= -1)
        {
            stepCounter.SubtractSteps(2);
        }
        else
        {
            stepCounter.SubtractSteps(1);
        }
        isMoving = true;
        tileSharer.UpdateTile(newTile);
        StartCoroutine(MoveToTile(moveDuration, tileDiff));
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
        LeaderMoved?.Invoke();
    }

    void TryRecruit()
    {
        
    }
}