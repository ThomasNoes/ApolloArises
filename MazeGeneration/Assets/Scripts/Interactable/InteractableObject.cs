﻿using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SVGrabbable), typeof(Rigidbody))]
public class InteractableObject : MonoBehaviour
{
    public PortalRenderController renderController;
    private Vector3 offsetVector;
    private GameObject thisObjCopy, mainObj, rightHandObj, leftHandObj;
    private bool copyExist, activated, inHand, justTeleported, inPortal, disableTeleport, thrownThrough;
    [HideInInspector] public bool isParentObject = true;
    private Collider currentCollider;
    private SVGrabbable svGrabbable;
    private int inCurrentMaze = 0;

    private void Start()
    {
        renderController = FindObjectOfType<PortalRenderController>();
        svGrabbable = GetComponent<SVGrabbable>();
        Invoke("DelayedStart", 0.4f);
    }

    private void DelayedStart()
    {
        if (isParentObject)
            activated = true;
    }

    private void Cooldown()
    {
        inHand = false;
    }

    private void LateUpdate()
    {
        if (thisObjCopy != null)
        {
            if (isParentObject && renderController != null)
            {
                if (renderController.currentMaze != inCurrentMaze)
                {
                    if (renderController.currentMaze == inCurrentMaze + 1 ||
                        renderController.currentMaze == inCurrentMaze - 1)
                    {
                        if (svGrabbable.inHand)
                            offsetVector *= -1;
                        else
                        {
                            transform.Translate(offsetVector, Space.World);
                            inCurrentMaze = renderController.currentMaze;
                            offsetVector *= -1;
                        }
                    }
                }

                if (svGrabbable.inHand && thrownThrough)
                {
                    thrownThrough = false;
                }
            }

            if (activated && copyExist)
            {
                thisObjCopy.transform.position = transform.position + offsetVector;
                thisObjCopy.transform.rotation = transform.rotation;
            }
        }
    }

    /// <param name="dir">false = prev, true = next</param>
    public void CopySpawner(bool dir, Collider col)
    {
        if (copyExist || !isParentObject || renderController == null)
            return;

        UpdateOffset(dir);

        thisObjCopy = Instantiate(gameObject, new Vector3(transform.position.x + offsetVector.x,
            transform.position.y + offsetVector.y, transform.position.z + offsetVector.z), transform.rotation);

        InteractableObject tempScript = thisObjCopy.GetComponent<InteractableObject>();
        tempScript.isParentObject = false;
        tempScript.mainObj = gameObject;
        thisObjCopy.GetComponent<Collider>().enabled = false;
        thisObjCopy.GetComponent<Rigidbody>().isKinematic = true;

        SVGrabbable grabbable = thisObjCopy.GetComponent<SVGrabbable>();

        if (grabbable != null) // check if grabbable in copy exist
        {
            grabbable.canGrab = false;
            grabbable.inHand = false;
            grabbable.isKnockable = false;
        }

        copyExist = true;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!isParentObject)
            return;

        if (activated)
            if (col.CompareTag("PortalGroundCol"))
            {
                NewTeleporter thisTeleporter = col.transform.parent.GetComponent<NewTeleporter>();
                inCurrentMaze = thisTeleporter.mazeID;

                if (thisObjCopy == null)
                    CopySpawner(thisTeleporter.isForwardTeleporter ? true : false, col);
            }

        if (col.CompareTag("EntryCol"))
        {
            NewTeleporter thisTeleporter = col.transform.parent.GetComponent<NewTeleporter>();
            currentCollider = col;
            inCurrentMaze = thisTeleporter.mazeID;

            if (thisObjCopy == null)
                CopySpawner(thisTeleporter.isForwardTeleporter ? true : false, col);

        }

        if (col.CompareTag("PortalRenderCol"))
        {
            NewTeleporter thisTeleporter = col.transform.parent.GetComponent<NewTeleporter>();
            inCurrentMaze = thisTeleporter.mazeID;

            if (thisObjCopy == null)
                CopySpawner(thisTeleporter.isForwardTeleporter ? true : false, col);

            if (svGrabbable.inHand)
            {
                //offsetVector *= -1; // TODO should be disabled?
                return;
            }

            if (thisObjCopy != null && !thrownThrough)
            {
                activated = false;
                thisObjCopy.transform.position = transform.position;
                transform.Translate(offsetVector, Space.World);
                offsetVector *= -1;

                if (thisTeleporter.isForwardTeleporter)
                    inCurrentMaze++;
                else
                    inCurrentMaze--;

                activated = true;
                thrownThrough = true;
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("EntryCol"))
        {
            if (thisObjCopy != null)
            {
                Vector3 currentRenderPlanePos = currentCollider.transform.parent.GetChild(0).transform.position;
                Vector3 currentRenderPlanePosNoY = new Vector3(currentRenderPlanePos.x, 0, currentRenderPlanePos.z);
                Vector3 currentPosNoY = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 currentEntryColNoY = new Vector3(currentCollider.transform.position.x, 0, currentCollider.transform.position.z);

                if (Vector3.Distance(currentEntryColNoY, currentRenderPlanePosNoY) <
                    Vector3.Distance(currentRenderPlanePosNoY, currentPosNoY))
                {
                    Destroy(thisObjCopy);
                    copyExist = false;
                }
            }
        }
    }

    /// <param name="dir">false: prev, true: next</param>
    public void UpdateOffset(bool dir)
    {
        if (renderController == null)
            return;

        offsetVector = dir ? PortalRenderController.SetNextOffset(inCurrentMaze) : PortalRenderController.SetPrevOffset(inCurrentMaze);
    }

    public void SetParentObject(GameObject parentObj)
    {
        mainObj = parentObj;
    }

    private void OnDestroy()
    {
        if (!isParentObject && mainObj != null)
            mainObj.GetComponent<InteractableObject>().copyExist = false;

        if (isParentObject && thisObjCopy != null)
            Destroy(thisObjCopy);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(InteractableObject))]
public class Interactable_Editor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var script = target as InteractableObject;
    }
}
#endif