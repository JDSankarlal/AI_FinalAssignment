using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject playerChar;
    Animator playerAnims;

    bool punch = true;

    void Start()
    {
        playerAnims = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Movement
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {

            playerChar.transform.Translate(Vector3.forward * Time.deltaTime * 4);

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

        else
        {
            playerAnims.SetBool("isRunning", false);
            playerAnims.SetBool("stops", true);
        }

        //Attack
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (punch == true)
            {
                playerAnims.SetBool("punching", true);
                //playerAnims.SetBool("punching", false);
            }
        }
    }
}
