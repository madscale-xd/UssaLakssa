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
    protected string compName;
    private bool isRecruited = false;
    private TileSharing tileSharer;
    private bool isMoving;
    private bool isLeader;
    private LeaderMovement leader;
    
    void Start()
    {
        tileSharer = GetComponent<TileSharing>();
        leader = FindFirstObjectByType<LeaderMovement>();
        leader.LeaderMoved += TryMoveComp;
        //tileSharer.CurrentTile;
    }

    void Update()
    {
    }

    void TryMoveComp()
    {
        if(isLeader) return;

    }

    public TileSharing leaderTile()
    {
        return tileSharer;
    }

    public void SetLeader()
    {
        isLeader = true;
    }
}
