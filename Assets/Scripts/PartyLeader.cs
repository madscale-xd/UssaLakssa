
using System.Collections.Generic;
using UnityEngine;

public class PartyLeader : MonoBehaviour
{
    [Header("Recruitment")]
    [SerializeField] private LayerMask companionLayers = ~0;

    [Header("Node Sharing")]
    [SerializeField] private float nodeOffsetMultiplier = 0.25f;

    private List<Companion> partyMembers = new List<Companion>();

    private GridPlayerMovement gridMovement;

    // Tracks the direction each character entered their current node from.
    private Vector3 leaderIncomingDirection;

    private Dictionary<Companion, Vector3> companionIncomingDirections =
        new Dictionary<Companion, Vector3>();

    public int PartySize
    {
        get
        {
            partyMembers.RemoveAll(member => member == null);
            return partyMembers.Count;
        }
    }

    private class NodeMember
    {
        public Companion companion;
        public bool isLeader;
        public Vector3 incomingDirection;
    }

    private void Awake()
    {
        gridMovement = GetComponent<GridPlayerMovement>();
    }

    private void Update()
    {
        CheckForRecruitment();
    }

    private void CheckForRecruitment()
    {
        if (gridMovement == null)
            return;

        float cellSize = Mathf.Max(gridMovement.CellSize, 0.001f);

        Vector3Int leaderNode = GetNode(
            gridMovement.LogicalPosition,
            cellSize
        );

        Companion[] companions = FindObjectsByType<Companion>(
            FindObjectsSortMode.None
        );

        foreach (Companion companion in companions)
        {
            if (companion == null || companion.IsRecruited)
                continue;

            Vector3Int companionNode = GetNode(
                companion.GridPosition,
                cellSize
            );

            if (leaderNode == companionNode)
                Recruit(companion);
        }
    }

    public bool Recruit(Companion companion)
    {
        if (companion == null || companion.IsRecruited)
            return false;

        companion.SetRecruited(this);
        partyMembers.Add(companion);

        companionIncomingDirections[companion] = Vector3.zero;

        RefreshNodeSharing(0.25f);

        Debug.Log("Recruited companion! Party size: " + PartySize);

        return true;
    }

    // Called whenever the Party Leader successfully moves.
    public void OnLeaderStep(Vector3 previousPosition, float moveDuration)
    {
        Vector3 currentPosition = gridMovement.LogicalPosition;

        // Record the Party Leader's incoming direction.
        leaderIncomingDirection = GetHorizontalDirection(
            currentPosition - previousPosition
        );

        Vector3 previousCharacterPosition = previousPosition;

        // Each companion moves into the previous position
        // of the character directly ahead of them.
        for (int i = 0; i < partyMembers.Count; i++)
        {
            Companion companion = partyMembers[i];

            if (companion == null)
                continue;

            Vector3 companionPreviousPosition = companion.GridPosition;

            // Record the direction this companion is entering from.
            Vector3 incomingDirection = GetHorizontalDirection(
                previousCharacterPosition - companionPreviousPosition
            );

            companionIncomingDirections[companion] = incomingDirection;

            companion.FollowStep(
                previousCharacterPosition,
                moveDuration
            );

            previousCharacterPosition = companionPreviousPosition;
        }

        RefreshNodeSharing(moveDuration);
    }

    private void RefreshNodeSharing(float moveDuration)
    {
        float cellSize = gridMovement != null
            ? gridMovement.CellSize
            : 1f;

        cellSize = Mathf.Max(cellSize, 0.001f);

        Dictionary<Vector3Int, List<NodeMember>> nodes =
            new Dictionary<Vector3Int, List<NodeMember>>();

        // Add the Party Leader first.
        Vector3Int leaderNode = GetNode(
            gridMovement.LogicalPosition,
            cellSize
        );

        AddToNode(nodes, leaderNode, new NodeMember
        {
            companion = null,
            isLeader = true,
            incomingDirection = leaderIncomingDirection
        });

        // Add all recruited companions.
        foreach (Companion companion in partyMembers)
        {
            if (companion == null)
                continue;

            Vector3Int node = GetNode(
                companion.GridPosition,
                cellSize
            );

            Vector3 incomingDirection = Vector3.zero;

            if (companionIncomingDirections.TryGetValue(
                companion,
                out Vector3 savedDirection))
            {
                incomingDirection = savedDirection;
            }

            AddToNode(nodes, node, new NodeMember
            {
                companion = companion,
                isLeader = false,
                incomingDirection = incomingDirection
            });
        }

        // Distribute characters sharing each node.
        foreach (var entry in nodes)
        {
            List<NodeMember> members = entry.Value;

            int count = members.Count;

            if (count == 1)
            {
                ApplyNodeOffset(members[0], Vector3.zero, moveDuration);
                continue;
            }

            // Generate all available diagonal positions.
            List<Vector3> availableOffsets = new List<Vector3>();

            for (int i = 0; i < count; i++)
            {
                availableOffsets.Add(
                    GetNodeOffset(i, count, cellSize)
                );
            }

            // Assign the closest available diagonal to each character.
            // The Party Leader is first in the list, so they have priority.
            foreach (NodeMember member in members)
            {
                int bestIndex = GetBestOffsetIndex(
                    member.incomingDirection,
                    availableOffsets
                );

                Vector3 chosenOffset = availableOffsets[bestIndex];

                ApplyNodeOffset(
                    member,
                    chosenOffset,
                    moveDuration
                );

                availableOffsets.RemoveAt(bestIndex);
            }
        }
    }

    
    private int GetBestOffsetIndex(
        Vector3 incomingDirection,
        List<Vector3> availableOffsets)
    {
        if (availableOffsets.Count <= 1)
            return 0;

        if (incomingDirection.sqrMagnitude < 0.001f)
            return 0;

        int bestIndex = 0;
        float bestDot = float.NegativeInfinity;

        // Reverse the movement direction to face the direction
        // the character came FROM.
        Vector3 preferredDirection = -incomingDirection.normalized;

        for (int i = 0; i < availableOffsets.Count; i++)
        {
            Vector3 offsetDirection = new Vector3(
                availableOffsets[i].x,
                0f,
                availableOffsets[i].z
            ).normalized;

            float dot = Vector3.Dot(
                preferredDirection,
                offsetDirection
            );

            if (dot > bestDot)
            {
                bestDot = dot;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    private void ApplyNodeOffset(
        NodeMember member,
        Vector3 offset,
        float moveDuration)
    {
        if (member.isLeader)
        {
            gridMovement.SetNodeSharingOffset(
                offset,
                moveDuration
            );
        }
        else if (member.companion != null)
        {
            member.companion.SetNodeSharingOffset(
                offset,
                moveDuration
            );
        }
    }

    private Vector3 GetHorizontalDirection(Vector3 direction)
    {
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return Vector3.zero;

        return direction.normalized;
    }

    private void AddToNode(
        Dictionary<Vector3Int, List<NodeMember>> nodes,
        Vector3Int node,
        NodeMember member)
    {
        if (!nodes.ContainsKey(node))
            nodes.Add(node, new List<NodeMember>());

        nodes[node].Add(member);
    }

    private Vector3Int GetNode(Vector3 position, float cellSize)
    {
        return new Vector3Int(
            Mathf.RoundToInt(position.x / cellSize),
            Mathf.RoundToInt(position.y / cellSize),
            Mathf.RoundToInt(position.z / cellSize)
        );
    }

    private Vector3 GetNodeOffset(int index, int count, float cellSize)
    {
        if (count <= 1)
            return Vector3.zero;

        float radius = cellSize * nodeOffsetMultiplier * Mathf.Sqrt(2f);

        float angle = (index * Mathf.PI * 2f / count)
            + Mathf.PI / 4f;

        return new Vector3(
            Mathf.Cos(angle) * radius,
            0f,
            Mathf.Sin(angle) * radius
        );
    }

    public int GetStepCost(bool isClimbing = false)
    {
        int baseCost = 1 + PartySize;

        if (isClimbing)
            baseCost *= 2;

        return baseCost;
    }
}
