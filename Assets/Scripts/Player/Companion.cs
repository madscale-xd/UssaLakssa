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
    [SerializeField] private LeaderMovement leader;
    protected string compName;
    private bool isRecruited = false;
    private TileSharing tileSharer;
    private bool isMoving;
    
    void Start()
    {
        tileSharer = GetComponent<TileSharing>();
        //tileSharer.CurrentTile;
    }

    void Update()
    {
    }
}
