using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPair : MonoBehaviour
{
    public Vector3 portalPosition;
    public Quaternion portalRotation;
    public Vector3 portalScale;
    public Vector3 offset;
    public GameObject entrancePortal;
    public GameObject entranceColliderObj;
    public  GameObject exitPortal;
    public  GameObject exitColliderObj;
    public GameObject portalCamera;
    public GameObject playerCamera;
    public CapsuleCollider playerCapsuleCol;
    public SphereCollider playerSphereCol;

    public Material CameraMat;

    public TeleportTrigger entranceTrigger;
    public TeleportTrigger exitTrigger;
    public Transform cameraRig;
    public Vector3 cameraRigToEntrance;
    public Vector3 cameraRigToExit;
    
    bool steamVRIsOn;

    public void PortalPairConstructor(Transform portalTransform, Vector3 offset, bool _SteamVR)
    {
        this.portalPosition = portalTransform.position;
        this.portalRotation = portalTransform.rotation;
        this.portalScale = portalTransform.localScale;
        this.offset = offset;
        this.steamVRIsOn = _SteamVR;
        SetVariables();
    }
    void SetVariables()
    {
        //set entrancePortal, exitPortal and portalCamera
        SearchThroughChildObjects();
        
        FindPlayerCameraAndCollider();

        entranceTrigger = entranceColliderObj.GetComponent<TeleportTrigger>();
        exitTrigger = exitColliderObj.GetComponent<TeleportTrigger>();

        cameraRig = GameObject.Find("[CameraRig]").transform;
        //cameraRigToEntrancePortal = cameraRig.position - entrancePortal.transform.position;
    }
    void FindPlayerCameraAndCollider()
    {
        if (steamVRIsOn)
        {
            playerCamera = GameObject.Find("VRCamera");
            playerCapsuleCol = GameObject.Find("BodyCollider").GetComponent<CapsuleCollider>();
        }
        else
        {
            playerCamera = GameObject.Find("FallbackObjects");
            playerSphereCol = GameObject.Find("HeadCollider").GetComponent<SphereCollider>();
        }
            
    }
    void SearchThroughChildObjects()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).name == "ent")
            {
                entrancePortal = transform.GetChild(i).gameObject;
                entranceColliderObj = entrancePortal.transform.GetChild(0).gameObject;
            }   
            else if(transform.GetChild(i).name == "exit")
            {
                exitPortal = transform.GetChild(i).gameObject;
                exitColliderObj = exitPortal.transform.GetChild(0).gameObject;
            }
            else if(transform.GetChild(i).name == "cam")
            {
                portalCamera = transform.GetChild(i).gameObject;
            }    
        }
    }
    void Start()
    {
        PositionPortals();
    }

    void Update()
    {
        UpdatePortalCameraTransform();
        CheckForTeleport();
        DebugPortal();
    }
    public void UpdatePortalCameraTransform()
    {
        cameraRigToEntrance = cameraRig.position - entrancePortal.transform.position;
        cameraRigToExit = cameraRig.position - exitPortal.transform.position;

        //Debug.Log(entrancePortal.);
        

        //Debug.Log(cameraRigToEntrance);
        //Debug.Log(cameraRigToExit);

        Vector3 playerOffsetFromPortal;
        if (cameraRigToEntrance.magnitude < cameraRigToExit.magnitude)
        {
            //portalCamera.transform.position = playerCamera.transform.position + offset;
            playerOffsetFromPortal = entrancePortal.transform.position - playerCamera.transform.position;
            portalCamera.transform.position = exitPortal.transform.position - playerOffsetFromPortal;
        }
        else
        {
            //portalCamera.transform.position = playerCamera.transform.position - offset;
            playerOffsetFromPortal = exitPortal.transform.position - playerCamera.transform.position;
            portalCamera.transform.position = entrancePortal.transform.position - playerOffsetFromPortal;
        }
        
        //
        //Vector3 playerOffsetFromPortal = entrancePortal.transform.position - playerCamera.transform.position;
        //portalCamera.transform.position = exitPortal.transform.position - playerOffsetFromPortal;
        //
        portalCamera.transform.forward = playerCamera.transform.forward;
    }
    public void SetUpMaterialForPortals()
    {
        //get the targettextures from the portalCamera and set it on the entrance and exit portals.
    }
    public void PositionPortals()
    {
        //positioning the entrance portal
        entrancePortal.transform.position = portalPosition;
        entrancePortal.transform.rotation = portalRotation;
        entrancePortal.transform.localScale = portalScale;
        
        //positioning the exit portal
        exitPortal.transform.position = portalPosition+offset;
        exitPortal.transform.rotation = Quaternion.Euler(portalRotation.eulerAngles.x,portalRotation.eulerAngles.y+180,portalRotation.eulerAngles.z);
        exitPortal.transform.localScale = portalScale;

        //offsetting the portal colliders based on the player collider radius

        if(steamVRIsOn)
        {
            Debug.Log("steamVR on");
            Vector3 entranceColliderPos = entrancePortal.transform.position + entrancePortal.transform.forward*playerCapsuleCol.radius; //the offset for the entrance portal
            entranceColliderObj.transform.position = entranceColliderPos;
            Vector3 exitColliderPos = exitPortal.transform.position + exitPortal.transform.forward*playerCapsuleCol.radius; //the offset for the entrance portal
            exitColliderObj.transform.position = exitColliderPos;
        } 
        else
        {
            Debug.Log("SphereCollider "+ playerSphereCol.radius);
            Vector3 entranceColliderPos = entrancePortal.transform.position + entrancePortal.transform.forward*playerSphereCol.radius; //the offset for the entrance portal
            entranceColliderObj.transform.position = entranceColliderPos;
            Vector3 exitColliderPos = exitPortal.transform.position + exitPortal.transform.forward*playerSphereCol.radius; //the offset for the entrance portal
            exitColliderObj.transform.position = exitColliderPos;
        }
        

    }

    public void CheckForTeleport()
    {
        //if(entranceTrigger.shouldTeleport || exitTrigger.shouldTeleport)        
        if(entranceColliderObj.GetComponent<TeleportTrigger>().shouldTeleport || exitColliderObj.GetComponent<TeleportTrigger>().shouldTeleport)
        {
            Debug.Log("teleport");
            TeleportPlayer();
            entranceColliderObj.GetComponent<TeleportTrigger>().shouldTeleport = false;
            exitColliderObj.GetComponent<TeleportTrigger>().shouldTeleport= false;
        }
    }

    void DebugPortal()
    {
        //Debug.DrawRay(entrancePortal.transform.position, entrancePortal.transform.up, Color.red,1);
        Debug.DrawLine(entrancePortal.transform.position,exitPortal.transform.position, Color.magenta,1);
        Debug.DrawRay(entrancePortal.transform.position,entrancePortal.transform.forward, Color.blue,1);
    }
    public void TeleportPlayer()
    {

        if (cameraRigToEntrance.magnitude < cameraRigToExit.magnitude)
        {
            cameraRig.position = cameraRig.position + offset;
        }
        else
        {
            cameraRig.position = cameraRig.position - offset;
        }

    }
}
