using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabinet : MonoBehaviour
{
    public int uniqueId;
    public GameObject door, objToSpawnInCabinet;
    private HingeJoint hingeJoint;
    private Vector3 anchor, axis;
    [HideInInspector] public float tileWidth;

    void Start()
    {
        if (door == null)
            return;

        hingeJoint = door.GetComponent<HingeJoint>();
        anchor = hingeJoint.anchor;
        axis = hingeJoint.axis;
        hingeJoint.anchor = Vector3.zero;
        hingeJoint.axis = Vector3.zero;

        Invoke("DelayedStart", 0.3f);
    }

    private void DelayedStart()
    {
        hingeJoint.anchor = anchor;
        hingeJoint.axis = axis;
    }

    public void SpawnItemInCabinet(Transform objTransform, ItemSpawner itemSpawner)
    {
        if (objToSpawnInCabinet != null)
        {
            GameObject tempObj = Instantiate(objToSpawnInCabinet, transform.position, Quaternion.identity);

            Key tempKey = tempObj.GetComponent<Key>();

            if (tempKey != null)
            {
                tempKey.itemSpawner = itemSpawner;
                tempKey.uniqueId = uniqueId;
            }
        }
    }
}