using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Text.RegularExpressions;
using System.Linq;

public class GifSwitcher : MonoBehaviour
{
    public Image gifTarget;
    public Button playPauseButton;
    public TMP_Text algorithmLabel;

    public float frameRate = 10f;

    private List<Sprite> frames = new List<Sprite>();
    private Coroutine playCoroutine;
    private bool isPlaying = false;
    private int currentIndex = 0;
    public int lastPage = -1;

    public int currentPage = 0;

    void Start()
    {
        Debug.Log("[GifSwitcher] Start() called");
        LoadGifByPage(0);

        if (playPauseButton != null)
        {
            playPauseButton.onClick.AddListener(TogglePlayPause);
            Debug.Log("[GifSwitcher] Play/Pause button listener attached.");
        }
        else
        {
            Debug.LogWarning("[GifSwitcher] playPauseButton is not assigned!");
        }
    }

    public void LoadGifByPage(int page)
    {
        Debug.Log($"[GifSwitcher] LoadGifByPage({page}) called");

        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning("[GifSwitcher] Skipped loading because GameObject is inactive.");
            return;
        }

        if (page == lastPage)
        {
            Debug.Log("[GifSwitcher] Page unchanged, skipping reload.");
            return;
        }

        lastPage = page;

        if (playCoroutine != null)
        {
            Debug.Log("[GifSwitcher] Stopping existing coroutine.");
            StopCoroutine(playCoroutine);
            playCoroutine = null;
        }

        string folderName;
        string algorithmName;

        switch (page)
        {
            case 0: folderName = "GIF_KMeans"; algorithmName = "K-Means Clustering"; break;
            case 1: folderName = "GIF_SVM"; algorithmName = "Support Vector Machine"; break;
            case 2: folderName = "GIF_MLP"; algorithmName = "Multi-Layer Perceptron"; break;
            default: folderName = "GIF_KMeans"; algorithmName = "K-Means Clustering"; break;
        }

        Debug.Log($"[GifSwitcher] Folder selected: {folderName}, Algorithm: {algorithmName}");

        if (algorithmLabel != null)
        {
            algorithmLabel.text = algorithmName;
            Debug.Log("[GifSwitcher] Algorithm label updated.");
        }
        else
        {
            Debug.LogWarning("[GifSwitcher] algorithmLabel is not assigned!");
        }

        frames.Clear();

        Sprite[] loadedSprites = Resources.LoadAll<Sprite>(folderName);

        Debug.Log($"[GifSwitcher] Raw loaded sprite count: {loadedSprites.Length}");

        frames.AddRange(
            loadedSprites.OrderBy(sprite =>
            {
                Match m = Regex.Match(sprite.name, @"\d+");
                int index = m.Success ? int.Parse(m.Value) : 0;
                Debug.Log($"[GifSwitcher] Sprite '{sprite.name}' parsed as index {index}");
                return index;
            })
        );

        Debug.Log($"[GifSwitcher] Sorted and added {frames.Count} frames from {folderName}");

        currentIndex = 0;

        if (frames.Count == 0)
        {
            Debug.LogWarning($"[GifSwitcher] No frames found in: {folderName}");
            return;
        }

        gifTarget.sprite = frames[0];
        Debug.Log("[GifSwitcher] Initial frame set to gifTarget.");
    }
    void OnEnable()
    {
        Debug.Log("[GifSwitcher] OnEnable called. Reloading current page.");
        LoadGifByPage(currentPage);
    }

    public void TogglePlayPause()
    {
        Debug.Log($"[GifSwitcher] TogglePlayPause() called. isPlaying = {isPlaying}");

        if (isPlaying)
        {
            if (playCoroutine != null)
            {
                StopCoroutine(playCoroutine);
                Debug.Log("[GifSwitcher] Playback paused.");
            }
            playCoroutine = null;
            isPlaying = false;
        }
        else
        {
            if (currentIndex >= frames.Count)
            {
                Debug.Log("[GifSwitcher] Reached end, restarting from beginning.");
                currentIndex = 0;
            }

            isPlaying = true;
            playCoroutine = StartCoroutine(PlayGif());
            Debug.Log("[GifSwitcher] Playback started.");
        }
    }


    IEnumerator PlayGif()
    {
        Debug.Log("[GifSwitcher] PlayGif coroutine started.");

        while (currentIndex < frames.Count)
        {
            gifTarget.sprite = frames[currentIndex];
            Debug.Log($"[GifSwitcher] Showing frame {currentIndex}/{frames.Count}");

            currentIndex++;
            yield return new WaitForSeconds(1f / frameRate);
        }

        Debug.Log("[GifSwitcher] Playback reached end of frames.");

        isPlaying = false;
        playCoroutine = null;
    }
}
