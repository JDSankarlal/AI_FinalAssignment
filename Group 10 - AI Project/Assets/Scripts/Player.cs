//
//This script handles all of the real player's functions
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    //Player's health
    public float pHealth = 5.0f;

    //Player/Guard gameobject
    public GameObject playerChar;
    GameObject guard;

    //Canvas Handling GameObject
    GameObject gameOverCanv;

    //Message box and health counter gameobject
    GameObject pMsg;
    GameObject pHealthTxt;

    //Weapon icons gameobjects
    GameObject pngSword;
    GameObject pngSpear;
    GameObject pngFist;
    GameObject pngAxe;

    //Particle effects gameobjects
    GameObject hitEnemy;
    GameObject punchSfx;
    GameObject slashSfx;
    GameObject swooshSfx;
    GameObject grabSfx;

    //Access ai's script in order to get its variables
    AIScript aiGuard;

    //Backup vector that stores the player's last position when fleeing/dying
    Vector3 backupPos;

    //Animation handler
    Animator playerAnims;

    //Checks if the player can move
    bool canMove = true;

    //Checks if the player is holding a weapon
    bool weaponHeld = false;

    //Checks which weapon the player is holding
    bool axeHold = false;
    bool swordHold = false;
    bool spearHold = false;
    bool fists = true;

    //Checks if the player is fleeing
    bool flee = false;

    //Checks if the player can attack
    bool canAttack = false;

    //Random values used during various functions
    float xRand;
    float zRand;

    //Counts the amount of lives (not health) the player has, game over if it hits 0
    int pLives = 1;

    //Position of the weapon icons
    Vector3 speechBubPos;

    void Start()
    {
        //Set all required variables
        guard = GameObject.Find("guard");
        playerAnims = GetComponent<Animator>();
        aiGuard = guard.GetComponent<AIScript>();
        backupPos = playerChar.transform.position;

        pngFist = GameObject.Find("fistsPng");
        pngSword = GameObject.Find("swPng");
        pngAxe = GameObject.Find("axePng");
        pngSpear = GameObject.Find("spPng");

        hitEnemy = GameObject.Find("aiHit");
        punchSfx = GameObject.Find("punch");
        slashSfx = GameObject.Find("slash");
        swooshSfx = GameObject.Find("swoosh");
        grabSfx = GameObject.Find("grab");

        pMsg = GameObject.Find("msgBox");
        pHealthTxt = GameObject.Find("pHealth");

        gameOverCanv = GameObject.Find("GameOver");
        gameOverCanv.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Menu");
        }

        //Update health counter text
        pHealthTxt.GetComponent<Text>().text = pHealth.ToString();

        //If the player has no health, run the respawn function through the use of the fleeing function
        if (pHealth <= 0.0f)
        {
            flee = true;
            pLives -= 1;
            pMsg.GetComponent<Text>().text = "You died!" + " You have " + pLives.ToString() + " lives left";
        }

        if (pLives <= 0)
        {
            //Show Game Over screen
            gameOverCanv.SetActive(true);
        }

        //Item Bubbles Conditions
        speechBubPos = new Vector3(playerChar.transform.position.x - 2.5f, 3.0f, playerChar.transform.position.z);

        //i.e: If player is holding an axe, show the axe weapon icon
        if (fists == true)
        {
            pngFist.SetActive(true);
            pngAxe.SetActive(false);
            pngSpear.SetActive(false);
            pngSword.SetActive(false);
            pngFist.transform.position = speechBubPos;    
        }

        if (axeHold == true)
        {
            pngFist.SetActive(false);
            pngAxe.SetActive(true);
            pngSpear.SetActive(false);
            pngSword.SetActive(false);
            pngAxe.transform.position = speechBubPos;
        }

        if (swordHold == true)
        {
            pngFist.SetActive(false);
            pngAxe.SetActive(false);
            pngSpear.SetActive(false);
            pngSword.SetActive(true);
            pngSword.transform.position = speechBubPos;
        }

        if (spearHold == true)
        {
            pngFist.SetActive(false);
            pngAxe.SetActive(false);
            pngSpear.SetActive(true);
            pngSword.SetActive(false);
            pngSpear.transform.position = speechBubPos;
        }

        //Movement
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (canMove == true)
            {
                playerChar.transform.Translate(Vector3.forward * Time.deltaTime * 8);

                if (Input.GetKey(KeyCode.D))
                {
                    playerChar.transform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
                }

                if (Input.GetKey(KeyCode.A))
                {
                    playerChar.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                }

                if (Input.GetKey(KeyCode.W))
                {
                    playerChar.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                }

                if (Input.GetKey(KeyCode.S))
                {
                    playerChar.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                }

                playerAnims.SetBool("isRunning", true);
                playerAnims.SetBool("stops", false);
            }
        }

        //If not moving, stop running animation
        else
        {
            playerAnims.SetBool("isRunning", false);
            playerAnims.SetBool("stops", true);
        }

        //Attacking Conditions
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Play swoosh sfx
            swooshSfx.GetComponent<AudioSource>().Play(0);

            //If holding a weapon and can attack the ai, play slash sound and deduct health from ai
            if (axeHold == true)
            {
                playerAnims.SetBool("attacking", true);
                canMove = false;
                StartCoroutine(animReset());
                if (canAttack == true)
                {
                    pMsg.GetComponent<Text>().text = "Hit enemy with axe!";
                    aiGuard.aiHealth -= 3;
                    axeHold = false;
                    weaponHeld = false;
                    fists = true;
                    hitEnemy.GetComponent<ParticleSystem>().Play();
                    slashSfx.GetComponent<AudioSource>().Play(1);
                    canAttack = false;
                    StartCoroutine(atkTO());
                }
            }

            if (swordHold == true)
            {
                playerAnims.SetBool("attacking", true);
                canMove = false;
                StartCoroutine(animReset());
                if (canAttack == true)
                {
                    pMsg.GetComponent<Text>().text = "Hit enemy with sword!";
                    aiGuard.aiHealth -= 2;
                    swordHold = false;
                    weaponHeld = false;
                    fists = true;
                    hitEnemy.GetComponent<ParticleSystem>().Play();
                    slashSfx.GetComponent<AudioSource>().Play(1);
                    canAttack = false;
                    StartCoroutine(atkTO());
                }
            }

            if (spearHold == true)
            {
                playerAnims.SetBool("attacking", true);
                canMove = false;
                StartCoroutine(animReset());
                if (canAttack == true)
                {
                    pMsg.GetComponent<Text>().text = "Hit enemy with spear!";
                    aiGuard.aiHealth -= 2;
                    spearHold = false;
                    weaponHeld = false;
                    fists = true;
                    hitEnemy.GetComponent<ParticleSystem>().Play();
                    slashSfx.GetComponent<AudioSource>().Play(1);
                    canAttack = false;
                    StartCoroutine(atkTO());
                }
            }

            //If using fists, play punch sound and deduct health from ai
            if (fists == true)
            {
                playerAnims.SetBool("attacking", true);
                canMove = false;
                StartCoroutine(animReset());
                if (canAttack == true)
                {
                    pMsg.GetComponent<Text>().text = "Hit enemy with fists!";
                    aiGuard.aiHealth -= 1;
                    hitEnemy.GetComponent<ParticleSystem>().Play();
                    punchSfx.GetComponent<AudioSource>().Play(0);
                    canAttack = false;
                    StartCoroutine(atkTO());
                }
            }
        }

        //If fleeing, respawn the player
        if (flee == true)
        {
            playerChar.transform.position = new Vector3(0.0f, 0.0f, -90.0f);
            pHealth = 5.0f;
            Debug.Log(playerChar.transform.position);
            //Respawn player to arena with full health
            StartCoroutine(timeout());
        }
    }

    //Checks collisions
    private void OnCollisionEnter(Collision collision)
    {
        //If not holding a weapon
        if (weaponHeld == false)
        {
            //If colliding with a weapon, pickup the weapon that is being collided with
            //+ respawn weapon object to another part of the arena
            if (collision.collider.name.Contains("Axe"))
            {
                pMsg.GetComponent<Text>().text = "You picked up an Axe";
                xRand = Random.Range(31.0f, -6.0f);
                zRand = Random.Range(21.0f, -8.0f);
                grabSfx.GetComponent<AudioSource>().Play(0);
                GameObject.Find("Axe").transform.position = new Vector3(xRand, -5.0f, zRand);
                GameObject.Find("AxePart").transform.position = new Vector3(xRand, -5.0f, zRand);
                axeHold = true;
                weaponHeld = true;
                fists = false;
            }

            if (collision.collider.name.Contains("Sword"))
            {
                pMsg.GetComponent<Text>().text = "You picked up a Sword";
                xRand = Random.Range(31.0f, -6.0f);
                zRand = Random.Range(21.0f, -8.0f);
                grabSfx.GetComponent<AudioSource>().Play(0);
                GameObject.Find("Sword").transform.position = new Vector3(xRand, -2.0f, zRand);
                GameObject.Find("SwordPart").transform.position = new Vector3(xRand, -5.0f, zRand);
                swordHold = true;
                weaponHeld = true;
                fists = false;
            }

            if (collision.collider.name.Contains("Spear"))
            {
                pMsg.GetComponent<Text>().text = "You picked up a Spear";
                xRand = Random.Range(31.0f, -6.0f);
                zRand = Random.Range(21.0f, -8.0f);
                grabSfx.GetComponent<AudioSource>().Play(0);
                GameObject.Find("Spear").transform.position = new Vector3(xRand, -4.0f, zRand);
                GameObject.Find("SpearPart").transform.position = new Vector3(xRand, -5.0f, zRand);
                spearHold = true;
                weaponHeld = true;
                fists = false;
            }
        }

        //If colliding with the ai, player can attack
        if (collision.collider.name.Contains("guard"))
        {
            canAttack = true;
        }

        //If colliding with the 'fleebox', player flees
        if (collision.collider.name.Contains("FleeBox"))
        {
            Debug.Log("Fleeing");
            pMsg.GetComponent<Text>().text = "You fled the arena";
            flee = true;
        }
        
    }

    //If exiting collision bounds, player cannot attack
    private void OnCollisionExit(Collision collision)
    {
        canAttack = false;
    }

    //timeout function that takes 4 seconds for the player to respwn after death/fleeing
    public IEnumerator timeout()
    {
        yield return new WaitForSeconds(1.0f);
        Debug.Log("Respawned");
        playerChar.transform.position = backupPos;
        flee = false;
    }

    //adds a timeout so the player cannot constantly deal the damage to the ai
    public IEnumerator atkTO()
    {
        yield return new WaitForSeconds(0.5f);
        canAttack = true;
    }

    //Resets the animations being player after 0.5 seconds
    public IEnumerator animReset()
    {
        yield return new WaitForSeconds(0.5f);

        if (fists == true)
        {
            playerAnims.SetBool("attacking", false);
            canMove = true;
        }

        else if (swordHold == true)
        {
            playerAnims.SetBool("attacking", false);
            canMove = true;
        }

        else if (spearHold == true)
        {
            playerAnims.SetBool("attacking", false);
            canMove = true;
        }

        else if (axeHold == true)
        {
            playerAnims.SetBool("attacking", false);
            canMove = true;
        }
    }
}
