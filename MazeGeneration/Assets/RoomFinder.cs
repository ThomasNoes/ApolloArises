using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomFinder : MonoBehaviour
{
    public Tile de;
    public int mazeID;
    public Tile[,] tileArray;
    List<Tile> clockTiles = new List<Tile>();
    List<Tile> counterTiles = new List<Tile>();
    bool roomisClockwise;
    public List<Tile> debugTiles = new List<Tile>();


    public RoomFinder(DeadEnd de, Tile[,] tileArray)
    {
        this.de = de.GetTile();
        mazeID = de.GetMazeID();
        this.tileArray = tileArray;
    }

    public bool SearchForRoom()
    {
        AddToClock(de);
        AddToCounter(de);

        bool roomClockWise = false;
        bool roomCounterWise = false;


        Tile leadIn;

        tileArray.GetLength(1);

        for (int i = 0; i < de.wallArray.Length; i++)
        {
            if (de.wallArray[i] == 1) // if the tile is traversable in that direction
            {
                switch (i)
                {
                    case 0:
                        leadIn = tileArray[de.GetRow() - 1,de.GetCol()];
                        roomClockWise = CheckPerpendicularDirection(leadIn, i, true);
                        roomCounterWise = CheckPerpendicularDirection(leadIn, i, false);
                        break;
                    case 1:
                        leadIn = tileArray[de.GetRow(), de.GetCol() + 1];
                        roomClockWise = CheckPerpendicularDirection(leadIn, i, true);
                        roomCounterWise = CheckPerpendicularDirection(leadIn, i, false);
                        break;
                    case 2:
                        leadIn = tileArray[de.GetRow() + 1, de.GetCol()];
                        roomClockWise = CheckPerpendicularDirection(leadIn, i, true);
                        roomCounterWise = CheckPerpendicularDirection(leadIn, i, false);
                        break;
                    case 3:
                        leadIn = tileArray[de.GetRow(), de.GetCol() - 1];
                        roomClockWise = CheckPerpendicularDirection(leadIn, i, true);
                        roomCounterWise = CheckPerpendicularDirection(leadIn, i, false);
                        break;
                    default:
                        break;
                }
            }
        }
        //check if a room exist and send back bool
        if (roomClockWise && roomCounterWise)
        {
            // what to do if both are possible rooms
        }
        else if (roomClockWise)
        {
            room
        }
        else if (roomCounterWise)
        {

        }
        return true;
    }

    private bool CheckPerpendicularDirection(Tile t, int i, bool clockWise)
    {
        bool roomThisWay = false;
        Tile leadIn;

        if (clockWise)
        {
            i = Modulus(i + 1);
            AddToClock(t);
        }
        else
        {
            i = Modulus(i - 1);
            AddToCounter(t);
        }

        int row = 0;
        int col = 0;

        switch (i)
        {
            case 0:
                row = t.GetRow() - 1;
                col = t.GetCol();
                break;
            case 1:
                row = t.GetRow();
                col = t.GetCol() + 1;

                break;
            case 2:
                row = t.GetRow() + 1;
                col = t.GetCol();

                break;
            case 3:
                row = t.GetRow();
                col = t.GetCol() - 1;
                break;
            default:
                break;
        }
        if (CheckBounds(row, col))
        {
            leadIn = tileArray[row, col];

            if (t.wallArray[i] == 1) // if the path is open
            {
                if (leadIn.isAStarTile && // if it is a a star tile
                    leadIn.prevDistance > 0 && leadIn.nextDistance > 0) // and it is not the portal tile
                {
                    roomThisWay = CheckPerpendicularDirection(leadIn, i, clockWise);
                }
            }
            else if (t.wallArray[i] == 0) // if the path is closed 
            {
                if (leadIn == de) // we found our way back to the deadend de
                {
                    Debug.Log("i found the deadend");
                    roomThisWay = true;
                    if (clockWise)
                    {
                        AddToClock(leadIn);
                    }
                    else
                    {
                        AddToCounter(leadIn);
                    }
                }
            }
        }
        return roomThisWay;
    }

    private bool CheckBounds(int row, int col)
    {
        bool inBounds = true;
        if (row < 0 ||
            col < 0 ||
            row >= tileArray.GetLength(0) ||
            col >= tileArray.GetLength(1))
        {
            inBounds = false;
        }

        return inBounds;
    }

    private void AddToClock(Tile t)
    {
        clockTiles.Add(t);
        Debug.Log("clockwise " + clockTiles.Count + " in maze "+ mazeID + " ----------------- added tile " + t.GetRow() + "," + t.GetCol());
        if (!debugTiles.Contains(t))
        {
            debugTiles.Add(t);
        }
    }

    private void AddToCounter(Tile t)
    {
        counterTiles.Add(t);
        Debug.Log("counterwise "+ counterTiles.Count + " in maze " + mazeID + " ----------------- added tile " + t.GetRow() + "," + t.GetCol());
        if (!debugTiles.Contains(t))
        {
            debugTiles.Add(t);
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

    public List<Tile> GetRoom()
    {
        if(roomisClockwise && clockTiles.Count ==4)
        {
            return clockTiles;
        }
        else if (!roomisClockwise && counterTiles.Count == 4)
        {
            return counterTiles;
        }
        else
        {
            return null;
        }
    }

    
}
