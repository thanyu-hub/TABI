using UnityEngine;
using Meta.XR.MRUtilityKit; // Important: for MRUKAnchor
using System.Collections;

public class SurfaceManager : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject previewPrefab; // Assign in Inspector
    public GameObject placedPrefab;  // Assign in Inspector

    [Header("Setup Settings")]
    public float waitBeforeSetup = 5.0f; // How long to wait before trying to setup (seconds)

    private void Start()
    {
        StartCoroutine(SetupAfterDelay());
    }

    private IEnumerator SetupAfterDelay()
    {
        yield return new WaitForSeconds(waitBeforeSetup);

        Debug.Log("[SurfaceManager] Starting setup...");

        var anchors = FindObjectsByType<MRUKAnchor>(FindObjectsSortMode.None);

        foreach (var anchor in anchors)
        {
            SetupSurface(anchor.gameObject);
        }

        Debug.Log($"[SurfaceManager] Setup complete. {anchors.Length} surfaces processed.");
    }

    private void SetupSurface(GameObject surface)
    {
        if (surface.GetComponent<SurfaceInteractable>() == null)
        {
            SurfaceInteractable interactable = surface.AddComponent<SurfaceInteractable>();

            // Assign prefabs
            interactable.previewPrefab = previewPrefab;
            interactable.placedPrefab = placedPrefab;
        }

        if (surface.GetComponent<Collider>() == null)
        {
            // Add a simple BoxCollider if missing
            surface.AddComponent<BoxCollider>();
        }
    }
}
