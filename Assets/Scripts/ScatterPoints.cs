using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;      // ▶ needed for Slider

public class ScatterPoints : MonoBehaviour
{
    /* ─────────── 0.  UI SLIDERS  ─────────── */
    [Header("UI Controls")]
    public Slider centroidsSlider;     // ▶ drag “Centroids” slider here

    [Header("Generator")]
    public Data data;  // drag blob/anisotropic/moon GO here

    /* ─────────── 1. DATASET SETTINGS ─────────── */
    [Header("Dataset")]
    [Min(1)] public int numberOfClusters = 3;          // overwritten by slider
    public float pointScale = 0.1f;


    /* ─────────── 2. CANVAS TARGET ─────────── */
    [Header("World‑Space Canvas")]
    public Transform targetCanvas;

    /* ─────────── 3. K‑MEANS SETTINGS ─────────── */
    [Header("K‑Means")]
    [Min(1)] public int maxIterations = 20;
    public float stepDelay = 1f;

    /* ─────────── 4. PREFABS ─────────── */
    [Header("Prefabs")]
    public Transform pointPrefab;
    public Transform centroidPrefab;

    /* ─────────── RUNTIME FIELDS ─────────── */
    public List<Transform> pointMarkers ;
    public List<Vector3> pointPositions;
    public int[] assignments;
    public Vector3[] centroids;
    Transform[] centroidMarkers;
    public float lineThickness = 0.5f;

    /* ─────────── UI HOOKS ─────────── */
    public void SamplePoints() => CreateDataset();
    public void RunOneIteration() => StartCoroutine(StepKMeans());
    public void RunUntilConverge() => StartCoroutine(AutoRun());
    public void ClearScene() => _ClearScene();              // expose to button

    private bool isRunning = false;

    private Material lineMaterial;

    void Awake()
    {
        /* ▶ wire slider callbacks once */
        centroidsSlider.onValueChanged.AddListener(_ => SyncUISliders());

        SyncUISliders();          // set initial values
    }

    /* ───────────────────────────────────────────── */
    #region UI → DATA  SYNC
    void SyncUISliders()
    {
        numberOfClusters = Mathf.RoundToInt(centroidsSlider.value);
    }
    #endregion

    /* ───────────────────────────────────────────── */
    #region DATASET  &  INITIAL CENTROIDS
    void CreateDataset()
    {
        _ClearScene();
        pointMarkers = data.pointMarkers;
        pointPositions = data.pointPositions;

        centroids = new Vector3[numberOfClusters];
        centroidMarkers = new Transform[numberOfClusters];
        assignments = new int[pointPositions.Count];

        for (int k = 0; k < numberOfClusters; k++)
        {
            Vector3 c = pointPositions[Random.Range(0, pointPositions.Count)];
            centroids[k] = c;

            var t = Instantiate(centroidPrefab, c, Quaternion.identity, targetCanvas);
            t.localScale = Vector3.one * pointScale * 2f;
            centroidMarkers[k] = t;
        }

        AssignPoints();
        UpdateCentroids();
    }
    #endregion

    /* ─────────── K‑MEANS CORE (unchanged) ─────────── */
    IEnumerator StepKMeans()
    {
        isRunning = true;
        yield return StartCoroutine(VisualizeAssignmentsTemporarily());
        AssignPoints();
        UpdateCentroids();
    }

    IEnumerator VisualizeAssignmentsTemporarily()
    {

        List<GameObject> lines = new List<GameObject>();

        // Create Line Renderers
        for (int j = 0; j < centroids.Length; j++)
        {
            for (int i = 0; i < pointPositions.Count; i++)
            {

                GameObject lineObj = new GameObject($"Point_{i}_Line");
                lineObj.transform.SetParent(targetCanvas); // Optional: Keep hierarchy clean
                LineRenderer lr = lineObj.AddComponent<LineRenderer>();

                lr.positionCount = 2;
                lr.SetPosition(0, pointPositions[i]);
                lr.SetPosition(1, centroids[j]);

                lr.startWidth = lineThickness;
                lr.endWidth = lineThickness;

                Color clusterColor = ClusterColor(j);
                lr.startColor = clusterColor;
                lr.endColor = clusterColor;


                // Create a basic material if none is assigned
                if (lineMaterial == null)
                {
                    lineMaterial = new Material(Shader.Find("Unlit/Color"));
                }

                lr.material = lineMaterial;
                lr.material.color = clusterColor; // Apply color to the default material
                Debug.LogWarning("Line Material not assigned in Inspector. Creating a default material.");


                lines.Add(lineObj);
            }
            yield return new WaitForSeconds(0.5f);
            foreach (var line in lines)
            {
                Destroy(line);
            }
            lines.Clear();
        }
    }
    public void AssignPoints()
    {

        Debug.LogWarning("AssignPoints called");

        for (int i = 0; i < pointPositions.Count; i++)
        {
            float best = float.MaxValue;
            int bestK = 0;
            for (int k = 0; k < numberOfClusters; k++)
            {
                float d = Vector3.SqrMagnitude(pointPositions[i] - centroids[k]);
                if (d < best) { best = d; bestK = k; }
            }
            assignments[i] = bestK;
            pointMarkers[i].GetComponent<Renderer>().material.color = ClusterColor(bestK);
        }
    }

    void UpdateCentroids()
    {
        for (int k = 0; k < numberOfClusters; k++)
        {
            var members = Enumerable.Range(0, pointPositions.Count)
                                    .Where(i => assignments[i] == k)
                                    .Select(i => pointPositions[i]).ToList();
            if (members.Count == 0) continue;

            Vector3 mean = Vector3.zero;
            foreach (var v in members) mean += v;
            mean /= members.Count;

            centroids[k] = mean;
            centroidMarkers[k].position = mean;
        }
    }

    /* ─────────── GRAB → LIVE UPDATE  ─────────── */
    void LateUpdate()
    {
        if (!isRunning) return; // skip if not running
        Debug.LogError("ScatterPoints:");

        bool moved = false;
        for (int k = 0; k < centroidMarkers?.Length; k++)
        {
            if (!centroidMarkers[k]) continue;

            Vector3 pos = centroidMarkers[k].position;
            if (pos != centroids[k]) { centroids[k] = pos; moved = true; }
        }
        for (int k = 0; k < pointMarkers?.Count; k++)
        {
            if (!pointMarkers[k]) continue;

            Vector3 pos = pointMarkers[k].position;
            if (pos != pointPositions[k]) { pointPositions[k] = pos; moved = true; }
        }
        if (moved) AssignPoints();    // recompute only when a centroid moved
    }

    /* ─────────── HELPERS ─────────── */
    static Color ClusterColor(int id)
    {
        Color[] palette =
        {
            Color.red, Color.green, Color.blue,
            Color.yellow, Color.cyan, Color.magenta
        };
        return palette[id % palette.Length];
    }

    void _ClearScene()
    {
        isRunning = false;
        for (int i = 0; i < pointPositions.Count; i++)
        {
            pointMarkers[i].GetComponent<Renderer>().material.color = Color.gray;
        }
        if (centroidMarkers != null)
            foreach (var t in centroidMarkers) if (t) Destroy(t.gameObject);
    }

    /* ─────────── AUTO‑RUN ─────────── */
    System.Collections.IEnumerator AutoRun()
    {
        int currentIter = 0;
        while (currentIter < maxIterations)
        {
            StepKMeans();
            currentIter++;
            yield return new WaitForSeconds(stepDelay);
        }
    }
}
