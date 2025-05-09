using UnityEngine;
using System.Collections.Generic;

public class SVMVisualizer : MonoBehaviour
{
    public int numberOfPoints = 100;
    public float spaceSize = 5f;
    public float learningRate = 0.02f;
    public float pointScale = 0.2f;
    public GameObject pointPrefab;
    public GameObject linePrefab;
    public float updateInterval = 0.1f;

    private List<GameObject> points = new List<GameObject>();
    private List<Vector2> positions = new List<Vector2>();
    private List<int> labels = new List<int>(); // +1 or -1

    private Vector2 w = new Vector2(0.1f, 0.1f);  // Initial weights
    private float b = 0f;                         // Initial bias

    private GameObject decisionLine;
    private GameObject marginPlusLine;
    private GameObject marginMinusLine;

    private float timer = 0f;

    void Start()
    {
        GeneratePoints();
        CreateLines();
        UpdateLines();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            StepSVM();       // one step of optimization
            UpdateLines();   // redraw lines
        }
    }

    void GeneratePoints()
    {
        for (int i = 0; i < numberOfPoints; i++)
        {
            Vector2 pos = new Vector2(
                Random.Range(-spaceSize, spaceSize),
                Random.Range(-spaceSize, spaceSize)
            );

            // Label manually using a hidden true hyperplane
            Vector2 trueW = new Vector2(1f, -1f);
            float trueB = 0f;
            float label = Mathf.Sign(Vector2.Dot(trueW, pos) + trueB);

            labels.Add((int)label);
            positions.Add(pos);

            GameObject p = Instantiate(pointPrefab);
            p.transform.localPosition = new Vector3(pos.x, pos.y, 0f);
            p.transform.localScale = Vector3.one * pointScale;
            p.GetComponent<Renderer>().material.color = label > 0 ? Color.red : Color.blue;

            points.Add(p);
        }
    }

    void StepSVM()
    {
        for (int i = 0; i < numberOfPoints; i++)
        {
            Vector2 x = positions[i];
            int y = labels[i];

            float margin = y * (Vector2.Dot(w, x) + b);

            if (margin < 1f) // hinge loss activated
            {
                w = w - learningRate * (-y * x);
                b = b - learningRate * (-y);
                HighlightPoint(i, Color.yellow);
            }
            else
            {
                HighlightPoint(i, labels[i] > 0 ? Color.red : Color.blue);
            }
        }
    }

    void HighlightPoint(int i, Color color)
    {
        points[i].GetComponent<Renderer>().material.color = color;
    }

    void CreateLines()
    {
        decisionLine = Instantiate(linePrefab);
        marginPlusLine = Instantiate(linePrefab);
        marginMinusLine = Instantiate(linePrefab);

        foreach (var line in new[] { decisionLine, marginPlusLine, marginMinusLine })
        {
            LineRenderer lr = line.GetComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.startWidth = 0.04f;
            lr.endWidth = 0.04f;
        }
    }

    void UpdateLines()
    {
        DrawSVMLine(decisionLine, 0f, Color.black);
        DrawSVMLine(marginPlusLine, 1f, Color.gray);
        DrawSVMLine(marginMinusLine, -1f, Color.gray);
    }

    void DrawSVMLine(GameObject lineObject, float offset, Color color)
    {
        LineRenderer lr = lineObject.GetComponent<LineRenderer>();
        lr.material.color = color;

        float x1 = -spaceSize;
        float x2 = spaceSize;

        Vector2 wDir = w.normalized;
        float m = -w.x / w.y;
        float c = -(b + offset) / w.y;

        float y1 = m * x1 + c;
        float y2 = m * x2 + c;

        lr.SetPosition(0, new Vector3(x1, y1, 0.01f));
        lr.SetPosition(1, new Vector3(x2, y2, 0.01f));
    }
}
