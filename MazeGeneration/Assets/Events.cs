using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour
{
    public AudioSource aus;

    private void Start()
    {
        aus = GetComponent<AudioSource>();
    }

    public void OnCustomButtonPress()
    {
        Debug.Log("We pushed our custom button!");

        aus.Play();
    }
}
