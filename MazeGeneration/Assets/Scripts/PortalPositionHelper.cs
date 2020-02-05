using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class PortalPositionHelper
{
    private static int maxRows = GameObject.FindObjectOfType<MapManager>().mazeRows;
    private static int maxCols = GameObject.FindObjectOfType<MapManager>().mazeCols;
    private static Dictionary<string, int[]> PortalEntranceArrayList = new Dictionary<string, int[]>
    {
        {"topLeftCorner", new int[2] {1,2}},
        {"topRightCorner", new int[2] {2,3}},
        {"bottomRightCorner", new int[2] {0,3}},
        {"bottomLeftCorner", new int[2] {0,1}},
        {"topRow", new int[3] {1,2,3}},
        {"rightColumn", new int[3] {0,2,3}},
        {"bottomRow", new int[3] {0,1,3}},
        {"leftColumn", new int[3] {0,1,2}},
        {"inside", new int[4] {0,1,2,3}},
    };

    private static List<TileInfo> CornerShutoffList = new List<TileInfo>
    {
        new TileInfo(0,1, -1),
        new TileInfo(0, maxCols - 2, -1),
        new TileInfo(maxRows - 2, 0, -1),
        new TileInfo(maxRows - 1, maxCols - 2, -1),
        new TileInfo(1, 0, -1),
        new TileInfo(1, maxCols - 1, -1),
        new TileInfo(maxRows - 1, 1, -1),
        new TileInfo(maxRows - 2, maxCols - 1, -1),
    };

    private static string GetDirectionArray(int row, int col)
    {
        if (row == 0)
        {
            if (col == 0)
                return "topLeftCorner";
            else if (col == maxCols - 1)
                return "topRightCorner";
            else
                return "topRow";
        }
        else if (row == maxRows - 1)
        {
            if (col == 0)
                return "bottomLeftCorner";
            else if (col == maxCols - 1)
                return "bottomRightCorner";
            else
                return "bottomRow";
        }
        else
        {
            if (col == 0)
                return "leftColumn";
            else if (col == maxCols - 1)
                return "rightColumn";
            else
                return "inside";
        }
    }

    // Returns a random array element except for the one specified in the flag argument
    private static int GetRandomArrayElementWithFlag(int[] arr, int flag)
    {
        int i = 0;
        do
        {
            i = Random.Range(0, arr.Length);
        }
        while (arr[i] == flag);

        return arr[i];
    }

    // If we want to choose an exit direction ourselves, we can use this method to access the possible exit directions
    private static int[] GetEntranceArray(string position)
    {
        return PortalEntranceArrayList[position];
    }

    public static int[] GetEntranceArray(int row, int col)
    {
        string direction = GetDirectionArray(row, col);
        return GetEntranceArray(direction);
    }

    public static int GetRandomPortalExit(int row, int col, int direction = -1)
    {
        string entrancePosition = GetDirectionArray(row, col);
        int[] directionArray = PortalEntranceArrayList[entrancePosition];
        return GetRandomArrayElementWithFlag(directionArray, direction);
    }

    public static List<TileInfo> GetAllCornerTiles() {
        return new List<TileInfo>(CornerShutoffList);
    }

    public static List<TileInfo> GetShutoffList(TileInfo tile)
    {
        List<TileInfo> shutoffIndexes = new List<TileInfo>();
        
        for (int i = 0; i < CornerShutoffList.Count; i++)
        {
            if (CornerShutoffList[i].IsSamePosition(tile))
            {
                shutoffIndexes.Add(CornerShutoffList[(i+4)%8]);
            }
        }
        return shutoffIndexes;
    }

}