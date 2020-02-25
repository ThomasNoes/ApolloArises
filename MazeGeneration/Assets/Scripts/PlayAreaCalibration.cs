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

    void Start()
    {
        Vector3 playAreaSize = OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea);
        transform.localScale = new Vector3(transform.localScale.x * playAreaSize.x, 1.0f, transform.localScale.x * playAreaSize.z);

        if (Application.isEditor)
        {
            helpText?.SetActive(false);
            active = false;
            return;
        }

        mapManager = GameObject.Find("MapManager");
        portalManager = GameObject.Find("Portal Manager");
        thisOvrManager = FindObjectOfType<OVRManager>();

        if (mapManager == null || portalManager == null)
            active = false;
        else
            Invoke("DelayedStart", 0.1f);
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
                    thisOvrManager.AllowRecenter = false;
            }
    }

}
