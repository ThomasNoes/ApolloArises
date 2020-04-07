namespace Assets.Scripts.Interactable
{
    using UnityEngine;
    using UnityEditor;

    [RequireComponent(typeof(OVRGrabbable), typeof(Rigidbody))]
    public class InteractableObject : MonoBehaviour
    {
        public PortalRenderController renderController;
        private Vector3 offsetVector;
        private GameObject thisObjCopy, mainObj, rightHandObj, leftHandObj;
        private bool copyExist, activated;
        [HideInInspector] public bool isParentObject = true, isInHand;

        private void Start()
        {
            renderController = FindObjectOfType<PortalRenderController>();
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
                        if (copyExist && thisObjCopy != null)
                        {
                            Destroy(thisObjCopy);
                            copyExist = false;
                        }

                        CopySpawner(col.transform.parent.gameObject.GetComponent<NewTeleporter>().isForwardTeleporter ? true : false, col);
                    }
                    if (col.CompareTag("PortalRenderCol"))
                    {
                        activated = false;
                        thisObjCopy.transform.position = transform.position;
                        transform.Translate(offsetVector, Space.World);
                        UpdateOffset(col.transform.parent.gameObject.GetComponent<NewTeleporter>().isForwardTeleporter);
                        Debug.Log("TELEPORTED");
                    }
                }
                else
                {
                    // TODO
                }

            //if (activated)
            //    if (!isInHand)
            //        if (col.CompareTag("PortalRenderCol"))
            //        {
            //            CopySpawner(col.transform.parent.gameObject.GetComponent<NewTeleporter>().isForwardTeleporter ? true : false, col);
            //        }
        }

        private void OnTriggerExit(Collider col)
        {
            //if (activated)
            //{
            //    if (!isInHand && isParentObject)
            //        if (col.CompareTag("PortalRenderCol"))
            //        {
            //            if (thisObjCopy != null)
            //                Destroy(thisObjCopy);
            //        }
            //}
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

            activated = true;
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