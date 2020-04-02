using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPlayer : MonoBehaviour
{
    public bool useDebugPlayer;

    private bool inEditor;

    private GameObject player;
    private GameObject debugPlayer;

    private PortalRenderController portalManager;

    private FollowCam portalCameraLeft;
    private FollowCam portalCameraRight;

    void Awake()
    {
        //find gameobjects
        player = GameObject.Find("Player");
        debugPlayer = GameObject.Find("DebugPlayer");

        portalManager = GameObject.Find("Portal Manager")?.GetComponent<PortalRenderController>(); ;

        portalCameraLeft = GameObject.Find("Next Maze Camera Left")?.GetComponent<FollowCam>();
        portalCameraRight = GameObject.Find("Next Maze Camera Right")?.GetComponent<FollowCam>();

        if (portalCameraLeft == null || portalCameraRight == null)
            return;

        //if (player == null || debugPlayer == null || portalManager == null)
        //    return;

        #if UNITY_EDITOR //check if it is running in editor or on quest
        inEditor = true;
        #endif

        if (inEditor) //if it is running in editor
        {
            
            if (useDebugPlayer)
            {
                UseDebugPlayer();
            }
            else
            {
                UsePlayer();
            }
        }
        else //if it is running on quest
        {
            UsePlayer();
        }

    }

    private void UsePlayer()
    {
        player?.SetActive(true);
        debugPlayer?.SetActive(false);

        portalManager.isStereoscopic = true;

        portalCameraLeft.isStereoscopic = true;
        portalCameraRight.isStereoscopic = true;
    }
    private void UseDebugPlayer()
    {
        player?.SetActive(false);
        debugPlayer?.SetActive(true);

        portalManager.isStereoscopic = false;

        portalCameraLeft.isStereoscopic = false;
        portalCameraRight.isStereoscopic = false;
    }


}
