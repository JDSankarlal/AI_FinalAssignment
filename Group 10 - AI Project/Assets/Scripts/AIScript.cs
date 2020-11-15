using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour
{
    // Start is called before the first frame update

    //AI Parameters
    public int aiHealth = 5;
    float aiSpeed;
    float axeDistance;
    float swordDistance;
    float spearDistance;

    //Other Variables
    public GameObject guardPf;

    bool isKO = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (aiHealth <= 0)
        {
            isKO = true;
            Destroy(guardPf);
        }
    }
}
