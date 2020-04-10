using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawForward : MonoBehaviour
{
    private void Start()
    {
        Debug.DrawRay(transform.position+Vector3.up, transform.forward*10, Color.green, 100);
    }
}
