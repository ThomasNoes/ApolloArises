using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAreaCalibration : MonoBehaviour
{
    public GameObject helpText;
    private GameObject mapManager;
    private GameObject portalManager;
    private bool active = true;
    private OVRManager thisOvrManager;
    private MazeDisabler mazeDisabler;

    void Start()
    {
        if (Application.isEditor)
        {
            helpText?.SetActive(false);
            active = false;
            gameObject.SetActive(false);
            return;
        }

        Vector3 playAreaSize = OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea);
        transform.localScale = new Vector3(transform.localScale.x * playAreaSize.x, 1.0f, transform.localScale.x * playAreaSize.z);

        mapManager = GameObject.Find("MapManager");
        portalManager = GameObject.Find("Portal Manager");
        thisOvrManager = FindObjectOfType<OVRManager>();

        if (mapManager == null || portalManager == null)
            active = false;
        else
        {
            Invoke("DelayedStart", 0.1f);
            mazeDisabler = mapManager.GetComponent<MazeDisabler>();
        }

        if (thisOvrManager != null)
        {
            thisOvrManager.reorientHMDOnControllerRecenter = true;
            thisOvrManager.AllowRecenter = true;
        }
    }

    void DelayedStart()
    {
        mapManager.gameObject.SetActive(false);
        portalManager.gameObject.SetActive(false);

        OVRManager.boundary.SetVisible(true);
    }

    void Update()
    {
        if (active)
            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                mapManager.gameObject.SetActive(true);
                portalManager.gameObject.SetActive(true);
                gameObject.SetActive(false);
                helpText?.SetActive(false);
                active = false;
                OVRManager.boundary.SetVisible(false);

                if (thisOvrManager != null)
                {
                    thisOvrManager.reorientHMDOnControllerRecenter = false;
                    thisOvrManager.AllowRecenter = false;
                }

                if (mazeDisabler != null)
                    mazeDisabler.Initialize();
            }
    }

}
