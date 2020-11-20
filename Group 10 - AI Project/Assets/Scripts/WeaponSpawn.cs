//
//This script handles the weapon spawning behavior at the beginning of the match
using UnityEngine;

public class WeaponSpawn : MonoBehaviour
{
    //Create gameobjects for each weapon
    public GameObject swordPf;
    public GameObject axePf;
    public GameObject spearPf;

    //Create gameobject for each weapon's particle effect
    public GameObject swrdPart;
    public GameObject sprPart;
    public GameObject axePart;

    //Random float values used to determine the position of the weapons in the arena
    float xRand;
    float zRand;

    // Start is called before the first frame update
    void Start()
    {
        //Calculate a random float value that is within the rand of the arena
        xRand = Random.Range(31.0f, -6.0f);
        zRand = Random.Range(21.0f, -8.0f);

        //Sets the positions of the axe,spear,sword and their particles to a random part of the arena
        axePf.transform.position = new Vector3(xRand, -5.0f, zRand);
        axePf.transform.localScale = new Vector3(8.0f, 8.0f, 8.0f);
        axePart.SetActive(true);
        axePart.transform.position = new Vector3(xRand, -5.0f, zRand);

        xRand = Random.Range(31.0f, -6.0f);
        zRand = Random.Range(21.0f, -8.0f);

        swordPf.transform.position = new Vector3(xRand, -2.0f, zRand);
        swordPf.transform.localScale = new Vector3(8.0f, 8.0f, 8.0f);
        swrdPart.SetActive(true);
        swrdPart.transform.position = new Vector3(xRand, -5.0f, zRand);

        xRand = Random.Range(31.0f, -6.0f);
        zRand = Random.Range(21.0f, -8.0f);

        spearPf.transform.position = new Vector3(xRand, -4.0f, zRand);
        spearPf.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        sprPart.SetActive(true);
        sprPart.transform.position = new Vector3(xRand, -5.0f, zRand);

    }
}
