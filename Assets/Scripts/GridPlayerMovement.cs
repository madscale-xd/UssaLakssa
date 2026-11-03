
using UnityEngine;
using UnityEngine.InputSystem;

public class GridPlayerMovement : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private float cellSize = 1f;

    [Header("Collision Checks")]
    [SerializeField] private Vector3 checkSize = new Vector3(0.8f, 0.8f, 0.8f);
    [SerializeField] private float checkHeightOffset = 0.5f;

    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.12f;

    private bool isMoving;

    private Vector3 logicalPosition;
    private Vector3 visualOffset;
    private Vector3 targetPosition;

    private float movementSpeed;

    private StepCount stepCount;
    private PartyLeader partyLeader;

    private int companionLayer;

    public float CellSize => cellSize;
    public Vector3 LogicalPosition => logicalPosition;

    private void Start()
    {
        Vector3 position = transform.position;

        position.x = Mathf.Round(position.x / cellSize) * cellSize;
        position.z = Mathf.Round(position.z / cellSize) * cellSize;

        logicalPosition = position;
        transform.position = position;
        targetPosition = position;

        stepCount = FindFirstObjectByType<StepCount>();
        partyLeader = GetComponent<PartyLeader>();

        companionLayer = LayerMask.NameToLayer("Companion");

        if (companionLayer == -1)
            Debug.LogWarning("The 'Companion' layer does not exist.");
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
            return;
        }

        if (Keyboard.current == null)
            return;

        Vector2Int direction = Vector2Int.zero;

        if (Keyboard.current.wKey.wasPressedThisFrame)
            direction = Vector2Int.up;
        else if (Keyboard.current.sKey.wasPressedThisFrame)
            direction = Vector2Int.down;
        else if (Keyboard.current.aKey.wasPressedThisFrame)
            direction = Vector2Int.left;
        else if (Keyboard.current.dKey.wasPressedThisFrame)
            direction = Vector2Int.right;

        if (direction != Vector2Int.zero)
            TryMove(direction);
    }

    private void TryMove(Vector2Int direction)
    {
        Vector3 forward = new Vector3(
            direction.x,
            0f,
            direction.y
        ) * cellSize;

        Vector3 current = logicalPosition;
        Vector3 next = current + forward;

        // 1. Normal movement.
        if (IsSpaceClear(next) &&
            IsSpaceOccupied(next + Vector3.down * cellSize))
        {
            StartMove(next);
            return;
        }

        // 2. Climb.
        Vector3 climbOne = next + Vector3.up * cellSize;

        if (IsSpaceClear(climbOne) &&
            IsSpaceOccupied(climbOne + Vector3.down * cellSize))
        {
            StartMove(climbOne, true);
            return;
        }

        // 3. Step down.
        Vector3 downOne = next + Vector3.down * cellSize;

        if (IsSpaceClear(next) &&
            IsSpaceClear(downOne) &&
            IsSpaceOccupied(downOne + Vector3.down * cellSize))
        {
            StartMove(downOne);
            return;
        }
    }

    private bool IsCompanionLayer(Collider hit)
    {
        return companionLayer != -1 &&
               hit.gameObject.layer == companionLayer;
    }

    private bool IsSpaceClear(Vector3 gridPosition)
    {
        Vector3 checkCenter =
            gridPosition + Vector3.up * checkHeightOffset;

        Collider[] hits = Physics.OverlapBox(
            checkCenter,
            checkSize * 0.5f,
            Quaternion.identity,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        foreach (Collider hit in hits)
        {
            if (hit.transform == transform ||
                hit.transform.IsChildOf(transform))
                continue;

            // Companion-layer objects do not block movement.
            if (IsCompanionLayer(hit))
                continue;

            Companion companion = hit.GetComponentInParent<Companion>();

            if (companion != null && companion.IsRecruited)
                continue;

            return false;
        }

        return true;
    }

    private bool IsSpaceOccupied(Vector3 gridPosition)
    {
        Vector3 checkCenter =
            gridPosition + Vector3.up * checkHeightOffset;

        Collider[] hits = Physics.OverlapBox(
            checkCenter,
            checkSize * 0.5f,
            Quaternion.identity,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        foreach (Collider hit in hits)
        {
            if (hit.transform == transform ||
                hit.transform.IsChildOf(transform))
                continue;

            // Companion-layer objects do not count as ground or obstacles.
            if (IsCompanionLayer(hit))
                continue;

            Companion companion = hit.GetComponentInParent<Companion>();

            if (companion != null && companion.IsRecruited)
                continue;

            return true;
        }

        return false;
    }

    private void StartMove(Vector3 destination, bool isClimbing = false)
    {
        Vector3 previousPosition = logicalPosition;

        logicalPosition = destination;
        targetPosition = logicalPosition + visualOffset;

        isMoving = true;

        if (partyLeader != null)
        {
            partyLeader.OnLeaderStep(
                previousPosition,
                moveDuration
            );
        }

        float distance = Vector3.Distance(
            transform.position,
            targetPosition
        );

        movementSpeed = distance / Mathf.Max(moveDuration, 0.001f);

        int stepCost = partyLeader != null
            ? partyLeader.GetStepCost(isClimbing)
            : (isClimbing ? 2 : 1);

        if (stepCount != null)
            stepCount.UseStep(stepCost);
    }

    public void SetNodeSharingOffset(Vector3 offset, float duration)
    {
        visualOffset = offset;
        targetPosition = logicalPosition + visualOffset;

        float distance = Vector3.Distance(
            transform.position,
            targetPosition
        );

        if (duration <= 0f)
        {
            transform.position = targetPosition;
            isMoving = false;
            return;
        }

        movementSpeed = distance / Mathf.Max(duration, 0.001f);
        isMoving = distance > 0.001f;
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            movementSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(
            transform.position + Vector3.up * checkHeightOffset,
            checkSize
        );
    }
}
