using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalRenderController : MonoBehaviour
{
    public ObliqueProjectionToQuad nextPortalCameraLeftEye;
    public ObliqueProjectionToQuad nextPortalCameraRightEye;

    public GameObject portalPrefab, newPortalPrefab;
    public bool useNewPortals = true, isStereoscopic;
    static public MapInfo[] mapSequence;
    public int sequenceLength, portalCount, currentMaze;
    public float portalWidth, pillarOffset = 0.1f;
    public GameObject[] prevProjectionQuadArray, nextProjectionQuadArray;
    [HideInInspector] public GameObject[] prevRenderQuadArray, nextRenderQuadArray;
    MapManager mapManager;

    Vector3 tempPos, nextOffset, prevOffset;

    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        mapSequence = mapManager.mapSequence;

        transform.position = mapManager.transform.position;
        transform.rotation = mapManager.transform.rotation;

        if (mapSequence.Length >1)
        {
            sequenceLength = mapSequence.Length;
            portalCount = mapSequence.Length - 1;
            portalWidth = mapManager.tileWidth;

            prevProjectionQuadArray = new GameObject[portalCount];
            nextProjectionQuadArray = new GameObject[portalCount];
            prevRenderQuadArray = new GameObject[portalCount];
            nextRenderQuadArray = new GameObject[portalCount];

            InitializePortals();
            SetProjectionQuads(true);
            OffsetCameras(0);
        }
    }

    private void PortalSetup(bool isForward, int i)
    {
        if (useNewPortals && newPortalPrefab != null)
        {
            Debug.Log("Using new portal setup");
            NewPortalSetup(isForward, i);
            return;
        }



        int j; // number added to i to distinguish between portals in the portal pair.
        string name;
        if (isForward)
        {
            j = 0;
            name = "Forward";
        }
        else
        {
            j = 1;
            name = "Back";
        }

        TileInfo currentPortal = mapSequence[i].endSeed;
        //currentPortal.PrintTile();
        tempPos = new Vector3(mapSequence[i+j].mapObject.transform.position.x + currentPortal.column * portalWidth,
            mapSequence[i+j].mapObject.transform.position.y,
            mapSequence[i+j].mapObject.transform.position.z - currentPortal.row * portalWidth);

        GameObject tempPortal = Instantiate(portalPrefab, tempPos, Quaternion.identity, transform);

        Teleporter tempScript = tempPortal.GetComponent<Teleporter>();
        BoxCollider bc = tempScript.renderQuad.GetComponent<BoxCollider>();

        tempPortal.transform.Rotate(0f, (180 * (1-j)) + 90f * currentPortal.direction, 0f);
        tempPortal.transform.Translate(0, 0, portalWidth / 2f - pillarOffset, Space.Self);

        tempScript.projectionQuad.Translate(isForward ? nextOffset : prevOffset, Space.World);

        tempScript.renderQuad.transform.localScale -= new Vector3((1 - portalWidth) + pillarOffset * 2f, 0, 0);
        tempScript.projectionQuad.transform.localScale -= new Vector3((1 - portalWidth) + pillarOffset * 2f, 0, 0);
        bc.center = new Vector3(0, 0, -portalWidth / 2.0f + pillarOffset);

        tempPortal.name = name + " Teleporter " + i;
        tempScript.renderQuad.name = name + "Render Quad " + i ;
        tempScript.projectionQuad.name = name + " Projection Quad " + i;
        tempPortal.transform.parent = transform;
        tempScript.portalID = i;
        tempScript.mazeID = i + j;
        tempScript.isForwardTeleporter = isForward;
        if (isForward)
        {
            //tempScript.cameraOffset = cameraOffset; // needs to be different
            nextProjectionQuadArray[i] = tempScript.projectionQuad.gameObject;
            nextRenderQuadArray[i] = tempScript.renderQuad.gameObject;
        }
        else
        {
            //tempScript.cameraOffset = cameraOffset; // needs to be different
            prevProjectionQuadArray[i] = tempScript.projectionQuad.gameObject;
            prevRenderQuadArray[i] = tempScript.renderQuad.gameObject;
        }

        tempScript.renderQuad.GetComponent<Renderer>().material = Resources.Load("Materials/Next" + (isStereoscopic ? "Stereo" : "Mono")) as Material;
    }

    private void NewPortalSetup(bool isForward, int i)
    {
        int j; // number added to i to distinguish between portals in the portal pair.
        string name;
        if (isForward)
        {
            j = 0;
            name = "Forward";
        }
        else
        {
            j = 1;
            name = "Back";
        }

        TileInfo currentPortal = mapSequence[i].endSeed;
        //currentPortal.PrintTile();
        tempPos = mapSequence[i + j].mapObject.transform.position;

        //tempPos = new Vector3(mapSequence[i + j].mapObject.transform.position.x + currentPortal.column * portalWidth,
            //mapSequence[i + j].mapObject.transform.position.y,
            //mapSequence[i + j].mapObject.transform.position.z - currentPortal.row * portalWidth);

        GameObject tempPortal = Instantiate(newPortalPrefab, tempPos, transform.rotation, transform);

        //translate 
        tempPortal.transform.Translate(currentPortal.column * portalWidth, 0, -currentPortal.row * portalWidth);

        NewTeleporter tempScript = tempPortal.GetComponent<NewTeleporter>();
        BoxCollider bc = tempScript.renderQuad.GetComponent<BoxCollider>();
        BoxCollider gbc = tempScript.groundCollider;

        tempPortal.transform.Rotate(0f, (180 * (1 - j)) + 90f * currentPortal.direction, 0f);
        tempPortal.transform.Translate(0, 0, portalWidth / 2f /*- pillarOffset*/, Space.Self); // TODO removed pillar offset

        tempScript.projectionQuad.Translate(isForward ? nextOffset : prevOffset, Space.Self);

        tempScript.renderQuad.transform.localScale -= new Vector3((1 - portalWidth), 0, 0);
        tempScript.projectionQuad.transform.localScale -= new Vector3((1 - portalWidth), 0, 0);

        //bc.center = new Vector3(0, 0, -portalWidth / 2.0f + pillarOffset);
        gbc.gameObject.transform.localPosition = new Vector3(0, tempScript.renderQuad.localPosition.y, -portalWidth / 2.0f + pillarOffset);
        gbc.size = new Vector3(portalWidth, tempScript.projectionQuad.localScale.y, portalWidth - (pillarOffset * 2.0f));

        tempPortal.name = name + " Teleporter " + i;
        tempScript.renderQuad.name = name + "Render Quad " + i;
        tempScript.projectionQuad.name = name + " Projection Quad " + i;
        //tempPortal.transform.parent = transform;
        tempScript.portalID = i;
        tempScript.mazeID = i + j;
        tempScript.isForwardTeleporter = isForward;
        if (isForward)
        {
            //tempScript.cameraOffset = cameraOffset; // needs to be different
            nextProjectionQuadArray[i] = tempScript.projectionQuad.gameObject;
            nextRenderQuadArray[i] = tempScript.renderQuad.gameObject;
        }
        else
        {
            //tempScript.cameraOffset = cameraOffset; // needs to be different
            prevProjectionQuadArray[i] = tempScript.projectionQuad.gameObject;
            prevRenderQuadArray[i] = tempScript.renderQuad.gameObject;
        }

        tempScript.renderQuad.GetComponent<Renderer>().material = Resources.Load("Materials/Next" + (isStereoscopic ? "Stereo" : "Mono")) as Material;
    }

    //private void OldSetup(int i)
    //{
    //    //Debug.Log("Portals will go here:");
    //        TileInfo currentPortal = mapSequence[i].endSeed;
    //        tempPos = new Vector3(mapSequence[i].mapObject.transform.position.x + currentPortal.column * portalWidth,
    //            mapSequence[i].mapObject.transform.position.y,
    //            mapSequence[i].mapObject.transform.position.z - currentPortal.row * portalWidth);

    //        GameObject tempPortal = Instantiate(portalPrefab, tempPos, Quaternion.identity);

    //        Teleporter tempScript = tempPortal.GetComponent<Teleporter>();
    //        BoxCollider bc = tempScript.renderQuad.GetComponent<BoxCollider>();

    //        tempPortal.transform.Rotate(0f, 180 + 90f * currentPortal.direction, 0f);
    //        tempPortal.transform.Translate(0, 0, portalWidth / 2f - pillarOffset, Space.Self);
    //        tempScript.projectionQuad.Translate(cameraOffset, 0, 0, Space.World);

    //        tempScript.renderQuad.transform.localScale -= new Vector3((1 - portalWidth) + pillarOffset * 2f, 0, 0);
    //        tempScript.projectionQuad.transform.localScale -= new Vector3((1 - portalWidth) + pillarOffset * 2f, 0, 0);
    //        bc.center = new Vector3(0, 0, -portalWidth / 2f + pillarOffset);

    //        tempPortal.name = "Forward Teleporter " + i;
    //        tempScript.renderQuad.name = "Forward Render Quad " + i;
    //        tempScript.projectionQuad.name = "Forward Projection Quad " + i;
    //        tempPortal.transform.parent = transform;
    //        tempScript.portalID = i;
    //        tempScript.isForwardTeleporter = true;
    //        //tempScript.cameraOffset = cameraOffset;
    //        nextProjectionQuadArray[i] = tempScript.projectionQuad.gameObject;
    //        nextRenderQuadArray[i] = tempScript.renderQuad.gameObject;
    //        tempScript.renderQuad.GetComponent<Renderer>().material = Resources.Load("Materials/Next" + (isStereoscopic ? "Stereo" : "Mono")) as Material;

    //        //we could find a way to remove the redundancy here

    //        tempPos = new Vector3(mapSequence[i + 1].mapObject.transform.position.x + currentPortal.column * portalWidth,
    //            mapSequence[i + 1].mapObject.transform.position.y,
    //            mapSequence[i + 1].mapObject.transform.position.z - currentPortal.row * portalWidth);

    //        tempPortal = Instantiate(portalPrefab, tempPos, Quaternion.identity);

    //        tempScript = tempPortal.GetComponent<Teleporter>();
    //        bc = tempScript.renderQuad.GetComponent<BoxCollider>();

    //        tempPortal.transform.Rotate(0f, 90f * currentPortal.direction, 0f);
    //        tempPortal.transform.Translate(0, 0, portalWidth / 2f - pillarOffset, Space.Self);
    //        tempScript.projectionQuad.Translate(-cameraOffset, 0, 0, Space.World);

    //        tempScript.renderQuad.transform.localScale -= new Vector3((1 - portalWidth) + pillarOffset * 2f, 0, 0);
    //        tempScript.projectionQuad.transform.localScale -= new Vector3((1 - portalWidth) + pillarOffset * 2f, 0, 0);
    //        bc.center = new Vector3(0, 0, -portalWidth / 2f + pillarOffset);

    //        tempPortal.name = "Back Teleporter " + i;
    //        tempScript.renderQuad.name = "Back Render Quad " + i;
    //        tempScript.projectionQuad.name = "Back Projection Quad " + i;
    //        tempPortal.transform.parent = transform;
    //        tempScript.portalID = i;
    //        tempScript.isForwardTeleporter = false;
    //        //tempScript.cameraOffset = cameraOffset;
    //        prevProjectionQuadArray[i] = tempScript.projectionQuad.gameObject;
    //        prevRenderQuadArray[i] = tempScript.renderQuad.gameObject;
    //        tempScript.renderQuad.GetComponent<Renderer>().material = Resources.Load("Materials/Next" + (isStereoscopic ? "Stereo" : "Mono")) as Material;
    //}

    void InitializePortals()
    {
        //Debug.Log("Portals will go here:");
        for (int i = 0; i < mapSequence.Length - 1; i++)
        {
            SetCameraOffsets(i, true);
            PortalSetup(true, i);
            PortalSetup(false, i);
        }

    }

    public void OffsetCameras(int currentMaze)
    {
        //previousPortalCameraLeftEye.GetComponent<FollowCam>().SetOffset(-cameraOffset);
        //previousPortalCameraRightEye.GetComponent<FollowCam>().SetOffset(-cameraOffset);

        SetCameraOffsets(currentMaze);

        //Debug.Log("nextOffset: " + nextOffset);
        //Debug.Log("prevOffset: " + prevOffset);

        nextPortalCameraLeftEye.GetComponent<FollowCam>().SetOffsets(nextOffset, prevOffset);
        nextPortalCameraRightEye.GetComponent<FollowCam>().SetOffsets(nextOffset, prevOffset);

        if (isStereoscopic)
        {
            //previousPortalCameraLeftEye.GetComponent<FollowCam>().isStereoscopic = true;
            //previousPortalCameraRightEye.GetComponent<FollowCam>().isStereoscopic = true;
            nextPortalCameraLeftEye.GetComponent<FollowCam>().isStereoscopic = true;
            nextPortalCameraRightEye.GetComponent<FollowCam>().isStereoscopic = true;
        }

    }

    public void SetProjectionQuads(bool dir) // true = next, false = prev
    {
        if (dir)
        {
            nextPortalCameraRightEye.projectionScreen = nextProjectionQuadArray[TrueModulus(currentMaze, portalCount)];
            if (isStereoscopic)
            {
                nextPortalCameraLeftEye.projectionScreen = nextProjectionQuadArray[TrueModulus(currentMaze, portalCount)];
            }
        }
        else
        {
            nextPortalCameraRightEye.projectionScreen = prevProjectionQuadArray[TrueModulus(currentMaze - 1, portalCount)];
            if (isStereoscopic)
            {
                nextPortalCameraLeftEye.projectionScreen = prevProjectionQuadArray[TrueModulus(currentMaze - 1, portalCount)];
            }
        }
    }

    public void TeleportPlayer(int mazeID)
    {
        currentMaze = mazeID;
        //SetProjectionQuads(true);
        //Debug.Log("Teleported player to: " + currentMaze);
    }

    int TrueModulus(int k, int n)
    {
        return ((k %= n) < 0) ? k + n : k;
    }

    public void SetCameraOffsets(int currentMaze, bool initializePhase = false)
    {
        nextOffset = SetNextOffset(currentMaze);
        if (initializePhase)
            prevOffset = SetPrevOffset(currentMaze + 1);
        else
            prevOffset = SetPrevOffset(currentMaze);
    }
    static public Vector3 SetNextOffset(int currentMaze)
    {
        Vector3 offset;
        int nextMaze = currentMaze + 1;

        if (nextMaze <= mapSequence.Length-1) // if there is a next maze
            offset = mapSequence[nextMaze].mapObject.transform.position - mapSequence[currentMaze].mapObject.transform.position; // TODO: prev and next seems to be mixed around?
        else
            offset = mapSequence[nextMaze-1].mapObject.transform.position - mapSequence[currentMaze-1].mapObject.transform.position; // TODO do not what to set it to

        return offset;
    }

    static public Vector3 SetPrevOffset(int currentMaze)
    {
        Vector3 offset;
        int prevMaze = currentMaze - 1;

        if (prevMaze >= 0) // if there is a previous maze 
            offset = mapSequence[prevMaze].mapObject.transform.position - mapSequence[currentMaze].mapObject.transform.position; // TODO: prev and next seems to be mixed around?
        else
            offset = mapSequence[prevMaze+1].mapObject.transform.position - mapSequence[currentMaze+1].mapObject.transform.position;  // TODO do not what to set it to

        return offset;
    }
}
