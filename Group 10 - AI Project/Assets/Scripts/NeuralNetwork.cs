using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    // Values used to create Neural Net
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

    float backupWeight;

    float value;

    float currWeight;

/////Neural Net Setup//////
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
            Debug.Log("Layer #: " + i);

            for (int k = 0; k < neurons[i].Length; k++)
            {
                perceptronWeights = new float[backupPerceptronLayer];
                Debug.Log("Node #: " + (k + 1));

                for (int u = 0; u < backupPerceptronLayer; u++)
                {
                    randVal = Random.Range(0.0f, 1.0f);
                    Debug.Log("Weight #" + (u + 1) + " is: " + randVal);
                    perceptronWeights[u] = randVal;
                }

                layerWList.Add(perceptronWeights);
            }

            weightsList.Add(layerWList.ToArray());
        }

        Debug.Log("Successfuly set weights");
        weights = weightsList.ToArray();
    }

    //Forward Propagation Function
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
                    value += weights[i - 1][o][p] * neurons[i - 1][p];
                }

                neurons[i][o] = Mathf.Tan(value);
            }
        }

        return neurons[neurons.Length - 1];
    }


    //This function will call all the training functions below and will take in all the important input values
    public void trainAI(float health, float distance, float swordDist, float spearDist, float axeDistance, float guardHealth /*+ rest of float input values */)
    {
        adjHealthWeights(health);
        adjDistWeights(distance);
        adjWeaponWeights(swordDist, spearDist, axeDistance);
        adjFleeWeight(guardHealth);
    }

    //INDIVIDUAL TRAINING FUNCTIONS
    
    //Adjust the health input weights based on ai/player health at the moment of calculation
    public void adjHealthWeights(float healthDifference)
    {
        //Assigning low / high health
        if (healthDifference >= 0.0f)
        {
            Debug.Log("AI Health is higher than player health");
            for (int i = 0; i < weights[0].Length; i++)
            {
                //Accessing weights going from Input Node #1 (Health) to all Hidden Layer Nodes
                currWeight = weights[0][i][0];
                backupWeight = currWeight;

                // If current weight is low
                if (currWeight <= 0.5f)
                {
                    randVal2 = Random.Range(0.2f, 0.3f);
                    weights[0][i][0] += randVal2;
                    Debug.Log("Previous weight was: " + backupWeight);
                    Debug.Log("Weight increased by: " + randVal2 + ". New Weight is: " + currWeight); 
                }

                // If current weight is high
                if (currWeight > 0.5f)
                {
                    randVal2 = Random.Range(0.05f, 0.1f);
                    weights[0][i][0] += randVal2;
                    Debug.Log("Previous weight was: " + backupWeight);
                    Debug.Log("Weight increased by: " + randVal2 + ". New Weight is: " + currWeight);

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
            // Reduce weight
            Debug.Log("AI Health is lower than player health");
            for (int i = 0; i < weights[0].Length; i++)
            {
                //Accessing weights going from Input Node #1 (Health) to all Hidden Layer Nodes
                currWeight = weights[0][i][0];
                backupWeight = currWeight;

                // If current weight is low
                if (currWeight <= 0.5f)
                {
                    randVal2 = Random.Range(0.05f, 0.1f);
                    weights[0][i][0] -= randVal2;
                    Debug.Log("Previous weight was: " + backupWeight);
                    Debug.Log("Weight increased by: " + randVal2 + ". New Weight is: " + currWeight);

                    //Limit the weight to 0.0f if it ever goes below that amount
                    if (currWeight <= 0.0f)
                    {
                        weights[0][i][0] = 0.0f;
                    }
                }

                // If current weight is high
                if (currWeight > 0.5f)
                {
                    randVal2 = Random.Range(0.2f, 0.3f);
                    weights[0][i][0] -= randVal2;
                    Debug.Log("Previous weight was: " + backupWeight);
                    Debug.Log("Weight increased by: " + randVal2 + ". New Weight is: " + currWeight);
                }
            }
        }
    }
    ///////////END OF FUNCTION//////////////

    //Adjusting the weights coming from the Distance input node
    public void adjDistWeights(float dist)
    {
        //Compare distance between player and ai
        if (dist >= 15.0f)
        {
            Debug.Log("AI is far from player");
            for (int i = 0; i < weights[0].Length; i++)
            {
                //Accessing weights going from Input Node #2 (Distance) to all Hidden Layer Nodes
                currWeight = weights[0][i][1];
                backupWeight = currWeight;

                // If current weight is low
                if (currWeight <= 0.5f)
                {
                    randVal2 = Random.Range(0.2f, 0.3f);
                    weights[0][i][1] += randVal2;
                    Debug.Log("Previous weight was: " + backupWeight);
                    Debug.Log("Weight increased by: " + randVal2 + ". New Weight is: " + currWeight);
                }

                // If current weight is high
                if (currWeight > 0.5f)
                {
                    randVal2 = Random.Range(0.05f, 0.1f);
                    weights[0][i][1] += randVal2;
                    Debug.Log("Previous weight was: " + backupWeight);
                    Debug.Log("Weight increased by: " + randVal2 + ". New Weight is: " + currWeight);

                    //Limit the weight to 1.0f if it ever exceeds that amount
                    if (currWeight >= 1.0f)
                    {
                        weights[0][i][1] = 1.0f;
                    }
                }
            }
        }

        else if (dist < 15.0f)
        {
            Debug.Log("AI is close to player");
            for (int i = 0; i < weights[0].Length; i++)
            {
                //Accessing weights going from Input Node #2 (Distance) to all Hidden Layer Nodes
                currWeight = weights[0][i][1];
                backupWeight = currWeight;

                // If current weight is low
                if (currWeight <= 0.5f)
                {
                    randVal2 = Random.Range(0.05f, 0.1f);
                    weights[0][i][1] -= randVal2;
                    Debug.Log("Previous weight was: " + backupWeight);
                    Debug.Log("Weight increased by: " + randVal2 + ". New Weight is: " + currWeight);

                    //Limit the weight to 0.0f if it ever goes below that amount
                    if (currWeight <= 0.0f)
                    {
                        weights[0][i][1] = 0.0f;
                    }
                }

                // If current weight is high
                if (currWeight > 0.5f)
                {
                    randVal2 = Random.Range(0.2f, 0.3f);
                    weights[0][i][1] -= randVal2;
                    Debug.Log("Previous weight was: " + backupWeight);
                    Debug.Log("Weight increased by: " + randVal2 + ". New Weight is: " + currWeight);
                }
            }
        }
    }
    ///////////END OF FUNCTION//////////////

    //Adjusting the weights coming from the sword distance and damage nodes
    public void adjWeaponWeights(float swDist, float spDist, float axeDist)
    {
        if (swDist < spDist && swDist < axeDist)
        {
            Debug.Log("Sword is closest to player");
            for (int i = 0; i < weights[0].Length; i++)
            {
                //Accessing weights going from Input Node #3 (Sword) to all Hidden Layer Nodes
                currWeight = weights[0][i][2];
                backupWeight = currWeight;

                // If current weight is low
                if (currWeight <= 0.5f)
                {
                    randVal2 = Random.Range(0.2f, 0.3f);
                    weights[0][i][2] += randVal2;
                    Debug.Log("Previous weight was: " + backupWeight);
                    Debug.Log("Weight increased by: " + randVal2 + ". New Weight is: " + currWeight);
                }

                // If current weight is high
                if (currWeight > 0.5f)
                {
                    randVal2 = Random.Range(0.05f, 0.1f);
                    weights[0][i][2] += randVal2;
                    Debug.Log("Previous weight was: " + backupWeight);
                    Debug.Log("Weight increased by: " + randVal2 + ". New Weight is: " + currWeight);

                    //Limit the weight to 1.0f if it ever exceeds that amount
                    if (currWeight >= 1.0f)
                    {
                        weights[0][i][2] = 1.0f;
                    }
                }

                //Reduce weights of Nodes #4 (Spear) and #5 (Axe)
                currWeight = weights[0][i][3];
                weights[0][i][3] -= 0.05f;

                currWeight = weights[0][i][4];
                weights[0][i][4] -= 0.05f;

            }
        }

        else if (spDist < swDist && spDist < axeDist)
        {
            Debug.Log("Spear is closest to player");
            for (int i = 0; i < weights[0].Length; i++)
            {
                //Accessing weights going from Input Node #4 (Spear) to all Hidden Layer Nodes
                currWeight = weights[0][i][3];
                backupWeight = currWeight;

                // If current weight is low
                if (currWeight <= 0.5f)
                {
                    randVal2 = Random.Range(0.2f, 0.3f);
                    weights[0][i][3] += randVal2;
                    Debug.Log("Previous weight was: " + backupWeight);
                    Debug.Log("Weight increased by: " + randVal2 + ". New Weight is: " + currWeight);
                }

                // If current weight is high
                if (currWeight > 0.5f)
                {
                    randVal2 = Random.Range(0.05f, 0.1f);
                    weights[0][i][3] += randVal2;
                    Debug.Log("Previous weight was: " + backupWeight);
                    Debug.Log("Weight increased by: " + randVal2 + ". New Weight is: " + currWeight);

                    //Limit the weight to 1.0f if it ever exceeds that amount
                    if (currWeight >= 1.0f)
                    {
                        weights[0][i][3] = 1.0f;
                    }
                }

                //Reduce weights of Nodes #3 (Sword) and #5 (Axe)
                currWeight = weights[0][i][2];
                weights[0][i][2] -= 0.05f;

                currWeight = weights[0][i][4];
                weights[0][i][4] -= 0.05f;

            }
        }

        else if (axeDist < swDist && axeDist < spDist)
        {
            Debug.Log("Axe is closest to player");
            for (int i = 0; i < weights[0].Length; i++)
            {
                //Accessing weights going from Input Node #5 (Axe) to all Hidden Layer Nodes
                currWeight = weights[0][i][4];
                backupWeight = currWeight;

                // If current weight is low
                if (currWeight <= 0.5f)
                {
                    randVal2 = Random.Range(0.2f, 0.3f);
                    weights[0][i][4] += randVal2;
                    Debug.Log("Previous weight was: " + backupWeight);
                    Debug.Log("Weight increased by: " + randVal2 + ". New Weight is: " + currWeight);
                }

                // If current weight is high
                if (currWeight > 0.5f)
                {
                    randVal2 = Random.Range(0.05f, 0.1f);
                    weights[0][i][4] += randVal2;
                    Debug.Log("Previous weight was: " + backupWeight);
                    Debug.Log("Weight increased by: " + randVal2 + ". New Weight is: " + currWeight);

                    //Limit the weight to 1.0f if it ever exceeds that amount
                    if (currWeight >= 1.0f)
                    {
                        weights[0][i][4] = 1.0f;
                    }
                }

                //Reduce weights of Nodes #3 (Sword) and #4 (Spear)
                currWeight = weights[0][i][2];
                weights[0][i][2] -= 0.05f;

                currWeight = weights[0][i][3];
                weights[0][i][3] -= 0.05f;

            }
        }
    }

    public void adjFleeWeight(float aiHealth)
    {
        //If AI Health is critical, increase flee weight
        if (aiHealth >= 3.0f)
        {
            for (int i = 0; i < weights[0].Length; i++)
            {
                Debug.Log("Cannot flee");
                currWeight = weights[0][i][8];
                backupWeight = currWeight;

                weights[0][i][8] = 0.0f;
            }
        }
        //Else, decrease it significantly
    }

    ///////////END OF FUNCTION//////////////

}
