using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update

    // Player Parameters
    public int pHealth = 5;
    public float pSpeed;

    // Other Variables
    public GameObject playerChar;
    GameObject guard;

    GameObject pngSword;
    GameObject pngSpear;
    GameObject pngFist;
    GameObject pngAxe;

    AIScript aiGuard;

    Vector3 backupPos;

    Animator playerAnims;

    bool canMove = true;

    bool weaponHeld = false;

    bool axeHold = false;
    bool swordHold = false;
    bool spearHold = false;
    bool fists = true;

    bool flee = false;

    bool canAttack = false;

    float xRand;
    float zRand;

    Vector3 speechBubPos;

    void Start()
    {
        guard = GameObject.Find("guard");
        playerAnims = GetComponent<Animator>();
        aiGuard = guard.GetComponent<AIScript>();
        backupPos = playerChar.transform.position;

        pngFist = GameObject.Find("fistsPng");
        pngSword = GameObject.Find("swPng");
        pngAxe = GameObject.Find("axePng");
        pngSpear = GameObject.Find("spPng");
    }

    // Update is called once per frame
    void Update()
    {
        //Item Bubbles Conditions
        speechBubPos = new Vector3(playerChar.transform.position.x - 2.5f, 3.0f, playerChar.transform.position.z);

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

        else
        {
            playerAnims.SetBool("isRunning", false);
            playerAnims.SetBool("stops", true);
        }

        //Attack
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (axeHold == true)
            {
                playerAnims.SetBool("attacking", true);
                Debug.Log("Axe Attack");
                canMove = false;
                StartCoroutine(animReset());
                if (canAttack == true)
                {
                    Debug.Log("Hit enemy with axe!");
                    aiGuard.aiHealth -= 3;
                    Debug.Log("Guard's Health is: " + aiGuard.aiHealth);
                }
            }

            if (swordHold == true)
            {
                playerAnims.SetBool("attacking", true);
                Debug.Log("Sword Slash");
                canMove = false;
                StartCoroutine(animReset());
                if (canAttack == true)
                {
                    Debug.Log("Hit enemy with sword!");
                    aiGuard.aiHealth -= 2;
                    Debug.Log("Guard's Health is: " + aiGuard.aiHealth);
                }
            }

            if (spearHold == true)
            {
                playerAnims.SetBool("attacking", true);
                Debug.Log("Spear Attack");
                canMove = false;
                StartCoroutine(animReset());
                if (canAttack == true)
                {
                    Debug.Log("Hit enemy with sword!");
                    aiGuard.aiHealth -= 2;
                    Debug.Log("Guard's Health is: " + aiGuard.aiHealth);
                }
            }

            if (fists == true)
            {
                playerAnims.SetBool("attacking", true);
                canMove = false;
                StartCoroutine(animReset());
                if (canAttack == true)
                {
                    Debug.Log("Hit enemy with fists!");
                    aiGuard.aiHealth -= 1;
                    Debug.Log("Guard's Health is: " + aiGuard.aiHealth);
                }
            }
        }
        if (flee == true)
        {
            Debug.Log("Player fled the arena");
            playerChar.transform.position = new Vector3(0.0f, 0.0f, -90.0f);
            Debug.Log(playerChar.transform.position);
            //Respawn player to arena with full health
            StartCoroutine(timeout());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision!");

        if (weaponHeld == false)
        {
            if (collision.collider.name.Contains("Axe"))
            {
                Debug.Log("Picked up Axe!");
                xRand = Random.Range(31.0f, -6.0f);
                zRand = Random.Range(21.0f, -8.0f);
                GameObject.Find("Axe").transform.position = new Vector3(xRand, -5.0f, zRand);
                GameObject.Find("AxePart").transform.position = new Vector3(xRand, -5.0f, zRand);
                axeHold = true;
                weaponHeld = true;
                fists = false;
            }

            if (collision.collider.name.Contains("Sword"))
            {
                Debug.Log("Picked up Sword!");
                xRand = Random.Range(31.0f, -6.0f);
                zRand = Random.Range(21.0f, -8.0f);
                GameObject.Find("Sword").transform.position = new Vector3(xRand, 1.0f, zRand);
                GameObject.Find("SwordPart").transform.position = new Vector3(xRand, -5.0f, zRand);
                swordHold = true;
                weaponHeld = true;
                fists = false;
            }

            if (collision.collider.name.Contains("Spear"))
            {
                Debug.Log("Picked up Spear!");
                xRand = Random.Range(31.0f, -6.0f);
                zRand = Random.Range(21.0f, -8.0f);
                GameObject.Find("Spear").transform.position = new Vector3(xRand, 1.0f, zRand);
                GameObject.Find("SpearPart").transform.position = new Vector3(xRand, -5.0f, zRand);
                spearHold = true;
                weaponHeld = true;
                fists = false;
            }
        }

        if (collision.collider.name.Contains("guard"))
        {
            Debug.Log("Press Space to attack!");
            canAttack = true;
        }

        if (collision.collider.name.Contains("FleeBox"))
        {
            Debug.Log("Fleeing");
            flee = true;
        }
        
    }

    private void OnCollisionExit(Collision collision)
    {
        canAttack = false;
    }

    public IEnumerator timeout()
    {
        yield return new WaitForSeconds(1.0f);
        Debug.Log("Respawned");
        playerChar.transform.position = backupPos;
        pHealth = 5;
        flee = false;
    }

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
