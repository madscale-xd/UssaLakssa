using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using MyGrid.Code;
using System;

public class Companion : MonoBehaviour
{
    [Header ("Sprites")]
    [SerializeField] protected Sprite walkSprite;
    [SerializeField] protected Sprite idleSprite;
    [SerializeField] protected Sprite attackSprite;
    [SerializeField] protected Sprite spellSprite;
    [SerializeField] private float moveDuration = 0.33f;
    protected string compName;
    private bool isRecruited = false;
    public bool IsRecruited => isRecruited;
    private TileSharing tileSharer;
    private bool isLeader;
    private LeaderMovement leader;
    private Companion followTarget;
    private StepCounter stepCounter;

    public void Start()
    {
        tileSharer = GetComponent<TileSharing>();
        leader = FindFirstObjectByType<LeaderMovement>();
        //tileSharer.CurrentTile;
        stepCounter = FindFirstObjectByType<StepCounter>();
    }
    public void TryMoveComp(Companion followTarget)
    {
        if(isLeader) return;
        if(!isRecruited) return;
        TileController targetTile = followTarget.companionTile().PreviousTile;
        float tileDiff = tileSharer.CurrentTile.transform.position.y - targetTile.transform.position.y;
        if (Mathf.Abs(tileDiff) > 1) return;
        if (tileDiff <= -1)
        {
            stepCounter.SubtractSteps(2);
        }
        else
        {
            stepCounter.SubtractSteps(1);
        }
        tileSharer.UpdateTile(targetTile);
        StartCoroutine(MoveToTile(moveDuration, targetTile, tileDiff));
    }
    public TileSharing companionTile()
    {
        return tileSharer;
    }

    public void SetLeader()
    {
        isLeader = true;
    }

    public void SetRecruited()
    {
        isRecruited = true;
    }
    private IEnumerator MoveToTile(float duration, TileController targetTile, float tileDiff)
    {
        Vector3 startPosition = transform.position;
        Vector3 newPosition = targetTile.transform.position;
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
        tileSharer.TryTileShare();
    }
}
