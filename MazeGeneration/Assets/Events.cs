using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour
{
    public AudioSource audio;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void OnCustomButtonPress()
    {
        Debug.Log("We pushed our custom button!");

        audio.Play();
    }
}
