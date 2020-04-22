﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacks;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CompanionPathFinding : MonoBehaviour
{
    Vector3 posOffset = new Vector3(0, -0.2f, 0);
    float height;
    LayerMask layerMask;

    public bool isFollowPlayer;
    public GameObject player;
    public Tile currentTile;
    public Tile targetTile;
    float tileWidth;

    //other scripts
    List<MapGenerator> maps;
    MapManager mm;
    TeleportableObject tele;
    BeaconManager bm;

    public float speed = 1.5f;
    public float angularSpeed = 1.75f;
    List<Tile> pathPoints = new List<Tile>();
    bool isTravelling = false;

    AStarPathFinding Astar = new AStarPathFinding();


    // Start is called before the first frame update
    void Start()
    {

        mm = GameObject.Find("MapManager").GetComponent<MapManager>();
        maps = mm.mapScripts;
        tele = GetComponent<TeleportableObject>();
        bm = GameObject.Find("BeaconManager").GetComponent<BeaconManager>();
        player = Camera.main.gameObject;
        tileWidth = mm.tileWidth;


        Tile tile = maps[0].aStarTiles[0];


        //placing companion on non a star tile
        foreach (Tile t in maps[0].tileArray)
        {
            if (!t.isAStarTile)
            {
                tile = t;
            }
        }

        height= FindObjectOfType<TerrainGenerator>().wallHeight;
        transform.position = GetCompanionPosFromTile(tile);


        // debug placing 
        //targetTile = maps[6].aStarTiles[3];// maps[1].aStarTiles.Count - 1];
        foreach (Tile t in maps[0].tileArray)
        {
            if (!t.isAStarTile)
            {
                targetTile = t;
            }
        }

        layerMask = LayerMask.GetMask("Floor");
    }

    // Update is called once per frame
    public void Update()
    {
        if (!isTravelling)
        {
            if (isFollowPlayer)
            {
                targetTile = GetTileUnderObject(player);
                currentTile = GetTileUnderObject(gameObject);
            }

            if (currentTile != targetTile && targetTile != null)
            {
                GoToTarget();
            }
        }
    }

    public void GoToTarget()
    {
        isTravelling = true;
        PlanRoute(targetTile);
        StartCoroutine("MoveToTarget");
    }

    private IEnumerator MoveToTarget()
    {
        Tile pastPoint = currentTile;
        bool teleAllowed = true;
        for (int i = 0; i < pathPoints.Count; i++)
        {
            //teleport
            if (pathPoints[i].isPortalTile && pastPoint.isPortalTile && teleAllowed) // if the tile companion is going a portal and is currently on a portal tile 
            {
                
                if (pathPoints[i].partOfMaze > pastPoint.partOfMaze) //teleport Next maze forward
                {
                    Debug.Log("forward tele: pathpoint " + pathPoints[i].name + " pastpoint " + pastPoint.name);
                    tele.TeleportFromIndex(true, pastPoint.partOfMaze);
                    teleAllowed = false;
                }
                else if (pathPoints[i].partOfMaze < pastPoint.partOfMaze) //teleport prev maze backward
                {
                    Debug.Log("backward tele: pathpoint " + pathPoints[i].name + " pastpoint " + pastPoint.name);
                    tele.TeleportFromIndex(false, pastPoint.partOfMaze);
                    teleAllowed = false;
                }
            }
            else
            {
                //traverse and rotate
                Vector3 point = GetCompanionPosFromTile(pathPoints[i]);

                Vector3 pastPointOffsetted = GetCompanionPosFromTile(pastPoint);

                Vector3 newDirection = (point - pastPointOffsetted).normalized;
                if(Vector3.Distance(point, pastPointOffsetted) < tileWidth + 0.1)
                {
                    //Debug.Log("The distance is correct" + Vector3.Distance(point, pastPointOffsetted));
                }
                else
                {
                    Debug.Log("The distance is incorrect" + Vector3.Distance(point, pastPointOffsetted));
                }

                if (newDirection.magnitude == 1)
                {
                    //rotate the companion to face the direction it is about to move to
                    while (Vector3.Angle(transform.forward, newDirection)>1.0f)
                    {
                        transform.forward = Vector3.RotateTowards(transform.forward, newDirection, angularSpeed * Time.deltaTime, 0.0f);
                        yield return null;
                    }
                }

                //new move to the next point!
                while (transform.position != point)
                {
                    transform.position = Vector3.MoveTowards(transform.position, point, speed * Time.deltaTime);
                    yield return null;
                }
                teleAllowed = true;
            }

            pastPoint = pathPoints[i];
        }
        //pathPoints.Clear();
        isTravelling = false;
    }

    private void PlanRoute(Tile target)
    {
        //currentTile = GetTileUnderObject(gameObject);

        if (target == null)
        {
            Debug.Log("target is null");
            return;
        }
        List<Tile> tempPath = new List<Tile>();
        //tempPath.Add(currentTile);

        int currentMaze = currentTile.partOfMaze;
        Tile tempTile = currentTile;
        Tile tempTarget;

        while (tempTile != target)
        {
            if (target.partOfMaze < tempTile.partOfMaze && // if the companion needs to travel to prev segment
                tempTile == maps[currentMaze].aStarTiles[0]) // it stands on the portal tile to the prev segment 
            {
                currentMaze--;
                tempTile = maps[currentMaze].tileArray[tempTile.GetRow(), tempTile.GetCol()];
            }
            else if (target.partOfMaze > tempTile.partOfMaze && // if the companion needs to travel to next segment
                tempTile == maps[currentMaze].aStarTiles[maps[currentMaze].aStarTiles.Count-1]) // it stands on the portal tile to the next segment 
            {
                currentMaze++;
                tempTile = maps[currentMaze].tileArray[tempTile.GetRow(), tempTile.GetCol()];
            }

            if (target.partOfMaze < tempTile.partOfMaze)
            {
                tempTarget = maps[currentMaze].aStarTiles[0];
            }
            else if (target.partOfMaze > tempTile.partOfMaze)
            {
                tempTarget = maps[currentMaze].aStarTiles[maps[currentMaze].aStarTiles.Count - 1];
            }
            else
            {
                tempTarget = target;
            }

            tempPath.AddRange(Astar.NPCPathFinding(maps[tempTile.partOfMaze].tileArray, tempTile, tempTarget, false, false));
            tempTile = tempTarget;
        }
        //tempPath now have the complete route for the companion to follow.
        //cut the route if there is a door blocking or the maze segment is turned of
        pathPoints = CutRouteAtBlockade(tempPath);
        //pathPoints.RemoveAt(0);
    }

    private List<Tile> CutRouteAtBlockade(List<Tile> route)
    {
        List<Tile> safeRoute = new List<Tile>();
        for (int i = 0; i < route.Count; i++)
        {
            if (route[i].blocked ) //stop the forloop and return the route so far
            {
                break;
            }
            if (bm.beacons.Count != 0)
            {
                if (!bm.beacons[route[i].partOfMaze].isActive)
                {
                    break;
                } 
            }
            else
            {
                safeRoute.Add(route[i]);
            }
        }
        return safeRoute;
    }

    private List<Tile> GetPartofAStarPath(Tile from, Tile to)
    {
        List<Tile> returnList = new List<Tile>();
        List<Tile> aStars = maps[from.partOfMaze].aStarTiles;


        int fromIndex = aStars.IndexOf(from);
        int toIndex = aStars.IndexOf(to);

        while (fromIndex != toIndex)
        {
            returnList.Add(aStars[fromIndex]);

            if (fromIndex < toIndex)
            {
                fromIndex++;
            }
            else
            {
                fromIndex--;
            }
        }
        returnList.Add(to);

        return returnList;
    }

   
    private void ListDebug()
    {
        for (int i = 0; i < maps.Count; i++)
        {
            for (int j = 0; j < maps[i].aStarTiles.Count; j++)
            {
                Debug.Log(maps[i].aStarTiles[j].name + " is astar number " + j + " in maze " + i);
            }
        }
    }


    private Tile GetTileUnderObject(GameObject go)
    {
        RaycastHit hit;

        //Debug.DrawRay(go.transform.position, Vector3.down * 7, Color.yellow, 25);

        if (Physics.Raycast(go.transform.position, Vector3.down, out hit, 7.0f, layerMask))
        {
            //Debug.Log(go.name+" has hit "+hit.collider.name +" on "+hit.collider.transform.parent.name + " on "+hit.collider.transform.parent.parent.name);
            Tile tempTile = hit.collider.gameObject.GetComponentInParent<Tile>();

            if (tempTile != null)
            {
                //Debug.Log("found a tile");
                return tempTile;
            }
        }
        else
        {
            Debug.Log("the companion is not over a tile. and it is assumed the companion has teleported incorrectly. so it is placed on the same tile as the player");
            Tile playerTile = GetTileUnderObject(player);
            transform.position = GetCompanionPosFromTile(playerTile);
            return playerTile;
        }
        return null;
    }

    private Vector3 GetCompanionPosFromTile(Tile tile)
    {
        return new Vector3(tile.transform.position.x, tile.transform.position.y + height, tile.transform.position.z) + posOffset;
    }


}

