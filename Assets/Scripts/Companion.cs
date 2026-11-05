using UnityEngine;

public class Companion : MonoBehaviour
{
    public bool IsRecruited { get; private set; }

    public Vector3 GridPosition { get; private set; }

    private PartyLeader partyLeader;

    private Vector3 targetPosition;
    private Vector3 visualOffset;

    private float movementSpeed;
    private bool isMoving;

    private ActionToSprite actionToSprite;

    private void Awake()
    {
        GridPosition = transform.position;
        targetPosition = transform.position;

        actionToSprite = GetComponent<ActionToSprite>();
    }

    private void Update()
    {
        if (!isMoving)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            movementSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
        {
            transform.position = targetPosition;
            isMoving = false;

            // Companion has finished moving.
            if (actionToSprite != null)
                actionToSprite.SetMoving(false);
        }
    }

    public void SetRecruited(PartyLeader leader)
    {
        if (IsRecruited)
            return;

        partyLeader = leader;
        IsRecruited = true;

        GridPosition = transform.position;
        targetPosition = transform.position;

        Debug.Log(gameObject.name + " has joined the party!");
    }

    public void FollowStep(Vector3 destination, float moveDuration)
    {
        if (!IsRecruited)
            return;

        GridPosition = destination;

        UpdateTarget(moveDuration);
    }

    public void SetNodeSharingOffset(Vector3 offset, float moveDuration)
    {
        visualOffset = offset;

        UpdateTarget(moveDuration);
    }

    private void UpdateTarget(float moveDuration)
    {
        targetPosition = GridPosition + visualOffset;

        float distance = Vector3.Distance(
            transform.position,
            targetPosition
        );

        if (moveDuration <= 0f)
        {
            transform.position = targetPosition;
            isMoving = false;

            if (actionToSprite != null)
                actionToSprite.SetMoving(false);

            return;
        }

        movementSpeed = distance / Mathf.Max(moveDuration, 0.001f);
        isMoving = distance > 0.001f;

        // Tell the sprite system that movement has started.
        if (actionToSprite != null)
        {
            Vector3 movement =
                targetPosition - transform.position;

            actionToSprite.SetHorizontalDirection(
                movement.x
            );

            actionToSprite.SetMoving(isMoving);
        }
    }
}