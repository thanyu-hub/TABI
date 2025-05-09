using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
//using UnityEngine.UIElements;

public class Data : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject blob;
    public GameObject moon;
    public GameObject aniso;
    public Toggle blobTog;
    public Toggle moonTog;
    public Toggle anisoTog;
    public List<Sample> samples;
    public List<Transform> pointMarkers = new();
    public List<Vector3> pointPositions = new();
    public List<Vector3> pointPositionsBefore = new();
    [Header("World‑Space Canvas")]
    public Transform targetCanvas;
    [Header("Prefabs")]
    public Transform pointPrefab;
    public float pointScale = 0.1f;
    public Slider densitySlider;       // ▶ drag “Density” slider here
    public List<GameObject> classPrefabs;  // assign in Inspector
    public GameObject bookManagerCanvas;
    public GameObject graphObject;
    void Awake()
    {
        /* ▶ wire slider callbacks once */
        densitySlider.onValueChanged.AddListener(_ => SyncUISliders());

        SyncUISliders();          // set initial values
    }

    /* ───────────────────────────────────────────── */
    void SyncUISliders()
    {
        int perGaussian = Mathf.RoundToInt(densitySlider.value) * 2;   // uniform density
        if (blobTog.isOn)
        {
            blob.GetComponent<BlobGenerator>().SetPointsPerBlob(perGaussian);
        }
        if (moonTog.isOn)
        {
            moon.GetComponent<MoonGenerator3D>().SetPointsPerBlob(perGaussian);
        }
        if (anisoTog.isOn)
        {
            aniso.GetComponent<AnisotropicGenerator3D>().SetPointsPerBlob(perGaussian);
        }

    }
    void DeactivateAllAlgorithms()
    {
        graphObject.GetComponent<ScatterPoints>().enabled   = false;
        graphObject.GetComponent<SVMCanvasVisualizer>().enabled = false;
        graphObject.GetComponent<MLPVisualization>().enabled = false;
    }

    public void Sample()
{
    // 1. まず全アルゴリズムを停止
    DeactivateAllAlgorithms();

    // 2. UI 値を反映
    SyncUISliders();

    // 3. トグルに応じて必ず点群を生成（ページに依存しない）
    if (blobTog.isOn)   GenerateBlob();
    if (moonTog.isOn)   GenerateMoon();
    if (anisoTog.isOn)  GenerateAniso();

    // 4. ページに合わせて、該当アルゴリズムだけを有効化＆実行
    int page = bookManagerCanvas.GetComponent<BookManager>().currentPage;
    if (page == 0)
    {
        var kmeans = graphObject.GetComponent<ScatterPoints>();
        kmeans.enabled = true;
        if (kmeans.centroids != null)
        {
            kmeans.pointMarkers = pointMarkers;
            kmeans.pointPositions = pointPositions;
            kmeans.assignments = new int[pointPositions.Count];
            kmeans.AssignPoints();
        }
    }
    else if (page == 1)
    {
        var svm = graphObject.GetComponent<SVMCanvasVisualizer>();
        svm.enabled = true;
        if (svm.decisionLine)
        {
            svm.positions.Clear();
            svm.labels.Clear();
            foreach (var s in samples)
            {
                svm.positions.Add(new Vector2(s.position.x, s.position.y));
                svm.labels.Add(s.label <= 0 ? -1 : 1);
            }
            svm.points = pointMarkers;
            svm.UpdateColors();
            svm.UpdateLine();
        }
    }
    else // page == 2
    {
        var mlp = graphObject.GetComponent<MLPVisualization>();
        mlp.enabled = true;
        if (mlp.isInit)
        {
            mlp.inputs.Clear();
            mlp.targets.Clear();
            foreach (var s in samples)
            {
                var p = s.position;
                mlp.inputs.Add(new double[] { p.x, p.y, p.z });
                mlp.targets.Add(s.label);
            }
            mlp.pointMarkers = pointMarkers;
            mlp.AssignPoints();
        }
    }
}

    public void GenerateBlob()
    {
        samples = blob.GetComponent<PointGenerator>().GetPoints();
        _ClearScene();
        pointMarkers.Clear();
        pointPositions.Clear();

        foreach (var ls in samples)
        {
            var lp = ls.position;
            pointPositionsBefore.Add(lp);
            Vector3 world = targetCanvas.TransformPoint(new Vector3(lp.x, lp.y, lp.z));
            pointPositions.Add(world);
            Debug.Log($"world {world}");
            var go = Instantiate(classPrefabs[ls.label], world, Quaternion.identity, targetCanvas);
            go.transform.localScale = Vector3.one * pointScale;
            pointMarkers.Add(go.transform);
        }
    }


    public void GenerateMoon()
    {
        samples = moon.GetComponent<PointGenerator>().GetPoints();
        _ClearScene();
        pointMarkers.Clear();
        pointPositions.Clear();

        foreach (var ls in samples)
        {
            var lp = ls.position;
            pointPositionsBefore.Add(lp);
            Vector3 world = targetCanvas.TransformPoint(new Vector3(lp.x, lp.y, lp.z));
            pointPositions.Add(world);
            Debug.Log($"world {world}");
            var go = Instantiate(classPrefabs[ls.label], world, Quaternion.identity, targetCanvas);
            go.transform.localScale = Vector3.one * pointScale;
            pointMarkers.Add(go.transform);
        }
    }
    public void GenerateAniso()
    {
        samples = aniso.GetComponent<PointGenerator>().GetPoints();
        _ClearScene();
        pointMarkers.Clear();
        pointPositions.Clear();

        foreach (var ls in samples)
        {
            var lp = ls.position;
            pointPositionsBefore.Add(lp);
            Vector3 world = targetCanvas.TransformPoint(new Vector3(lp.x, lp.y, lp.z));
            pointPositions.Add(world);
            Debug.Log($"world {world}");
            var go = Instantiate(classPrefabs[ls.label], world, Quaternion.identity, targetCanvas);
            go.transform.localScale = Vector3.one * pointScale;
            pointMarkers.Add(go.transform);
        }
    }

    void _ClearScene()
    {
        foreach (var t in pointMarkers) if (t) Destroy(t.gameObject);
    }
}
