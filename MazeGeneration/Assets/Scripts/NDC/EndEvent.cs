using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndEvent : MonoBehaviour {
    // AudioSource metalDoorSlamSound;
    public GameObject fadeCube;
    public float doorShutdelay = 10;
    float currentTime;
    bool timerActive = false;

    void Start () {
        fadeCube = GameObject.Find ("FadeToBlackCube");
        fadeCube.SetActive (false);
    }

    void Update () {
        if (timerActive) {
            if (doorShutdelay <= Time.time - currentTime) {
                FindObjectOfType<AudioManager> ().Play ("MetalDoorSlam");
                fadeCube.SetActive (true);
                timerActive = false;
            }
        }
    }

    public void executeLastEvent () {
        currentTime = Time.time;
        timerActive = true;
    }
}