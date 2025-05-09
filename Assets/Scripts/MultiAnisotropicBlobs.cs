using System;
using System.Collections.Generic;
using UnityEngine;

public class AnisotropicGenerator : PointGenerator
{
    public Vector2[] centers;               // set length = numBlobs
    public Matrix2x2[] transforms;          // parallel array

    public int pointsPerBlob = 100;
    public float stdDev = 1f;


    public override List<Sample> GetPoints()
    {
        var pts = new List<Sample>();
        var rnd = new System.Random();
        for (int b = 0; b < centers.Length; b++)
            for (int i = 0; i < pointsPerBlob; i++)
            {
                float x = (float) SampleGaussian(rnd, 0, stdDev);
                float y = (float) SampleGaussian(rnd, 0, stdDev);
                // apply anisotropic 2×2
                var M = transforms[b];
                float xn = M.m00 * x + M.m01 * y + centers[b].x;
                float yn = M.m10 * x + M.m11 * y + centers[b].y;
                pts.Add(new Sample(new Vector3(xn, yn, 0f), b));
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

// helper struct for inspector
[Serializable]
public struct Matrix2x2
{
    public float m00, m01, m10, m11;
}