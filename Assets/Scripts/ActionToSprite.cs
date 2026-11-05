using UnityEngine;
using System.Collections;

public class ActionToSprite : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite movementSprite;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Flip Settings")]
    [SerializeField] private float flipDuration = 0.25f;

    private int lastHorizontalDirection = 0;

    private Coroutine flipCoroutine;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        SetIdle();
    }

    public void SetMoving(bool moving)
    {
        if (spriteRenderer == null)
            return;

        if (moving)
        {
            if (movementSprite != null)
                spriteRenderer.sprite = movementSprite;
        }
        else
        {
            SetIdle();
        }
    }

    public void SetIdle()
    {
        if (spriteRenderer == null)
            return;

        if (idleSprite != null)
            spriteRenderer.sprite = idleSprite;
    }

    public void SetHorizontalDirection(float horizontalDirection)
    {
        // Ignore vertical movement.
        if (Mathf.Abs(horizontalDirection) < 0.001f)
            return;

        int currentDirection = horizontalDirection > 0f ? 1 : -1;

        // First horizontal movement.
        if (lastHorizontalDirection == 0)
        {
            lastHorizontalDirection = currentDirection;

            // Sprite starts facing RIGHT.
            // If the first movement is LEFT, flip immediately.
            if (currentDirection < 0)
                Flip();

            return;
        }

        // Same horizontal direction as before.
        if (currentDirection == lastHorizontalDirection)
            return;

        // Horizontal direction changed.
        lastHorizontalDirection = currentDirection;

        Flip();
    }

    private void Flip()
    {
        if (flipCoroutine != null)
            StopCoroutine(flipCoroutine);

        float targetY = transform.localEulerAngles.y + 180f;

        flipCoroutine = StartCoroutine(
            SmoothRotateY(targetY)
        );
    }

    private IEnumerator SmoothRotateY(float targetY)
    {
        Quaternion startRotation = transform.localRotation;

        Quaternion targetRotation = Quaternion.Euler(
            startRotation.eulerAngles.x,
            targetY,
            startRotation.eulerAngles.z
        );

        float elapsed = 0f;

        while (elapsed < flipDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / Mathf.Max(flipDuration, 0.001f)
            );

            t = Mathf.SmoothStep(0f, 1f, t);

            transform.localRotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                t
            );

            yield return null;
        }

        transform.localRotation = targetRotation;
        flipCoroutine = null;
    }
}