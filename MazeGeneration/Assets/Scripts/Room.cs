using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int mazeID; // the maze segment that the room is in
    public List<Tile> tiles; // should only contain empty tiles which content can be place before used to generating content
    public List<Tile> entryTiles; // should only contain tiles lead in and out of the maze. [0] is closest to exit portal. [1] is closest to the entrance portal

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

    
}
