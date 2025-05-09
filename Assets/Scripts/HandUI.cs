using UnityEngine;

public class MiddleThumbSwipeUIOpener : MonoBehaviour
{
    public OVRHand hand;
    public float swipeThreshold = 0.2f; // Move at least 20cm
    public float maxSwipeTime = 0.5f;    // Complete swipe within 0.5 seconds

    private Vector3 startSwipePos;
    private float swipeStartTime;
    private bool isTrackingSwipe = false;

    void Update()
    {
        if (hand == null || hand.PointerPose == null)
            return;

        bool thumbPinching = hand.GetFingerIsPinching(OVRHand.HandFinger.Thumb);
        bool middlePinching = hand.GetFingerIsPinching(OVRHand.HandFinger.Middle);

        Vector3 currentPos = hand.PointerPose.position;

        if (!isTrackingSwipe)
        {
            if (thumbPinching && middlePinching)
            {
                startSwipePos = currentPos;
                swipeStartTime = Time.time;
                isTrackingSwipe = true;
                Debug.Log("[MiddleThumbSwipeUIOpener] Pinch detected, ready to start swipe");
            }
        }
        else
        {
            if (!(thumbPinching && middlePinching))
            {
                Debug.Log("[MiddleThumbSwipeUIOpener] Pinch released, swipe canceled");
                isTrackingSwipe = false;
                return;
            }

            float elapsed = Time.time - swipeStartTime;
            float verticalMovement = startSwipePos.y - currentPos.y;

            Debug.Log($"[MiddleThumbSwipeUIOpener] Swiping: time {elapsed:F2}s, movement {verticalMovement:F3}m");

            if (elapsed > maxSwipeTime)
            {
                Debug.Log("[MiddleThumbSwipeUIOpener] Timeout, swipe canceled");
                isTrackingSwipe = false;
                return;
            }

            if (verticalMovement > swipeThreshold)
            {
                Debug.Log("[MiddleThumbSwipeUIOpener] Success! Downward swipe detected with thumb and middle finger!");
                OpenUI();
                isTrackingSwipe = false;
            }
        }
    }

    void OpenUI()
    {
        Debug.Log("[MiddleThumbSwipeUIOpener] Displaying UI!");
        // Add code here to activate the UI Canvas, etc.
    }
}
