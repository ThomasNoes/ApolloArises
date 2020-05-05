using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricityOutline : MonoBehaviour
{
    public Transform[] points;
    private AudioSource AudioSource;
    private LineRenderer lr;
    private bool soundPlayed;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        AudioSource = GetComponent<AudioSource>();

        lr.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            lr.SetPosition(i, points[i].position);
        }

        if (!soundPlayed)
        {
            AudioSource?.Play();
            soundPlayed = true;
        }

    }

}
