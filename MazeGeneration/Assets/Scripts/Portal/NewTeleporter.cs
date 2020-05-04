using System;
using System.Collections.Generic;
using Assets.Scripts.Camera;
using UnityEngine;

public class NewTeleporter : MonoBehaviour
{
    public bool isForwardTeleporter;
    public int portalID, mazeID;
    public Transform renderQuad, projectionQuad;
    public BoxCollider renderPlaneCollider, groundCollider, entryCollider;

    private List<GameObject> teleportCopies;
    private GameObject player;

    private CharacterController charControl;
    private CamPosSwitcher cPosSwitcher;
    private MazeDisabler mazeDisabler;
    private PortalRenderController prController;

    private LayerMask layerMask;

    private bool cooldownActive;
    private Vector3 nextOffset, prevOffset;

    void Start()
    {
        teleportCopies = new List<GameObject>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            charControl = player.GetComponent<CharacterController>();
        cPosSwitcher = FindObjectOfType<CamPosSwitcher>();
        mazeDisabler = FindObjectOfType<MazeDisabler>();
        nextOffset = PortalRenderController.SetNextOffset(mazeID);
        prevOffset = PortalRenderController.SetPrevOffset(mazeID);
        prController = transform.parent.GetComponent<PortalRenderController>();
    }

    public void AddTeleportCopy(GameObject obj)
    {
        if (teleportCopies != null)
            teleportCopies.Add(obj);
    }

    public void Teleport(Collider col)
    {
        if (cooldownActive)
            return;

        StartCooldown(0.15f);

        if (isForwardTeleporter)
        {
            if (prController != null)
            {
                prController.TeleportPlayer(portalID + 1);
                prController.OffsetCameras(mazeID + 1);
            }
            if (charControl != null)
                charControl.enabled = false;
            player.transform.Translate(nextOffset, Space.World);
            if (charControl != null)
                charControl.enabled = true;

            cPosSwitcher?.PositionSwitchOverride(true);
        }
        else
        {
            if (prController != null)
            {
                prController.TeleportPlayer(portalID);
                prController.OffsetCameras(mazeID - 1);
            }
            if (charControl != null)
                charControl.enabled = false;
            player.transform.Translate(prevOffset, Space.World);
            if (charControl != null)
                charControl.enabled = true;

            cPosSwitcher?.PositionSwitchOverride(false);
        }

        //if (teleportCopies.Count > 0)
        //{
        //    foreach (var item in teleportCopies)
        //    {
        //        Destroy(item);
        //    }
        //}

        mazeDisabler?.UpdateDisabled();
    }

    public PortalRenderController getPortalRenderController()
    {
        if (prController == null)
            return null;
        else
            return prController;
    }

    private void StartCooldown(float time)
    {
        cooldownActive = true;
        Invoke("Cooldown", time);
    }
    private void Cooldown() { cooldownActive = false; }
}