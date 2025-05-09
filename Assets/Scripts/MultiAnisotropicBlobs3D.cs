using System;
using System.Collections.Generic;
using UnityEngine;

public class AnisotropicGenerator3D : PointGenerator
{
    public int numBlobs = 2;
    //public Vector3[] centers;               // set length = numBlobs
    //public Matrix3x3[] transforms;          // parallel array

    public int pointsPerBlob = 100;
    public float stdDev = 1f;
    public override List<Sample> GetPoints()
    {
        var pts = new List<Sample>();
        System.Random rand = new System.Random();

        // 3D centers 
        double[][] centers = new double[][]
        {
        new double[] { 1, 3, -1 },
        new double[] { -1, 3, -1 }
        };

        // 3x3 transformation matrices 
        double[][,] transforms = new double[][,]
        {
        new double[,] { { 0.1, 0.0, 0.0 }, { 0.0, 4.0, 0.0 }, { 0.0, 0.0, 6.0 } },
        new double[,] { { 0.1, 0.0, 0.0 }, { 0.0, 4.0, 0.0 }, { 0.0, 0.0, 6.0 } }
        };

        for (int blob = 0; blob < numBlobs; blob++)
        {
            var center = centers[blob];
            var transform = transforms[blob];

            for (int i = 0; i < pointsPerBlob-1; i++)
            {
                // Sample isotropic Gaussian in 3D 
                double x = SampleGaussian(rand, 0, stdDev);
                double y = SampleGaussian(rand, 0, stdDev);
                double z = SampleGaussian(rand, 0, stdDev);

                // Apply 3x3 affine transform 
                double x_new = transform[0, 0] * x + transform[0, 1] * y + transform[0, 2] * z + center[0];
                double y_new = transform[1, 0] * x + transform[1, 1] * y + transform[1, 2] * z + center[1];
                double z_new = transform[2, 0] * x + transform[2, 1] * y + transform[2, 2] * z + center[2];

                // Output 3D point (optionally include label) 
                pts.Add(new Sample(new Vector3((float)x_new, (float)y_new, (float)z_new), blob));
            }
            if (blob == 0)
            {
                pts.Add(new Sample(new Vector3((float)4, (float)3, (float)-1), blob));
            }
            if (blob == 1)
            {
                pts.Add(new Sample(new Vector3((float)-4, (float)3, (float)-1), blob));
            }
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
        pointsPerBlob = perGaussian;
    }
}

// helper struct for inspector
[Serializable]
public struct Matrix3x3
{
    public float m00, m01, m02, m10, m11, mp12,m20,m21,m22;
}