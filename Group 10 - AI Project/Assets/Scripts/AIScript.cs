//
//This script handles all functions relating to the AI player
using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class AIScript : MonoBehaviour
{
    //Init all necessary parameters

    //AI health
    public float aiHealth = 5.0f;

    //Player's health
    public float pHealth;

    //Health difference variable
    float healthDif;

    //Variables needed to calculate distance/position between ai/player/and any other objects in the scene
    public float distanceAiP;
    public Vector3 pPos;
    public Vector3 aiPos;
    Vector3 axePos;
    Vector3 swordPos;
    Vector3 spearPos;
    float axeDist;
    float swordDist;
    float spearDist;

    //Random float values used for various functions
    float xRand;
    float zRand;

    //Neural Network object
    NeuralNetwork ntwrk;

    //Sets up input layer array (9 input nodes)
    float[] inputs = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f};

    //Sets up output layer array
    float[] outputs;

    //Sets up the input, hidden, and output layers
    //9 Input nodes, 4 hidden nodes, 5 output nodes
    int[] layers = {9, 4, 5 };

    //AI player prefab game object
    public GameObject guardPf;

    //Message box game object
    GameObject msg;

    //Player/Guard game objects
    GameObject player;
    GameObject guard;

    //Reference to player's values such as health
    Player playerVals;

    //Weapon Icons gameobjects
    GameObject aiPngFist;
    GameObject aiPngAxe;
    GameObject aiPngSword;
    GameObject aiPngSpear;

    //Sound effects gameobjects
    GameObject hitPlayer;
    GameObject aiPunchSfx;
    GameObject aiSlashSfx;
    GameObject aiSwooshSfx;
    GameObject aiGrabSfx;
    GameObject gHealthTxt;

    //Position of the weapon icon
    Vector3 aiPngPos;

    //Position of the AI's spawn point
    Vector3 guardSpawnPos;

    //Animation handler
    Animator aiAnim;

    //Bools that state which action they should perform
    bool chaseAxe = false;
    bool chaseSword = false;
    bool chaseSpear = false;
    bool chasePlayer = false;

    //Bools that check which weapon the AI is holding
    bool aiSpear = false;
    bool aiSword = false;
    bool aiAxe = false;
    bool aiFists = true;
    bool aiWeaponHold = false;

    //Damages of the 3 weapons + fists
    float spearDmg = 1.5f;
    float swordDmg = 3.0f;
    float axeDmg = 2.0f;
    float fistsDmg = 1.0f;

    //Learning rate multiplier that increases by .1f for each guard spawn (up until the 5th guard)
    float learnRate = 1.0f;

    //Bools that are used to play the animations
    bool aiRun = false;
    bool aiAtk = false;

    //AI will flee if true
    bool aiFlee = false;

    //The current guard # the player is facing
    int guardCount = 1;

    //AI will run the neural network if these are set to true and '1'
    bool isDeciding = true;
    int decisionAmount = 1;

    void Start()
    {
        //Set up all variables that were initialized earlier
        guardSpawnPos = guardPf.transform.position;
        aiAnim = GetComponent<Animator>();
        player = GameObject.Find("player");
        playerVals = player.GetComponent<Player>();
        guard = GameObject.Find("guard");
        ntwrk = guard.GetComponent<NeuralNetwork>();
        ntwrk = new NeuralNetwork(layers);
        
        aiPngFist = GameObject.Find("fistsPngAi");
        aiPngAxe = GameObject.Find("axePngAi");
        aiPngSpear = GameObject.Find("spPngAi");
        aiPngSword = GameObject.Find("swPngAi");

        hitPlayer = GameObject.Find("pHit");
        aiPunchSfx = GameObject.Find("punch");
        aiSlashSfx = GameObject.Find("slash");
        aiSwooshSfx = GameObject.Find("swoosh");
        aiGrabSfx = GameObject.Find("grab");

        msg = GameObject.Find("msgBox");
        gHealthTxt = GameObject.Find("gHealth");

    }

    // Update is called once per frame
    void Update()
    {
        //Update the real player's health
        pHealth = playerVals.pHealth;

        //Update the text showing the AI's health
        gHealthTxt.GetComponent<Text>().text = aiHealth.ToString();

        //Constantly update position of weapon speech bubbles
        aiPngPos = new Vector3(guardPf.transform.position.x - 2.5f, 3.0f, guardPf.transform.position.z);
        swordPos = GameObject.Find("Sword").transform.position;
        spearPos = GameObject.Find("Spear").transform.position;
        axePos = GameObject.Find("Axe").transform.position;

        //Weapon Speech Bubble Icon Conditions
        //i.e: if ai has a sword, disable all other pngs except for the sword png
        if (aiFists == true)
        {
            aiPngFist.SetActive(true);
            aiPngAxe.SetActive(false);
            aiPngSword.SetActive(false);
            aiPngSpear.SetActive(false);
            aiPngFist.transform.position = aiPngPos;
        }

        if (aiSword == true)
        {
            aiPngFist.SetActive(false);
            aiPngAxe.SetActive(false);
            aiPngSword.SetActive(true);
            aiPngSpear.SetActive(false);
            aiPngSword.transform.position = aiPngPos;
        }

        if (aiAxe == true)
        {
            aiPngFist.SetActive(false);
            aiPngAxe.SetActive(true);
            aiPngSword.SetActive(false);
            aiPngSpear.SetActive(false);
            aiPngAxe.transform.position = aiPngPos;
        }

        if (aiSpear == true)
        {
            aiPngFist.SetActive(false);
            aiPngAxe.SetActive(false);
            aiPngSword.SetActive(false);
            aiPngSpear.SetActive(true);
            aiPngSpear.transform.position = aiPngPos;
        }

        if (aiWeaponHold == false)
        {
            aiPngFist.SetActive(true);
            aiPngAxe.SetActive(false);
            aiPngSword.SetActive(false);
            aiPngSpear.SetActive(false);
            aiPngFist.transform.position = aiPngPos;
        }

        //AI respawn condition
        //If AI health is 0, remove all weapons and make sure it doesn't chase anything
        if (aiHealth <= 0)
        {
            chaseAxe = false;
            chaseSpear = false;
            chaseSword = false;
            chasePlayer = false;
            guardPf.transform.position = new Vector3(0.0f, 0.0f, -80.0f);
            decisionAmount = 0;
            msg.GetComponent<Text>().text = "Guard #" + guardCount + " has been eliminated!";

            //If this is less than the 6th AI to respawn, increase the learning rate
            if (guardCount < 6)
            {
                learnRate += 0.1f;
            }

            //Respawn timeout
            StartCoroutine(guardRespawn());
            StartCoroutine(aiAtkTimeout());

            guardCount += 1;

        }

        //Chase weapon conditions
        //i.e: if chasing axe, run towards axe position
        if (chaseAxe == true)
        {
            guardPf.transform.position = Vector3.MoveTowards(guardPf.transform.position, new Vector3(GameObject.Find("Axe").transform.position.x, guardPf.transform.position.y, GameObject.Find("Axe").transform.position.z), 0.03f);
            guardPf.transform.LookAt(GameObject.Find("Axe").transform.position);
            aiRun = true;
        }

        if (chaseSword == true)
        {
            guardPf.transform.position = Vector3.MoveTowards(guardPf.transform.position, new Vector3(GameObject.Find("Sword").transform.position.x, guardPf.transform.position.y, GameObject.Find("Sword").transform.position.z), 0.03f);
            guardPf.transform.LookAt(GameObject.Find("Sword").transform.position);
            aiRun = true;
        }

        if (chaseSpear == true)
        {
            guardPf.transform.position = Vector3.MoveTowards(guardPf.transform.position, new Vector3(GameObject.Find("Spear").transform.position.x, guardPf.transform.position.y, GameObject.Find("Spear").transform.position.z), 0.03f);
            guardPf.transform.LookAt(GameObject.Find("Spear").transform.position);
            aiRun = true;
        }

        if (chasePlayer == true)
        {
            guardPf.transform.position = Vector3.MoveTowards(guardPf.transform.position, new Vector3(GameObject.Find("player").transform.position.x, guardPf.transform.position.y, GameObject.Find("player").transform.position.z), 0.03f);
            guardPf.transform.LookAt(GameObject.Find("player").transform.position);
            aiRun = true;
        }

        //Running animation conditions
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

        //Attack condition
        if (aiAtk == true)
        {
            //Debug.Log("Attacking");
            aiAnim.SetBool("aiAtk", true);

            //Reset animation after each attack
            StartCoroutine(animReset());
        }

        //Flee condition
        if (aiFlee == true)
        {
            guardPf.transform.position = Vector3.MoveTowards(guardPf.transform.position, new Vector3(GameObject.Find("FleeBox").transform.position.x, guardPf.transform.position.y, GameObject.Find("FleeBox").transform.position.z), 0.03f);
            aiRun = true;
        }

        // Neural Network 
        //If the AI is deciding
        if (isDeciding == true && decisionAmount == 1)
        {
            //Get the current position of the ai and player
            aiPos = GameObject.Find("guard").transform.position;
            pPos = GameObject.Find("player").transform.position;

            //Check the health difference
            healthDif = aiHealth - pHealth;

            //Get the ai's distance between it and the weapons and player
            swordDist = Vector3.Distance(aiPos, swordPos);
            axeDist = Vector3.Distance(aiPos, axePos);
            spearDist = Vector3.Distance(aiPos, spearPos);
            distanceAiP = Vector3.Distance(aiPos, pPos);

            //Add all necessary variables to the input array
            inputs[0] = healthDif;
            inputs[1] = distanceAiP;
            inputs[2] = swordDist;
            inputs[3] = spearDist;
            inputs[4] = axeDist;
            inputs[5] = swordDmg;
            inputs[6] = spearDmg;
            inputs[7] = axeDmg;
            inputs[8] = aiHealth;

            //Run the neural network through the feed forward function
            outputs = ntwrk.feedFrwrd(inputs);

            //Run the training function so the weights are updated accordingly depending on the situation
            //The training function takes in 5 of the inputs, and the learning rate
            ntwrk.trainAI(inputs[0], inputs[1], inputs[2], inputs[3], inputs[4], inputs[8], learnRate);

            //Check which output is higher and assign output to an action:
            if (outputs.Max() == outputs[0])
            {
                //Do output action #1
                chasePlayer = true;
                aiFists = true;
                decisionAmount = 0;
            }

            if (outputs.Max() == outputs[1])
            {
                //Do output action #2
                chaseAxe = true;
                decisionAmount = 0;
            }

            if (outputs.Max() == outputs[2])
            {
                //Do output action #3
                chaseSword = true;
                decisionAmount = 0;
            }

            if (outputs.Max() == outputs[3])
            {
                //Do output action #4
                chaseSpear = true;
                decisionAmount = 0;
            }

            if (outputs.Max() == outputs[4])
            {
                //Do output action #5
                aiFlee = true;
                decisionAmount = 0;
            }

        }
    }

    //Respawn function for the AI, which has a timer of 4 seconds
    public IEnumerator guardRespawn()
    {
        aiHealth = 5;
        yield return new WaitForSeconds(4f);
        guardPf.transform.position = guardSpawnPos;
    }

    //Attack timeout function for the AI, which has a timer of 1 second
    public IEnumerator aiAtkTimeout()
    {
        yield return new WaitForSeconds(1f);
        decisionAmount = 1;
    }

    //Checks if a collision has been made, and runs the correct statement depending on what the AI is colliding with
    private void OnCollisionEnter(Collision collision)
    {
        //If Ai is colliding with the player
        if (collision.collider.name.Contains("player"))
        {
            //Play swoosh sfx
            aiSwooshSfx.GetComponent<AudioSource>().Play(0);

            //Ai can attack
            aiAtk = true;

            //If ai is holding axe, hit player with it
            if (aiAxe == true)
            {
                msg.GetComponent<Text>().text = "Guard hit player with Axe!";
                playerVals.pHealth -= axeDmg;
                chasePlayer = false;
                aiAxe = false;
                aiRun = false;
                aiFists = false;
                aiWeaponHold = false;
                isDeciding = true;
                hitPlayer.GetComponent<ParticleSystem>().Play();
                aiSlashSfx.GetComponent<AudioSource>().Play(0);
                StartCoroutine(aiAtkTimeout());
            }

            //If ai is holding sword, hit player with it
            if (aiSword == true)
            {
                msg.GetComponent<Text>().text = "Guard hit player with Sword!";
                playerVals.pHealth -= swordDmg;
                chasePlayer = false;
                aiSword = false;
                aiRun = false;
                aiFists = false;
                aiWeaponHold = false;
                isDeciding = true;
                hitPlayer.GetComponent<ParticleSystem>().Play();
                aiSlashSfx.GetComponent<AudioSource>().Play(0);
                StartCoroutine(aiAtkTimeout());
            }

            //If ai is holding spear, hit player with it
            if (aiSpear == true)
            {
                msg.GetComponent<Text>().text = "Guard hit player with Spear!";
                playerVals.pHealth -= spearDmg;
                chasePlayer = false;
                aiSpear = false;
                aiRun = false;
                aiFists = false;
                aiWeaponHold = false;
                isDeciding = true;
                hitPlayer.GetComponent<ParticleSystem>().Play();
                aiSlashSfx.GetComponent<AudioSource>().Play(0);
                StartCoroutine(aiAtkTimeout());
            }

            //If ai is just using fists, hit player with it
            if (aiFists == true)
            {
                msg.GetComponent<Text>().text = "Guard hit player with Fists!";
                playerVals.pHealth -= fistsDmg;
                chasePlayer = false;
                aiFists = false;
                aiRun = false;
                isDeciding = true;
                hitPlayer.GetComponent<ParticleSystem>().Play();
                aiPunchSfx.GetComponent<AudioSource>().Play(0);
                StartCoroutine(aiAtkTimeout());
            }
        }

        //AI Weapon Pickup Conditions
        //If AI is not holding a weapon
        if (aiWeaponHold == false)
        {
            // AI Collides with a Spear
            // Pickup spear
            if (collision.collider.name.Contains("Spear"))
            {
                msg.GetComponent<Text>().text = "Guard picked up a Spear";
                xRand = Random.Range(31.0f, -6.0f);
                zRand = Random.Range(21.0f, -8.0f);
                aiGrabSfx.GetComponent<AudioSource>().Play(0);
                //Respawn the weapon to another location in the arena
                GameObject.Find("Spear").transform.position = new Vector3(xRand, -4.0f, zRand);
                GameObject.Find("SpearPart").transform.position = new Vector3(xRand, -5.0f, zRand);
                aiSpear = true;
                chaseSpear = false;
                chaseAxe = false;
                chaseSword = false;
                aiWeaponHold = true;
                aiFists = false;
                aiRun = false;
                aiFlee = false;
                chasePlayer = true;
                isDeciding = false;
            }

            // AI Collides with Sword
            // Pickup sword
            if (collision.collider.name.Contains("Sword"))
            {
                msg.GetComponent<Text>().text = "Guard picked up a Sword";
                xRand = Random.Range(31.0f, -6.0f);
                zRand = Random.Range(21.0f, -8.0f);
                aiGrabSfx.GetComponent<AudioSource>().Play(0);
                GameObject.Find("Sword").transform.position = new Vector3(xRand, -2.0f, zRand);
                GameObject.Find("SwordPart").transform.position = new Vector3(xRand, -5.0f, zRand);
                chaseSpear = false;
                chaseAxe = false;
                chaseSword = false;
                aiSword = true;
                aiWeaponHold = true;
                aiFists = false;
                aiRun = false;
                aiFlee = false;
                chasePlayer = true;
                isDeciding = false;
            }

            //AI Collides with Axe
            //Pickup axe
            if (collision.collider.name.Contains("Axe"))
            {
                msg.GetComponent<Text>().text = "Guard picked up an Axe";
                xRand = Random.Range(31.0f, -6.0f);
                zRand = Random.Range(21.0f, -8.0f);
                aiGrabSfx.GetComponent<AudioSource>().Play(0);
                GameObject.Find("Axe").transform.position = new Vector3(xRand, -5.0f, zRand);
                GameObject.Find("AxePart").transform.position = new Vector3(xRand, -5.0f, zRand);
                aiAxe = true;
                chaseSpear = false;
                chaseAxe = false;
                chaseSword = false;
                aiWeaponHold = true;
                aiFists = false;
                aiRun = false;
                aiFlee = false;
                chasePlayer = true;
                isDeciding = false;
            }
        }

        //AI Flees if colliding with the 'fleebox'
        if (collision.collider.name.Contains("FleeBox"))
        {
            msg.GetComponent<Text>().text = "Guard fled the arena!";
            aiFlee = false;
            aiRun = false;

            //Set ai health to 0 so it runs the respawn function
            aiHealth = 0;
        }
    }

    //Animation reset function that runs every time the AI attacks
    public IEnumerator animReset()
    {
        yield return new WaitForSeconds(0.4f);

        aiAnim.SetBool("aiAtk", false);
        aiAtk = false;
    }
}
