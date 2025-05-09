using System.IO;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class BookManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform visualsRoot;
    public GameObject textBlockPrefab;

    [Header("Book Data")]
    public string jsonFileName = "book";

    private PageData[] pages;
    public int currentPage = 0;

    private float lastPageChangeTime = -1f;
    public float pageChangeCooldown = 0.1f;

    public GifSwitcher gifSwitcher;

    void Start()
    {
        StartCoroutine(LoadJson());
    }

    IEnumerator LoadJson()
    {
       TextAsset jsonAsset = Resources.Load<TextAsset>(jsonFileName);
        if (jsonAsset == null)
        {
            yield break;
        }
        string json = jsonAsset.text;
        pages = JsonHelper.FromJson<PageData>(json);

        ShowPage(0);
    }

    public void ShowPage(int index)
    {
        Debug.Log($"ShowPage: {index}");
        if (pages == null || index < 0 || index >= pages.Length)
        {
            return;
        }

        currentPage = index;

        foreach (Transform child in visualsRoot)
            Destroy(child.gameObject);

        CreateTextBlock(pages[index].title, 36);

        string formattedBody = pages[index].body.Replace("\n\n", "\n");
        CreateTextBlock(formattedBody, 20);
        if (gifSwitcher != null)
            Debug.Log($"GifSwitcher: {gifSwitcher}");
            gifSwitcher.currentPage = currentPage;
            gifSwitcher.LoadGifByPage(currentPage);
    }
    public void ShowPage(int index, double loss)
    {
        if (pages == null || index < 0 || index >= pages.Length) return;
        currentPage = index;

        // clear old content
        foreach (Transform child in visualsRoot)
            Destroy(child.gameObject);

        // title
        CreateTextBlock(pages[index].title, 36);

        // replace <loss> and normalize line breaks
        string body = pages[index].body
            .Replace("<loss>", loss.ToString("F2"));

        CreateTextBlock(body, 20);

        if (gifSwitcher != null)
            Debug.Log($"GifSwitcher: {gifSwitcher}");
            gifSwitcher.LoadGifByPage(currentPage);
    }
    private void CreateTextBlock(string content, float fontSize)
    {
        if (textBlockPrefab == null) return;

        GameObject go = Instantiate(textBlockPrefab, visualsRoot);
        TMP_Text tmp = go.GetComponentInChildren<TMP_Text>();
        if (tmp == null) return;

        tmp.text = content;
        tmp.fontSize = fontSize;
    }

    public void NextPage()
    {
        // Need to check if the cooldown has passed before changing the page for distant raycast bug (multiple hits)
        if (Time.time - lastPageChangeTime < pageChangeCooldown) return;
        lastPageChangeTime = Time.time;
        ShowPage(currentPage + 1);

    }

    public void PrevPage()
    {
            // Need to check if the cooldown has passed before changing the page for distant raycast bug (multiple hits)

        if (Time.time - lastPageChangeTime < pageChangeCooldown) return;
        lastPageChangeTime = Time.time;
        ShowPage(currentPage - 1);
    }
}

[System.Serializable]
public class PageData
{
    public string title;
    public string body;
}
