using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextSceneButton : MonoBehaviour
{
    MapManager mm;
    List<MapGenerator> maps;
    Tile targetTile;

    bool registerPress = false;

    public float height;

    public bool inEndMaze; 

    // Start is called before the first frame update
    void Start()
    {
        mm = GameObject.Find("MapManager").GetComponent<MapManager>();
        maps = mm.mapScripts;

        if (inEndMaze)
        {
            targetTile = FindStartEndTile(maps[maps.Count - 1]);
        }
        else
        {
            targetTile = FindStartEndTile2(maps[0]);
        }

        transform.rotation = transform.rotation * mm.transform.rotation;
        transform.position = new Vector3(targetTile.transform.position.x, height, targetTile.transform.position.z);
    }

    public void GoToNextScene()
    {
        if (registerPress)
        {
            FinalTestSceneManager.instance.GoToNextScene();
        }
        registerPress = true;
    }
    private Tile FindStartEndTile2(MapGenerator map)
    {
        foreach (Tile t in map.tileArray) // finding the ideal tile
        {
            if (!t.isOuterTile && !t.isPortalTile && t.isRoomTile)
            {
                return t;
            }
        }
        return null;
    }
        private Tile FindStartEndTile(MapGenerator map)
    {
        foreach (Tile t in map.tileArray) // finding the ideal tile
        {
            if (t.isOuterTile && !t.isPortalTile && t.isRoomTile)
            {
                return t;
            }
        }
        foreach (Tile t in map.tileArray) // else allow for outertiles
        {
            if (!t.isPortalTile && t.isRoomTile)
            {
                return t;
            }
        }
        foreach (Tile t in map.tileArray) // else just place it on a tile that is not a outer tile
        {
            if (!t.isOuterTile)
            {
                return t;
            }
        }
        return null;
    }

}
