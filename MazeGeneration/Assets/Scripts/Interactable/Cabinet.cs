using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabinet : MonoBehaviour
{
    public int uniqueId;
    public GameObject handle, door, objToSpawnInCabinet;
    private SVGrabbable svGrabbable;
    private HingeJoint hingeJoint;
    private Vector3 anchor, axis;
    private float speed = 10.0f;
    [HideInInspector] public float tileWidth;

    void Start()
    {
        if (door == null)
            return;

        hingeJoint = door.GetComponent<HingeJoint>();

        if (handle != null)
        {
            svGrabbable = handle.GetComponent<SVGrabbable>();
        }

        if (hingeJoint == null)
            return;

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

    //private void FixedUpdate()
    //{
    //    if (handle == null || svGrabbable == null || door == null)
    //        return;

    //    Vector3 targetDirection = handle.transform.position - door.transform.position;

    //    // The step size is equal to speed times frame time.
    //    float singleStep = speed * Time.deltaTime;

    //    // Rotate the forward vector towards the target direction by one step
    //    Vector3 newDirection = Vector3.RotateTowards(door.transform.position, targetDirection, singleStep, 0.0f);

    //    // Calculate a rotation a step closer to the target and applies rotation to this object
    //    door.transform.rotation = Quaternion.LookRotation(newDirection);
    //}

    public void SpawnItemInCabinet(Transform objTransform, ItemSpawner itemSpawner)
    {
        if (objToSpawnInCabinet != null)
        {
            Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z);
            Quaternion spawnRot = Quaternion.Euler(90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            GameObject tempObj = Instantiate(objToSpawnInCabinet, spawnPos, spawnRot, itemSpawner.gameObject.transform);

            Key tempKey = tempObj.GetComponent<Key>();

            if (tempKey != null)
            {
                tempKey.itemSpawner = itemSpawner;
                tempKey.uniqueId = uniqueId;
            }
        }
    }
}