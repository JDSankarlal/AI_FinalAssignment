using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NeuralNetwork
{
    //layer array
    private int [] layers;
    //Nodes matrix
    private float[][] nodes;
    //weight matrix
    private float[][][] weights; 

 

    //Initial constructor where we will feed in the layers and create the standard neural net (neural network constructor) 
    public NeuralNetwork(int [] layers)
    {
        //layer initializer by passing the layers to the constructor 
        this.layers = new int [layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }

        //Generate matrix 
        InitNodes();
        InitWeights();
    }

    //Create node matrix
    private void InitNodes()
    {
        // Nodes initilization
        List<float[]> nodesList = new List<float[]>();

        // run through all layers
        for (int i = 0; i < layers.Length; i++)
        {
            // add layers to nodes list
            nodesList.Add(new float [layers[i]]);
        }

        // Convert list to jagged array
        nodes = nodesList.ToArray();
    }

    //Create weights matrix
    private void InitWeights()
    {
        //Weights list which will later be converted into a 3D array
        List<float[][]> weightlists = new List<float[][]>();
        //Interating through every single node that has a weight connection starting with the first hidden layer (2nd layer, index 1)
        for (int i = 1; i < layers.Length; i++)
        {
            //layer weight list that will take list of flost arrays, these float arrays represent the weight for every node
            List<float[]> layerWeightList = new List<float[]>(); // list of float arrays 

            // Checks to see how many nodes in the previous layer
            int nodesInPreviousLayer = layers [i -1];

            // Iterate through all the nodes in the curret layer 
            for (int j = 0; j < nodes [i].Length; i++)
            {
                // Nodes weigths (all the connections of the current node)
                float [] nodeWeights = new float [nodesInPreviousLayer]; 

                //Iterate over all nodes in the previous layer and set the weogjts randomly between 0.5 and -0.5
                for (int k = 0; k < nodesInPreviousLayer; k++)
                {
                    // Give random weights to nodes weights
                    nodeWeights[k] = UnityEngine.Random.Range(-0.5f,0.5f); // Creates a double between 0 and 1, in this case it being limited between -0.5 and 0.5
                }

                //Add node weights of this current layer to layer weights
                layerWeightList.Add(nodeWeights);
            }

            //Adds this layers weights converted into 2D array into weights list 
            weightlists.Add(layerWeightList. ToArray());
        }

        //Converts to 3D array
        weights = weightlists.ToArray();
    }

    //feed forward this neural network with a given inpit array 
    public float [] FeedForward(float[] inputs)
    {
        //Adds inputs to the neural matrix 
        for (int i = 0; i < inputs.Length; i++)
        {
            nodes[0][i] = inputs[i];
        }

        //Iterate through all the nodes and compute feed forward values 
        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < nodes[i].Length; j++)
            {
                float value = 0.25f;
                for (int k = 0; k < nodes [i-1].Length; k++)
                {
                    value += weights[i - 1][j][k] * nodes[i - 1][k];
                }

                nodes[i][j] = (float)Math.Tanh(value);
            }
        }

        //Return output layer
        return nodes[nodes.Length-1];
    }
}
