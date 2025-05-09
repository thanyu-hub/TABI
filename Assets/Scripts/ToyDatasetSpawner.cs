using System;
using UnityEngine;

public class ToyDatasetSpawner : MonoBehaviour
{
    [Header("General")]
    [SerializeField] int pointsPerCluster = 100;
    [SerializeField] float stdDev = 0.5f;
    [SerializeField] GameObject pointPrefab;   // assign a low-poly sphere
    [SerializeField] bool useAnisotropic = false;
    [SerializeField] bool useNoisyMoons = false;

    [Header("VR safety")]
    [SerializeField] float scale = 0.1f;       // world-space scale so clusters fit in view
    [SerializeField] Transform parent;         // optional container for hierarchy tidy-up

    readonly System.Random rnd = new System.Random();

    void Start()
    {
        if (useNoisyMoons) SpawnNoisyMoons();
        else SpawnBlobs(useAnisotropic);
    }

    /* ---------- 1. Isotropic or anisotropic blobs ---------- */
    void SpawnBlobs(bool anisotropic)
    {
        var centers = new Vector2[] { new(-3, 3), new(3, 3), new(-3, -3), new(3, -3) };
        var T = new[,] { { 1.5, 0.6 }, { 0.3, 0.8 } };            // same for every blob for demo

        for (int c = 0; c < centers.Length; ++c)
            for (int i = 0; i < pointsPerCluster; ++i)
            {
                double x = SampleGaussian(0, stdDev);
                double y = SampleGaussian(0, stdDev);

                if (anisotropic)
                {
                    double tmpX = T[0, 0] * x + T[0, 1] * y;
                    double tmpY = T[1, 0] * x + T[1, 1] * y;
                    x = tmpX; y = tmpY;
                }

                Vector3 pos = new((float)(x + centers[c].x), 0, (float)(y + centers[c].y));
                SpawnPoint(pos * scale, c);
            }
    }

    /* ---------- 2. Noisy moons ---------- */
    void SpawnNoisyMoons()
    {
        int n = pointsPerCluster;
        double r = 1.0, off = 0.5, noise = 0.1;

        // upper moon
        for (int i = 0; i < n; ++i)
        {
            double t = rnd.NextDouble() * Math.PI;
            double x = r * Math.Cos(t) + SampleGaussian(0, noise);
            double y = r * Math.Sin(t) + SampleGaussian(0, noise);
            SpawnPoint(new Vector3((float)x, 0, (float)y) * scale, 0);
        }

        // lower moon
        for (int i = 0; i < n; ++i)
        {
            double t = rnd.NextDouble() * Math.PI;
            double x = r * Math.Cos(t) + r * 0.5 + SampleGaussian(0, noise);
            double y = -r * Math.Sin(t) - off + SampleGaussian(0, noise);
            SpawnPoint(new Vector3((float)x, 0, (float)y) * scale, 1);
        }
    }

    /* ---------- utilities ---------- */
    double SampleGaussian(double mean, double sd)
    {
        double u1 = 1.0 - rnd.NextDouble();
        double u2 = 1.0 - rnd.NextDouble();
        double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return mean + sd * z;
    }

    void SpawnPoint(Vector3 pos, int label)
    {
        var go = Instantiate(pointPrefab, pos, Quaternion.identity, parent);
        var mat = go.GetComponent<Renderer>().material;
        mat.color = Color.HSVToRGB((label * 0.25f) % 1f, 0.7f, 0.9f);  // simple coloring
    }
}
