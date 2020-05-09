using UnityEngine;
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
    private bool copyExist, activated, inHand, justTeleported, inPortal, inEntryCol, thrownThrough;
    [HideInInspector] public bool isParentObject = true;
    private Collider currentCollider;
    private SVGrabbable svGrabbable;
    private Rigidbody rb;
    private int inCurrentMaze = 0;

    private void Start()
    {
        renderController = FindObjectOfType<PortalRenderController>();
        svGrabbable = GetComponent<SVGrabbable>();
        rb = GetComponent<Rigidbody>();

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
        if (thisObjCopy != null && activated)
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
                        else if (!thrownThrough && !inEntryCol)
                        {
                            transform.Translate(offsetVector, Space.World);
                            inCurrentMaze = renderController.currentMaze;
                            offsetVector *= -1;
                        }
                    }
                }
            }

            if (copyExist)
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

        Collider[] colliders = thisObjCopy.GetComponents<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

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
            inEntryCol = true;
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

                thrownThrough = true;
                Invoke("IsThrownCooldown", 1.2f);

                activated = true;
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

            inEntryCol = false;
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

    private void IsThrownCooldown()
    {
        thrownThrough = false;
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