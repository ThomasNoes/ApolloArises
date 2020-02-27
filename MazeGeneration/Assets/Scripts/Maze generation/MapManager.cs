using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public enum MazePlacementType
    {
        OrderedAlongX,
        orderedAlongXProportionally,
        orderedDiagonally,
        randomButIncreasesAlongY
    }
    List<Vector3> spawnPoints = new List<Vector3>();

    public enum PortalGenerationType
    {
        Everywhere,
        Hallways
    }

    public GameObject gizmo;

    public GameObject[] mazeGeneratorPrefab;
    public bool usePlayAreaCenter, setDimensionsAutomatically;
    public MazePlacementType MazePlacement;
    public bool StartMazeInOrigin;

    private Vector3 minimumBound = Vector3.zero;
    private Vector3 maximumBound;

    public Transform playerHead;
    private Vector3 playAreaCenter;
    GameObject tempMap;

    public int mazeRows;
    public int mazeCols;
    public float tileWidth = 0.8f;
    public int startRow;
    public int startCol;

    public Vector3 playAreaSize;


    private int minMazeSize;
    public bool isMapSeeded;
    public int randomGeneratorSeed;
    public PortalGenerationType portalGenerationLocation;
    public TileInfo[] portalInfo;
    public MapInfo[] mapSequence;

    void Awake()
    {
        #if UNITY_ANDROID
        if (!Application.isEditor)
        {
            playAreaSize = GetCameraRigSize();

            if (setDimensionsAutomatically)
            {
                mazeRows = Mathf.RoundToInt(playAreaSize.x / tileWidth);
                mazeCols = Mathf.RoundToInt(playAreaSize.z / tileWidth);
            }
        }
#endif

        if (isMapSeeded)
            Random.InitState(randomGeneratorSeed);
        minMazeSize = SetMinMazeSize(); //3x3 is the minimum size, 4x4 if there are rooms as well.
        if (mazeRows < minMazeSize)
            mazeRows = minMazeSize;
        if (mazeCols < minMazeSize)
            mazeCols = minMazeSize;

        if (mapSequence.Length > 1) // if map sequence is more than 1, it means the maps needs to 
            portalInfo = new TileInfo[mapSequence.Length - 1];

        if (usePlayAreaCenter)
            GetStartSeedFromPlayerPosition(out startCol, out startRow, false);
        else
            GetStartSeedFromPlayerPosition(out startCol, out startRow, true);

        if (startRow < 0 || startRow >= mazeRows || startCol < 0 || startCol >= mazeCols || isMapSeeded) //if map is seeded maze starts from 0;0
        {
            startRow = 0;
            startCol = 0;
            //Debug.Log("Player was out of game area, Maze starts from (0;0).");
        }

            GenerateMapSequenceHallway();

        OffsetMap();

        //maybe add script to find player head so we don't have to drag it in
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
            switch (MazePlacement)
            {
                case MazePlacementType.OrderedAlongX:
                    PlaceOrderedAlongX(i);
                    break;
                case MazePlacementType.orderedAlongXProportionally:
                    PlaceOrderedAlongXProportionally(i);
                    break;
                case MazePlacementType.orderedDiagonally:
                    PlaceOrderedDiagonally(i);
                    break;
                case MazePlacementType.randomButIncreasesAlongY:
                    PlaceRandomButIncreaseY(i, mapSequence.Length-1);
                    break;
                default:
                    break;
            }

            tempMap.name = i.ToString() + " - " + mapSequence[i].mapType.ToString();
            tempMap.transform.parent = transform;
            mapSequence[i].mapObject = tempMap;
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
                    mapSequence[i].endSeed = GenerateRandomHallwayDeadEnd(mapSequence[i].startSeed, i); 
            }
            mapScript.Generate(mapSequence[i], i, mapSequence.Length);
            //if (mapSequence[i].isEndSeeded == false)
            if (i + 1 < mapSequence.Length && mapSequence[i + 1].mapType == 0) // if we are not at the last maze segment and the next segment is a maze segment
                mapSequence[i].endSeed = mapScript.GetRandomDeadEndHallway(mapSequence[i].startSeed);



            Debug.Log("A star here");
            AStarPathFinding aStar = new AStarPathFinding();
            //A star Path Finding
            if (i == 0)
            {
                //we need a start position
            }
            else if (i == mapSequence.Length - 1)
            {
                //we need a end destination
            }
            else
            {

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

        possibleCoordinates.Remove(flag.GetLeadInCoord()); //Remove Lead-in
        for (int i = 0; i < 2; i++) //Remove corners
        {
            for (int j = 0; j < 2; j++)
            {
                TileInfo cornerTile = new TileInfo(i * (mazeRows - 1), j * (mazeCols - 1), -1);
                possibleCoordinates.Remove(cornerTile);
                //    Debug.Log("Removed corner (" + cornerTile.row + ";" + cornerTile.column + ")");
            }
        }

        List<TileInfo> shutoffCorners = PortalPositionHelper.GetShutoffList(startCoord); //Remove corner shutoffs
        //if (shutoffCorners.Count > 0) // &&
            //mazeRows > 3 && mazeCols > 3)
        //{
        //    foreach (TileInfo soc in shutoffCorners)
        //    {
        //        possibleCoordinates.Remove(soc);
        //    }
        //}

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
            else if (t.IsLeadingIntoEntrance(flag)) //Remove directions that lead into reserved tile next to entrance this tile 
            {
                //Debug.Log("removed " + t.TileToString());
                tilesToRemove.Add(t);
            }
            else if (t.IsVisibleThroughEntrance(flag))
            {
                //Debug.Log("entrance is "+ flag.TileToString() +  " removed " + t.TileToString());
                tilesToRemove.Add(t);
            }
            else if (t.BothPortalsVisibleFromLeadIn(flag))
            {
                //Debug.Log(flag.TileToString() + " removed " + t.TileToString() + "maze: " + mazeSegment);
                tilesToRemove.Add(t);
            }
            else if (t.IsAdjacentLaneWithSameDirection(flag))
            {
                //Debug.Log("entrance is "+ flag.TileToString() +  " removed " + t.TileToString());
                //Debug.Log("Maze Segment " + mazeSegment);
                tilesToRemove.Add(t);
            }

            if (shutoffCorners.Count > 0 )
            {
                TileInfo behindFlag = flag.GetBehindCoord();
                foreach (TileInfo soc in shutoffCorners)
                {
                    if (t.IsSamePosition(soc))
                    {
                        TileInfo behindT = t.GetBehindCoord();

                        if (!behindFlag.IsSamePosition(behindT))
                        {
                            tilesToRemove.Add(t);
                        }
                    }
                }
            }

            // remove some based on start coordinate (called flag)
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
        playAreaCenter = new Vector3(chaperone.x / 2.0f, 0.0f, chaperone.z / 2.0f);

        if (chaperone != null)
        {
            size = new Vector3(Mathf.Round(chaperone.x), 0, Mathf.Round(chaperone.z));
        }
        return size;
    }

    void OffsetMap()
    {
        transform.Translate((-playAreaSize.x / 2f) / 2f, 0, (playAreaSize.z / 2f) / 2f);
    }

    void GetStartSeedFromPlayerPosition(out int col, out int row, bool usePlayerPos)
    {
        if (usePlayerPos)
        {
            col = Mathf.RoundToInt(Mathf.Abs((playerHead.position.x - (-playAreaSize.x / 2f + tileWidth / 2f)) / tileWidth));
            row = Mathf.RoundToInt(Mathf.Abs((playerHead.position.z - (playAreaSize.z / 2f - tileWidth / 2f)) / tileWidth));
        }
        else
        {
            col = Mathf.RoundToInt(Mathf.Abs((playAreaCenter.x - (-playAreaSize.x / 2f + tileWidth / 2f)) / tileWidth));
            row = Mathf.RoundToInt(Mathf.Abs((playAreaCenter.z - (playAreaSize.z / 2f - tileWidth / 2f)) / tileWidth));
        }

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

    private Vector3 SetMaximumBound(float openness)
    {
        Vector3 returnVector;

        //finding the minimum space required
        float x = mazeCols * tileWidth * mapSequence.Length;
        float y = mapSequence.Length * 1.8f; //i will assume the average person is 1.8  meter 
        float z= mazeRows * tileWidth * mapSequence.Length;

        returnVector = new Vector3(x,y,z);

        returnVector *= openness; //multiply with openness for how open/bare you towers to be 

        return returnVector;
    }

    private bool OverlapCheck(Vector3 spawnPoint)
    {
        float minDistance = Mathf.Sqrt(mazeCols * mazeCols + mazeRows * mazeRows); // i treat the segment as a triablge and find the hypothenuse

        foreach  (Vector3 point in spawnPoints)
        {
            spawnPoint.y = point.y; // i ignore y by have the two points share y.
            if (Vector3.Distance(spawnPoint, point) < minDistance)
            {
                Debug.Log("it overlaps!");
                return true;
            }
        }
        return false;
    }

    private void PlaceOrderedAlongX(int index)
    {
        Vector3 mapSpawnPoint = new Vector3(transform.position.x + index * (mazeCols * tileWidth + 1), 0, 0);
        tempMap = Instantiate(mazeGeneratorPrefab[(int)mapSequence[index].mapType], mapSpawnPoint, Quaternion.identity);
        //Debug.Log("maze position is "+ tempMap.transform.position);
    }
    private void PlaceOrderedAlongXProportionally(int index)
    {
        Vector3 mapSpawnPoint = new Vector3((transform.position.x + index) * index * (mazeCols * tileWidth + 1), 0, (transform.position.z + index) * index * (mazeCols * tileWidth + 1));
        tempMap = Instantiate(mazeGeneratorPrefab[(int)mapSequence[index].mapType], mapSpawnPoint, Quaternion.identity);
        //Debug.Log("maze position is "+ tempMap.transform.position);
    }

    private void PlaceOrderedDiagonally(int index)
    {
        float distance = transform.position.x + index * (mazeCols * tileWidth + 1);
        Vector3 mapSpawnPoint = new Vector3(distance, distance, distance);
        tempMap = Instantiate(mazeGeneratorPrefab[(int)mapSequence[index].mapType], mapSpawnPoint, Quaternion.identity);
    }
    private void PlaceRandomButIncreaseY(int index, int length)
    {
        maximumBound = SetMaximumBound(3);

        Instantiate(gizmo, minimumBound, Quaternion.identity);
        Instantiate(gizmo, maximumBound, Quaternion.identity);

        //find the y increase
        float yRatio = (float)index / length;

        float y = (1.0f - yRatio) * minimumBound.y + yRatio * maximumBound.y;

        Vector3 mapSpawnPoint = Vector3.zero;

        if (index == 0)
        {
            if (StartMazeInOrigin)
            {
                mapSpawnPoint = Vector3.zero;
            }
            else
            {
                mapSpawnPoint = (minimumBound + maximumBound) / 2;
                mapSpawnPoint.y = y;
            }

        }
        else
        {
            bool itOverlaps = true;
            while(itOverlaps)
            {
                float x = Random.Range(minimumBound.x, maximumBound.x);
                float z = Random.Range(minimumBound.z, maximumBound.z);
                mapSpawnPoint = new Vector3(x, y, z);
                itOverlaps = OverlapCheck(mapSpawnPoint);
            }

        }
        tempMap = Instantiate(mazeGeneratorPrefab[(int)mapSequence[index].mapType], mapSpawnPoint, Quaternion.identity);
        spawnPoints.Add(mapSpawnPoint);
    }

}
