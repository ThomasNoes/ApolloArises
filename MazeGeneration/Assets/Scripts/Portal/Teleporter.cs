using System.Collections.Generic;
using Assets.Scripts.Camera;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public bool tutorialMode;
    public bool isForwardTeleporter;
    public int portalID;
    public int mazeID;
    public Transform renderQuad;
    public Transform projectionQuad;
    private List<GameObject> teleportCopies;
    private GameObject player;
    private CharacterController charControl;
    private CamPosSwitcher cPosSwitcher;
    private PortalRenderController prc;

    Vector3 nextOffset;
    Vector3 prevOffset;

    void Start()
    {
        teleportCopies = new List<GameObject>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            charControl = player.GetComponent<CharacterController>();
        cPosSwitcher = Camera.main.gameObject.GetComponent<CamPosSwitcher>();
        nextOffset = PortalRenderController.SetNextOffset(mazeID);
        prevOffset = PortalRenderController.SetPrevOffset(mazeID);
        prc = GameObject.Find("Portal Manager").GetComponent<PortalRenderController>();
    }

    public void AddTeleportCopy(GameObject obj)
    {
        if (teleportCopies != null)
            teleportCopies.Add(obj);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlayerCollider")
        {
            //Debug.Log(other.name + " Exited " + transform.name);
            PortalRenderController prController = null;
            if (!tutorialMode)
            {
                prController = transform.parent.GetComponent<PortalRenderController>();
            }
            Vector3 playerNoYAxis = new Vector3(other.transform.position.x, 0, other.transform.position.z);
            BoxCollider thisCollider = GetComponentInChildren<BoxCollider>();
            Vector3 colliderWorldPos = transform.TransformPoint(thisCollider.center);
            Vector3 colliderNoYAxis = new Vector3(colliderWorldPos.x, 0, colliderWorldPos.z);
            Vector3 renderPlaneNoYAxis = new Vector3(renderQuad.position.x, 0, renderQuad.position.z);



            //offsets are static for some reason, we need to fix that
            if (Vector3.Magnitude(playerNoYAxis - renderPlaneNoYAxis) < Vector3.Magnitude(colliderNoYAxis - renderPlaneNoYAxis))
            {
                //Debug.Log(Vector3.Magnitude(playerNoYAxis - renderPlaneNoYAxis) + " lower than " + Vector3.Magnitude(colliderNoYAxis - renderPlaneNoYAxis));
                if (isForwardTeleporter)
                {
                    if (prController != null)
                        prController.TeleportPlayer(portalID + 1);
                    if (charControl != null)
                        charControl.enabled = false;
                    player.transform.Translate(nextOffset, Space.World);
                    prc.OffsetCameras(mazeID+1);
                    if (charControl != null)
                        charControl.enabled = true;

                    cPosSwitcher?.PositionSwitch(true);//maybe opposite
                    //player.transform.SetPositionAndRotation(new Vector3(player.transform.position.x + cameraOffset,
                    //    player.transform.position.y, player.transform.position.z), player.transform.rotation);
                }
                else
                {
                    if (prController != null)
                        prController.TeleportPlayer(portalID);
                    if (charControl != null)
                        charControl.enabled = false;
                    player.transform.Translate(prevOffset, Space.World);
                    prc.OffsetCameras(mazeID-1);
                    if (charControl != null)
                        charControl.enabled = true;

                    cPosSwitcher?.PositionSwitch(false); //maybe opposite
                    //player.transform.SetPositionAndRotation(new Vector3(player.transform.position.x - cameraOffset, 
                    //    player.transform.position.y, player.transform.position.z), player.transform.rotation);
                }

                if (teleportCopies.Count > 0)
                {
                    foreach (var item in teleportCopies)
                    {
                        Destroy(item);
                    }
                }

            }
        }

    }
}
