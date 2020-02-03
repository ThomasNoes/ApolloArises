using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFOV : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Camera cam = GetComponent<Camera>();
        Camera main = GameObject.Find("VRCamera").GetComponent<Camera>();
        cam.fieldOfView = main.fieldOfView;
    }
}
