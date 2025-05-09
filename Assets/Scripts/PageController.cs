using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PageController : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public List<string> pages;
    private int currentPage = 0;

    void Start()
    {
        Debug.Log("PageController started.");
        ShowPage(0);
    }

    public void ShowPage(int index)
    {
        if (index < 0 || index >= pages.Count)
        {
            Debug.LogWarning($"Invalid page index: {index}");
            return;
        }
        currentPage = index;
        textDisplay.text = pages[currentPage];
        Debug.Log($"Showing page {currentPage}: {pages[currentPage]}");
    }

    public void PreviousPage()
    {
        Debug.Log("Navigating to the previous page.");
        ShowPage(currentPage - 1);
    }

    public void NextPage()
    {
        Debug.Log("Navigating to the next page.");
        ShowPage(currentPage + 1);
    }
}
