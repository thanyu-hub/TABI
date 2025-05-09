using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class MLP : MonoBehaviour
{
    private int inputSize = 3;
    private List<int> layerSizes;
    private List<double[,]> weights;
    private List<double[]> biases;
    private System.Random rnd = new System.Random();
    public int[] hiddenLayers;
    public void Start()
    {
        layerSizes = new List<int> { inputSize };
        layerSizes.AddRange(hiddenLayers);
        layerSizes.Add(1); // Output layer (1 neuron for binary classification)

        weights = new List<double[,]>();
        biases = new List<double[]>();

        for (int i = 0; i < layerSizes.Count - 1; i++)
        {
            int inSize = layerSizes[i];
            int outSize = layerSizes[i + 1];
            var w = new double[inSize, outSize];
            var b = new double[outSize];

            for (int j = 0; j < inSize; j++)
                for (int k = 0; k < outSize; k++)
                    w[j, k] = rnd.NextDouble() * 2 - 1;

            for (int k = 0; k < outSize; k++)
                b[k] = rnd.NextDouble() * 2 - 1;

            weights.Add(w);
            biases.Add(b);
        }
    }

    public void Reset()
    {
        layerSizes = new List<int> { inputSize };
        layerSizes.AddRange(hiddenLayers);
        layerSizes.Add(1); // Output layer (1 neuron for binary classification)

        weights = new List<double[,]>();
        biases = new List<double[]>();

        for (int i = 0; i < layerSizes.Count - 1; i++)
        {
            int inSize = layerSizes[i];
            int outSize = layerSizes[i + 1];
            var w = new double[inSize, outSize];
            var b = new double[outSize];

            for (int j = 0; j < inSize; j++)
                for (int k = 0; k < outSize; k++)
                    w[j, k] = rnd.NextDouble() * 2 - 1;

            for (int k = 0; k < outSize; k++)
                b[k] = rnd.NextDouble() * 2 - 1;

            weights.Add(w);
            biases.Add(b);
        }
    }
    private double Sigmoid(double x) => 1.0 / (1.0 + Math.Exp(-x));
    private double SigmoidDerivative(double x) => x * (1 - x);

    public double Predict(double[] input)
    { 
        double[] output = input;

        for (int l = 0; l < weights.Count; l++)
        {
            double[] next = new double[layerSizes[l + 1]];

            for (int j = 0; j < next.Length; j++)
            {
                next[j] = biases[l][j];
                for (int i = 0; i < output.Length; i++)
                    next[j] += output[i] * weights[l][i, j];
                next[j] = Sigmoid(next[j]);
            }

            output = next;
        }

        return output[0];
    }

    public double TrainOneSample(double[] input, double target, double lr = 0.1)
    {
        
        // Forward pass
        List<double[]> activations = new List<double[]>();
        activations.Add(input);

        for (int l = 0; l < weights.Count; l++)
        {
            double[] prev = activations[activations.Count - 1];
            double[] next = new double[layerSizes[l + 1]];

            for (int j = 0; j < next.Length; j++)
            {
                next[j] = biases[l][j];
                for (int i = 0; i < prev.Length; i++)
                    next[j] += prev[i] * weights[l][i, j];
                next[j] = Sigmoid(next[j]);
            }

            activations.Add(next);
        }

        double output = activations[activations.Count - 1][0];
        double error = target - output;
        double loss = error * error;
        // Backward pass
        List<double[]> deltas = new List<double[]>();
        double[] delta = new double[1];
        delta[0] = error * SigmoidDerivative(output);
        deltas.Insert(0, delta);

        for (int l = weights.Count - 2; l >= 0; l--)
        {
            double[] nextDelta = new double[layerSizes[l + 1]];
            double[] act = activations[l + 1];

            for (int i = 0; i < nextDelta.Length; i++)
            {
                double sum = 0;
                for (int j = 0; j < deltas[0].Length; j++)
                    sum += weights[l + 1][i, j] * deltas[0][j];
                nextDelta[i] = sum * SigmoidDerivative(act[i]);
            }

            deltas.Insert(0, nextDelta);
        }

        // Update weights and biases
        for (int l = 0; l < weights.Count; l++)
        {
            double[] act = activations[l];
            for (int i = 0; i < act.Length; i++)
                for (int j = 0; j < deltas[l].Length; j++)
                    weights[l][i, j] += lr * deltas[l][j] * act[i];

            for (int j = 0; j < deltas[l].Length; j++)
                biases[l][j] += lr * deltas[l][j];
        }
        return loss;
    }
    public void Train(double[][] inputs, double[] targets, int epochs = 1000, double lr = 0.1)
    {
        for (int epoch = 0; epoch < epochs; epoch++)
        {
            double loss = 0;

            for (int sample = 0; sample < inputs.Length; sample++)
            {
                double[] input = inputs[sample];
                double target = targets[sample];

                // Forward pass
                List<double[]> activations = new List<double[]>();
                activations.Add(input);

                for (int l = 0; l < weights.Count; l++)
                {
                    double[] prev = activations[activations.Count - 1];
                    double[] next = new double[layerSizes[l + 1]];

                    for (int j = 0; j < next.Length; j++)
                    {
                        next[j] = biases[l][j];
                        for (int i = 0; i < prev.Length; i++)
                            next[j] += prev[i] * weights[l][i, j];
                        next[j] = Sigmoid(next[j]);
                    }

                    activations.Add(next);
                }

                double output = activations[activations.Count - 1][0];
                double error = target - output;
                loss += error * error;

                // Backward pass
                List<double[]> deltas = new List<double[]>();
                double[] delta = new double[1];
                delta[0] = error * SigmoidDerivative(output);
                deltas.Insert(0, delta);

                for (int l = weights.Count - 2; l >= 0; l--)
                {
                    double[] nextDelta = new double[layerSizes[l + 1]];
                    double[] act = activations[l + 1];

                    for (int i = 0; i < nextDelta.Length; i++)
                    {
                        double sum = 0;
                        for (int j = 0; j < deltas[0].Length; j++)
                            sum += weights[l + 1][i, j] * deltas[0][j];
                        nextDelta[i] = sum * SigmoidDerivative(act[i]);
                    }

                    deltas.Insert(0, nextDelta);
                }

                // Update weights and biases
                for (int l = 0; l < weights.Count; l++)
                {
                    double[] act = activations[l];
                    for (int i = 0; i < act.Length; i++)
                        for (int j = 0; j < deltas[l].Length; j++)
                            weights[l][i, j] += lr * deltas[l][j] * act[i];

                    for (int j = 0; j < deltas[l].Length; j++)
                        biases[l][j] += lr * deltas[l][j];
                }
            }

        }
    }

    public double EvaluateAccuracy(double[] outputs, double[] targets)
    {
        int correct = 0;

        for (int i = 0; i < outputs.Length; i++)
        {
            double prediction = outputs[i];
            int predictedLabel = prediction >= 0.5 ? 1 : 0;
            int trueLabel = (int)targets[i];

            if (predictedLabel == trueLabel)
                correct++;
        }

        return (double)correct / outputs.Length;
    }
}

