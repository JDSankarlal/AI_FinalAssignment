using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawn : MonoBehaviour
{

    public GameObject swordPf;
    public GameObject axePf;
    public GameObject spearPf;

    public GameObject swrdPart;
    public GameObject sprPart;
    public GameObject axePart;

    float xRand;
    float zRand;

    int spawn = 0;

    // Start is called before the first frame update
    void Start()
    {
        xRand = Random.Range(31.0f, -6.0f);
        zRand = Random.Range(21.0f, -8.0f);

        axePf.transform.position = new Vector3(xRand, -5.0f, zRand);
        axePf.transform.localScale = new Vector3(8.0f, 8.0f, 8.0f);
        axePart.SetActive(true);
        axePart.transform.position = new Vector3(xRand, -5.0f, zRand);

        xRand = Random.Range(31.0f, -6.0f);
        zRand = Random.Range(21.0f, -8.0f);

        swordPf.transform.position = new Vector3(xRand, 1.0f, zRand);
        swordPf.transform.localScale = new Vector3(8.0f, 8.0f, 8.0f);
        swrdPart.SetActive(true);
        swrdPart.transform.position = new Vector3(xRand, -5.0f, zRand);

        xRand = Random.Range(31.0f, -6.0f);
        zRand = Random.Range(21.0f, -8.0f);

        spearPf.transform.position = new Vector3(xRand, 1.0f, zRand);
        spearPf.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        sprPart.SetActive(true);
        sprPart.transform.position = new Vector3(xRand, -5.0f, zRand);

        Debug.Log("Weapon Spawn is: " + xRand + " " + zRand);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
