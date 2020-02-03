/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGenerator : MonoBehaviour
{

   public bool steamVRIsOn = false;
    private int wallWidth;
    private int mazeWidth;
    public GameObject portalPairPrefab;
    private GameObject[] portalPairArr;

    private EventCallbacks.TerrainGenerator tg;


    public int[] testingRows;
    public int[] testingCols;
    public int[] testingDirs;
    public int testingWides;

    void Start()
    {
        tg = GameObject.Find("TileGenerator").GetComponent<EventCallbacks.TerrainGenerator>();
    }

    public void GeneratePortals(TileInfo[] pi, float tileWidth, float wallWidth) // Generate the portal pairs at the specified locations around the mazes. Takes arrays of entrance rows and cols + directions
    {
        MapManager mapGenScript = GameObject.Find("MapManager").GetComponent<MapManager>();   
        mazeWidth = mapGenScript.mazeCols*(int)tileWidth;
        //float tileWidthFloat = tileWidth;       // Convert int to float

        portalPairArr = new GameObject[mapGenScript.mazeCount-1]; // set lenght of portalPairArr to the amount of Pairs needed

        for (int i = 0; i < mapGenScript.mazeCount-1; i++) // Must be mazes -1, as one pair is reserved for room exit and entrance. 
        {

            //entrance portal tranform
            GameObject mazeObject = GameObject.Find("MapManager/" + i + " - Maze");  // Find specific maze            
            GameObject nextMazeObject = GameObject.Find("MapManager/" + (i + 1) + " - Maze" );    // Find specific maze + 1
            Transform transformHelper = transform; // for calculating the proper portal position as it depends on an object's forward vector. set to transform temporally 

            //set the transform of the transformHelper
            Vector3 entrancePortalScale = new Vector3(tileWidth, tg.wallHeight,tileWidth);
            transformHelper.localScale = entrancePortalScale;
            

            Quaternion entrancePortalRotation = Quaternion.Euler(0, 90 * pi[i].direction+180, 0);
            transformHelper.rotation = entrancePortalRotation; //give transformHelper the correct rotation

            Vector3 entrancePortalPosition = new Vector3(
                mazeObject.transform.position.x + tileWidth * pi[i].column,
                 tg.wallHeight/2,
                mazeObject.transform.position.z - tileWidth * pi[i].row); //positioning the portal at the center of the dead end tile
            entrancePortalPosition += -transformHelper.forward * ((tileWidth / 2.0f)-wallWidth);//set the portal at the correct position within the tile
            Debug.Log("tileWidth "+tileWidth);
            Debug.Log("wallWidth " + wallWidth);
            transformHelper.position = entrancePortalPosition; //give transformHelper the correct position
            
            Vector3 mazeOffset = nextMazeObject.transform.position - mazeObject.transform.position; //distance between to adjadent mazes

            GameObject newPair = Instantiate(portalPairPrefab,new Vector3(0,0,0),Quaternion.Euler(0, 0, 0),transform); //instantiate new portalPair and set the parent to be the portal generator
            newPair.name = "PortalPair " +i;
            PortalPair pp = newPair.GetComponent<PortalPair>();
            pp.PortalPairConstructor(transformHelper, mazeOffset, steamVRIsOn); 
            portalPairArr[i] = newPair; 

        }
    }
}
 */