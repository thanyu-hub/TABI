using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MLPVisualization : MonoBehaviour
{
    /* ─────────── 0. UI SLIDERS ─────────── */
    [Header("UI Controls")]
    public Slider densitySlider;       // ▶ drag “Density” slider here


    [Header("Generator")]
    public Data data;  // drag blob/anisotropic/moon GO here

    /* ─────────── 2. DATASET SETTINGS ─────────── */
    [Header("Dataset")]
    public float pointScale = 0.1f;
    public float depthOffset = -60f;

    /* ─────────── 3. CANVAS TARGET ─────────── */
    [Header("World-Space Canvas")]
    public Transform targetCanvas;
    [Min(0f)] public float canvasMargin = 10f;

    /* ─────────── 4. MLP SETTINGS ─────────── */
    [Header("MLP Settings")]
    public float mlpLearningRate = 0.1f;

    /* ─────────── 5. PREFABS ─────────── */
    [Header("Prefabs")]
    public List<GameObject> classPrefabs;  // assign in Inspector
    /* ─────────── RUNTIME FIELDS ─────────── */
    public List<Transform> pointMarkers;
    public List<Vector3> pointPositions;
    public List<double[]> inputs = new List<double[]>();
    public List<double> targets = new List<double>();
    public double currentLoss=999;
    public BookManager bookManager;
    public bool isInit = false;


    /* ─────────── UI HOOKS ─────────── */
    public void SamplePoints() => CreateDataset();
    public void RunOneIteration() => TrainOneStep();
    public void ClearScene() => _ClearScene();

    void CreateDataset()
    {
        _ClearScene();
        inputs.Clear();
        targets.Clear();

        List<Sample> samples = data.samples;

        foreach (var s in samples)
        {
            var lp = s.position;
            inputs.Add(new double[] { lp.x, lp.y, lp.z });
            targets.Add(s.label);
        }
        pointMarkers = data.pointMarkers;
        AssignPoints();
        isInit = true;
    }

    void TrainOneStep()
    {
        MLP mlp = GetComponent<MLP>();
        Debug.Log($"inputs.Count = {inputs.Count}");
        Debug.Log($"mlp = null {mlp == null}");
        if (mlp == null || inputs.Count == 0) return;
        double loss = 0;
        Debug.Log($"1loss = {loss}");
        // one-step SGD on current sample
        int epochs = 100;
        for (int ep = 0; ep < epochs; ep++)
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                loss += mlp.TrainOneSample(inputs[i], targets[i], mlpLearningRate);
            }
        }
        loss = loss / (epochs * inputs.Count);
        Debug.Log($"2loss = {loss}");
        AssignPoints();
        currentLoss = loss;
        bookManager.ShowPage(bookManager.currentPage, currentLoss);
    }

    public void AssignPoints()
    {
        MLP mlp = GetComponent<MLP>();
        for (int i = 0; i < pointMarkers.Count; i++)
        {
            double pred = mlp.Predict(inputs[i]);
            int label = pred >= 0.5 ? 1 : 0;
            pointMarkers[i].GetComponent<Renderer>().material.color = ClusterColor(label);
        }
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
        GetComponent<MLP>().Reset();
        for (int i = 0; i < pointMarkers.Count; i++)
        {
            pointMarkers[i].GetComponent<Renderer>().material.color = Color.gray;
        }
    }
}
