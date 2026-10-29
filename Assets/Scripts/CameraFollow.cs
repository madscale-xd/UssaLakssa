
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 8f, -10f);
    [SerializeField] private float followSpeed = 8f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );

        transform.LookAt(target.position);
    }
}
