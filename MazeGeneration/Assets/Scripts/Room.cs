﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int mazeID; // the maze segment that the room is in
    public List<Tile> tiles = new List<Tile>();// should only contain empty tiles which content can be place before used to generating content
    public Tile exitTile;
    public Tile entranceTile;


    public Room(RoomFinder rf)
    {
        mazeID = rf.mazeID;
        tiles = rf.GetRoom();
        FindEntryTiles();
    }

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
        Tile closeToEntrance = tiles[0];
        Tile closeToExit = tiles[0]; // set them to something and skip index 0

        for (int i = 1; i < tiles.Count; i++)
        {
            if (tiles[i].isAStarTile) // has to be an Astar tile
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

        }

        exitTile = closeToExit;
        entranceTile = closeToEntrance;
    }

    public void CreateRoom()
    {
        foreach (Tile t in tiles)
        {
            t.isRoomTile = true;
        }
        Tile.ConnectTiles(tiles[0], tiles[3]);
        //Tile.ConnectTiles(tileArray[tiles[0].GetRow(), tiles[0].GetCol()], tileArray[tiles[3].GetRow(), tiles[3].GetCol()]);
    }

    public void DebugRoom()
    {
        Debug.Log("Room at DeadEnd (" + tiles[0].GetRow()+ ","+ tiles[0].GetCol() + ") in maze " +mazeID);
    }
}
