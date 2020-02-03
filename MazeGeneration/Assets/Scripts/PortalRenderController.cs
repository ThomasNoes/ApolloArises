﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalRenderController : MonoBehaviour
{
    public ObliqueProjectionToQuad previousPortalCameraLeftEye;
    public ObliqueProjectionToQuad previousPortalCameraRightEye;
    public ObliqueProjectionToQuad nextPortalCameraLeftEye;
    public ObliqueProjectionToQuad nextPortalCameraRightEye;

    public GameObject portalPrefab;
    public bool isStereoscopic;
    public int mazeCount;
    public int portalCount;
    public int currentMaze;
    public float cameraOffset;
    public float portalWidth;
    public float pillarOffset = 0.1f;
    public GameObject[] prevProjectionQuadArray;
    public GameObject[] nextProjectionQuadArray;
    MapManager mapManager;

    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        mazeCount = mapManager.mapSequence.Length;
        portalCount = mazeCount - 1;
        cameraOffset = (float)mapManager.mazeCols * mapManager.tileWidth + 1f;
        portalWidth = mapManager.tileWidth;

        transform.position = mapManager.transform.position;

        prevProjectionQuadArray = new GameObject[portalCount];
        nextProjectionQuadArray = new GameObject[portalCount];

        InitializePortals();
        SetProjectionQuads();
        OffsetCameras();
    }

    void InitializePortals()
    {
        Debug.Log("Portals will go here:");
        for (int i = 0; i < mazeCount - 1; i++)
        {
            TileInfo currentPortal = mapManager.mapSequence[i].endSeed;
            currentPortal.PrintTile();
            GameObject tempPortal = Instantiate(portalPrefab, new Vector3(transform.position.x + i * cameraOffset + currentPortal.column * portalWidth, 0, transform.position.z - currentPortal.row * portalWidth), Quaternion.identity);
            Teleporter tempScript = tempPortal.GetComponent<Teleporter>();
            BoxCollider bc = tempScript.renderQuad.GetComponent<BoxCollider>();

            tempPortal.transform.Rotate(0f, 180 + 90f * currentPortal.direction, 0f);
            tempPortal.transform.Translate(0, 0, portalWidth / 2f - pillarOffset, Space.Self);
            tempScript.projectionQuad.Translate(cameraOffset, 0, 0, Space.World);

            tempScript.renderQuad.transform.localScale -= new Vector3((1 - portalWidth) + pillarOffset * 2f, 0, 0);
            tempScript.projectionQuad.transform.localScale -= new Vector3((1 - portalWidth) + pillarOffset * 2f, 0, 0);
            bc.center = new Vector3(0, 0, -portalWidth / 2f + pillarOffset);

            tempPortal.name = "Forward Teleporter " + i;
            tempScript.renderQuad.name = "Forward Render Quad " + i;
            tempScript.projectionQuad.name = "Forward Projection Quad " + i;
            tempPortal.transform.parent = transform;
            tempScript.portalID = i;
            tempScript.isForwardTeleporter = true;
            tempScript.cameraOffset = cameraOffset;
            nextProjectionQuadArray[i] = tempScript.projectionQuad.gameObject;
            tempScript.renderQuad.GetComponent<Renderer>().material = Resources.Load("Materials/Next" + (isStereoscopic ? "Stereo" : "Mono")) as Material;

            //we could find a way to remove the redundancy here

            tempPortal = Instantiate(portalPrefab, new Vector3(transform.position.x + (i + 1) * cameraOffset + currentPortal.column * portalWidth, 0, transform.position.z - currentPortal.row * portalWidth), Quaternion.identity);
            tempScript = tempPortal.GetComponent<Teleporter>();
            bc = tempScript.renderQuad.GetComponent<BoxCollider>();

            tempPortal.transform.Rotate(0f, 90f * currentPortal.direction, 0f);
            tempPortal.transform.Translate(0, 0, portalWidth / 2f - pillarOffset, Space.Self);
            tempScript.projectionQuad.Translate(-cameraOffset, 0, 0, Space.World);

            tempScript.renderQuad.transform.localScale -= new Vector3((1 - portalWidth) + pillarOffset * 2f, 0, 0);
            tempScript.projectionQuad.transform.localScale -= new Vector3((1 - portalWidth) + pillarOffset * 2f, 0, 0);
            bc.center = new Vector3(0, 0, -portalWidth / 2f + pillarOffset);

            tempPortal.name = "Back Teleporter " + i;
            tempScript.renderQuad.name = "Back Render Quad " + i;
            tempScript.projectionQuad.name = "Back Projection Quad " + i;
            tempPortal.transform.parent = transform;
            tempScript.portalID = i;
            tempScript.isForwardTeleporter = false;
            tempScript.cameraOffset = cameraOffset;
            prevProjectionQuadArray[i] = tempScript.projectionQuad.gameObject;
            tempScript.renderQuad.GetComponent<Renderer>().material = Resources.Load("Materials/Prev" + (isStereoscopic ? "Stereo" : "Mono")) as Material;
        }

    }

    // This is for debugging, we can remove it later
    void Update()
    {
        if (Input.GetKeyUp("space"))
        {
            currentMaze++;
            Debug.Log(currentMaze % portalCount);
            TeleportPlayer(currentMaze);
            //Camera.main.transform.Translate(cameraOffset, 0, 0, Space.World);
        }
        if (Input.GetKeyUp("n"))
        {
            currentMaze--;
            Debug.Log(currentMaze % portalCount);
            TeleportPlayer(currentMaze);
            //Camera.main.transform.Translate(cameraOffset, 0, 0, Space.World);
        }
    }

    void OffsetCameras()
    {
        previousPortalCameraLeftEye.GetComponent<FollowCam>().SetOffset(-cameraOffset);
        previousPortalCameraRightEye.GetComponent<FollowCam>().SetOffset(-cameraOffset);
        nextPortalCameraLeftEye.GetComponent<FollowCam>().SetOffset(cameraOffset);
        nextPortalCameraRightEye.GetComponent<FollowCam>().SetOffset(cameraOffset);
        if (isStereoscopic)
        {
            previousPortalCameraLeftEye.GetComponent<FollowCam>().isStereoscopic = true;
            previousPortalCameraRightEye.GetComponent<FollowCam>().isStereoscopic = true;
            nextPortalCameraLeftEye.GetComponent<FollowCam>().isStereoscopic = true;
            nextPortalCameraRightEye.GetComponent<FollowCam>().isStereoscopic = true;
        }

    }

    void SetProjectionQuads()
    {
        previousPortalCameraRightEye.projectionScreen = prevProjectionQuadArray[TrueModulus(currentMaze - 1, portalCount)];
        nextPortalCameraRightEye.projectionScreen = nextProjectionQuadArray[TrueModulus(currentMaze, portalCount)];
        if (isStereoscopic)
        {
            previousPortalCameraLeftEye.projectionScreen = prevProjectionQuadArray[TrueModulus(currentMaze - 1, portalCount)];
            nextPortalCameraLeftEye.projectionScreen = nextProjectionQuadArray[TrueModulus(currentMaze, portalCount)];
        }
    }

    public void TeleportPlayer(int mazeID)
    {
        currentMaze = mazeID;
        SetProjectionQuads();
    }

    int TrueModulus(int k, int n)
    {
        return ((k %= n) < 0) ? k + n : k;
    }
}
