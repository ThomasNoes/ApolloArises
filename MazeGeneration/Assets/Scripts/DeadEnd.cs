using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnd : MonoBehaviour
{
    public Tile de;
    public int mazeID;

    public DeadEnd(Tile de, int mazeID)
    {
        this.de = de;
        this.mazeID = mazeID;
    }

    public Tile GetTile()
    {
        return de;
    }


}
