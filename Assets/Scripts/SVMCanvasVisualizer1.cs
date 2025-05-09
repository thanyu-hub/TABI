using UnityEngine;
using System.Collections.Generic;

using UnityEngine.UI;
using System;      // ▶ needed for Slider
public class SVMCanvasVisualizerBackup : MonoBehaviour
{
    [Header("Canvas Settings")]
    public Transform targetCanvas;

    [Header("Generator")]
    public Data data;  // drag blob/anisotropic/moon GO here
    [Header("SVM Hyperparameters")]
    public float learningRate = 0.02f;


    public GameObject linePrefab;

    public float lineThickness = 0.01f;  // Radius of the cylinder

    public List<Transform> points ;
    public List<Vector2> positions = new List<Vector2>();
    public List<int> labels = new List<int>();        // +1 or -1
    public Vector2 w;
    public float b;
    public GameObject decisionLine;

    public void Sample()
    {
        // Regenerate dataset and reset SVM parameters
        ClearScene();
        w = new Vector2(1f, 0f);  // Reset to initial vertical line
        b = 0f;
        GeneratePointsOnCanvas();

        if (!decisionLine)
            CreateLine();

        UpdateLine();
    }

    public void StepOne()
    {
        StepSVM();
        UpdateLine();
    }
    public void ResetCanvas()
    {
        ClearScene();
        positions.Clear();
        labels.Clear();

        if (decisionLine)
            Destroy(decisionLine);
        decisionLine = null;

        w = new Vector2(1f, 0f);
        b = 0f;
    }
    void Start()
    {
    }

    void Update()
    {
    }

    void GeneratePointsOnCanvas()
    {
        ClearScene();
        positions.Clear();
        labels.Clear();

        Vector2 canvasHalf = Vector2.one * 0.5f;

        int clusterIndex = 0;  // To alternate labels per cluster
        List<Sample> samples = data.samples;
        foreach (var ls in samples)
        {
            // Assign label based on cluster (alternate labels)
            int clusterLabel = ls.label;
            clusterIndex++;
            float x = ls.position.x;
            float y = ls.position.y;
            float z = ls.position.z;
            positions.Add(new Vector2(x, y));

            labels.Add(clusterLabel <= 0? -1 : 1);

        }
        points = data.pointMarkers;
        UpdateColors();
    }

    void StepSVM()
    {
        int N = points.Count;
        for (int i = 0; i < N; i++)
        {
            Vector2 x = positions[i];
            int y = labels[i];
            float margin = y * (Vector2.Dot(w, x) - b);
            if (margin < 1f)  // hinge loss. 
            {
                w -= learningRate * (-y * x);
                b -= learningRate * (y);
            }
        }
        UpdateColors();
    }
    public void UpdateColors()
    {
        for (int i = 0; i < points.Count; i++)
        {
            float score = Vector2.Dot(w, positions[i]) - b;
            points[i].GetComponent<Renderer>().material.color = score >= 0f ? Color.red : Color.green;
        }
    }
    void CreateLine()
    {
        decisionLine = Instantiate(linePrefab, targetCanvas);
        UpdateLine();  // Call immediately to ensure correct initial position
    }

    public void UpdateLine()
    {
        const float eps = 1e-6f;
        bool isVertical = Mathf.Abs(w.y) < eps;
        Vector2 half = Vector2.one * 0.5f;
        Vector2 pA, pB;
        if (isVertical)
        {
            float x = -b / w.x;
            pA = new Vector2(x, -half.y);
            pB = new Vector2(x, half.y);
        }
        else
        {
            float m = -w.x / w.y;
            float c = b / w.y;
            pA = new Vector2(-half.x, m * -half.x + c);
            pB = new Vector2(half.x, m * half.x + c);
        }

        Vector3 wA = new Vector3(
            Mathf.Clamp(pA.x, -half.x,  half.x ),
            Mathf.Clamp(pA.y, -half.y,  half.y ),
            0f
        );
        Vector3 wB = new Vector3(
            Mathf.Clamp(pB.x, -half.x,  half.x ),
            Mathf.Clamp(pB.y, -half.y,  half.y ),
            0f
        );

        Vector3 p1 = targetCanvas.TransformPoint(wA);
        Vector3 p2 = targetCanvas.TransformPoint(wB);

        Vector3 dir = p2 - p1;
        float length = dir.magnitude;
        Vector3 mid = p1 + dir * 0.5f;

        decisionLine.transform.position = mid;
        decisionLine.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir.normalized);
        decisionLine.transform.localScale = new Vector3(
            lineThickness,
            1f,
            1f
        );

    }


    void ClearScene()
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i].GetComponent<Renderer>().material.color = Color.gray;
        }
    }

}
