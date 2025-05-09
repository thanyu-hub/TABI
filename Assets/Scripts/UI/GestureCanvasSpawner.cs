using UnityEngine;
using Meta.XR.MRUtilityKit;
public class GestureCanvasSpawner : MonoBehaviour
{
    public GameObject uiCanvasPrefab;
    private GameObject spawnedCanvas;
    public Transform playerCamera;

    public Transform rayStartPoint;

    public EffectMesh roomGuardianEffectMesh;

    public GameObject panelObject;
    public Transform canvasPanelTransform;

    public GameObject simulatorObject;

    public GameObject gifSwitcher;

    // public void SpawnCanvas()
    // {
    //     if (spawnedCanvas != null) return;

    //     spawnedCanvas = Instantiate(uiCanvasPrefab);

    //     UICameraFollower follow = spawnedCanvas.GetComponent<UICameraFollower>();
    //     if (follow != null)
    //     {
    //         follow.cameraTransform = playerCamera;
    //     }

    //     UIFadeInSlide fade = spawnedCanvas.GetComponent<UIFadeInSlide>();
    //     if (fade != null)
    //     {
    //         fade.Play();
    //     }

    //     MRUKRaycaster raycaster = spawnedCanvas.GetComponentInChildren<MRUKRaycaster>();
    //     if (raycaster != null)
    //     {
    //         raycaster.rayStartPoint = rayStartPoint;
    //         raycaster.roomGuardianEffectMesh = roomGuardianEffectMesh;
    //         raycaster.panelObject = panelObject;
    //         raycaster.canvasPanelTransform = canvasPanelTransform;
    //         raycaster.simulatorObject = simulatorObject;
    //     }
    // }

    public void CloseCanvas()
    {
        if (spawnedCanvas != null)
        {

            MRUKRaycaster raycaster = spawnedCanvas.GetComponentInChildren<MRUKRaycaster>();
            if (raycaster != null)
            {
                raycaster.deActivateRayCastMode();
            }

            UIFadeInSlide fade = spawnedCanvas.GetComponent<UIFadeInSlide>();
            if (fade != null)
            {
                fade.PlayFadeOut(() =>
                {
                    Destroy(spawnedCanvas);
                });
            }
            else
            {
                Destroy(spawnedCanvas);
            }
        }
    }
}
