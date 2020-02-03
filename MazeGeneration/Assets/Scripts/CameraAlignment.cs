using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraAlignment : MonoBehaviour
{

    public GameObject playerCamera;
    public GameObject playerPortal;
    public GameObject nextCamera;
    public GameObject nextPortal;

    
    
    Vector3 playerOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerToPortalDirection = playerPortal.transform.position - playerCamera.transform.position;
        float playerToPortalDistance = playerToPortalDirection.magnitude;
        playerToPortalDirection = playerToPortalDirection.normalized;


        nextCamera.transform.position = nextPortal.transform.position - (-playerToPortalDirection*playerToPortalDistance);
        nextCamera.transform.forward = -playerCamera.transform.forward;

        float angularDifferenceBetweenPortalRotations= Quaternion.Angle(playerPortal.transform.rotation, nextPortal.transform.rotation);

        Quaternion nextPlayerToPortalDirection =  Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, Vector3.up);





        Debug.Log("angular "+ angularDifferenceBetweenPortalRotations); 

        //playerOffset = playerPortal.transform.position - playerCamera.transform.position;
        //nextCamera.transform.position = nextPortal.transform.position + playerOffset;

        

        

        //Quaternion portalRotationDifference = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, Vector3.up);
        //Vector3 newCameraDirection = portalRotationDifference * playerCamera.transform.forward;
        //nextCamera.transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);
    
    }
}
