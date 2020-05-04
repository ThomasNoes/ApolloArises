// This script should only be used on objects that are NOT interactable. For interactable objects use InteractableObject script!
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class TeleportableObject : MonoBehaviour
{
    public bool teleportOnCollision = false;
    public float cooldown = 0.0f;

    [HideInInspector] public PortalRenderController renderController;
    private Vector3 offsetVector;
    private GameObject thisObjCopy, mainObj, rightHandObj, leftHandObj;
    private bool copyExist, activated;
    [HideInInspector] public bool isParentObject = true, groundCooldown, renderCooldown;
    private Collider currentCollider;
    private int inCurrentMaze = 0;

    private void Start()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        renderController = FindObjectOfType<PortalRenderController>();
        Invoke("DelayedStart", 0.3f);
    }

    private void DelayedStart()
    {
        if (isParentObject)
            activated = true;
    }

    private void LateUpdate()
    {
        if (activated && copyExist)
        {
            if (thisObjCopy != null)
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

        TeleportableObject tempScript = thisObjCopy.GetComponent<TeleportableObject>();
        tempScript.isParentObject = false;
        tempScript.mainObj = gameObject;
        thisObjCopy.GetComponent<Collider>().enabled = false;
        thisObjCopy.GetComponent<Rigidbody>().isKinematic = true;

        //if (col != null)
        //{
        //    NewTeleporter tempTeleScript = col.gameObject.GetComponent<NewTeleporter>();

        //    if (tempTeleScript != null)
        //        tempTeleScript.AddTeleportCopy(thisObjCopy);
        //}

        copyExist = true;
    }

    public void CopyDespawnSelf()
    {
        if (!activated)
            Destroy(this);
    }

    public void DestroyCopy()
    {
        if (thisObjCopy != null)
            Destroy(thisObjCopy);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (activated)
        {
            if (col.CompareTag("PortalGroundCol"))
            {
                NewTeleporter thisTeleporter = col.transform.parent.GetComponent<NewTeleporter>();

                inCurrentMaze = thisTeleporter.mazeID;

                if (!groundCooldown)
                {
                    if (cooldown >= 0.2f)
                    {
                        groundCooldown = true;
                        Invoke("GroundCooldown", cooldown);
                    }

                    if (thisObjCopy == null)
                        CopySpawner(thisTeleporter.isForwardTeleporter ? true : false, col);
                }
            }
            if (col.CompareTag("EntryCol"))
            {
                currentCollider = col;
                NewTeleporter thisTeleporter = col.transform.parent.GetComponent<NewTeleporter>();

                inCurrentMaze = thisTeleporter.mazeID;

                if (thisObjCopy == null)
                    CopySpawner(thisTeleporter.isForwardTeleporter ? true : false, col);

            }

            if (col.CompareTag("PortalRenderCol"))
            {
                if (teleportOnCollision)
                {
                    if (!renderCooldown)
                    {
                        if (cooldown >= 0.2f)
                        {
                            renderCooldown = true;
                            Invoke("RenderCooldown", cooldown);
                        }

                        inCurrentMaze = col.transform.parent.GetComponent<NewTeleporter>().mazeID;

                        activated = false;
                        thisObjCopy.transform.position = transform.position;
                        transform.Translate(offsetVector, Space.World);
                        offsetVector *= -1;
                        //UpdateOffset(col.transform.parent.gameObject.GetComponent<NewTeleporter>().isForwardTeleporter);
                        activated = true;
                    }
                }
                else
                    inCurrentMaze = col.transform.parent.GetComponent<NewTeleporter>().mazeID;
            }
            
        }
    }

    public void Teleport(bool isForward)
    {
        activated = false;
        thisObjCopy.transform.position = transform.position;
        transform.Translate(offsetVector, Space.World);

        offsetVector *= -1;

        activated = true;
    }

    public void TeleportFromIndex(bool isForward, int currentIndex)
    {
        activated = false;

        if (isForward)
            inCurrentMaze = currentIndex + 1;
        else
            inCurrentMaze = currentIndex - 1;

        thisObjCopy.transform.position = transform.position;
        transform.Translate(offsetVector, Space.World);

        offsetVector *= -1;

        activated = true;
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
                }
            }
        }
    }

    /// <param name="dir">false: prev, true: next</param>
    public void UpdateOffset(bool dir, int currentIndex)
    {
        if (renderController == null)
            return;

        offsetVector = dir ? PortalRenderController.SetNextOffset(currentIndex) : PortalRenderController.SetPrevOffset(currentIndex);
    }

    public void UpdateOffset(bool dir)
    {
        UpdateOffset(dir, inCurrentMaze);
    }

    public void SetParentObject(GameObject parentObj)
    {
        mainObj = parentObj;
    }

    private void OnDestroy()
    {
        if (!isParentObject && mainObj != null)
            mainObj.GetComponent<TeleportableObject>().copyExist = false;
    }

    private void RenderCooldown()
    {
        renderCooldown = false;
    }

    private void GroundCooldown()
    {
        groundCooldown = false;
    }
}