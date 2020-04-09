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

    private void SpawnKey(int index)
    {
        if (keyPrefab == null)
            return;

        //GameObject tempKey = Instantiate(doorPrefab, mapManager.deadEndList,
        //            Quaternion.identity, room.exitTile.gameObject.transform);

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
                GameObject tempDoor = Instantiate(doorPrefab, mapManager.roomList[i].exitTile.gameObject.transform.position,
                    Quaternion.identity, mapManager.roomList[i].exitTile.gameObject.transform);

                SpawnKey(i);
            }

            counter = counter++ % spawnFrequency;
        }

    }
}
