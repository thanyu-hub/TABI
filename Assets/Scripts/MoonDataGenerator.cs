using System;
using System.Collections.Generic;
using UnityEngine;

public class MoonGenerator : PointGenerator
{
    public int pointsPerBlob = 200;
    public float radius = 1f;
    public float offsetY = 0.05f;
    public float noise = 0.1f;

    public override List<Sample> GetPoints()
    {
        var pts = new List<Sample>();
        var rnd = new System.Random();
        // top half-circle
        for (int i = 0; i < pointsPerBlob; i++)
        {
            double θ = Math.PI * rnd.NextDouble();
            double x = radius * Math.Cos(θ) + SampleGaussian(rnd, 0, noise);
            double y = radius * Math.Sin(θ) + SampleGaussian(rnd, 0, noise);
            pts.Add(new Sample(new Vector3((float)x, (float)y, 0f), 0));
        }
        // bottom half-circle
        for (int i = 0; i < pointsPerBlob; i++)
        {
            double θ = Math.PI * rnd.NextDouble();
            double x = radius * Math.Cos(θ) + radius + SampleGaussian(rnd, 0, noise);
            double y = -radius * Math.Sin(θ) - offsetY + SampleGaussian(rnd, 0, noise);
            pts.Add(new Sample(new Vector3((float)x, (float)y, 0f), 1));
        }
        return ScaleToRange(pts, -0.5f, 0.5f);
    }

    double SampleGaussian(System.Random rand, double mean, double sd)
    {
        double u1 = 1.0 - rand.NextDouble();
        double u2 = 1.0 - rand.NextDouble();
        double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
        return mean + sd * z;
    }

    public override void SetPointsPerBlob(int perGaussian)
    {
        pointsPerBlob = perGaussian;
    }
}
