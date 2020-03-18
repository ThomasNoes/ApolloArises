using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileInfo
{
    public int row;
    public int column;
    public int direction;
    public string spelledDirection;

    private static Dictionary<int, string> SpelledDirection = new Dictionary<int, string>
    {
        {-1, "No Direction"},
        {0, "North"},
        {1, "East"},
        {2, "South"},
        {3, "West"},
    };

    public TileInfo(int _row, int _col, int _dir)
    {
        row = _row;
        column = _col;
        direction = _dir;
        spelledDirection = SpelledDirection[_dir];

    }

    public TileInfo(TileInfo obj)
    {
        row = obj.row;
        column = obj.column;
        direction = obj.direction;
        spelledDirection = SpelledDirection[obj.direction];
    }

    public bool IsInCorner()
    {
        int mazeRows = GameObject.Find("MapManager").GetComponent<MapManager>().mazeRows; //these should be globals somewhere...
        int mazeCols = GameObject.Find("MapManager").GetComponent<MapManager>().mazeCols;
        bool inCorner = false;
        if (row == 0 && column == 0 ||
            row == 0 && column == (mazeCols - 1) ||
            row == (mazeRows - 1) && column == 0 ||
            row == (mazeRows - 1) && column == (mazeCols - 1))
        {
            //Debug.Log("tile (" + row + ";" + column + ") is in a corner");
            inCorner = true;
        }
        return inCorner;
    }

    public bool IsPerpendicular() // to the egde of the map sequence
    {
        int mazeRows = GameObject.Find("MapManager").GetComponent<MapManager>().mazeRows;
        int mazeCols = GameObject.Find("MapManager").GetComponent<MapManager>().mazeCols;
        bool perpendicular = false;
        switch (direction)
        {
            case 0:
                if (row == mazeRows - 1)
                    perpendicular = true;
                break;
            case 1:
                if (column == 0)
                    perpendicular = true;
                break;
            case 2:
                if (row == 0)
                    perpendicular = true;
                break;
            case 3:
                if (column == mazeCols - 1)
                    perpendicular = true;
                break;
            default:
                break;
        }
        return perpendicular;
    }

    public TileInfo GetLeadInCoord()
    {
        int r = row;
        int c = column;
        switch (direction)
        {
            case 0:
                r--;
                break;
            case 1:
                c++;
                break;
            case 2:
                r++;
                break;
            case 3:
                c--;
                break;
            default:
                break;
        }
        return new TileInfo(r, c, -1);
    }

    public TileInfo GetBehindCoord()
    {
        int r = row;
        int c = column;
        switch (direction)
        {
            case 0:
                r++;
                break;
            case 1:
                c--;
                break;
            case 2:
                r--;
                break;
            case 3:
                c++;
                break;
            default:
                break;
        }
        return new TileInfo(r, c, -1);
    }

    public bool IsLeadingIntoEntrance(TileInfo entrance)
    {
        TileInfo neighbour = GetLeadInCoord();
        return (neighbour.IsSamePosition(entrance));
    }

    public bool IsVisibleThroughEntrance(TileInfo entrance)
    {
        bool visible = false;

        if (entrance.GetOppositeDirection() == 2 && spelledDirection == "North")
        {
            if (entrance.column == column && entrance.row > row)
            {
                visible = true;
            }
        }
        if (entrance.GetOppositeDirection() == 0 && spelledDirection == "South")
        {
            if (entrance.column == column && entrance.row < row)
            {
                visible = true;
            }
        }
        if (entrance.GetOppositeDirection() == 3 && spelledDirection == "East")
        {
            if (entrance.row == row && entrance.column < column)
            {
                visible = true;
            }
        }
        if (entrance.GetOppositeDirection() == 1 && spelledDirection == "West")
        {
            if (entrance.row == row && entrance.column > column)
            {
                visible = true;
            }
        }

        return visible;
    }

    public bool BothPortalsVisibleFromLeadIn(TileInfo entrance)
    {
        bool visible = false;
        TileInfo leadIn = entrance.GetLeadInCoord();

        if (entrance.direction == 0 || entrance.direction == 2)
        {
            if (leadIn.row == row)
            {
                if (column < leadIn.column && direction == 1)
                {
                    visible = true;
                }
                if (column > leadIn.column && direction == 3)
                {
                    visible = true;
                }
            }
        }

        if (entrance.direction == 3 || entrance.direction == 1)
        {
            if (leadIn.column == column)
            {
                if (row < leadIn.row && direction == 2)
                {
                    visible = true;
                }
                if (row > leadIn.row && direction == 0)
                {
                    visible = true;
                }
            }
        }

        return visible;
    }

    public bool IsAdjacentLaneWithSameDirection(TileInfo entrance)
    {
        bool adjacentAndSame = false;

        if (entrance.direction == direction) //check if they are pointing in same direction //maybe this needs to be opposite
        {
            if(spelledDirection == "North" || spelledDirection == "South") // check if the direction is vertical
            {
                if (entrance.column == column + 1 || entrance.column == column - 1) //check if it is the adjacent column
                {
                    adjacentAndSame = true;
                }
            }
            if (spelledDirection == "East" || spelledDirection == "West") // check if the direction is horizontal
            {
                if (entrance.row == row + 1 || entrance.row == row - 1) //check if it is the adjacent row
                {
                    adjacentAndSame = true;
                }
            }

        }


        return adjacentAndSame;
    }

    public bool IsSamePosition(TileInfo tile)
    {
        return (row == tile.row && column == tile.column);
    }

    public static bool operator ==(TileInfo lhs, TileInfo rhs)
    {
        return (lhs.row == rhs.row && lhs.column == rhs.column && lhs.direction == rhs.direction);
    }
    public static bool operator !=(TileInfo lhs, TileInfo rhs)
    {
        return !(lhs.row == rhs.row && lhs.column == rhs.column && lhs.direction == rhs.direction);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        else
        {
            TileInfo tile = (TileInfo)obj;
            return (row == tile.row && column == tile.column && direction == tile.direction);
        }
    }

    public int GetOppositeDirection(int i)
    {
        return (i + 2) % 4;
    }
    private int GetOppositeDirection()
    {
        return (direction + 2) % 4;
    }

    public override int GetHashCode()
    {
        return row + column + direction;
    }

    public void PrintTile() {
        Debug.Log("(row " + row + "; column " + column + "; direction " + direction + ")");
    }
    public string TileToString()
    {
        return "(row " + row + "; column " + column + "; direction " + spelledDirection + ")";
    }
}
