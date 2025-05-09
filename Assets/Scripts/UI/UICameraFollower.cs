using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UICameraFollower : MonoBehaviour
{
    public Transform handTransform;
    public Vector3 localOffset = new Vector3(0f, 0.1f, -0.1f);
    public float followSpeed = 10.0f;
    public float fadeDuration = 0.5f;
    public float moveDistance = 0.2f;

    [Header("Camera Facing")]
    public Transform cameraTransform;

    private CanvasGroup canvasGroup;
    private Vector3 targetPosition;

    private LeftHandTextManager script;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        var tutorialObj = transform.parent.Find("tutorial");
        if (tutorialObj != null)
        {
            script = tutorialObj.GetComponent<LeftHandTextManager>();
        }
        else
        {
            Debug.LogError("LeftHandTextManager script not found in the parent object.");
        }
    }


    private void Update()
    {
        if (handTransform == null) return;

        targetPosition = ComputeTargetPosition();
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
        if (cameraTransform != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
        }
    }

    private Vector3 ComputeTargetPosition()
    {
        return handTransform.position
             + handTransform.right * localOffset.x
             + handTransform.up * localOffset.y
             + handTransform.forward * localOffset.z;
    }

    public void ShowUI()
    {
        gameObject.SetActive(true);
        script.SetTextState(LeftHandTextManager.State.UI);
        StartCoroutine(AnimateIn());
    }

    public void HideUI()
    {
        StartCoroutine(AnimateOut(() =>
        {
            // script?.SetTextState(LeftHandTextManager.State.Default);
            gameObject.SetActive(false);
        }));
    }

    private IEnumerator AnimateIn()
    {
        float elapsed = 0f;
        Vector3 finalPos = ComputeTargetPosition();
        Vector3 startPos = finalPos + Vector3.up * moveDistance;

        transform.position = startPos;
        canvasGroup.alpha = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            transform.position = Vector3.Lerp(startPos, finalPos, t);
            canvasGroup.alpha = t;

            yield return null;
        }

        transform.position = finalPos;
        canvasGroup.alpha = 1f;
    }

    private IEnumerator AnimateOut(System.Action onComplete)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * moveDistance;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            transform.position = Vector3.Lerp(startPos, endPos, t);
            canvasGroup.alpha = 1f - t;

            yield return null;
        }

        transform.position = endPos;
        canvasGroup.alpha = 0f;
        onComplete?.Invoke();
    }
}
