using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public bool tutorialMode;
    public bool isForwardTeleporter;
    public int portalID;
    public Transform renderQuad;
    public Transform projectionQuad;
    public float cameraOffset;
    private List<GameObject> teleportCopies;
    private GameObject player;
    private CharacterController charControl;

    void Start()
    {
        teleportCopies = new List<GameObject>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            charControl = player.GetComponent<CharacterController>();
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
                    charControl.enabled = false;
                    player.transform.Translate(cameraOffset, 0, 0, Space.World);
                    charControl.enabled = true;
                    //player.transform.SetPositionAndRotation(new Vector3(player.transform.position.x + cameraOffset,
                    //    player.transform.position.y, player.transform.position.z), player.transform.rotation);
                }
                else
                {
                    if (prController != null)
                        prController.TeleportPlayer(portalID);
                    charControl.enabled = false;
                    player.transform.Translate(-cameraOffset, 0, 0, Space.World);
                    charControl.enabled = true;
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
        //advance culling mask array
    }
}
