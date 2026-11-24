using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using System;

public class OverworldManager : MonoBehaviour
{
    public static OverworldManager Instance {get; private set;}
    private LeaderMovement leader;
    private List<Companion> partyList;
    private List<Enemy> enemyList;
    [SerializeField] private float delayBetweenTurns;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        leader = FindFirstObjectByType<LeaderMovement>();
        leader.LeaderMoved += StartResolution;
        partyList = leader.CompanionList();
        enemyList = new List<Enemy>(FindObjectsByType<Enemy>(FindObjectsSortMode.None));
    }
    void UpdateLeader()
    {
        leader = FindFirstObjectByType<LeaderMovement>();
    }
    //Turn order: Pre-move Party Items -> Leader -> Companions -> Post-move items -> Enemies -> Post-enemy move items
    void StartResolution()
    {
        StartCoroutine(PartyMove());
    }
    IEnumerator PartyMove()
    {
        partyList = leader.CompanionList();

        for (int i = 0; i < partyList.Count; i++)
        {
            if(i == 0)
            {
                yield return new WaitForSeconds(delayBetweenTurns);
                partyList[i].TryMoveComp(leader.partyLeader());
            }
            else
            {
                yield return new WaitForSeconds(delayBetweenTurns);
                partyList[i].TryMoveComp(partyList[i-1]);
            }
        }
        StartCoroutine(ConcludeEnemies());
    }
    IEnumerator ConcludeEnemies()
    {
        enemyList = new List<Enemy>(FindObjectsByType<Enemy>(FindObjectsSortMode.None));
        for (int i = 0; i < enemyList.Count; i++)
        {
            yield return new WaitForSeconds(delayBetweenTurns);
            yield return StartCoroutine(enemyList[i].TryMoveEnemy());
        }
        leader.setMoving(false);
    }
    void OnDisable()
    {
        leader.LeaderMoved -= StartResolution;
    }
}