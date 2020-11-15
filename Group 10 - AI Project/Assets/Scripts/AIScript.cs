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
    GameObject player;
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

    void Start()
    {
        guardSpawnPos = guardPf.transform.position;
        aiAnim = GetComponent<Animator>();
        player = GameObject.Find("player");
        playerVals = player.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (aiHealth <= 0)
        {
            isKO = true;
            guardPf.transform.position = new Vector3(0.0f, 0.0f, -80.0f);
            StartCoroutine(guardRespawn());
        }

        if (chaseAxe == true)
        {
            guardPf.transform.position = Vector3.MoveTowards(guardPf.transform.position, new Vector3(GameObject.Find("Axe").transform.position.x, guardPf.transform.position.y, GameObject.Find("Axe").transform.position.z), 0.1f);
            aiRun = true;
        }

        if (chaseSword == true)
        {
            guardPf.transform.position = Vector3.MoveTowards(guardPf.transform.position, new Vector3(GameObject.Find("Sword").transform.position.x, guardPf.transform.position.y, GameObject.Find("Sword").transform.position.z), 0.1f);
            aiRun = true;
        }

        if (chaseSpear == true)
        {
            guardPf.transform.position = Vector3.MoveTowards(guardPf.transform.position, new Vector3(GameObject.Find("Spear").transform.position.x, guardPf.transform.position.y, GameObject.Find("Spear").transform.position.z), 0.1f);
            aiRun = true;
        }

        if (chasePlayer == true)
        {
            guardPf.transform.position = Vector3.MoveTowards(guardPf.transform.position, new Vector3(GameObject.Find("player").transform.position.x, guardPf.transform.position.y, GameObject.Find("player").transform.position.z), 0.1f);
            aiRun = true;
        }

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

        if (aiAtk == true)
        {
            //Debug.Log("Attacking");
            aiAnim.SetBool("aiAtk", true);
            StartCoroutine(animReset());
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
        Debug.Log("Timeout");
        yield return new WaitForSeconds(1f);
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
                StartCoroutine(aiAtkTimeout());
            }

            if (aiSword == true)
            {
                Debug.Log("Hit by Sword");
                playerVals.pHealth -= 2;
                StartCoroutine(aiAtkTimeout());
            }

            if (aiSpear == true)
            {
                Debug.Log("Hit by Spear");
                playerVals.pHealth -= 2;
                StartCoroutine(aiAtkTimeout());
            }

            if (aiFists == true)
            {
                Debug.Log("Hit by Fists");
                playerVals.pHealth -= 1;
                StartCoroutine(aiAtkTimeout());
            }
        }

        //AI Weapon Pickup
        if (aiWeaponHold == false)
        {
            if (collision.collider.name.Contains("Spear"))
            {
                Debug.Log("Guard picked up Spear");
                Destroy(GameObject.Find("Spear"));
                Destroy(GameObject.Find("SpearPart"));
                aiSpear = true;
                chaseSpear = false;
                aiWeaponHold = true;
                aiFists = false;
                aiRun = false;
            }

            if (collision.collider.name.Contains("Sword"))
            {
                Debug.Log("Guard picked up Sword");
                Destroy(GameObject.Find("Sword"));
                Destroy(GameObject.Find("SwordPart"));
                chaseSword = false;
                aiSword = true;
                aiWeaponHold = true;
                aiFists = false;
                aiRun = false;
            }

            if (collision.collider.name.Contains("Axe"))
            {
                Debug.Log("Guard picked up Axe");
                Destroy(GameObject.Find("Axe"));
                Destroy(GameObject.Find("AxePart"));
                aiAxe = true;
                chaseAxe = false;
                aiWeaponHold = true;
                aiFists = false;
                aiRun = false;
            }
        }
    }

    public IEnumerator animReset()
    {
        yield return new WaitForSeconds(0.4f);

        aiAnim.SetBool("aiAtk", false);
        aiAtk = false;
    }
}
