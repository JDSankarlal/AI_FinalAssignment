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

    float healthDif;

    public float distanceAiP;
    public Vector3 pPos;
    public Vector3 aiPos;

    Vector3 axePos;
    Vector3 swordPos;
    Vector3 spearPos;
    Vector3 fleePos;

    float axeDist;
    float swordDist;
    float spearDist;
    //float fleeDist;

    float xRand;
    float zRand;

    NeuralNetwork ntwrk;

    //Sets up input layer array
    float[] inputs = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f};

    //Sets up output layer array
    float[] outputs;

    //Sets up the input, hidden, and output layers
    int[] layers = {9, 4, 5 };

    //Other Variables
    public GameObject guardPf;
    GameObject player;
    GameObject guard;
    Player playerVals;

    GameObject aiPngFist;
    GameObject aiPngAxe;
    GameObject aiPngSword;
    GameObject aiPngSpear;

    GameObject hitPlayer;
    GameObject aiPunchSfx;
    GameObject aiSlashSfx;
    GameObject aiSwooshSfx;
    GameObject aiGrabSfx;

    Vector3 aiPngPos;

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

    float spearDmg = 1.5f;
    float swordDmg = 3.0f;
    float axeDmg = 2.0f;
    float fistsDmg = 1.0f;

    bool aiRun = false;
    bool aiAtk = false;
    bool aiFlee = false;

    int guardCount = 1;

    bool isDeciding = false;

    int decisionAmount = 0;

    void Start()
    {
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

    }

    // Update is called once per frame
    void Update()
    {
        //Constantly update position of weapon speech bubbles
        aiPngPos = new Vector3(guardPf.transform.position.x - 2.5f, 3.0f, guardPf.transform.position.z);
        swordPos = GameObject.Find("Sword").transform.position;
        spearPos = GameObject.Find("Spear").transform.position;
        axePos = GameObject.Find("Axe").transform.position;

        //Weapon Speech Bubble Icon Conditions
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
        if (aiHealth <= 0)
        {
            isKO = true;
            chaseAxe = false;
            chaseSpear = false;
            chaseSword = false;
            chasePlayer = false;
            guardPf.transform.position = new Vector3(0.0f, 0.0f, -80.0f);
            decisionAmount = 0;
            StartCoroutine(guardRespawn());
            StartCoroutine(aiAtkTimeout());
            Debug.Log("Guard #: " + guardCount + " has appeared!");
            guardCount += 1;

            //Copy Neural Net Data here:

        }

        //Chase weapon conditions
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

        // Neural Net
        if (Input.GetKeyDown(KeyCode.M))
        {
            isDeciding = true;
            decisionAmount = 1;
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            isDeciding = false;
        }

        if (isDeciding == true && decisionAmount == 1)
        {
            aiPos = GameObject.Find("guard").transform.position;
            pPos = GameObject.Find("player").transform.position;



            healthDif = aiHealth - pHealth;

            swordDist = Vector3.Distance(aiPos, swordPos);
            axeDist = Vector3.Distance(aiPos, axePos);
            spearDist = Vector3.Distance(aiPos, spearPos);
            distanceAiP = Vector3.Distance(aiPos, pPos);
            Debug.Log("Distance is: " + distanceAiP);

            inputs[0] = healthDif;
            inputs[1] = distanceAiP;
            inputs[2] = swordDist;
            inputs[3] = spearDist;
            inputs[4] = axeDist;
            inputs[5] = swordDmg;
            inputs[6] = spearDmg;
            inputs[7] = axeDmg;
            inputs[8] = aiHealth;

            outputs = ntwrk.feedFrwrd(inputs);
            Debug.Log("Outputs are: " + outputs[0] + "," + outputs[1] + ", " + outputs[2] + ", " + outputs[3] + ", " + outputs[4]);

            //ntwrk.train();
            ntwrk.trainAI(inputs[0], inputs[1], inputs[2], inputs[3], inputs[4], inputs[8]);

            //Check which output is higher and assign output to an action:
            if (outputs.Max() == outputs[0])
            {
                //Do output action #1
                Debug.Log("Chasing player with fists");
                chasePlayer = true;
                aiFists = true;
                decisionAmount = 0;
            }

            if (outputs.Max() == outputs[1])
            {
                //Do output action #2
                Debug.Log("Using axe");
                chaseAxe = true;
                decisionAmount = 0;
            }

            if (outputs.Max() == outputs[2])
            {
                //Do output action #3
                Debug.Log("Using sword");
                chaseSword = true;
                decisionAmount = 0;
            }

            if (outputs.Max() == outputs[3])
            {
                //Do output action #3
                Debug.Log("Using spear");
                chaseSpear = true;
                decisionAmount = 0;
            }

            if (outputs.Max() == outputs[4])
            {
                Debug.Log("Output 1 is: " + outputs[0] + "Output 2 is: " + outputs[1] + "Output 3 is: " + outputs[2] + "Output 4 is: " + outputs[3] + "Output 5 is: " + outputs[4]);
                Debug.Log("Fleeing. Output was: " + outputs[4] );
                aiFlee = true;
                decisionAmount = 0;
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
        decisionAmount = 1;
        Debug.Log("Timeout");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // AI Attacks
        if (collision.collider.name.Contains("player"))
        {
            Debug.Log("AI Hits Player");
            aiSwooshSfx.GetComponent<AudioSource>().Play(0);
            aiAtk = true;

            if (aiAxe == true)
            {
                Debug.Log("Hit by Axe");
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

            if (aiSword == true)
            {
                Debug.Log("Hit by Sword");
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

            if (aiSpear == true)
            {
                Debug.Log("Hit by Spear");
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

            if (aiFists == true)
            {
                Debug.Log("Hit by Fists");
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
            if (collision.collider.name.Contains("Spear"))
            {
                Debug.Log("Guard picked up Spear");
                xRand = Random.Range(31.0f, -6.0f);
                zRand = Random.Range(21.0f, -8.0f);
                aiGrabSfx.GetComponent<AudioSource>().Play(0);
                GameObject.Find("Spear").transform.position = new Vector3(xRand, 1.0f, zRand);
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
            if (collision.collider.name.Contains("Sword"))
            {
                Debug.Log("Guard picked up Sword");
                xRand = Random.Range(31.0f, -6.0f);
                zRand = Random.Range(21.0f, -8.0f);
                aiGrabSfx.GetComponent<AudioSource>().Play(0);
                GameObject.Find("Sword").transform.position = new Vector3(xRand, 1.0f, zRand);
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
            if (collision.collider.name.Contains("Axe"))
            {
                Debug.Log("Guard picked up Axe");
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

        //AI Flee
        if (collision.collider.name.Contains("FleeBox"))
        {
            aiFlee = false;
            aiRun = false;
            Debug.Log("Guard fled the arena");
            //Code for respawn similar to player
            aiHealth = 0;
        }
    }

    public IEnumerator animReset()
    {
        yield return new WaitForSeconds(0.4f);

        aiAnim.SetBool("aiAtk", false);
        aiAtk = false;
    }
}
