using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sounCollsion : MonoBehaviour
{
    AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!audio.isPlaying)
        {
            audio.Play();
        }

    }
}
