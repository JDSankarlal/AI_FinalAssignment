using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{

    private int[] layers;
    private float[][] neurons;
    private float[][][] weights;

    List<float[]> neuronsList = new List<float[]>();
    List<float[][]> weightsList = new List<float[][]>();
    List<float[]> layerWList = new List<float[]>();

    int backupPerceptronLayer;
    float[] perceptronWeights;

    float randVal;
    float randVal2;

    float bias = 0.3f;

    float currWeight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public NeuralNetwork(int[] layers)
    {
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }

        createPerceptrons();
        setWeights();
    }

    //Creates Perceptrons
    private void createPerceptrons()
    {
        //Checks all layers
        for (int i = 0; i < layers.Length; i++)
        {
            //Adds layer to a list
            neuronsList.Add(new float[layers[i]]);
        }

        //Converts the list to an array
        Debug.Log("Successfuly set perceptrons");
        neurons = neuronsList.ToArray();
    }

    //Sets up the Weights
    private void setWeights()
    {
        for (int i = 1; i < layers.Length; i++)
        {
            backupPerceptronLayer = layers[i - 1];

            for (int k = 0; k < neurons[i].Length; k++)
            {
                perceptronWeights = new float[backupPerceptronLayer];

                for (int u = 0; u < backupPerceptronLayer; u++)
                {
                    randVal = Random.Range(0.0f, 1.0f);
                    perceptronWeights[u] = randVal;
                }

                layerWList.Add(perceptronWeights);
            }

            weightsList.Add(layerWList.ToArray());
        }

        Debug.Log("Successfuly set weights");
        weights = weightsList.ToArray();
    }

    public float[] feedFrwrd(float[] inputs)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        for (int i = 1; i < layers.Length; i++)
        {
            for (int o = 0; o < neurons[i].Length; o++)
            {
                for (int p = 0; p < neurons[i - 1].Length; p++)
                {
                    bias += weights[i - 1][o][p] * neurons[i - 1][p];
                }

                neurons[i][o] = Mathf.Tan(bias);
            }
        }

        return neurons[neurons.Length - 1];
    }

    public void train()
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int o = 0; o < weights[i].Length; o++)
            {
                for (int p = 0; p < weights[i][o].Length; p++)
                {
                    currWeight = weights[i][o][p];

                    //Change the current weight
                    randVal2 = Random.Range(0.0f, 1000.0f);

                    weights[i][o][p] = currWeight;
                }
            }
        }
    }
}
