using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapGenerator : MonoBehaviour
{
    public int mazeRows; // size of maze
    public int mazeColumns; // size of maze
    public float tileWidth;
    public GameObject tilePrefab;
    public Tile[,] tileArray; //
    public int[,] mazeIntArray;
    public List<Tile> aStarTiles;

    public void Initialize()
    {
        tileArray = new Tile[mazeRows, mazeColumns];
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
            }
        }
        //Debug.Log(name + " initialized.");
    }

    //I added this for debug purposes, we can remove it when we have a floor texture
    void Update()
    {
        if (Input.GetKeyUp("b")) OpenAllDeadEnds();
    }

    protected void GenerateIntArray()
    {
        mazeIntArray = new int[mazeRows, mazeColumns]; // will be filled with the ID for each tile in tileArray
        for (int i = 0; i < mazeRows; i++)
        {
            for (int j = 0; j < mazeColumns; j++)
            {
                if (tileArray[i, j] != null)
                {
                    EventCallbacks.GenerateTerrainEvent gtei = new EventCallbacks.GenerateTerrainEvent();
                    mazeIntArray[i, j] = tileArray[i, j].GetTileID();
                    gtei.go = tileArray[i, j].gameObject;
                    gtei.wallArray = tileArray[i, j].GetWallArray();
                    gtei.tileWidth = tileWidth;

                    //ID Changing when creating new tile
                    gtei.FireEvent();
                }
                else
                    mazeIntArray[i, j] = 0;
                //Debug.Log(tileWidth + " generated int array");
            }
        }
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

    public List<TileInfo> GetDeadEndListTileInfo()
    {
        List<TileInfo> deadEndList = new List<TileInfo>();
        for (int i = 0; i < mazeRows; i++)
        {
            for (int j = 0; j < mazeColumns; j++)
            {
                if (mazeIntArray[i, j] == 1 || mazeIntArray[i, j] == 2 || mazeIntArray[i, j] == 4 || mazeIntArray[i, j] == 8)
                {
                    deadEndList.Add(new TileInfo(i, j, (int)Mathf.Log(mazeIntArray[i, j], 2)));
                    //Debug.Log("" + i + " " + j);
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

    public abstract void Generate();
    public abstract void Generate(MapInfo info);
    public abstract void Generate(TileInfo info, string roomName = "RoomTemplate");
    public abstract void Generate(int startRow, int startCol, int startDir, string roomName = "RoomTemplate");
    public abstract void Generate(int startRow, int startCol, int startDir, int endRow, int endCol, int endDir, string roomName = "RoomTemplate");
}
