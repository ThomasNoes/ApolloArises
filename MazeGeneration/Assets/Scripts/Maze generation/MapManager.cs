using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject[] mazeGeneratorPrefab;
    public Transform playerHead;
    GameObject tempMap;

    public int mazeRows;
    public int mazeCols;
    public float tileWidth = 1f;
    public int startRow;
    public int startCol;

    public Vector3 playAreaSize;
    public enum PortalGenerationType
    {
        Everywhere,
        Hallways
    }
    private int minMazeSize;
    public bool isMapSeeded;
    public int randomGeneratorSeed;
    public PortalGenerationType portalGenerationLocation;
    public TileInfo[] portalInfo;
    public MapInfo[] mapSequence;

    /*
    *   IF YOU ADD ROOMS, THEY HAVE TO ALTERNATE WITH MAZES!!!
    *   IF YOU ADD ROOMS, THEY HAVE TO ALTERNATE WITH MAZES!!!
    *   IF YOU ADD ROOMS, THEY HAVE TO ALTERNATE WITH MAZES!!!
    *   IF YOU ADD ROOMS, THEY HAVE TO ALTERNATE WITH MAZES!!!
    *   IF YOU ADD ROOMS, THEY HAVE TO ALTERNATE WITH MAZES!!!
     */

    void Awake()
    {
        if (isMapSeeded)
            Random.InitState(randomGeneratorSeed);
        minMazeSize = SetMinMazeSize(); //3x3 is the minimum size, 4x4 if there are rooms as well.
        if (mazeRows < minMazeSize)
            mazeRows = minMazeSize;
        if (mazeCols < minMazeSize)
            mazeCols = minMazeSize;

        if (mapSequence.Length > 1) // if map sequence is more than 1, it means the maps needs to 
            portalInfo = new TileInfo[mapSequence.Length - 1];

        //playAreaSize = GetCameraRigSize();
        GetStartSeedFromPlayerPosition(out startCol, out startRow);

        if (startRow < 0 || startRow >= mazeRows || startCol < 0 || startCol >= mazeCols || isMapSeeded) //if map is seeded maze starts from 0;0
        {
            startRow = 0;
            startCol = 0;
            //Debug.Log("Player was out of game area, Maze starts from (0;0).");
        }
        if (portalGenerationLocation == PortalGenerationType.Everywhere)
        {
            GenerateMapSequence();
        }
        else
        {
            GenerateMapSequenceHallway();
        }

        OffsetMap();
        /*
        //This is to debug the portal infos in the console
        for (int i = 0; i < portalInfo.Length; i++)
        {
            Debug.Log("pp for maze: " + i + " r: " + portalInfo[i].row + " c: " + portalInfo[i].column + " d: " + portalInfo[i].direction);
        }
        */
        //maybe add script to find player head so we don't have to drag it in
    }

    void GenerateMapSequence() // is not called since we can not 
    {
        if (mapSequence.Length > 0)
        {
            mapSequence[0].startSeed.row = startRow;
            mapSequence[0].startSeed.column = startCol;
            mapSequence[0].startSeed.direction = GenerateRandomStartDirection(startRow, startCol);
        }

        for (int i = 0; i < mapSequence.Length; i++)
        {
            Vector3 mapSpawnPoint = new Vector3(transform.position.x + i * (mazeCols * tileWidth + 1), 0, 0);
            tempMap = Instantiate(mazeGeneratorPrefab[(int)mapSequence[i].mapType], mapSpawnPoint, Quaternion.identity);
            tempMap.name = i.ToString() + " - " + mapSequence[i].mapType.ToString();
            tempMap.transform.parent = transform;

            MapGenerator mapScript = tempMap.GetComponent<MapGenerator>();
            mapScript.SetDimensions(mazeRows, mazeCols, tileWidth);
            mapScript.Initialize();

            //calculate start seed
            if (i > 0)
            {
                mapSequence[i].startSeed = new TileInfo(mapSequence[i - 1].endSeed);
                mapSequence[i].startSeed.direction = PortalPositionHelper.GetRandomPortalExit(mapSequence[i].startSeed.row, mapSequence[i].startSeed.column, mapSequence[i - 1].endSeed.direction);
            }
            if ((int)mapSequence[i].mapType == 1)
            {
                if (!((mapSequence[i].startSeed.row == 0 || mapSequence[i].startSeed.row == mazeRows - 1) && (mapSequence[i].startSeed.column == 0 || mapSequence[i].startSeed.column == mazeCols - 1)))
                {
                    mapSequence[i].startSeed.row = 0;
                    mapSequence[i].startSeed.column = 0;
                    mapSequence[i].startSeed.direction = PortalPositionHelper.GetRandomPortalExit(mapSequence[i].startSeed.row, mapSequence[i].startSeed.column);
                }
            }
            else
            {
                if (i + 1 < mapSequence.Length && (int)mapSequence[i + 1].mapType == 1) //Change this so we can use the enum
                {
                    mapSequence[i].isEndSeeded = true;
                    mapSequence[i].endSeed = GenerateRandomCorner(mapSequence[i].startSeed); //this will introduce errors if they are next to each other, need to fix
                }

            }
            mapScript.Generate(mapSequence[i]);
            if (mapSequence[i].isEndSeeded == false)
                mapSequence[i].endSeed = mapScript.GetRandomDeadEnd(mapSequence[i].startSeed);
            if (i < portalInfo.Length)
                portalInfo[i] = new TileInfo(mapSequence[i].endSeed);
        }
    }

    void GenerateMapSequenceHallway()
    {
        if (mapSequence.Length > 0)
        {
            mapSequence[0].startSeed.row = startRow;
            mapSequence[0].startSeed.column = startCol;
            mapSequence[0].startSeed.direction = GenerateRandomStartDirection(startRow, startCol);
        }

        for (int i = 0; i < mapSequence.Length; i++)
        {
            //Debug.Log("Starting Maze " + i);
            Vector3 mapSpawnPoint = new Vector3(transform.position.x + i * (mazeCols * tileWidth + 1), 0, 0);
            tempMap = Instantiate(mazeGeneratorPrefab[(int)mapSequence[i].mapType], mapSpawnPoint, Quaternion.identity);
            tempMap.name = i.ToString() + " - " + mapSequence[i].mapType.ToString();
            tempMap.transform.parent = transform;

            MapGenerator mapScript = tempMap.GetComponent<MapGenerator>();
            mapScript.SetDimensions(mazeRows, mazeCols, tileWidth);
            mapScript.Initialize();
            //Debug.Log("Maze " + i + " Initialized!");

            //calculate start seed
            if (i > 0) // setting the start seed in each maze segment that is not the first one.
            {
                mapSequence[i].startSeed = new TileInfo(mapSequence[i - 1].endSeed);
                mapSequence[i].startSeed.direction = (mapSequence[i - 1].endSeed.direction + 2) % 4; //rotate 180 degrees

                //mapSequence[i].startSeed.direction = PortalPositionHelper.GetRandomPortalExit(mapSequence[i].startSeed.row, mapSequence[i].startSeed.column, mapSequence[i - 1].endSeed.direction);
            }
            if ((int)mapSequence[i].mapType == 1) // if the current map is a room
            {
                TileInfo startNoDir = new TileInfo(mapSequence[i].startSeed.row, mapSequence[i].startSeed.column, -1);
                List<TileInfo> possibleStarts = PortalPositionHelper.GetAllCornerTiles();
                if (possibleStarts.Contains(startNoDir) == false)
                {
                    //Debug.Log("Start seed not possible for rooms, generating room at (0,1,1)");
                    mapSequence[i].startSeed = new TileInfo(0, 1, 3);
                }
            }
            else
            {
                //if the current is maze and next is room we need to set the end seed
                if (i + 1 < mapSequence.Length && (int)mapSequence[i + 1].mapType == 1) //Change this so we can use the enum
                {
                    //Debug.Log("Can't do rooms with this method yet...");
                    i++;
                }
            } 
            if (i + 1 < mapSequence.Length) // if we are not at the last maze segments yet.
            {
                mapSequence[i].isEndSeeded = true;
                //Debug.Log("Generating End Seed For Maze " + i);
                if ((int)mapSequence[i + 1].mapType == 1) // if the next maze segment is a room
                    mapSequence[i].endSeed = GenerateRandomRoomDeadEnd(mapSequence[i].startSeed);
                else
                    mapSequence[i].endSeed = GenerateRandomHallwayDeadEnd(mapSequence[i].startSeed, i); 
            }
            mapScript.Generate(mapSequence[i]);
            //if (mapSequence[i].isEndSeeded == false)
            if (i + 1 < mapSequence.Length && mapSequence[i + 1].mapType == 0) // if we are not at the last maze segment and the next segment is a maze segment
                mapSequence[i].endSeed = mapScript.GetRandomDeadEndHallway(mapSequence[i].startSeed);

            //A star Path Finding
            if (i == 0)
            {
                //we need a start position
            }
            else if (i == mapSequence.Length-1)
            {
                //we need a end destination
            }
            else
            {
                AStarPathFinding aStar = new AStarPathFinding();
                aStar.BeginAStar(mapScript.tileArray, mapSequence[i].startSeed, mapSequence[i].endSeed);
            }


            if (i < portalInfo.Length)
                portalInfo[i] = new TileInfo(mapSequence[i].endSeed);
        }
    }

    int GenerateRandomStartDirection(int row, int col)
    {
        return PortalPositionHelper.GetRandomPortalExit(row, col);
    }

    TileInfo GenerateRandomRoomDeadEnd(TileInfo flag)
    {
        TileInfo startCoord = new TileInfo(flag.row, flag.column, -1);
        List<TileInfo> possibleCoordinates = PortalPositionHelper.GetAllCornerTiles();

        foreach (var v in possibleCoordinates)
            v.PrintTile();

        ////Remove Start
        //if(possibleCoordinates.Remove(startCoord))
        //    Debug.Log("Removed Start (" + startCoord.row + ";" + startCoord.column + ")");

        ////Remove Lead-in
        //if (possibleCoordinates.Remove(flag.GetNeighbourCoord()))
        //    Debug.Log("Removed Lead-in (" + flag.GetNeighbourCoord().row + ";" + flag.GetNeighbourCoord().column + ")");

        //Remove corner shutoffs
        List<TileInfo> shutoffCorners = PortalPositionHelper.GetShutoffList(startCoord);
        //if (shutoffCorners.Count > 0)
        //{
        //    Debug.Log("Start (" + startCoord.row + ";" + startCoord.column + ") Shuts off corners.");
        //    foreach (TileInfo t in shutoffCorners)
        //    {
        //        if (possibleCoordinates.Remove(t))
        //            Debug.Log("(" + t.row + ";" + t.column + ") Removed.");
        //    }
        //}

        // Generate directions
        // These are the Dead-end directions in the maze befor the room so they have to point
        // the opposite direction from where we want the room's entrance corridor to start!
        List<TileInfo> possibleTiles = new List<TileInfo>();
        foreach (TileInfo t in possibleCoordinates)
        {
            int dir = 0;
            if (t.row == 0 || t.row == mazeRows - 1)
            {
                if (t.column < mazeCols / 2)
                    dir = 1; //Roight
                else
                    dir = 3; //Left
            }
            else if (t.column == 0 || t.column == mazeCols - 1)
            {
                if (t.row < mazeRows / 2)
                    dir = 2; //Doon
                else
                    dir = 0; //Oop
            }
            possibleTiles.Add(new TileInfo(t.row, t.column, dir));
        }

        List<TileInfo> tilesToRemove = new List<TileInfo>();
        foreach (TileInfo t in possibleTiles)
        {
            if (t.IsLeadingIntoEntrance(flag))
            {
                tilesToRemove.Add(t);
                //Debug.Log("Tile (" + t.row + ";" + t.column + ";" + t.direction + ") leads into entrance");
            }
        }

        foreach (TileInfo t in tilesToRemove)
        {
            if (possibleTiles.Remove(t))
            {
                //Debug.Log("Tile (" + t.row + ";" + t.column + ";" + t.direction + ") removed");
            }
        }

        int idx = Random.Range(0, possibleTiles.Count);
        return possibleTiles[idx];
    }

    TileInfo GenerateRandomHallwayDeadEnd(TileInfo flag, int mazeSegment)
    {

        //Geeting all tile position in a maze segment 
        TileInfo startCoord = new TileInfo(flag.row, flag.column, -1);
        List<TileInfo> possibleCoordinates = new List<TileInfo>();
        for (int i = 0; i < mazeRows; i++)
        {
            for (int j = 0; j < mazeCols; j++)
            {
                possibleCoordinates.Add(new TileInfo(i, j, -1));
            }
        }

        //Removing entire tiles
        possibleCoordinates.Remove(startCoord); //Remove Start

        possibleCoordinates.Remove(flag.GetNeighbourCoord()); //Remove Lead-in

        for (int i = 0; i < 2; i++) //Remove corners
        {
            for (int j = 0; j < 2; j++)
            {
                TileInfo cornerTile = new TileInfo(i * (mazeRows - 1), j * (mazeCols - 1), -1);
                possibleCoordinates.Remove(cornerTile);
                //    Debug.Log("Removed corner (" + cornerTile.row + ";" + cornerTile.column + ")");
            }
        }

        if(!(mazeCols <= 3 && mazeRows <=3)) //if the maze is 3x3 or smaller, there is not space for small room unless we allow for shutting of corners
        {
            List<TileInfo> shutoffCorners = PortalPositionHelper.GetShutoffList(startCoord); //Remove corner shutoffs
            if (shutoffCorners.Count > 0)
            {
            //    Debug.Log("Start (" + startCoord.row + ";" + startCoord.column + ") Shuts off corners.");
                foreach (TileInfo t in shutoffCorners)
                {
                    possibleCoordinates.Remove(t);
            //            Debug.Log("(" + t.row + ";" + t.column + ") Removed.");
                }
            }
        }
        

        //Now we have all tiles that a portal can be placed on.
        //Now we include the possible directions the portal can have on the remaining tiles.
        List<TileInfo> possibleTiles = new List<TileInfo>();
        foreach (TileInfo t in possibleCoordinates)
        {
            int[] possibleDirections = PortalPositionHelper.GetEntranceArray(t.row, t.column); //each possible direction based on the placement the tile has in the maze segment.
            for (int i = 0; i < possibleDirections.Length; i++)
            {
                TileInfo tileToAdd = new TileInfo(t.row, t.column, possibleDirections[i]); 
                
                possibleTiles.Add(tileToAdd); // add multiple tiles in the same position with different directions
            }
        }

       
        List<TileInfo> tilesToRemove = new List<TileInfo>(); //list of tiles with certain directions that will be removed.

        foreach (TileInfo t in possibleTiles)
        {

            if (t.IsPerpendicular()) //Remove direction that are perpendicular to maze edges  
            {
                //Instantiate(debugGizmo, new Vector3(transform.position.x + t.column * tileWidth, 0, transform.position.z - t.row * tileWidth), Quaternion.identity);
                tilesToRemove.Add(t);          
            }
            else if (t.IsLeadingIntoEntrance(flag)) //Remove directions that lead into reserved tile next to entrance
            {
                //Debug.Log("removed " + t.TileToString());
                tilesToRemove.Add(t);
            }
            else if (t.IsVisibleThroughEntrance(flag))
            {
                //Debug.Log("entrance is "+ flag.TileToString() +  " removed " + t.TileToString());
                //Debug.Log("Maze Segment " + mazeSegment);
                tilesToRemove.Add(t);
            }
            else if (t.IsAdjacentwithSameDirection(flag))
            {
                //Debug.Log("entrance is "+ flag.TileToString() +  " removed " + t.TileToString());
                //Debug.Log("Maze Segment " + mazeSegment);
                tilesToRemove.Add(t);
            }

            // remove some based on start coordinate (called flag)
            //Remove exit tiles that are visible through entrance portal
            //Remove exit tiles that are in view of the entrance portal
        }

        

        foreach (TileInfo t in tilesToRemove)
        {
            possibleTiles.Remove(t);

        }

        



        //Debug.Log("All possible Tiles\n---------------------");
        //foreach (TileInfo t in possibleTiles)
        //{
        //    Debug.Log("(" + t.row + ";" + t.column + ";" + t.direction + ")");
        //}

        int idx = Random.Range(0, possibleTiles.Count);
        return possibleTiles[idx];
    }

    
    TileInfo GenerateRandomCorner(TileInfo flag) // why do we want a corner
    {
        int row, col, dir;
        do
        {
            row = Mathf.RoundToInt(Random.value) * (mazeRows - 1);
            col = Mathf.RoundToInt(Random.value) * (mazeCols - 1);
            dir = GenerateRandomStartDirection(row, col);
        }
        while (row == flag.row && col == flag.column);
        return new TileInfo(row, col, dir);
    }

    public Vector3 GetCameraRigSize()
    {
        Vector3 size = new Vector3(playAreaSize.x, 0, playAreaSize.z);
        Vector3 chaperone = OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea);

        if (chaperone != null)
        {
            Debug.Log(chaperone);
            size = new Vector3(Mathf.Round(chaperone.x), 0, Mathf.Round(chaperone.z));
        }
        return size;
    }

    void OffsetMap()
    {
        transform.Translate(-playAreaSize.x / 2f + tileWidth / 2f, 0, playAreaSize.z / 2f - tileWidth / 2f);
    }

    void GetStartSeedFromPlayerPosition(out int col, out int row)
    {
        col = Mathf.RoundToInt(Mathf.Abs((playerHead.position.x - (-playAreaSize.x / 2f + tileWidth / 2f)) / tileWidth));
        row = Mathf.RoundToInt(Mathf.Abs((playerHead.position.z - (playAreaSize.z / 2f - tileWidth / 2f)) / tileWidth));

        return;
    }

    int SetMinMazeSize()
    {
        foreach (MapInfo m in mapSequence)
        {
            if ((int)m.mapType == 1)
                return 4;
        }
        return 3;
    }
}
