using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject keyPrefab, doorPrefab;
    public bool spawnKeys = true, spawnDoors = true;
    [Tooltip("Example: 2 means for every second room")] public int spawnFrequency = 2;
    private MapManager mapManager;

    void Start()
    {
        mapManager = GameObject.FindGameObjectWithTag("MapManager")?.GetComponent<MapManager>();
        if (mapManager == null)
            mapManager = FindObjectOfType<MapManager>();

        if (mapManager != null)
        {
            if (spawnDoors)
                DoorAndKeySpawner();
        }

    }

    private bool SpawnKey(int mazeIndex)
    {
        if (keyPrefab == null)
            return false;

        if (mazeIndex - 1 >= 0)
        {
            for (int i = mapManager.deadEndList.Length - 1; i >= 0; i--)
            {
                GameObject tempKey = Instantiate(keyPrefab,
                    mapManager.deadEndList[mazeIndex - 1][mapManager.deadEndList[mazeIndex - 1].Count - 1].transform
                        .position,
                    Quaternion.identity, transform);
            }
        }
        else
            return false;

        return true;
    }

    private void DoorAndKeySpawner()
    {
        if (doorPrefab == null)
            return;

        int counter = 0;
        for (int i = 0; i < mapManager.roomList.Count; i++)
        {
            if (counter == 0)
            {
                if (SpawnKey(mapManager.roomList[i].mazeID))
                {
                    GameObject tempDoor = Instantiate(doorPrefab, mapManager.roomList[i].exitTile.gameObject.transform.position,
                                             Quaternion.identity, mapManager.roomList[i].exitTile.gameObject.transform);
                }
            }

            counter = counter++ % spawnFrequency;
        }

    }
}
