using UnityEngine;
using System.Collections;

public class UIFadeInSlide : MonoBehaviour
{
    public float fadeDuration = 0.5f;
    public float moveDistance = 0.2f;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Play()
    {
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.localPosition + Vector3.up * moveDistance;
        Vector3 targetPos = transform.localPosition;

        transform.localPosition = startPos;
        canvasGroup.alpha = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            canvasGroup.alpha = t;

            yield return null;
        }

        transform.localPosition = targetPos;
        canvasGroup.alpha = 1f;
    }

    public void PlayFadeOut(System.Action onComplete = null)
    {
        StartCoroutine(AnimateFadeOut(onComplete));
    }

    public void ShowUI()
    {
        gameObject.SetActive(true);
        Play();
    }

    public void HideUI()
    {
        PlayFadeOut(() => gameObject.SetActive(false));
    }

    private IEnumerator AnimateFadeOut(System.Action onComplete)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = startPos + Vector3.up * moveDistance;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            canvasGroup.alpha = 1f - t;

            yield return null;
        }

        transform.localPosition = endPos;
        canvasGroup.alpha = 0f;

        onComplete?.Invoke();
    }
}
