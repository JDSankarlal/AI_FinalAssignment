//
//This script handles all the functions relating to the Neural Network itself
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    // Values used to create Neural Net

    //Arrays that store the amount of layers, perceptrons, and weights
    private int[] layers;
    private float[][] perceptrons;
    private float[][][] weights;

    //Lists that will be used to push the values of the layers, perceptrons, and weights into their respective arrays
    List<float[]> perceptronList = new List<float[]>();
    List<float[][]> weightsList = new List<float[][]>();
    List<float[]> layerWList = new List<float[]>();

    //Backup variables for the perceptron layers and their weights
    int backupPerceptronLayer;
    float[] perceptronWeights;

    //Random values used to calculate the weights
    float randVal;
    float randVal2;

    //Init the value that will be pushed into each perceptron
    float value = 0.0f;

    //Init the value that will store the current weight being calculated during the initial neural network setup, and training functions
    float currWeight;

    /////Neural Net Setup//////

    //Initial constructor where we will feed in the layers and create the standard neural net (neural network constructor) 
    public NeuralNetwork(int[] layers)
    {
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }

        //These functions create the perceptron and weights matrices
        createPerceptrons();
        setWeights();
    }

    //Creates Perceptrons Matrix
    private void createPerceptrons()
    {
        //Checks all layers
        for (int i = 0; i < layers.Length; i++)
        {
            //Adds the current layer 'i' to a list
            perceptronList.Add(new float[layers[i]]);
        }

        //Converts the final list to an array and adds it to the perceptrons array
        Debug.Log("Successfuly set perceptrons");
        perceptrons = perceptronList.ToArray();
    }

    //Sets up the Weights for the network
    private void setWeights()
    {
        //Checks all layers
        for (int i = 1; i < layers.Length; i++)
        {
            // Backs up the current layer
            backupPerceptronLayer = layers[i - 1];
            Debug.Log("Layer #: " + i);

            //Checks all nodes in that layer
            for (int k = 0; k < perceptrons[i].Length; k++)
            {
                //Takes the amount of nodes from the current layer and assigns a weight to each of them
                perceptronWeights = new float[backupPerceptronLayer];
                Debug.Log("Node #: " + (k + 1));

                //Checks all connected weights
                for (int u = 0; u < backupPerceptronLayer; u++)
                {
                    //Randomly assigns a weight between 0 and 1
                    randVal = Random.Range(0.0f, 1.0f);
                    Debug.Log("Weight #" + (u + 1) + " is: " + randVal);
                    perceptronWeights[u] = randVal;
                }

                //Adds all the weights the layer weight list
                layerWList.Add(perceptronWeights);
            }

            //Converts the current layer weight list to an array and adds it to the weights list array
            weightsList.Add(layerWList.ToArray());
        }

        //Converts the full weight list to an array and adds it to the weights array
        Debug.Log("Successfuly set weights");
        weights = weightsList.ToArray();
    }

    //Forward Propagation Function (this is where all the input calculations are made)
    public float[] feedFrwrd(float[] inputs)
    {
        //Assigns each input value to an input perceptron
        for (int i = 0; i < inputs.Length; i++)
        {
            perceptrons[0][i] = inputs[i];
        }

        //Runs through all layers and nodes and assigns each nodes a value by multiplying the current node's value by its connected weights and perceptron values
        for (int i = 1; i < layers.Length; i++)
        {
            for (int o = 0; o < perceptrons[i].Length; o++)
            {
                for (int p = 0; p < perceptrons[i - 1].Length; p++)
                {
                    value = weights[i - 1][o][p] * perceptrons[i - 1][p];
                }

                //Activation function for each perceptron (Sigmoid)
                perceptrons[i][o] = value / (1 + Mathf.Abs(value));
            }
        }
        //Returns the final values of the perceptrons
        return perceptrons[perceptrons.Length - 1];
    }

    /////TRAINING FUNCTIONS/////

    //This function will call all the training functions below and will take in all the important input values
    public void trainAI(float health, float distance, float swordDist, float spearDist, float axeDistance, float guardHealth, float learnRate /*+ rest of float input values */)
    {
        adjHealthWeights(health, learnRate);
        adjDistWeights(distance, learnRate);
        adjWeaponWeights(swordDist, spearDist, axeDistance, learnRate);
        adjFleeWeight(guardHealth, learnRate);
    }

    //INDIVIDUAL TRAINING FUNCTIONS
    
    //Adjust the health input/output weights based on ai/player health difference at the moment of calculation
    public void adjHealthWeights(float healthDifference, float lRate)
    {
        //Runs this condition if the ai has more health than the player
        if (healthDifference >= 0.0f)
        {
            //Runs through how many sets of weights are present in layer 1
            for (int i = 0; i < weights[0].Length; i++)
            {
                //Accessing weights going from Input Node #1 (Health) to all Hidden Layer Nodes
                currWeight = weights[0][i][0];

                // If current weight is low
                if (currWeight <= 0.5f)
                {
                    //Increase weights going from input node/towards output node by the specified random value * the learning rate
                    randVal2 = Random.Range(0.2f, 0.3f);
                    weights[0][i][0] += randVal2 * lRate;
                }

                // If current weight is high
                if (currWeight > 0.5f)
                {
                    //Increase weights going from input node/towards output node by the specified random value * the learning rate
                    randVal2 = Random.Range(0.05f, 0.1f);
                    weights[0][i][0] += randVal2 * lRate;

                    //Limit the weight to 1.0f if it ever exceeds that amount
                    if (currWeight >= 1.0f)
                    {
                        weights[0][i][0] = 1.0f;
                    }
                }
            }
        }

        else if (healthDifference < 0.0f)
        {
            // Reduce weights if player has more health than ai
            for (int i = 0; i < weights[0].Length; i++)
            {
                //Accessing weights going from Input Node #1 (Health) to all Hidden Layer Nodes
                currWeight = weights[0][i][0];

                // If current weight is low
                if (currWeight <= 0.5f)
                {
                    //Decrease weights going from input node/towards output node by the specified random value * the learning rate
                    randVal2 = Random.Range(0.05f, 0.1f);
                    weights[0][i][0] -= randVal2 * lRate;

                    //Limit the weight to 0.0f if it ever goes below that amount
                    if (currWeight <= 0.0f)
                    {
                        weights[0][i][0] = 0.0f;
                    }
                }

                // If current weight is high
                if (currWeight > 0.5f)
                {
                    //Decrease weights going from input node/towards output node by the specified random value * the learning rate
                    randVal2 = Random.Range(0.2f, 0.3f);
                    weights[0][i][0] -= randVal2 * lRate;
                }
            }
        }
    }
    ///////////END OF FUNCTION//////////////

    //Adjusting the weights coming from the Distance input node
    public void adjDistWeights(float dist, float lRate)
    {
        //Compare distance between player and ai
        //Runs condition if distance is less than 15 units
        if (dist >= 15.0f)
        {
            //Runs through how many sets of weights are present in layer 1
            for (int i = 0; i < weights[0].Length; i++)
            {
                //Accessing weights going from Input Node #2 (Distance) to all Hidden Layer Nodes
                currWeight = weights[0][i][1];

                // If current weight is low
                if (currWeight <= 0.5f)
                {
                    //Increase weights going from input node/towards output node by the specified random value * the learning rate
                    randVal2 = Random.Range(0.2f, 0.3f);
                    weights[0][i][1] += randVal2 * lRate;
                    perceptrons[2][0] *= (weights[0][i][1]);
                }

                // If current weight is high
                if (currWeight > 0.5f)
                {
                    //Increase weights going from input node/towards output node by the specified random value * the learning rate
                    randVal2 = Random.Range(0.05f, 0.1f);
                    weights[0][i][1] += randVal2 * lRate;
                    perceptrons[2][0] *= (weights[0][i][1]);

                    //Limit the weight to 1.0f if it ever exceeds that amount
                    if (currWeight >= 1.0f)
                    {
                        weights[0][i][1] = 1.0f;
                    }
                }
            }
        }

        //Runs condition if distance is less than 15 units
        else if (dist < 15.0f)
        {
            //Runs through how many sets of weights are present in layer 1
            for (int i = 0; i < weights[0].Length; i++)
            {
                //Accessing weights going from Input Node #2 (Distance) to all Hidden Layer Nodes
                currWeight = weights[0][i][1];

                // If current weight is low
                if (currWeight <= 0.5f)
                {
                    //Decrease weights going from input node/towards output node by the specified random value * the learning rate
                    randVal2 = Random.Range(0.05f, 0.1f);
                    weights[0][i][1] -= randVal2 * lRate;
                    perceptrons[2][0] *= (weights[0][i][1]);

                    //Limit the weight to 0.0f if it ever goes below that amount
                    if (currWeight <= 0.0f)
                    {
                        weights[0][i][1] = 0.0f;
                    }
                }

                // If current weight is high
                if (currWeight > 0.5f)
                {
                    //Decrease weights going from input node/towards output node by the specified random value * the learning rate
                    randVal2 = Random.Range(0.2f, 0.3f);
                    weights[0][i][1] -= randVal2 * lRate;
                    perceptrons[2][0] *= (weights[0][i][1]);

                }
            }
        }
    }
    ///////////END OF FUNCTION//////////////

    //Adjusting the weights coming from the sword/spear/axe distance nodes
    public void adjWeaponWeights(float swDist, float spDist, float axeDist, float lRate)
    {
        //Runs condition if sword is closest to player
        if (swDist < spDist && swDist < axeDist)
        {
            //Runs through how many sets of weights are present in layer 1
            for (int i = 0; i < weights[0].Length; i++)
            {
                //Accessing weights going from Input Node #3 (Sword) to all Hidden Layer Nodes
                currWeight = weights[0][i][2];

                // If current weight is low
                if (currWeight <= 0.5f)
                {
                    //Increase weights going from input node/towards output node by the specified random value * the learning rate
                    randVal2 = Random.Range(0.2f, 0.3f);
                    weights[0][i][2] += randVal2 * lRate;
                    perceptrons[2][2] *= (weights[0][i][2]);
                }

                // If current weight is high
                if (currWeight > 0.5f)
                {
                    //Increase weights going from input node/towards output node by the specified random value * the learning rate
                    randVal2 = Random.Range(0.05f, 0.1f);
                    weights[0][i][2] += randVal2 * lRate;
                    perceptrons[2][2] *= (weights[0][i][2]);

                    //Limit the weight to 1.0f if it ever exceeds that amount
                    if (currWeight >= 1.0f)
                    {
                        weights[0][i][2] = 1.0f;
                    }
                }

                //Reduce weights of Input Nodes #4 (Spear) and #5 (Axe) by 0.05 * the learning rate
                currWeight = weights[0][i][3];
                weights[0][i][3] -= 0.05f * lRate;

                currWeight = weights[0][i][4];
                weights[0][i][4] -= 0.05f * lRate;

            }
        }

        //Runs condition is spear is closest to player
        else if (spDist < swDist && spDist < axeDist)
        {
            //Runs through how many sets of weights are present in layer 1
            for (int i = 0; i < weights[0].Length; i++)
            {
                //Accessing weights going from Input Node #4 (Spear) to all Hidden Layer Nodes
                currWeight = weights[0][i][3];

                // If current weight is low
                if (currWeight <= 0.5f)
                {
                    //Increase weights going from input node/towards output node by the specified random value * the learning rate
                    randVal2 = Random.Range(0.2f, 0.3f);
                    weights[0][i][3] += randVal2 * lRate;
                    perceptrons[2][3] *= (weights[0][i][3]);
                }

                // If current weight is high
                if (currWeight > 0.5f)
                {
                    //Increase weights going from input node/towards output node by the specified random value * the learning rate
                    randVal2 = Random.Range(0.05f, 0.1f);
                    weights[0][i][3] += randVal2 * lRate;
                    perceptrons[2][3] *= (weights[0][i][3]);

                    //Limit the weight to 1.0f if it ever exceeds that amount
                    if (currWeight >= 1.0f)
                    {
                        weights[0][i][3] = 1.0f;
                    }
                }

                //Reduce weights of Input Nodes #3 (Sword) and #5 (Axe) by 0.05 * the learning rate
                currWeight = weights[0][i][2];
                weights[0][i][2] -= 0.05f;

                currWeight = weights[0][i][4];
                weights[0][i][4] -= 0.05f;

            }
        }

        //Runs condition if axe is closest to player
        else if (axeDist < swDist && axeDist < spDist)
        {
            //Runs through how many sets of weights are present in layer 1
            for (int i = 0; i < weights[0].Length; i++)
            {
                //Accessing weights going from Input Node #5 (Axe) to all Hidden Layer Nodes
                currWeight = weights[0][i][4];

                // If current weight is low
                if (currWeight <= 0.5f)
                {
                    //Increase weights going from input node/towards output node by the specified random value * the learning rate
                    randVal2 = Random.Range(0.2f, 0.3f);
                    weights[0][i][4] += randVal2 * lRate;
                    perceptrons[2][1] *= (weights[0][i][4]);
                }

                // If current weight is high
                if (currWeight > 0.5f)
                {
                    //Increase weights going from input node/towards output node by the specified random value * the learning rate
                    randVal2 = Random.Range(0.05f, 0.1f);
                    weights[0][i][4] += randVal2 * lRate;
                    perceptrons[2][1] *= (weights[0][i][4]);

                    //Limit the weight to 1.0f if it ever exceeds that amount
                    if (currWeight >= 1.0f)
                    {
                        weights[0][i][4] = 1.0f;
                    }
                }

                //Reduce Input weights of Nodes #3 (Sword) and #4 (Spear) by 0.05 * the learning rate
                currWeight = weights[0][i][2];
                weights[0][i][2] -= 0.05f;

                currWeight = weights[0][i][3];
                weights[0][i][3] -= 0.05f;

            }
        }
    }

    //Adjust the weights coming from the ai health's input node. This function also determines the weights that are connected to the 'Flee' output node
    public void adjFleeWeight(float aiHealth, float lRate)
    {
        //If AI health is non-critical, reduce flee weight
        if (aiHealth >= 3.0f)
        {
            //Runs through the size of the hidden layer (3)
            for (int i = 0; i < 3; i++)
            {
                //Accessing incoming weights towards Output Node #5 and Input Node #9
                currWeight = weights[1][8][i];

                //Adjust weights going to Output node #5 / weights going from Input node #9
                weights[0][i][8] *= 0.05f * lRate;
                weights[1][8][i] *= 0.05f * lRate;
                perceptrons[2][4] *= weights[1][8][i];
            }
        }

        //If AI Health is critical, increase flee weight
        else if (aiHealth < 3.0f)
        {
            //Runs through the size of the hidden layer (3)
            for (int i = 0; i < 3; i++)
            {
                //Accessing incoming weights towards Output Node #5 and Input Node #9
                currWeight = weights[1][8][i];

                //Adjust weights going to Output node #5 / weights going from Input node #9
                weights[0][i][8] *= 0.05f * lRate;
                weights[1][8][i] *= 0.9f * lRate;
                perceptrons[2][4] *= weights[1][8][i];
            }
        }
    }

    ///////////END OF FUNCTION//////////////

}
