namespace Assets.Scripts.Interactable
{
    using UnityEngine;
    using UnityEditor;

    [RequireComponent(typeof(OVRGrabbable), typeof(Rigidbody))]
    public class InteractableObject : MonoBehaviour
    {
        public PortalRenderController renderController;
        private Vector3 prevOffsetVector, nextOffsetVector;
        private GameObject thisObjCopy, mainObj, rightHandObj, leftHandObj;
        private bool copyExist, activated;
        [HideInInspector] public bool isParentObject = true;

        private void Start()
        {
            renderController = FindObjectOfType<PortalRenderController>();
            Invoke("DelayedStart", 0.5f);
        }

        private void DelayedStart()
        {
            if (renderController == null)
                return;

            prevOffsetVector = renderController.GetPrevOffset();
            nextOffsetVector = renderController.GetNextOffset();

            if (isParentObject)
                activated = true;
        }

        private void LateUpdate()
        {
            if (activated && copyExist)
            {
                if (thisObjCopy != null)
                {
                    thisObjCopy.transform.position = transform.position + nextOffsetVector;
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
            if (copyExist || !isParentObject)
                return;

            if (dir)
                thisObjCopy = Instantiate(gameObject, new Vector3(transform.position.x + nextOffsetVector.x, transform.position.y + nextOffsetVector.y, transform.position.z + nextOffsetVector.z), transform.rotation);
            else
                thisObjCopy = Instantiate(gameObject, new Vector3(transform.position.x + prevOffsetVector.x, transform.position.y + prevOffsetVector.y, transform.position.z + prevOffsetVector.z), transform.rotation);

            InteractableObject tempScript = thisObjCopy.GetComponent<InteractableObject>();
            tempScript.isParentObject = false;
            tempScript.mainObj = gameObject;
            thisObjCopy.GetComponent<Collider>().enabled = false;
            thisObjCopy.GetComponent<Rigidbody>().isKinematic = true;

            if (col != null)
            {
                col.gameObject.GetComponent<Teleporter>()?.AddTeleportCopy(thisObjCopy);
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
                if (CompareTag("PortalGroundCol"))
                {
                    CopySpawner(col.gameObject.GetComponent<Teleporter>().isForwardTeleporter ? true : false, col);
                }
        }

        private void OnTriggerExit(Collider col)
        {
            if (activated)
                if (CompareTag("PortalGroundCol"))
                {
                    Destroy(thisObjCopy);
                }
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