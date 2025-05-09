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
        // 掴んだ後にどれだけY方向に動かしたかを見る（ローカル空間でもOK）
        Vector3 delta = rotatorHandle.position - initialHandlePos;
        float movementAlongZ = Vector3.Dot(delta, simulatorRoot.forward); // 手前/奥方向の動き

        // 回転角を決定（感度は適宜調整）
        float rawAngle = movementAlongZ * 180f; // 例えば Z方向に1m動いたら180°
        float snappedAngle = Mathf.Round(rawAngle / 15f) * 15f;

        // X軸だけ回転
        Quaternion xRot = Quaternion.AngleAxis(snappedAngle, Vector3.right);
        simulatorRoot.rotation = initialRotation * xRot;
    }
}
