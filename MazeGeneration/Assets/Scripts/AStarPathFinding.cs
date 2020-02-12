using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class AStarPathFinding : MonoBehaviour
{
    // https://www.youtube.com/watch?v=-L-WgKMFuhE

    GameObject gizmoPrefab;

    GameObject start;
    GameObject end;
    
    GameObject[] openTiles;
    GameObject[] closedTiles;

    static GameObject[] aStarTiles;


    //pathFinding
    public static GameObject[] BeginAStar(Tile[,] tileArray, TileInfo start, TileInfo end)
    {

        return aStarTiles;
    }

    //draw Astar path
    public void DrawAStarPath()
    {
        if(gizmoPrefab == null)
        {
            SetGizmoPrefab();
        }
        foreach (GameObject t in aStarTiles)
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
        foreach (GameObject t in openTiles)
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
        foreach (GameObject t in openTiles)
        {
            DrawGizmo(t, Color.red);
        }
    }

    private void DrawGizmo (GameObject theTile, Color color)
    {
        GameObject newObject = Instantiate(gizmoPrefab,theTile.transform);
        newObject.GetComponent<drawGizmo>().SetColor(color);
    }

    private void SetGizmoPrefab()
    {
        gizmoPrefab = Resources.Load<GameObject>("Prefabs/Gizmo");
    }
}
