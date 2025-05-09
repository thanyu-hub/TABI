using TMPro;
using UnityEngine;

public class TurorialTextManager : MonoBehaviour
{
    public TextMeshProUGUI textComponent;

    public void ShowMessage(string message)
    {
        if (textComponent != null)
        {
            textComponent.text = message;
        }
    }

    public void ThumbsUpShowMessage(string message)
    {
        if (textComponent != null)
        {
            // textComponent.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
            textComponent.text = message;
        }
    }

    public void ClearMessage()
    {
        if (textComponent != null)
        {
            // textComponent.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
            textComponent.text = "While giving a thumbs-up gesture, the menu remains open.";
        }
    }
}
