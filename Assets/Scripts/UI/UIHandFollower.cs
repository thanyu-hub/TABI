using UnityEngine;

public class UIHandFollower : MonoBehaviour
{
    public enum TrackMode
    {
        Local,
        Global
    }

    [Header("Tracking Settings")]
    public Transform handTransform;
    public TrackMode trackingMode = TrackMode.Local;

    [Header("Offset")]
    public Vector3 localOffset = new Vector3(0f, 0.1f, -0.1f);
    public Vector3 worldOffset = new Vector3(0f, 0.1f, -0.1f);

    [Header("Camera Facing")]
    public Transform cameraTransform;

    [Header("Motion")]
    public float followSpeed = 10.0f;

    private Vector3 targetPosition;

    void Update()
    {
        if (handTransform == null) return;

        if (trackingMode == TrackMode.Local)
        {
            targetPosition = handTransform.position
                            + handTransform.right * localOffset.x
                            + handTransform.up * localOffset.y
                            + handTransform.forward * localOffset.z;
        }
        else if (trackingMode == TrackMode.Global)
        {
            targetPosition = handTransform.position + worldOffset;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
        if (cameraTransform != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
        }

    }
}
