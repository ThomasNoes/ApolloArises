﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class AStarPathFinding : MonoBehaviour
{
    // https://www.youtube.com/watch?v=-L-WgKMFuhE

    GameObject gizmoPrefab;

    Tile[,] tileArray;
    Tile start;
    Tile goal;
    bool startIsPortal;
    bool goalIsPortal;
    
    List<Tile> openTiles = new List<Tile>();
    List<Tile> closedTiles = new List<Tile>();
    List<Tile> aStarTiles = new List<Tile>();

    //pathFinding
    public List<Tile> BeginAStar(Tile[,] tileArray, TileInfo startInfo, TileInfo goalInfo, bool startIsPortal = true, bool goalIsPortal = true)
    {
        this.tileArray = tileArray;
        start = tileArray[startInfo.row, startInfo.column];
        goal = tileArray[goalInfo.row, goalInfo.column];

        this.startIsPortal = startIsPortal;
        this.goalIsPortal = goalIsPortal;

        SetHCosts(tileArray); // the distance each tile has to the goal should not change. so here i set them all. 

        openTiles.Add(start);

        while (openTiles.Count > 0)
        {
            FindPath();
        }


        SetDistanceForRemainingTiles(aStarTiles);

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
                        neighbor = tileArray[current.GetRow() - 1, current.GetCol()]; //neighbor tile north
                        CheckNeighbor(current, neighbor);
                        break;
                    case 1:
                        neighbor = tileArray[current.GetRow(), current.GetCol() + 1]; //neighbor tile east
                        CheckNeighbor(current, neighbor);
                        break;
                    case 2:
                        neighbor = tileArray[current.GetRow() + 1, current.GetCol()]; //neighbor tile south
                        CheckNeighbor(current, neighbor);
                        break;
                    case 3:
                        neighbor = tileArray[current.GetRow(), current.GetCol() - 1]; //neighbor tile west
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

        ApproveTiles(path);

        return path;
    }

    private void ApproveTiles(List<Tile> tiles)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            if ((i == 0 && !startIsPortal) || (i == tiles.Count-1 && !goalIsPortal))
            {
                tiles[i].SetPortalDistance(i, tiles.Count, false);
            }
            else
            {
                tiles[i].SetPortalDistance(i, tiles.Count);
            }

            tiles[i].SetAsAstarTile();
            tiles[i].aStarId = i;
        }
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
        return (Mathf.Abs(from.GetRow() - to.GetRow()) + Mathf.Abs(from.GetCol() - to.GetCol()));
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

    private void SetDistanceForRemainingTiles(List<Tile> tiles)
    {
        for (int i = 1; i < tiles.Count-1; i++)
        {
            CheckForNoneMarkedTile(tiles[i]);
        }
        

        
    }
    private void CheckForNoneMarkedTile(Tile t)
    {
        for (int i = 0; i < t.wallArray.Length; i++)
        {
            if (t.wallArray[i] == 1) // if the tile is traversable in that direction
            {
                Tile neighbor = new Tile();
                switch (i)
                {
                    case 0:
                        neighbor = tileArray[t.GetRow() - 1, t.GetCol()]; //neighbor tile north
                        MarkTile(neighbor, t);
                        break;
                    case 1:
                        neighbor = tileArray[t.GetRow(), t.GetCol() + 1]; //neighbor tile east
                        MarkTile(neighbor, t);
                        break;
                    case 2:
                        neighbor = tileArray[t.GetRow() + 1, t.GetCol()]; //neighbor tile south
                        MarkTile(neighbor, t);
                        break;
                    case 3:
                        neighbor = tileArray[t.GetRow(), t.GetCol() - 1]; //neighbor tile west
                        MarkTile(neighbor, t);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void MarkTile(Tile neighbor, Tile t)
    {
        if (!neighbor.isAStarTile && !neighbor.isMarked)
        {
            neighbor.isMarked = true;
            neighbor.SetPortalDistance(t);
            CheckForNoneMarkedTile(neighbor);
        }
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

        foreach (Tile t in openTiles)
        {
            DrawGizmo(t, Color.red);
        }
    }

    public void DrawGizmo (Tile t, Color color, float scale = 0.25f)
    {
        if (gizmoPrefab == null)
        {
            SetGizmoPrefab();
        }
        GameObject gizmo = Instantiate(gizmoPrefab, t.transform);
        gizmo.GetComponent<drawGizmo>().SetColor(color);
        gizmo.GetComponent<drawGizmo>().SetSize(scale);
        gizmo.name = "Gizmo: R" + t.GetRow() + "C" + t.GetCol();
    }

    private void SetGizmoPrefab()
    {
        gizmoPrefab = Resources.Load<GameObject>("Prefabs/Gizmo");
    }
}
