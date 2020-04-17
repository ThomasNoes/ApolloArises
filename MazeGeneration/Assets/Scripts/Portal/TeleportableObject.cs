﻿// This script should only be used on objects that are NOT interactable. For interactable objects use InteractableObject script!
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
    private WaitForSeconds delay;

    private void Start()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        renderController = FindObjectOfType<PortalRenderController>();
        delay = new WaitForSeconds(1.0f);
        Invoke("DelayedStart", 0.5f);
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

    private void Update()   // TODO: Used for debugging, remove later
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            CopySpawner(true, null);
        }
    }

    /// <param name="dir">false = prev, true = next</param>
    public void CopySpawner(bool dir, Collider col)
    {
        if (copyExist || !isParentObject || renderController == null)
            return;

        if (dir)
            offsetVector = renderController.GetNextOffset();
        else
            offsetVector = renderController.GetPrevOffset();

        thisObjCopy = Instantiate(gameObject, new Vector3(transform.position.x + offsetVector.x, transform.position.y + offsetVector.y, transform.position.z + offsetVector.z), transform.rotation);

        TeleportableObject tempScript = thisObjCopy.GetComponent<TeleportableObject>();
        tempScript.isParentObject = false;
        tempScript.mainObj = gameObject;
        thisObjCopy.GetComponent<Collider>().enabled = false;
        thisObjCopy.GetComponent<Rigidbody>().isKinematic = true;

        if (col != null)
        {
            NewTeleporter tempTeleScript = col.gameObject.GetComponent<NewTeleporter>();

            if (tempTeleScript != null)
                tempTeleScript.AddTeleportCopy(thisObjCopy);
        }

        copyExist = true;
    }

    public void CopyDespawnSelf()
    {
        if (!activated)
            Destroy(this);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (activated)
        {
            if (col.CompareTag("PortalGroundCol"))
            {
                if (!groundCooldown)
                {
                    StopAllCoroutines();

                    if (cooldown >= 0.2f)
                    {
                        groundCooldown = true;
                        Invoke("GroundCooldown", cooldown);
                    }

                    if (thisObjCopy == null)
                        CopySpawner(col.transform.parent.gameObject.GetComponent<NewTeleporter>().isForwardTeleporter ? true : false, col);
                }
            }
            if (teleportOnCollision)
                if (col.CompareTag("PortalRenderCol"))
                {
                    if (!renderCooldown)
                    {
                        StopAllCoroutines();

                        if (cooldown >= 0.2f)
                        {
                            renderCooldown = true;
                            Invoke("RenderCooldown", cooldown);
                        }

                        activated = false;
                        thisObjCopy.transform.position = transform.position;
                        transform.Translate(offsetVector, Space.World);
                        UpdateOffset(col.transform.parent.gameObject.GetComponent<NewTeleporter>().isForwardTeleporter);
                        activated = true;
                    }
                }
        }
    }

    public void Teleport(bool isForward)
    {
        StopAllCoroutines();

        activated = false;
        thisObjCopy.transform.position = transform.position;
        transform.Translate(offsetVector, Space.World);
        UpdateOffset(isForward);
        activated = true;
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("PortalGroundCol"))
        {
            StartCoroutine(InColliderCheck());
        }
    }

    private IEnumerator InColliderCheck()
    {
        yield return delay;

        if (thisObjCopy != null)
            Destroy(thisObjCopy);
    }

    /// <param name="dir">false: prev, true: next</param>
    public void UpdateOffset(bool dir)
    {
        if (renderController == null)
            return;

        if (dir)
            offsetVector = renderController.GetCurrentPos() - renderController.GetNextMazePos();
        else
            offsetVector = renderController.GetCurrentPos() - renderController.GetPrevMazePos();
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