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
    [SerializeField] Companion currentLeader;

    void Start()
    {
        //currentLeader = GetComponent<Companion>();  //change in selection screens
        currentLeader.SetLeader();
    }

    void Update()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame && !isMoving)
        {
            TryMove(currentLeader.leaderTile().CurrentTile.GetNeighbour(Direction.Up));
        }

        if (Keyboard.current.sKey.wasPressedThisFrame && !isMoving)
        {
            TryMove(currentLeader.leaderTile().CurrentTile.GetNeighbour(Direction.Down));
        }

        if (Keyboard.current.dKey.wasPressedThisFrame && !isMoving)
        {
            TryMove(currentLeader.leaderTile().CurrentTile.GetNeighbour(Direction.Right));
        }

        if (Keyboard.current.aKey.wasPressedThisFrame && !isMoving)
        {
            TryMove(currentLeader.leaderTile().CurrentTile.GetNeighbour(Direction.Left));
        }
    }

    void TryMove(TileController newTile)
    {
        if (newTile == null) return;
        float tileDiff = currentLeader.leaderTile().CurrentTile.transform.position.y - newTile.transform.position.y;
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
        currentLeader.leaderTile().UpdateTile(newTile);
        StartCoroutine(MoveToTile(moveDuration, tileDiff));
    }

    private IEnumerator MoveToTile(float duration, float tileDiff)
    {
        Vector3 startPosition = currentLeader.transform.position;
        Vector3 newPosition = currentLeader.leaderTile().CurrentTile.transform.position;
        newPosition.y = currentLeader.transform.position.y - tileDiff;
        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            float progress = elapsedTime / duration;
            currentLeader.transform.position = Vector3.Lerp(startPosition, newPosition, progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        currentLeader.transform.position = newPosition;
        isMoving = false;
        currentLeader.leaderTile().TryTileShare();
        LeaderMoved?.Invoke();
    }
    void TryRecruit()
    {
        
    }
}