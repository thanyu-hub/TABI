using UnityEngine;

public class GifToggle : MonoBehaviour
{
    public GameObject gifPanel;

    public void ToggleGif()
    {
        if (gifPanel != null)
        {
            bool currentlyActive = gifPanel.activeSelf;
            gifPanel.SetActive(!currentlyActive);
            Debug.Log($"[GifToggle] gifPanel is now {(gifPanel.activeSelf ? "Active" : "Inactive")}");
        }
        else
        {
            Debug.LogWarning("[GifToggle] gifPanel is not assigned.");
        }
    }
}
