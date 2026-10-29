
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
    private Vector3 targetPosition;

    private void Start()
    {
        Vector3 position = transform.position;

        position.x = Mathf.Round(position.x / cellSize) * cellSize;
        position.z = Mathf.Round(position.z / cellSize) * cellSize;

        transform.position = position;
        targetPosition = position;
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

        Vector3 current = transform.position;
        Vector3 next = current + forward;

        // 1. Normal movement: destination is clear and supported.
        if (IsSpaceClear(next) &&
            IsSpaceOccupied(next + Vector3.down * cellSize))
        {
            StartMove(next);
            return;
        }

        // 2. Climb: destination is one unit forward and one unit up.
        Vector3 climbOne = next + Vector3.up * cellSize;

        if (IsSpaceClear(climbOne) &&
            IsSpaceOccupied(climbOne + Vector3.down * cellSize))
        {
            StartMove(climbOne);
            return;
        }

        // 3. Step down: try one unit below the forward destination.
        Vector3 downOne = next + Vector3.down * cellSize;

        if (IsSpaceClear(next) &&
            IsSpaceClear(downOne) &&
            IsSpaceOccupied(downOne + Vector3.down * cellSize))
        {
            StartMove(downOne);
            return;
        }

        // 4. No valid move.
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

            return true;
        }

        return false;
    }

    private void StartMove(Vector3 destination)
    {
        targetPosition = destination;
        isMoving = true;
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            (cellSize / moveDuration) * Time.deltaTime
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
