using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class MapGenerator : MonoBehaviour
{
    public int mazeRows; // size of maze
    public int mazeColumns; // size of maze
    public float tileWidth;
    public GameObject tilePrefab;
    public Tile[,] tileArray; //
    public int[,] mazeIntArray;
    public bool[,] pillarBoolArray;
    public List<Tile> aStarTiles;
    public int mazeCount;

    public void Initialize(int index)
    {
       /* MaterialSwitcher(index);*/ // Texture switch

        tileArray = new Tile[mazeRows, mazeColumns];
        mazeCount = index;
        //float mazeHalfWidth = mazeRows / 2f; // Add scalability with tile width!
        //float mazeHalfHeight = mazeColumns / 2f; // Add scalability with tile height!
        for (int i = 0; i < mazeRows; i++) // nested forloop for positioning each tile in the individual mazes
        {
            for (int j = 0; j < mazeColumns; j++)
            {
                Vector3 tileSpawnPosition = new Vector3(transform.position.x + j * tileWidth, 0, transform.position.z - i * tileWidth);
                GameObject emptyTile = Instantiate(tilePrefab, tileSpawnPosition, Quaternion.identity);
                //emptyTile.name = "Tile " + (mazeColumns * i + j).ToString();
                emptyTile.name = "Tile R" + i + "C" + j;
                emptyTile.transform.parent = transform;
                tileArray[i, j] = emptyTile.GetComponent<Tile>();
                tileArray[i, j].SetWidth(tileWidth);
                tileArray[i, j].SetRowAndColumn(i,j);
                tileArray[i, j].partOfMaze = index;
            }
        }
        //Debug.Log(name + " initialized.");
    }

    //I added this for debug purposes, we can remove it when we have a floor texture
    //void Update()
    //{
    //    if (Input.GetKeyUp("b")) OpenAllDeadEnds();
    //}

    public void GenerateIntArray()
    {
        mazeIntArray = new int[mazeRows, mazeColumns]; // will be filled with the ID for each tile in tileArray
        pillarBoolArray = new bool[mazeRows + 1, mazeColumns + 1];

        for (int i = 0; i < mazeRows; i++)
        {
            for (int j = 0; j < mazeColumns; j++)
            {
                if (tileArray[i, j] != null)
                {
                    Tile tempTile = tileArray[i, j];
                    EventCallbacks.GenerateTerrainEvent gtei = new EventCallbacks.GenerateTerrainEvent();
                    mazeIntArray[i, j] = tileArray[i, j].GetTileID();

                    gtei.go = tempTile.gameObject;
                    gtei.wallArray = tempTile.GetWallArray();
                    gtei.tileWidth = tileWidth;
                    gtei.mapGeneratorRef = this;

                    gtei.tileRowPos = i; gtei.tileColPos = j;
                    gtei.tileArray = tileArray;

                    //ID Changing when creating new tile
                    gtei.FireEvent();

                    if (tempTile.wallArray[0] == 0 && tempTile.wallArray[1] == 0 &&
                        tempTile.wallArray[2] == 0 && tempTile.wallArray[3] == 0)
                        tempTile.isShutOffTile = true;
                }
                else
                    mazeIntArray[i, j] = 0;
                //Debug.Log(tileWidth + " generated int array");
            }
        }

        // Generate towers event:
        EventCallbacks.GenerateTowersEvent gtwe = new EventCallbacks.GenerateTowersEvent();
        gtwe.go = gameObject;
        gtwe.widthX = mazeColumns * tileWidth;
        gtwe.widthY = mazeRows * tileWidth;
        gtwe.tileWidth = tileWidth;
        gtwe.FireEvent();
    }

    private void MaterialSwitcher(int index)
    {
        // Texture switch event:
        EventCallbacks.TextureSwitchEvent tse = new EventCallbacks.TextureSwitchEvent();
        tse.partOfMaze = index;
        tse.FireEvent();
    }

    public void SetDimensions(int rows, int cols, float width)
    {
        mazeRows = rows;
        mazeColumns = cols;
        tileWidth = width;
    }
    public List<int[]> GetDeadEndList()
    {
        List<int[]> deadEnd = new List<int[]>();
        for (int i = 0; i < mazeRows; i++)
        {
            for (int j = 0; j < mazeColumns; j++)
            {
                if (mazeIntArray[i, j] == 1 || mazeIntArray[i, j] == 2 || mazeIntArray[i, j] == 4 || mazeIntArray[i, j] == 8)
                {
                    deadEnd.Add(new int[] { i, j });
                    //Debug.Log("" + i + " " + j);
                }
            }
        }
        return deadEnd;
    }

    public List<Tile> UpdateDeadEndTileBools()
    {
        if (tileArray == null)
        {
            Debug.LogError("Error: Tile array not set yet! - Null ref");
            return null;
        }

        List<int[]> deadEnds = GetDeadEndList();
        List<Tile> returnList = new List<Tile>();

        foreach (int[] deadEnd in deadEnds)
        {
            if (!tileArray[deadEnd[0], deadEnd[1]].isPortalTile)
            {
                tileArray[deadEnd[0], deadEnd[1]].isDeadEnd = true;
                returnList.Add(tileArray[deadEnd[0], deadEnd[1]]);
            }
        }

        return returnList;
    }

    public List<TileInfo> GetDeadEndListTileInfo()
    {
        List<TileInfo> deadEndList = new List<TileInfo>();
        for (int i = 0; i < mazeRows; i++)
        {
            for (int j = 0; j < mazeColumns; j++)
            {
                int ID = tileArray[i, j].GetTileID();
                if (ID == 1 || ID == 2 || ID == 4 || ID == 8) // if it is a dead end
                {
                    deadEndList.Add(new TileInfo(i, j, (int)Mathf.Log(ID, 2)));
                    //Debug.Log("" + i + " " + j);
                }
            }
        }
        return deadEndList;
    }

    public TileInfo GetFurthestDeadEnd(TileInfo portal, int mazeID)
    {
        int distance = 0;
        int bigDistance =0;
        TileInfo FurthestTile = portal; //it needs to be an instance and portal tileinfo is what i have. it should not be the furthest deadend anyway ;)

        for (int i = 0; i < mazeRows; i++)
        {
            for (int j = 0; j < mazeColumns; j++)
            {
                int ID = tileArray[i, j].GetTileID();
                if (ID == 1 || ID == 2 || ID == 4 || ID == 8) // if it is a dead end
                {
                    if (!(i == portal.row && j == portal.column))
                    {
                        distance = Mathf.Abs(portal.row - tileArray[i, j].GetRow()) + Mathf.Abs(portal.column - tileArray[i, j].GetCol());
                        if (distance > bigDistance)
                        {
                            bigDistance = distance;
                            FurthestTile = new TileInfo(tileArray[i, j].GetRow(), tileArray[i, j].GetCol(), -1);
                        }
                    }


                }
            }
        }
        return FurthestTile;
    }

    public List<DeadEnd> GetDeadEndListTile(TileInfo portal, int mazeID)
    {
        List<DeadEnd> deadEndList = new List<DeadEnd>();
        for (int i = 0; i < mazeRows; i++)
        {
            for (int j = 0; j < mazeColumns; j++)
            {
                int ID = tileArray[i, j].GetTileID();
                if (ID == 1 || ID == 2 || ID == 4 || ID == 8) // if it is a dead end
                {
                    if (!(i == portal.row && j == portal.column))
                    {
                        deadEndList.Add(new DeadEnd(tileArray[i, j], mazeID));
                    }


                }
            }
        }
        return deadEndList;
    }

    public List<DeadEnd> GetDeadEndListTile(TileInfo start, TileInfo end, int mazeID)
    {
        List<DeadEnd> deadEndList = new List<DeadEnd>();
        for (int i = 0; i < mazeRows; i++)
        {
            for (int j = 0; j < mazeColumns; j++)
            {
                int ID = tileArray[i, j].GetTileID();
                if (ID == 1 || ID == 2 || ID == 4 || ID == 8) // if it is a dead end
                {
                    if (i == start.row && j == start.column){ }
                    else if(i == end.row && j == end.column){ }
                    else
                    {
                        deadEndList.Add(new DeadEnd(tileArray[i, j], mazeID));
                    }


                }
            }
        }
        return deadEndList;
    }

    public int[] GetFirstDeadEnd(int entranceRow, int entranceCol)
    {
        List<int[]> deadEndList = GetDeadEndList();
        foreach (int[] deadEnd in deadEndList)
        {
            if (deadEnd[0] == entranceRow && deadEnd[1] == entranceCol)
                continue;
            else
                return deadEnd;
        }
        return new int[] { -1, -1 };
    }

    public int[] GetRandomDeadEnd(int entranceRow, int entranceCol)
    {
        List<int[]> deadEndList = GetDeadEndList();
        int[] deadEnd = new int[] { -1, -1 };
        do
        {
            int idx = Random.Range(0, deadEndList.Count);
            deadEnd = deadEndList[idx];
        }
        while (deadEnd[0] == entranceRow && deadEnd[1] == entranceCol);
        return deadEnd;
    }

    public TileInfo GetRandomDeadEnd(TileInfo entrance)
    {
        int[] deadEnd = GetRandomDeadEnd(entrance.row, entrance.column);
        int deadEndDirection = (int)Mathf.Log(mazeIntArray[deadEnd[0], deadEnd[1]], 2);
        return new TileInfo(deadEnd[0], deadEnd[1], deadEndDirection);
    }

    public TileInfo GetRandomDeadEndHallway(TileInfo entrance)
    {
        int error = 0;
        List<TileInfo> deadEndList = GetDeadEndListTileInfo();
        TileInfo deadEnd = new TileInfo(0, 0, 0); //this will crash the game later if there is no suitable dead end. Should generate in a way that there is always one
        do
        {
            int idx = Random.Range(0, deadEndList.Count);
            deadEnd = deadEndList[idx];
            error++;
            //Debug.Log("Error " + error);
        }
        while ((deadEnd.IsSamePosition(entrance) || deadEnd.IsInCorner() || deadEnd.IsPerpendicular()) && error < 30);
        if (error >= 29)
            Debug.Log("There were no suitable dead ends in " + gameObject.name + ", exiting to avoid infinite loop");
        return deadEnd;
    }


    //I added this for debug purposes, opens up all dead-ends on the floor so we can check if the portals are seamless
    public void OpenAllDeadEnds()
    {
        for (int i = 0; i < mazeIntArray.GetLength(0); i++)
        {
            for (int j = 0; j < mazeIntArray.GetLength(1); j++)
            {
                if (mazeIntArray[i, j] == 1 || mazeIntArray[i, j] == 4)
                {
                    tileArray[i, j].SetTileID(5);
                }
                else if (mazeIntArray[i, j] == 2 || mazeIntArray[i, j] == 8)
                {
                    tileArray[i, j].SetTileID(10);
                }
            }
        }
    }

    public void FindOuterWalls(TileInfo portal, int maxIndex)
    {

        //List<Tile> outerTiles = new List<Tile>();
        List<Tile> portals = new List<Tile>();
        List<Tile> leadIns = new List<Tile>();
        portals.Add(InfoToTile(portal));
        leadIns.Add(InfoToTile(portal.GetLeadInCoord()));

        FindOuterWalls(portals, leadIns, maxIndex);

        //return outerTiles;
    }

    public void FindOuterWalls(TileInfo start, TileInfo goal, int maxIndex)
    {
        //List<Tile> outerTiles = new List<Tile>();

        List<Tile> portals = new List<Tile>();
        List<Tile> leadIns = new List<Tile>();

        portals.Add(InfoToTile(start));
        leadIns.Add(InfoToTile(start.GetLeadInCoord()));
        portals.Add(InfoToTile(goal));
        leadIns.Add(InfoToTile(goal.GetLeadInCoord()));

        FindOuterWalls(portals, leadIns, maxIndex);

        //return outerTiles;
    }

    public void FindOuterWalls(int maxIndex)
    {
        List<Tile> portals = new List<Tile>();
        List<Tile> leadIns = new List<Tile>();
        FindOuterWalls(portals, leadIns, maxIndex);
    }

    public List<Tile> FindOuterWalls(List<Tile> portals, List<Tile> leadIns, int maxIndex)
    {

        List<Tile> outerTiles = new List<Tile>();

        List<Tile> both = new List<Tile>();
        both.AddRange(portals);
        both.AddRange(leadIns);

        foreach (Tile t in tileArray)
        {
            if (OuterTileCheck(t))
            {
                if (!LeadinOrPortalTile(t, both))
                {
                    SetOuterwalls(t, 2, maxIndex); // make it possible to make outer wall completely open or window
                }
            }
        }

        for (int i = 0; i < leadIns.Count; i++)
        {
            if (leadIns[i].isOuterTile)
            {
                SetOuterwalls(leadIns[i], 1, maxIndex); //only allow them to be windows
            }
        }

        return outerTiles;
    }

    private void SetOuterwalls(Tile t, int maxWallType, int maxIndex)
    {
        int[] innerWalls = PortalPositionHelper.GetEntranceArray(t.GetRow(), t.GetCol());

        for (int i = 0; i < t.outerWalls.Length; i++)
        {
            if (!innerWalls.Contains(i))
            {
                if (maxIndex < maxWallType)
                {
                    t.outerWalls[i] = maxIndex;
                }
                else
                {
                    t.outerWalls[i] = maxWallType;
                }
                
            }
        }
    }

    private bool LeadinOrPortalTile(Tile t, List<Tile> both)
    {
        foreach (Tile tile in both)
        {
            if (t == tile)
            {

                return true;
            }
        }
        return false;
    }

    private bool OuterTileCheck(Tile t)
    {

        if (t.GetRow() == 0 ||
            t.GetCol() == 0 ||
            t.GetRow() == mazeRows - 1 ||
            t.GetCol() == mazeColumns - 1)
        {
                                t.isOuterTile = true;
            return true;
        }
        return false;
    }

    public Tile InfoToTile(TileInfo ti)
    {
        return tileArray[ti.row, ti.column];
    }

    public abstract void Generate();
    public abstract void Generate(MapInfo[] info, int i);
    public abstract void Generate(TileInfo info, string roomName = "RoomTemplate");
    public abstract void Generate(int startRow, int startCol, int startDir, string roomName = "RoomTemplate");
    public abstract void Generate(int startRow, int startCol, int startDir, int endRow, int endCol, int endDir, string roomName = "RoomTemplate");
}
