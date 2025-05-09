using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PageManager : MonoBehaviour
{
    public Button nextButton;
    public Button previousButton;

    // Sliders to disable
    public GameObject densitySlider;
    public GameObject centroidSlider;

    // Buttons to change functions
    public Button initButton;
    public Button stepButton;
    public Button resetButton;

    // References to your scripts
    public GameObject graphObject;
    public Data data;
    public GameObject bookManagerCanvas;

    public GameObject simulator;


    void Start()
    {
        nextButton.onClick.AddListener(SwitchPage);
        previousButton.onClick.AddListener(SwitchPage);
        SwitchPage();
    }

    IEnumerator DelayedDeactivate(float delay)
    {
        yield return new WaitForSeconds(delay);
        simulator.SetActive(false);
    }

    void SwitchPage()
    {
        if (bookManagerCanvas.GetComponent<BookManager>().currentPage == 1)
        {
            // Disable sliders
            centroidSlider.SetActive(false);

            // Clear old listeners
            initButton.onClick.RemoveAllListeners();
            stepButton.onClick.RemoveAllListeners();
            resetButton.onClick.RemoveAllListeners();
            var kmeans = graphObject.GetComponent<ScatterPoints>();
            kmeans.ClearScene();
            var mlp = graphObject.GetComponent<MLPVisualization>();
            mlp.ClearScene();
            // Call methods from SVMCanvasVisualizer
            var svm = graphObject.GetComponent<SVMCanvasVisualizer>();
            initButton.onClick.AddListener(() => svm.Sample());
            stepButton.onClick.AddListener(() => svm.StepOne());
            resetButton.onClick.AddListener(() =>
            {
                svm.ResetCanvas();
                StartCoroutine(DelayedDeactivate(delay: 0.5f));
            });

        }
        else
        {
            if (bookManagerCanvas.GetComponent<BookManager>().currentPage == 0)
            {
                // Re-enable sliders
                centroidSlider.SetActive(true);

                // Clear old listeners
                initButton.onClick.RemoveAllListeners();
                stepButton.onClick.RemoveAllListeners();
                resetButton.onClick.RemoveAllListeners();
                var svm = graphObject.GetComponent<SVMCanvasVisualizer>();
                svm.ResetCanvas();
                // Call methods from ScatterPoints
                var kmeans = graphObject.GetComponent<ScatterPoints>();
                initButton.onClick.AddListener(() => kmeans.SamplePoints());
                stepButton.onClick.AddListener(() => kmeans.RunOneIteration());
                resetButton.onClick.AddListener(() =>
                {
                    kmeans.ClearScene();
                    StartCoroutine(DelayedDeactivate(kmeans.centroids.Length * 0.5f));
                });
            }
            else
            {
                centroidSlider.SetActive(false);

                // Clear old listeners
                initButton.onClick.RemoveAllListeners();
                stepButton.onClick.RemoveAllListeners();
                resetButton.onClick.RemoveAllListeners();
                var svm = graphObject.GetComponent<SVMCanvasVisualizer>();
                svm.ResetCanvas();
                // Call methods from ScatterPoints
                var mlp = graphObject.GetComponent<MLPVisualization>();
                initButton.onClick.AddListener(() => mlp.SamplePoints());
                stepButton.onClick.AddListener(() => mlp.RunOneIteration());
                resetButton.onClick.AddListener(() =>
                {
                    mlp.ClearScene();
                    StartCoroutine(DelayedDeactivate(delay: 0.5f));
                });

            }
        }
    }
}
