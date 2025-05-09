using UnityEngine;

public class SurfaceInteractable : MonoBehaviour
{
    public GameObject previewPrefab;
    public GameObject placedPrefab;

    private GameObject previewInstance;
    private bool isDragging = false;
    private Vector3 startPoint;
    private Vector3 surfaceNormal;
    private Transform selectingTransform;

    public void StartDrag(Transform interactorTransform)
    {
        selectingTransform = interactorTransform;
        isDragging = true;

        if (previewPrefab != null)
        {
            previewInstance = Instantiate(previewPrefab);
        }

        Debug.Log("[SurfaceInteractable] Start Drag");
    }

    public void EndDrag(Transform interactorTransform)
    {
        if (!isDragging)
            return;

        if (placedPrefab != null && previewInstance != null)
        {
            Vector3 placementPos = previewInstance.transform.position;
            Quaternion placementRot = previewInstance.transform.rotation;
            Vector3 placementScale = previewInstance.transform.localScale;

            Instantiate(placedPrefab, placementPos, placementRot).transform.localScale = placementScale;
        }

        Destroy(previewInstance);
        isDragging = false;
        selectingTransform = null;

        Debug.Log("[SurfaceInteractable] End Drag");
    }

    private void Update()
    {
        if (isDragging && selectingTransform != null && previewInstance != null)
        {
            Ray ray = new Ray(selectingTransform.position, selectingTransform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, 10f))
            {
                UpdatePreview(hit.point, hit.normal);
            }
        }
    }

    private void UpdatePreview(Vector3 point, Vector3 normal)
    {
        previewInstance.transform.position = point;
        previewInstance.transform.rotation = Quaternion.LookRotation(normal);
        previewInstance.transform.localScale = new Vector3(0.2f, 0.2f, 0.01f); // Example
    }
}
