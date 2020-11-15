using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update

    // Player Parameters
    public int pHealth;
    public float pSpeed;

    // Other Variables
    public GameObject playerChar;
    GameObject guard;
    AIScript aiGuard;

    Animator playerAnims;

    bool canMove = true;

    bool weaponHeld = false;

    bool axeHold = false;
    bool swordHold = false;
    bool fists = true;

    bool canAttack = false;

    void Start()
    {
        guard = GameObject.Find("guard");
        playerAnims = GetComponent<Animator>();
        aiGuard = guard.GetComponent<AIScript>();
    }

    // Update is called once per frame
    void Update()
    {
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
                canMove = false;
                StartCoroutine(animReset());
                if (canMove == true)
                {
                    Debug.Log("Hit enemy with axe!");
                    aiGuard.aiHealth -= 2;
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
                    aiGuard.aiHealth -= 3;
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision!");

        if (weaponHeld == false)
        {
            if (collision.collider.name.Contains("Axe"))
            {
                Debug.Log("Picked up Axe!");
                Destroy(GameObject.Find("Axe"));
                axeHold = true;
                weaponHeld = true;
                fists = false;
            }

            if (collision.collider.name.Contains("Sword"))
            {
                Debug.Log("Picked up Sword!");
                Destroy(GameObject.Find("Sword"));
                swordHold = true;
                weaponHeld = true;
                fists = false;
            }
        }

        if (collision.collider.name.Contains("guard"))
        {
            Debug.Log("Press Space to attack!");
            canAttack = true;
        }
        
    }

    private void OnCollisionExit(Collision collision)
    {
        canAttack = false;
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

        else if (axeHold == true)
        {
            playerAnims.SetBool("attacking", false);
            canMove = true;
        }
    }
}
