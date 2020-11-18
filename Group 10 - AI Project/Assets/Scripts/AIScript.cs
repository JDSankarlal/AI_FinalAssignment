using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIScript : MonoBehaviour
{
    // Start is called before the first frame update

    //AI Parameters
    public float aiHealth = 5.0f;
    public float pHealth = 2.0f;

    public float distanceAiP;
    public Vector3 pPos;
    public Vector3 aiPos;

    float axeDistance;
    float swordDistance;
    float spearDistance;

    float xRand;
    float zRand;

    NeuralNetwork ntwrk;

    //Sets up input layer array
    float[] inputs = { 0.0f, 0.0f, 0.0f, 0.0f};

    //Sets up output layer array
    float[] outputs;

    //Sets up the input, hidden, and output layers
    int[] layers = {7, 3, 4 };

    //Other Variables
    public GameObject guardPf;
    GameObject player;
    GameObject guard;
    Player playerVals;

    Vector3 guardSpawnPos;

    Animator aiAnim;

    bool isKO = false;

    bool chaseAxe = false;
    bool chaseSword = false;
    bool chaseSpear = false;
    bool chasePlayer = false;

    bool aiSpear = false;
    bool aiSword = false;
    bool aiAxe = false;
    bool aiFists = true;
    bool aiWeaponHold = false;

    bool aiRun = false;
    bool aiAtk = false;
    bool aiFlee = false;

    int guardCount = 1;

    bool isDeciding = false;

    void Start()
    {
        guardSpawnPos = guardPf.transform.position;
        aiAnim = GetComponent<Animator>();
        player = GameObject.Find("player");
        playerVals = player.GetComponent<Player>();
        guard = GameObject.Find("guard");
        ntwrk = guard.GetComponent<NeuralNetwork>();
        ntwrk = new NeuralNetwork(layers);
        
    }

    // Update is called once per frame
    void Update()
    {
        //AI repawn condition
        if (aiHealth <= 0)
        {
            isKO = true;
            chaseAxe = false;
            chaseSpear = false;
            chaseSword = false;
            chasePlayer = false;
            guardPf.transform.position = new Vector3(0.0f, 0.0f, -80.0f);
            StartCoroutine(guardRespawn());
            Debug.Log("Guard #: " + guardCount + " has appeared!");
            guardCount += 1;

            //Copy Neural Net Data here:

        }

        //Chase weapon condition
        if (chaseAxe == true)
        {
            guardPf.transform.position = Vector3.MoveTowards(guardPf.transform.position, new Vector3(GameObject.Find("Axe").transform.position.x, guardPf.transform.position.y, GameObject.Find("Axe").transform.position.z), 0.03f);
            aiRun = true;
        }

        if (chaseSword == true)
        {
            guardPf.transform.position = Vector3.MoveTowards(guardPf.transform.position, new Vector3(GameObject.Find("Sword").transform.position.x, guardPf.transform.position.y, GameObject.Find("Sword").transform.position.z), 0.03f);
            aiRun = true;
        }

        if (chaseSpear == true)
        {
            guardPf.transform.position = Vector3.MoveTowards(guardPf.transform.position, new Vector3(GameObject.Find("Spear").transform.position.x, guardPf.transform.position.y, GameObject.Find("Spear").transform.position.z), 0.03f);
            aiRun = true;
        }

        if (chasePlayer == true)
        {
            guardPf.transform.position = Vector3.MoveTowards(guardPf.transform.position, new Vector3(GameObject.Find("player").transform.position.x, guardPf.transform.position.y, GameObject.Find("player").transform.position.z), 0.03f);
            aiRun = true;
        }

        //Running condition
        if (aiRun == true)
        {
            aiAnim.SetBool("isRunning", true);
            aiAnim.SetBool("stop", false);
        }

        if (aiRun == false)
        {
            aiAnim.SetBool("isRunning", false);
            aiAnim.SetBool("stop", true);
        }

        //Atk condition
        if (aiAtk == true)
        {
            //Debug.Log("Attacking");
            aiAnim.SetBool("aiAtk", true);
            StartCoroutine(animReset());
        }

        //Flee condition
        if (aiFlee == true)
        {
            guardPf.transform.position = Vector3.MoveTowards(guardPf.transform.position, new Vector3(GameObject.Find("FleeBox").transform.position.x, guardPf.transform.position.y, GameObject.Find("FleeBox").transform.position.z), 0.03f);
            aiRun = true;
        }

        //Debug
        if(Input.GetKeyDown(KeyCode.L))
        {
            aiRun = true;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            aiRun = false;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            aiAtk = true;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            chaseSword = true;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            chasePlayer = true;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            aiFlee = true;
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            chasePlayer = false;
            aiRun = false;
        }

        // Neural Net
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    isDeciding = true;
        //}

        if (Input.GetKeyDown(KeyCode.N))
        {
            isDeciding = false;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            aiPos = GameObject.Find("guard").transform.position;

            pPos = GameObject.Find("player").transform.position;

            distanceAiP = Vector3.Distance(aiPos, pPos);
            Debug.Log("Distance is: " + distanceAiP);

            inputs[0] = distanceAiP;
            inputs[1] = aiHealth;
            inputs[2] = pHealth;

            outputs = ntwrk.feedFrwrd(inputs);
            Debug.Log("Outputs are: " + outputs[0] + "," + outputs[1] + ", " + outputs[2] + ", " + outputs[3]);

            //ntwrk.train();
            //ntwrk.adjHealthWeights(2.0f, 5.0f);

            //Check which output is higher and assign output to an action:
            if (outputs.Max() == outputs[0])
            {
                //Do output action #1
                Debug.Log("Chasing player");
                chasePlayer = true;
                aiFists = true;
            }

            if (outputs.Max() == outputs[1])
            {
                //Do output action #2
                Debug.Log("Using axe");
                chaseAxe = true;
            }

            if (outputs.Max() == outputs[2])
            {
                //Do output action #3
                Debug.Log("Using sword");
                chaseSword = true;
            }

            if (outputs.Max() == outputs[3])
            {
                //Do output action #3
                Debug.Log("Using spear");
                chaseSpear = true;
            }

        }
    }

    public IEnumerator guardRespawn()
    {
        aiHealth = 5;
        isKO = false;
        yield return new WaitForSeconds(4f);
        guardPf.transform.position = guardSpawnPos;
        Debug.Log("Respawned guard at: " + guardSpawnPos);
    }

    public IEnumerator aiAtkTimeout()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("Timeout");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // AI Attacks
        if (collision.collider.name.Contains("player"))
        {
            Debug.Log("AI Hits Player");
            aiAtk = true;

            if (aiAxe == true)
            {
                Debug.Log("Hit by Axe");
                playerVals.pHealth -= 3;
                chasePlayer = false;
                aiAxe = false;
                aiRun = false;
                aiWeaponHold = false;
            }

            if (aiSword == true)
            {
                Debug.Log("Hit by Sword");
                playerVals.pHealth -= 2;
                chasePlayer = false;
                aiSword = false;
                aiRun = false;
                aiWeaponHold = false;
            }

            if (aiSpear == true)
            {
                Debug.Log("Hit by Spear");
                playerVals.pHealth -= 2;
                chasePlayer = false;
                aiSpear = false;
                aiRun = false;
                aiWeaponHold = false;
            }

            if (aiFists == true)
            {
                Debug.Log("Hit by Fists");
                playerVals.pHealth -= 1;
                chasePlayer = false;
                aiFists = false;
                aiRun = false;
            }
        }

        //AI Weapon Pickup
        if (aiWeaponHold == false)
        {
            if (collision.collider.name.Contains("Spear"))
            {
                Debug.Log("Guard picked up Spear");
                xRand = Random.Range(31.0f, -6.0f);
                zRand = Random.Range(21.0f, -8.0f);
                GameObject.Find("Spear").transform.position = new Vector3(xRand, 1.0f, zRand);
                GameObject.Find("SpearPart").transform.position = new Vector3(xRand, -5.0f, zRand);
                aiSpear = true;
                chaseSpear = false;
                chaseAxe = false;
                chaseSword = false;
                aiWeaponHold = true;
                aiFists = false;
                aiRun = false;
                chasePlayer = true;
            }

            if (collision.collider.name.Contains("Sword"))
            {
                Debug.Log("Guard picked up Sword");
                xRand = Random.Range(31.0f, -6.0f);
                zRand = Random.Range(21.0f, -8.0f);
                GameObject.Find("Sword").transform.position = new Vector3(xRand, 1.0f, zRand);
                GameObject.Find("SwordPart").transform.position = new Vector3(xRand, -5.0f, zRand);
                chaseSpear = false;
                chaseAxe = false;
                chaseSword = false;
                aiSword = true;
                aiWeaponHold = true;
                aiFists = false;
                aiRun = false;
                chasePlayer = true;
            }

            if (collision.collider.name.Contains("Axe"))
            {
                Debug.Log("Guard picked up Axe");
                xRand = Random.Range(31.0f, -6.0f);
                zRand = Random.Range(21.0f, -8.0f);
                GameObject.Find("Axe").transform.position = new Vector3(xRand, -5.0f, zRand);
                GameObject.Find("AxePart").transform.position = new Vector3(xRand, -5.0f, zRand);
                aiAxe = true;
                chaseSpear = false;
                chaseAxe = false;
                chaseSword = false;
                aiWeaponHold = true;
                aiFists = false;
                aiRun = false;
                chasePlayer = true;
            }
        }

        //AI Flee
        if (collision.collider.name.Contains("FleeBox"))
        {
            aiFlee = false;
            aiRun = false;
            Debug.Log("Guard fled the arena");
            //Code for respawn similar to player
        }
    }

    public IEnumerator animReset()
    {
        yield return new WaitForSeconds(0.4f);

        aiAnim.SetBool("aiAtk", false);
        aiAtk = false;
    }
}
