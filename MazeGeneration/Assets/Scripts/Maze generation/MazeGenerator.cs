using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MapGenerator
{

    // Random starting positions
    public override void Generate()
    {
        int startRow = Random.Range(0, mazeRows);
        int startCol = Random.Range(0, mazeColumns);
        RecursiveDFS(startRow, startCol);

        //Debug.Log(name + " generated random maze");
        GenerateIntArray();
    }

    //Generates an integer array with the tileIDs in it. Will be used to find dead ends for portal placement
    public override void Generate(MapInfo[] mapSequence, int i)
    {
        if (!mapSequence[i].isEndSeeded) // if this is the lazt maze segments (?) Or it is a room
        {
            Generate(mapSequence[i].startSeed);
        }
        else //if both start and end are seeded
        {
            Generate(mapSequence[i].startSeed, mapSequence[i].endSeed);
        }
        //all tile ID should be set now. Find optimal route in this maze segment!

        if (i + 1 < mapSequence.Length)
            mapSequence[i].endSeed = GetRandomDeadEndHallway(mapSequence[i].startSeed);

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

                    aStarTiles = aStar.BeginAStar(tileArray, mapSequence[i].startSeed, mapSequence[i].endSeed);
                }
        //make rooms here
        List<DeadEnd>  deadends = GetDeadEndListTile(mapSequence[i].startSeed, mapSequence[i].endSeed, i);

        foreach (DeadEnd de in deadends)
        {
            RoomFinder rf = new RoomFinder(de, tileArray );
            rf.SearchForRoom();
            Debug.Log("------------new deadend---------- ");

            foreach (Tile t in rf.debugTiles)
            {
                aStar.DrawGizmo(t, Color.magenta, 0.25f);
            }
           
        }

        

        GenerateIntArray();
    }

    public override void Generate(TileInfo startSeed, string roomName = "RoomTemplate")
    {
        Generate(startSeed.row, startSeed.column, startSeed.direction);
    }

    public void Generate(TileInfo startSeed, TileInfo endSeed)
    {
        Generate(startSeed.row, startSeed.column, startSeed.direction, endSeed.row, endSeed.column, endSeed.direction);
    }

    // Maze generation using recursive DFS with a starting position and direction as seed.
    // It makes the first connection manually, then calls the RecursiveDFS() method to generate the rest of the maze.
    public override void Generate(int startRow, int startCol, int startDirection, string roomName = "RoomTemplate")
    {
        tileArray[startRow, startCol].OpenWall(startDirection);
        switch (startDirection)
        {
            case 0:
                RecursiveDFS(startRow - 1, startCol);
                Tile.ConnectTiles(tileArray[startRow, startCol], tileArray[startRow - 1, startCol], startDirection);
                break;
            case 1:
                RecursiveDFS(startRow, startCol + 1);
                Tile.ConnectTiles(tileArray[startRow, startCol], tileArray[startRow, startCol + 1], startDirection);
                break;
            case 2:
                RecursiveDFS(startRow + 1, startCol);
                Tile.ConnectTiles(tileArray[startRow, startCol], tileArray[startRow + 1, startCol], startDirection);
                break;
            case 3:
                RecursiveDFS(startRow, startCol - 1);
                Tile.ConnectTiles(tileArray[startRow, startCol], tileArray[startRow, startCol - 1], startDirection);
                break;
            default:
                break;
        }
        //Debug.Log(name + " generated seeded maze, startPos: (" + startRow + ";" + startCol + ";" + startDirection + ").");
        //GenerateIntArray();
    }

    // Overload of the seeded maze generation method with a specified end position and direction as well.
    // It opens the end tile so it will be ignored by the generator method, then calls the GenerateSeededMaze(int,int,int) method,
    // finally it connects the end tile with its neighbour to the specified direction.
    public override void Generate(int startRow, int startCol, int startDirection, int endRow, int endCol, int endDirection, string roomName = "RoomTemplate")
    {
        tileArray[endRow, endCol].OpenWall(endDirection);

        Generate(startRow, startCol, startDirection);

        switch (endDirection) // connecting tiles - open the wall on the other side
        {
            case 0:
                Tile.ConnectTiles(tileArray[endRow, endCol], tileArray[endRow - 1, endCol], endDirection);
                break;
            case 1:
                Tile.ConnectTiles(tileArray[endRow, endCol], tileArray[endRow, endCol + 1], endDirection);
                break;
            case 2:
                Tile.ConnectTiles(tileArray[endRow, endCol], tileArray[endRow + 1, endCol], endDirection);
                break;
            case 3:
                Tile.ConnectTiles(tileArray[endRow, endCol], tileArray[endRow, endCol - 1], endDirection);
                break;
            default:
                break;
        }
        //Debug.Log(name + "endPos: (" + endRow + ";" + endCol + ").");
        //GenerateIntArray();
    }


    //Probably should be a coroutine as the generation hangs the game on play pretty bad.
    /*
    Generates a perfect maze using recursive depth first search. First it gets an array of random directions, then
    using a switch, it goes through them. In the switch, the method does a bounds check on the neighbour in the specified direction,
    then checks if it has been visited yet. If not, then it connects the tile to the neighbour, then calls itself (RecursiveDFS(int,int))
    on the neighbour tile. The method returns when there are no more empty neighbours.
     */
    void RecursiveDFS(int row, int col)
    {
        int[] directions = GenerateRandomDirections();
        for (int i = 0; i < 4; i++)
        {
            switch (directions[i])
            {
                case 0:
                    if (row - 1 < 0)
                        continue;
                    if (tileArray[row - 1, col].GetTileID() == 0)
                    {
                        Tile.ConnectTiles(tileArray[row, col], tileArray[row - 1, col], 0);
                        RecursiveDFS(row - 1, col);
                    }
                    break;
                case 1:
                    if (col + 1 > mazeColumns - 1)
                        continue;
                    if (tileArray[row, col + 1].GetTileID() == 0)
                    {
                        Tile.ConnectTiles(tileArray[row, col], tileArray[row, col + 1], 1);
                        RecursiveDFS(row, col + 1);
                    }
                    break;
                case 2:
                    if (row + 1 > mazeRows - 1)
                        continue;
                    if (tileArray[row + 1, col].GetTileID() == 0)
                    {
                        Tile.ConnectTiles(tileArray[row, col], tileArray[row + 1, col], 2);
                        RecursiveDFS(row + 1, col);
                    }
                    break;
                case 3:
                    if (col - 1 < 0)
                        continue;
                    if (tileArray[row, col - 1].GetTileID() == 0)
                    {
                        Tile.ConnectTiles(tileArray[row, col], tileArray[row, col - 1], 3);
                        RecursiveDFS(row, col - 1);
                    }
                    break;
                default:
                    break;
            }
        }
    }


    // This method returns returns a 4 length array with random directions occuring once in each index.
    // It first creates a base array where the directions are in order, picks one randomly,
    // puts it into the first index of the resul array and sets it into a -1 "flag" to be ignored in the base array.
    private int[] GenerateRandomDirections()
    {
        int[] baseArray = new int[4] { 0, 1, 2, 3 };
        int[] result = new int[4] { 0, 0, 0, 0 };
        for (int i = 0; i < 4;)
        {
            int temp = Random.Range(0, 4);
            if (baseArray[temp] != -1)
            {
                result[i] = baseArray[temp];
                baseArray[temp] = -1;
                i++;
            }
        }
        return result;
    }

    // This method could be used later if we want to simplify the generator methods.
    // It returns if the neighbour in the specified direction is inbounds or not.
    private bool IsNeighbourInbounds(int row, int col, int direction)
    {
        switch (direction)
        {
            case 0:
                return (row - 1 >= 0);
            case 1:
                return (col + 1 <= mazeColumns - 1);
            case 2:
                return (row + 1 <= mazeRows - 1);
            case 3:
                return (col - 1 >= 0);
            default:
                return false;
        }
    }
}
