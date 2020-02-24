using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MapInfo
{
    public enum MapType
    {
        Maze,
        Room
    }

    public MapType mapType;
    public string roomName;
    public bool isEndSeeded;
    public TileInfo startSeed;
    public TileInfo endSeed;
    public GameObject mapObject;
}
