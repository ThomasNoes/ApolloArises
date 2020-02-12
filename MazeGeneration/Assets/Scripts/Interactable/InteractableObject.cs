namespace Assets.Scripts.Interactable
{
    using UnityEngine;
    using UnityEditor;

    [RequireComponent(typeof(OVRGrabbable), typeof(Rigidbody))]
    public class InteractableObject : MonoBehaviour
    {
        public float offset = 4.0f; // TODO: Temporary, make based on maze offset.
        private Vector3 prevOffsetVector, nextOffsetVector;
        private GameObject thisObjectCopy, parentObject;
        private bool copyExist, activated;
        [HideInInspector] public bool isParentObject = true;

        private void Start()
        {
            prevOffsetVector = new Vector3(-offset, 0, 0);
            nextOffsetVector = new Vector3(offset, 0, 0);
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
                if (thisObjectCopy != null)
                {
                    thisObjectCopy.transform.position = transform.position + nextOffsetVector;
                    thisObjectCopy.transform.rotation = transform.rotation;
                }
            }
        }

        //private void Update()   // TODO: Used for debugging, remove later
        //{
        //    if (Input.GetKeyDown(KeyCode.F))
        //    {
        //        CopySpawner(1, null);
        //    }
        //}

        public void CopySpawner(int dir, Collider col)    // 0 = prev, 1 = next
        {
            if (copyExist || !isParentObject)
                return;

            thisObjectCopy = Instantiate(gameObject, new Vector3(transform.position.x + offset, transform.position.y, transform.position.z), transform.rotation);

            InteractableObject tempScript = thisObjectCopy.GetComponent<InteractableObject>();
            tempScript.isParentObject = false;
            tempScript.parentObject = gameObject;
            thisObjectCopy.GetComponent<Collider>().enabled = false;
            thisObjectCopy.GetComponent<Rigidbody>().isKinematic = true;

            if (col != null)
            {
                col.gameObject.GetComponent<Teleporter>().AddTeleportCopy(thisObjectCopy);
            }
            
            copyExist = true;
        }

        private void OnTriggerEnter(Collider col)
        {
            if (activated)
                if (CompareTag("Portal"))
                {
                    CopySpawner(col.gameObject.GetComponent<Teleporter>().isForwardTeleporter ? 1 : 0, col);
                }
        }

        public void SetParentObject(GameObject parentObj)
        {
            parentObject = parentObj;
        }

        private void OnDestroy()
        {
            if (!isParentObject && parentObject != null)
                parentObject.GetComponent<InteractableObject>().copyExist = false;
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