using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabinet : MonoBehaviour
{
    public int uniqueId;
    public GameObject handle, door, objToSpawnInCabinet;
    [HideInInspector] public float tileWidth;

    public void SpawnItemInCabinet(Transform objTransform, ItemSpawner itemSpawner)
    {
        if (objToSpawnInCabinet != null)
        {
            Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z);
            Quaternion spawnRot = Quaternion.Euler(90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            GameObject tempObj = Instantiate(objToSpawnInCabinet, spawnPos, spawnRot, objTransform.parent != null ? objTransform.parent : objTransform);

            Key tempKey = tempObj.GetComponent<Key>();

            if (tempKey != null)
            {
                tempKey.itemSpawner = itemSpawner;
                tempKey.uniqueId = uniqueId;
            }
        }
    }
}