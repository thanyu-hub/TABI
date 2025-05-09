using System.Collections.Generic;
using UnityEngine;

// put this in PointGenerator.cs
public abstract class PointGenerator : MonoBehaviour
{
    // Return a list of (x,y) samples in local canvas space
    public abstract List<Sample> GetPoints();
    public abstract void SetPointsPerBlob(int perGaussian);

    protected List<Sample> ScaleToRange(List<Sample> pts, float targetMin, float targetMax)
    {
        // 3. Find the actual min and max values for each dimension across all points
        float minX = float.PositiveInfinity, maxX = float.NegativeInfinity;
        float minY = float.PositiveInfinity, maxY = float.NegativeInfinity;
        float minZ = float.PositiveInfinity, maxZ = float.NegativeInfinity;

        foreach (var sample in pts)
        {
            Vector3 pos = sample.position;
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
            if (pos.z < minZ) minZ = pos.z;
            if (pos.z > maxZ) maxZ = pos.z;
        }

        // 4. Define target range and calculate current ranges
        float targetRange = targetMax - targetMin; // This will be 1.0f

        float currentRangeX = maxX - minX;
        float currentRangeY = maxY - minY;
        float currentRangeZ = maxZ - minZ;

        // Use a small epsilon to handle cases where all points have the same coordinate (range is zero)
        const float epsilon = 1e-6f;

        // 5. Apply scaling to each point
        var rescaledPts = new List<Sample>(pts.Count); // Create a new list for scaled points
        foreach (var sample in pts)
        {
            Vector3 oldPos = sample.position;
            float newX, newY, newZ;

            // Scale X coordinate
            if (Mathf.Abs(currentRangeX) < epsilon)
            {
                // If all X values are the same, map them to the center of the target range (0.0)
                newX = targetMin + targetRange / 2.0f;
            }
            else
            {
                // Apply Min-Max Scaling formula: New = T_min + (Old - O_min) * (T_max - T_min) / (O_max - O_min)
                newX = targetMin + (oldPos.x - minX) * targetRange / currentRangeX;
            }

            // Scale Y coordinate
            if (Mathf.Abs(currentRangeY) < epsilon)
            {
                newY = targetMin + targetRange / 2.0f;
            }
            else
            {
                newY = targetMin + (oldPos.y - minY) * targetRange / currentRangeY;
            }

            // Scale Z coordinate
            if (Mathf.Abs(currentRangeZ) < epsilon)
            {
                newZ = targetMin + targetRange / 2.0f;
            }
            else
            {
                newZ = targetMin + (oldPos.z - minZ) * targetRange / currentRangeZ;
            }

            // Add the rescaled point to the new list, keeping the original label
            rescaledPts.Add(new Sample(new Vector3(newX, newY, newZ), sample.label));
        }

        // --- End Min-Max Scaling ---

        return rescaledPts; // Return the list containing the rescaled points
    }
}

public struct Sample
{
    public Vector3 position;
    public int label;     // cluster ID

    public Sample(Vector3 pos, int lbl)
    {
        position = pos;
        label = lbl;
    }
}