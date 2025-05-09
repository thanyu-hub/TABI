using UnityEngine;
using System.Collections.Generic;

using UnityEngine.UI;
using System;      // ▶ needed for Slider
public class SVMCanvasVisualizer : MonoBehaviour
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
    public List<Vector3> positions = new List<Vector3>();
    public List<int> labels = new List<int>();        // +1 or -1
    public Vector3 w;
    public float b;
    public GameObject decisionLine;

    private Material lineMaterial;

    public void Sample()
    {
        // Regenerate dataset and reset SVM parameters
        ClearScene();
        w = new Vector3(1f, 1f, 0f);  // Reset to initial vertical line
        b = 0f;
        GeneratePointsOnCanvas();

        if (!decisionLine)
            CreateLine();

        UpdateLine();
    }

    public void StepOne() => StartCoroutine(StepOne1());
    public System.Collections.IEnumerator StepOne1()
    {
        yield return StartCoroutine(VisualizeAssignmentsTemporarily());
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

        w = new Vector3(1f, 1f, 0f);
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

        Vector3 canvasHalf = Vector3.one * 0.5f;

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
            positions.Add(new Vector3(x, y, z));

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
            Vector3 x = positions[i];
            int y = labels[i];
            float margin = y * (Vector3.Dot(w, x) - b);
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
            float score = Vector3.Dot(w, positions[i]) - b;
            points[i].GetComponent<Renderer>().material.color = score >= 0f ? Color.red : Color.green;
        }
    }
    void CreateLine()
    {
        decisionLine = Instantiate(linePrefab, targetCanvas);
        UpdateLine();  // Call immediately to ensure correct initial position
    }
    System.Collections.IEnumerator VisualizeAssignmentsTemporarily()
    {
        /* 0. housekeeping */
        List<GameObject> lines = new List<GameObject>();
        Vector3 n = w.normalized;                // plane normal
        float invMag = 1f / w.magnitude;            // 1 / ‖w‖

        /* 1. create a two-vertex LineRenderer for every point */
        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 p = positions[i];                          // sample
            float d = (Vector3.Dot(w, p) - b) * invMag;           // signed distance
            Vector3 foot = p - d * n;                                  // projection on plane
                                                                       // alternative: Vector3 foot = Vector3.ProjectOnPlane(p, n) + n * (b / w.magnitude);  :contentReference[oaicite:0]{index=0}

            GameObject lineObj = new GameObject($"Point_{i}_Drop");
            lineObj.transform.SetParent(targetCanvas);

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.SetPosition(0, targetCanvas.TransformPoint(p));
            lr.SetPosition(1, targetCanvas.TransformPoint(foot));                                    // :contentReference[oaicite:1]{index=1}

            lr.startWidth = lr.endWidth = 0.001f;

            Color c = labels[i] > 0 ? Color.red : Color.green;          // reuse class color
            lr.startColor = lr.endColor = c;

            // fallback material if prefab not supplied
            // lr.material = new Material(
            //     Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));

            if (lineMaterial == null)
            {
                lineMaterial = new Material(Shader.Find("Unlit/Color"));
            }

            lr.material = lineMaterial;
            lr.material.color = c;

            lines.Add(lineObj);
        }

        /* 2. keep them visible for a second */
        yield return new WaitForSeconds(1.0f);

        /* 3. clean-up */
        foreach (var g in lines) Destroy(g);
    }
    public void UpdateLine()
    {
        Vector3 canvasHalf = Vector3.one * 0.5f;
        // 1. Unit normal
        Vector3 n = w.normalized;

        // 2. Projection of the origin onto the plane
        Vector3 center = targetCanvas.TransformPoint(n * (b / w.magnitude));

        // 3. Rotate default plane (+Y up) so that +Y aligns with n
        decisionLine.transform.rotation = Quaternion.FromToRotation(Vector3.up, n);   // docs  :contentReference[oaicite:0]{index=0}

        // 4. Position the plane’s center
        decisionLine.transform.position = center;

        // 5. Scale the 10 m × 10 m Unity plane so that the visible square is 2 × halfExtent
        decisionLine.transform.localScale = new Vector3(1.2f, 0.05f, 1.2f);
    }


    void ClearScene()
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i].GetComponent<Renderer>().material.color = Color.gray;
        }
    }

}
