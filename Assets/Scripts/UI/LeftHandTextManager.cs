using TMPro;
using UnityEngine;

public class LeftHandTextManager : MonoBehaviour
{
    [Header("UI Settings")]
    public TextMeshProUGUI textMeshPro;

    public enum State{
        Default,
        UI,
        RayCastMode,
        None
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }

        // Set the initial state of the text to default
        SetTextState(State.Default);
    }

    public void SetTextState(State state)
    {
        switch (state)
        {
            case State.Default:
                textMeshPro.text = "Thumbs-up gesture with your left hand opens the menu.";
                textMeshPro.color = Color.white;
                break;
            case State.UI:
                textMeshPro.text = "'Define Workspace' places the UI and Simulator into the scene.";
                textMeshPro.color = Color.yellow;
                break;
            case State.RayCastMode:
                textMeshPro.text = "Pinch to start from your preferred location in the blue area, drag diagonally, then release to define your workspace.";
                textMeshPro.color = Color.green;
                break;
            case State.None:
                textMeshPro.text = string.Empty; // Clear the text
                break;
        }
    }
    public void SetText(string text)
    {
        textMeshPro.text = text;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
