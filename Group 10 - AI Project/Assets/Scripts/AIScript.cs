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

    Vector3 guardSpawnPos;

    bool isKO = false;

    void Start()
    {
        guardSpawnPos = guardPf.transform.position;
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
    }

    public IEnumerator guardRespawn()
    {
        aiHealth = 5;
        isKO = false;
        yield return new WaitForSeconds(4f);
        guardPf.transform.position = guardSpawnPos;
        Debug.Log("Respawned guard at: " + guardSpawnPos);
    }
}
