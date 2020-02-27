using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int mazeID; // the maze segment that the room is in
    public List<Tile> tiles; // should only contain empty tiles which content can be place before used to generating content
    public List<Tile> entryTiles; // should only contain tiles lead in and out of the maze. [0] is closest to exit portal. [1] is closest to the entrance portal
    public Tile leadIn;
    public Tile clock;
    public Tile counter;

    public void AddTile(Tile t)
    {
        tiles.Add(t);
    }

    public void AddTiles(List<Tile> t)
    {
        tiles.AddRange(t);
    }

    public void SetMazeID(int mazeID)
    {
        this.mazeID = mazeID;
    }

    public void FindEntryTiles()
    {
        entryTiles.Clear();
        Tile closeToEntrance = tiles[0];
        Tile closeToExit = tiles[0]; // set them to something and skip index 0

        for (int i = 1; i < tiles.Count; i++)
        {
            if (tiles[i].prevDistance > closeToEntrance.prevDistance)
            {
                closeToEntrance = tiles[i];
            }
            if (tiles[i].nextDistance > closeToExit.nextDistance)
            {
                closeToExit = tiles[i];
            }
        }

        entryTiles.Add(closeToExit);
        entryTiles.Add(closeToEntrance);

        tiles.Remove(closeToExit);
        tiles.Remove(closeToEntrance);
    }

    public Tile SearchForRoom(DeadEnd de, Tile[,] tileArray)
    {
        // find the lead in tile -> have row, column and direction
        bool isClockWise;

        int[] wallArray = de.GetWalls();
        for (int i = 0; i < wallArray.Length; i++)
        {
            if (wallArray[i] == 1) // if the tile is traversable in that direction
            {
                switch (i)
                {
                    case 0:
                        leadIn = tileArray[de.GetRow() - 1, de.GetCol()]; //neighbor tile north 
                        CheckAdjacentDirection(i, tileArray, true);
                        CheckAdjacentDirection(i, tileArray, false);
                        break;
                    case 1:
                        leadIn = tileArray[de.GetRow(), de.GetCol() + 1]; //neighbor tile east
                        CheckAdjacentDirection(i, tileArray, true);
                        CheckAdjacentDirection(i, tileArray, false);
                        break;
                    case 2:
                        leadIn = tileArray[de.GetRow() + 1, de.GetCol()]; //neighbor tile south
                        CheckAdjacentDirection(i, tileArray, true);
                        CheckAdjacentDirection(i, tileArray, false);
                        break;
                    case 3:
                        leadIn = tileArray[de.GetRow(), de.GetCol() - 1]; //neighbor tile west
                        CheckAdjacentDirection(i, tileArray, true);
                        CheckAdjacentDirection(i, tileArray, false);
                        break;
                    default:
                        break;
                }
            }


        }
        

        return clock;
    }

    private void CheckAdjacentDirection(int i, Tile[,] tileArray, bool clockwise)
    {
        int adjacent;
        if (clockwise)
        {
            adjacent = Modulus(i + 1);
        }
        else
        {
            adjacent = Modulus(i - 1);
        }


        if(leadIn.wallArray[adjacent] == 1)
        {
            switch (adjacent)
            {
                case 0:
                    if (tileArray[leadIn.GetRow() - 1, leadIn.GetCol()].isAStarTile)
                    {
                        clock = tileArray[leadIn.GetRow() - 1, leadIn.GetCol()];
                    }
                    break;
                case 1:

                    if (tileArray[leadIn.GetRow(), leadIn.GetCol() + 1].isAStarTile)
                    {
                        clock = tileArray[leadIn.GetRow(), leadIn.GetCol() + 1];
                    }
                    break;
                case 2:

                    if (tileArray[leadIn.GetRow() + 1, leadIn.GetCol()].isAStarTile)
                    {
                        clock = tileArray[leadIn.GetRow() + 1, leadIn.GetCol()];
                    }
                    break;
                case 3:

                    if (tileArray[leadIn.GetRow(), leadIn.GetCol() - 1].isAStarTile)
                    {
                        clock = tileArray[leadIn.GetRow(), leadIn.GetCol() - 1];
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private int Modulus(int input)
    {
        if (input == -1)
        {
            return 3;
        }
        else if (input == 4)
        {
            return 0;
        }
        return input;
        
    }
}
