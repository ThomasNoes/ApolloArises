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

    public TileInfo GetNeighbourCoord()
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

    public bool IsLeadingIntoEntrance(TileInfo entrance)
    {
        TileInfo neighbour = GetNeighbourCoord();
        return (neighbour.IsSamePosition(entrance));
    }

    public bool IsVisibleThroughEntrance(TileInfo entrance)
    {
        bool visible = false;
        Debug.Log("entrance " + entrance.spelledDirection + " and this tile " + spelledDirection);


        return visible;
    }

    public bool IsInViewWithEntrance(TileInfo entrance)
    {
        //To Do

        return true;
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

    public override int GetHashCode()
    {
        return row + column + direction;
    }

    public void PrintTile() {
        Debug.Log("(" + row + ";" + column + ";" + direction + ")");
    }
}
