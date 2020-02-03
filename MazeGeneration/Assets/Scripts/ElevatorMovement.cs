using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorMovement : MonoBehaviour
{
    public Vector3 elevatorVelocity;
    public float finalElevatorHeight;
    private bool goingDown = false;
    private bool goingUp = false;
    private float renderIntensity;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        renderIntensity = RenderSettings.ambientIntensity;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown("b"))
        {
            StartCoroutine("MoveDown");
        }

        if (Input.GetKeyDown("v"))
        {
            StartCoroutine("MoveUp");
        } */


    }

    public void DownBtn()
    {
        if (GameObject.FindGameObjectWithTag("FuseBox").GetComponent<FuseBoxPuzzle>().correctPlugs == 5 && !goingDown && !goingUp)
        {
            StartCoroutine("MoveDown");
        }
        else
        {
            Debug.Log("Puzzle not done!! " + GameObject.FindGameObjectWithTag("FuseBox").GetComponent<FuseBoxPuzzle>().correctPlugs + "/5 plugs are placed correctly.");
            // Puzzle not solved
            // Some sound feedback or something??
        }
    }

    public void UpBtn()
    {

        if (GameObject.FindGameObjectWithTag("FuseBox").GetComponent<FuseBoxPuzzle>().correctPlugs == 5 && !goingDown && !goingUp)
        {
            StartCoroutine("MoveUp");
        }
        else
        {
            Debug.Log("Puzzle not done!! " + GameObject.FindGameObjectWithTag("FuseBox").GetComponent<FuseBoxPuzzle>().correctPlugs + "/5 plugs are placed correctly.");
            // Puzzle not solved
            // Some sound feedback or something??
        }
    }

    public IEnumerator MoveDown()
    {

        goingUp = false;
        StopCoroutine("goingUp");
        goingDown = true;
        GameObject.FindObjectOfType<AudioManager>().Play("ElevatorRunningSound");
        while (transform.position.y > finalElevatorHeight && goingDown)
        {
            transform.position = transform.position + elevatorVelocity * Time.fixedDeltaTime;
            player.transform.position = new Vector3(player.transform.position.x, transform.position.y + elevatorVelocity.y * Time.fixedDeltaTime, player.transform.position.z);
            yield return new WaitForSeconds(0f);
        }
        goingDown = false;
        RenderSettings.ambientIntensity = 0;

    }

    public IEnumerator MoveUp()
    {
        if (GameObject.FindGameObjectWithTag("FuseBox").GetComponent<FuseBoxPuzzle>().correctPlugs == 5)
        {
            goingDown = false;
            StopCoroutine("goingDown");
            goingUp = true;
            GameObject.FindObjectOfType<AudioManager>().Play("ElevatorRunningSound");
            while (transform.position.y < 0 && goingUp)
            {
                transform.position = transform.position - elevatorVelocity * Time.fixedDeltaTime;
                player.transform.position = new Vector3(player.transform.position.x, transform.position.y + elevatorVelocity.y * Time.fixedDeltaTime, player.transform.position.z);
                yield return new WaitForSeconds(0f);
            }
            goingUp = false;
            RenderSettings.ambientIntensity = 1;
        }
        else
        {
            // Puzzle not solved
            // Some sound feedback or something??
            Debug.Log("Puzzle not done!! " + GameObject.FindGameObjectWithTag("FuseBox").GetComponent<FuseBoxPuzzle>().correctPlugs + "/5 plugs are placed correctly.");
        }
    }
}
