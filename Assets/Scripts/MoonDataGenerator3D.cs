using System;
using System.Collections.Generic;
using UnityEngine;

public class MoonGenerator3D : PointGenerator
{
    public int samplesPerClass = 200;
    public double noise = 0.1; public double radius = 1.0; public double offset = -0.5f;
    public override List<Sample> GetPoints()
    {
        var pts = new List<Sample>();
         // vertical separation between moons 

        System.Random rand = new System.Random();

        // Top moon (label 0) 
        for (int i = 0; i < samplesPerClass; i++)
        {
            double theta = Math.PI * rand.NextDouble(); // [0, π] 
            double x = radius * Math.Cos(theta);
            double y = radius * Math.Sin(theta);
            double z = radius * Math.Sin(2 * theta); // <-- creates a 3D arc 

            x += SampleGaussian(rand, 0, noise);
            y += SampleGaussian(rand, 0, noise);
            z += SampleGaussian(rand, 0, noise);

            pts.Add(new Sample(new Vector3((float)x, (float)y, (float)z), 0));
        }

        // Bottom moon (label 1) 
        for (int i = 0; i < samplesPerClass; i++)
        {
            double theta = Math.PI * rand.NextDouble(); // [0, π] 
            double x = radius * Math.Cos(theta) + radius * 0.5;
            double y = -radius * Math.Sin(theta) - offset;
            double z = -radius * Math.Sin(2 * theta); // <-- mirrored 3D arc 

            x += SampleGaussian(rand, 0, noise);
            y += SampleGaussian(rand, 0, noise);
            z += SampleGaussian(rand, 0, noise);
            pts.Add(new Sample(new Vector3((float)x, (float)y, (float)z), 1));

        }

        return ScaleToRange(pts, -0.5f, 0.5f);
    }

    static double SampleGaussian(System.Random rand, double mean, double stdDev)
    {
        double u1 = 1.0 - rand.NextDouble();
        double u2 = 1.0 - rand.NextDouble();
        double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
        return mean + stdDev * z;
    }
    public override void SetPointsPerBlob(int perGaussian)
    {
        samplesPerClass = perGaussian;
    }

}
