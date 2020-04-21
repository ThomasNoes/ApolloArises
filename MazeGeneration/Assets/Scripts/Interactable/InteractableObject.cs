namespace Assets.Scripts.Interactable
{
    using UnityEngine;
    using System.Collections;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    [RequireComponent(typeof(OVRGrabbable), typeof(Rigidbody))]
    public class InteractableObject : MonoBehaviour
    {
        public PortalRenderController renderController;
        private Vector3 offsetVector;
        private GameObject thisObjCopy, mainObj, rightHandObj, leftHandObj;
        private bool copyExist, activated;
        [HideInInspector] public bool isParentObject = true, isInHand;
        private Collider currentCollider;

        private void Start()
        {
            renderController = FindObjectOfType<PortalRenderController>();
            Invoke("DelayedStart", 0.4f);
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

        //private void Update()   // TODO: Used for debugging, remove later
        //{
        //    if (Input.GetKeyDown(KeyCode.F))
        //    {
        //        CopySpawner(true, null);
        //    }
        //}

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

            InteractableObject tempScript = thisObjCopy.GetComponent<InteractableObject>();
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
                if (!isInHand)
                {
                    if (col.CompareTag("PortalGroundCol"))
                    {
                        if (thisObjCopy == null)
                            CopySpawner(col.transform.parent.gameObject.GetComponent<NewTeleporter>().isForwardTeleporter ? true : false, col);
                    }

                    if (col.CompareTag("EntryCol"))
                    {
                        currentCollider = col;

                        if (thisObjCopy == null)
                            CopySpawner(col.transform.parent.gameObject.GetComponent<NewTeleporter>().isForwardTeleporter ? true : false, col);

                    }

                    if (col.CompareTag("PortalRenderCol"))
                    {
                        StopAllCoroutines();

                        activated = false;
                        thisObjCopy.transform.position = transform.position;
                        transform.Translate(offsetVector, Space.World);
                        UpdateOffset(col.transform.parent.gameObject.GetComponent<NewTeleporter>().isForwardTeleporter);
                        activated = true;
                    }
                }
                else
                {
                    // TODO
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
                    }
                }
            }
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
                mainObj.GetComponent<InteractableObject>().copyExist = false;
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
}