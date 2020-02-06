using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyScript : MonoBehaviour
{

    public GameObject PortalPairPrefab;

    PortalPair pp;

    public  GameObject entrancePortal;
    public  GameObject exitPortal;
    public  GameObject portalCamera;
    public GameObject playerCamera;
    public Material cameraMat;

    // Start is called before the first frame update
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            
            Instantiate(PortalPairPrefab);
            pp = PortalPairPrefab.GetComponent<PortalPair>();
            //pp.PortalPairConstructor(entrancePortal,exitPortal,portalCamera,playerCamera,cameraMat);
            //pp.PortalPairConstructor(entrancePortal.transform.position, entrancePortal.transform.rotation, exitPortal.transform.position - entrancePortal.transform.position); 
        }
    }
}
