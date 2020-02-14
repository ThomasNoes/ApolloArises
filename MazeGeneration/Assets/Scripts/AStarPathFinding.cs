using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class AStarPathFinding : MonoBehaviour
{
    // https://www.youtube.com/watch?v=-L-WgKMFuhE

    GameObject gizmoPrefab;

    Tile[,] tileArray;
    Tile start;
    Tile goal;
    
    List<Tile> openTiles = new List<Tile>();
    List<Tile> closedTiles = new List<Tile>();
    List<Tile> aStarTiles = new List<Tile>();

    //pathFinding
    public List<Tile> BeginAStar(Tile[,] tileArray, TileInfo startInfo, TileInfo goalInfo)
    {
        this.tileArray = tileArray;
        start = tileArray[startInfo.row, startInfo.column];
        goal = tileArray[goalInfo.row, goalInfo.column];

        SetHCosts(tileArray); // the distance each tile has to the goal should not change. so here i set them all. 

        openTiles.Add(start);

        while (openTiles.Count > 0)
        {
            FindPath();
        }

        DrawAStarPath();
        DrawGizmo(start, Color.green);
        DrawGizmo(goal, Color.red);

        return aStarTiles;
    }

    private void FindPath()
    {
        Tile current = FindLowestFTile(openTiles);
        openTiles.Remove(current);
        closedTiles.Add(current);


        if (current == goal)
        {
            aStarTiles = RetracePath(start, goal);
            return;
        }

        //check the adjacent tiles
        for (int i = 0; i < current.wallArray.Length; i++)
        {
            if (current.wallArray[i] == 1) // if the tile is traversable in that direction
            {
                Tile neighbor = new Tile();
                switch (i)
                {
                    case 0:
                        neighbor = tileArray[current.row - 1, current.column]; //neighbor tile north
                        CheckNeighbor(current, neighbor);
                        break;
                    case 1:
                        neighbor = tileArray[current.row, current.column + 1]; //neighbor tile north
                        CheckNeighbor(current, neighbor);
                        break;
                    case 2:
                        neighbor = tileArray[current.row + 1, current.column]; //neighbor tile north
                        CheckNeighbor(current, neighbor);
                        break;
                    case 3:
                        neighbor = tileArray[current.row, current.column - 1]; //neighbor tile north
                        CheckNeighbor(current, neighbor);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void CheckNeighbor(Tile current, Tile neighbor)
    {
        if (!closedTiles.Contains(neighbor))
        {
            int newGCost = current.GetG() + GetDistance(current, neighbor);
            if (newGCost < neighbor.GetG() // if new path to the neighbor tile is shorter
                || !openTiles.Contains(neighbor)) // or if it is not the open tiles list
            {
                neighbor.SetG(newGCost);
                neighbor.SetParent(current);
                if (!openTiles.Contains(neighbor))
                {
                    openTiles.Add(neighbor);
                }
            }

        }
    }

    private List<Tile> RetracePath(Tile start, Tile goal)
    {
        List<Tile> path = new List<Tile>();
        Tile current = goal;

        while (current != start)
        {
            path.Add(current);
            current = current.GetParent();
        }
        path.Add(current);
        path.Reverse();

        return path;
    }

    private void SetHCosts(Tile[,] tiles)
    {
        foreach (Tile t in tiles)
        {
            t.SetH(GetDistance(t, goal));
        }
    }

    private int GetDistance(Tile from, Tile to)
    {
        //manhattan Distance
        return (Mathf.Abs(from.row - to.row) + Mathf.Abs(from.column - to.column));
    }

    private Tile FindLowestFTile(List<Tile> tiles)
    {
        Tile returnTile = tiles[0];

        foreach (Tile t in tiles)
        {
            if (t.GetF() < returnTile.GetF() 
                || t.GetF() == returnTile.GetF() && t.GetH() < returnTile.GetF()) // if they are equal choose based on H cost
            {
                returnTile = t;
            }
        }
        return returnTile; 
    }

    //draw Astar path
    private void DrawAStarPath()
    {
        if(gizmoPrefab == null)
        {
            SetGizmoPrefab();
        }
        foreach (Tile t in aStarTiles)
        {
            DrawGizmo(t, Color.blue);
        }

    }

    //draw open tiles
    public void DrawOpenPath()
    {
        if (gizmoPrefab == null)
        {
            SetGizmoPrefab();
        }
        foreach (Tile t in openTiles)
        {
            DrawGizmo(t, Color.green);
        }

    }

    //draw closed tiles
    public void DrawClosedPath()
    {
        if (gizmoPrefab == null)
        {
            SetGizmoPrefab();
        }
        foreach (Tile t in openTiles)
        {
            DrawGizmo(t, Color.red);
        }
    }

    private void DrawGizmo (Tile t, Color color)
    {
        GameObject gizmo = Instantiate(gizmoPrefab, t.transform);
        gizmo.GetComponent<drawGizmo>().SetColor(color);
        gizmo.name = "Gizmo: R" + t.row + "C" + t.column;
    }

    private void SetGizmoPrefab()
    {
        gizmoPrefab = Resources.Load<GameObject>("Prefabs/Gizmo");
    }
}
