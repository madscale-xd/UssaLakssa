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
    [SerializeField] private Companion currentLeader;
    public Companion CurrentLeader => currentLeader;
    private List<Companion> partyMembers = new List<Companion>();

    void Start()
    {
        //currentLeader = GetComponent<Companion>();  //change in selection screens
        currentLeader.SetLeader();
    }

    void Update()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame && !isMoving)
        {
            TryMove(currentLeader.companionTile().CurrentTile.GetNeighbour(Direction.Up));
        }

        if (Keyboard.current.sKey.wasPressedThisFrame && !isMoving)
        {
            TryMove(currentLeader.companionTile().CurrentTile.GetNeighbour(Direction.Down));
        }

        if (Keyboard.current.dKey.wasPressedThisFrame && !isMoving)
        {
            TryMove(currentLeader.companionTile().CurrentTile.GetNeighbour(Direction.Right));
        }

        if (Keyboard.current.aKey.wasPressedThisFrame && !isMoving)
        {
            TryMove(currentLeader.companionTile().CurrentTile.GetNeighbour(Direction.Left));
        }
    }

    void TryMove(TileController newTile)
    {
        if (newTile == null) return;
        float tileDiff = currentLeader.companionTile().CurrentTile.transform.position.y - newTile.transform.position.y;
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
        currentLeader.companionTile().UpdateTile(newTile);
        StartCoroutine(MoveToTile(moveDuration, tileDiff));
    }

    private IEnumerator MoveToTile(float duration, float tileDiff)
    {
        Vector3 startPosition = currentLeader.transform.position;
        Vector3 newPosition = currentLeader.companionTile().CurrentTile.transform.position;
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
        currentLeader.companionTile().TryTileShare();
        TryRecruit();
        LeaderMoved?.Invoke();
        PartyMove();
    }
    void TryRecruit()
    {
        List<TileSharing> possComps = currentLeader.companionTile().TileOccupancy();
        if(possComps.Count > 1)
        {
            foreach(TileSharing unit in possComps)
            {
                Companion companion = unit.CompanionCall();
                if ( companion != null && companion != currentLeader && !companion.IsRecruited)
                {
                    companion.SetRecruited();
                    partyMembers.Add(companion);
                }
            }
        }
    }
    public List<Companion> CompanionList()
    {
        return partyMembers;
    }

    void PartyMove()
    {
        for (int i = 0; i < CompanionList().Count; i++)
        {
            if(i == 0)
            {
                CompanionList()[i].TryMoveComp(currentLeader);
            }
            else
            {
                CompanionList()[i].TryMoveComp(CompanionList()[i-1]);
            }
        }
    }
}