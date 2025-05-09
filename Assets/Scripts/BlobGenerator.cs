using System;
using System.Collections.Generic;
using UnityEngine;

public class BlobGenerator : PointGenerator
{
    public int numBlobs = 4;
    public int pointsPerBlob = 100;
    public float stdDev = 0.5f;

    public override List<Sample> GetPoints()
    {
        var pts = new List<Sample>();
        var rnd = new System.Random();
        // generate centers
        var centers = new Vector3[numBlobs];
        for (int i = 0; i < numBlobs; i++)
            centers[i] = new Vector3(
                (float)rnd.NextDouble(),
                (float)rnd.NextDouble(),
                (float)rnd.NextDouble()
            );
        // sample around each center
        for (int i = 0; i < numBlobs; i++)
            for (int j = 0; j < pointsPerBlob; j++)
            {
                double x = SampleGaussian(rnd, centers[i].x, stdDev);
                double y = SampleGaussian(rnd, centers[i].y, stdDev);
                double z = SampleGaussian(rnd, centers[i].z, stdDev);
                pts.Add(new Sample(new Vector3((float)x, (float)y, (float)z), i));
            }

        return ScaleToRange(pts, -0.5f, 0.5f);

    }

    
    double SampleGaussian(System.Random rand, double mean, double sd)
    {
        double u1 = 1.0 - rand.NextDouble();
        double u2 = 1.0 - rand.NextDouble();
        double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return mean + sd * z;
    }
    public override void SetPointsPerBlob(int perGaussian)
    {
        pointsPerBlob = perGaussian;
    }
}
