using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    public bool shouldTeleport = false;

    void OnTriggerEnter(Collider Other)
    {
        if(Other.tag == "Player")
        {
            shouldTeleport=true;
        }
    }

}
