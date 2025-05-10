using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class SimulatorRotator : MonoBehaviour
{
    public Transform simulatorRoot;
    public Transform rotatorHandle;
    public HandGrabInteractable interactable;

    private bool isGrabbing = false;
    private Vector3 initialHandlePos;
    private Quaternion initialRotation;

    private void Update()
    {
        // 掴まれているかどうかをチェック
        bool currentlyGrabbing = interactable.Interactors.Count > 0;
        if (currentlyGrabbing && !isGrabbing)
        {
            OnGrabStart();
        }
        else if (!currentlyGrabbing && isGrabbing)
        {
            OnGrabEnd();
        }

        if (isGrabbing)
        {
            UpdateRotation();
        }
    }

    private void OnGrabStart()
    {
        isGrabbing = true;
        initialHandlePos = rotatorHandle.position;
        initialRotation = simulatorRoot.rotation;
    }

    private void OnGrabEnd()
    {
        isGrabbing = false;
    }

    private void UpdateRotation()
    {
        Vector3 delta = rotatorHandle.position - initialHandlePos;
        float movementAlongZ = Vector3.Dot(delta, simulatorRoot.forward);

        float rawAngle = movementAlongZ * 180f;
        float snappedAngle = Mathf.Round(rawAngle / 15f) * 15f;

        Quaternion xRot = Quaternion.AngleAxis(snappedAngle, Vector3.right);
        simulatorRoot.rotation = initialRotation * xRot;
    }
}
