﻿namespace Assets.Scripts.Interactable
{
    using UnityEngine;
    using UnityEditor;

    [RequireComponent(typeof(OVRGrabbable), typeof(Rigidbody))]
    public class InteractableObject : MonoBehaviour
    {
        public float offset = 4.0f; // TODO: Temporary, make based on maze offset.
        private GameObject[] thisObjectCopies;
        private bool copyExist;

        private void Start()
        {
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                CopySpawner();
            }
        }

        public void CopySpawner()
        {
            if (copyExist)
                return;

            GameObject tempObject = new GameObject();

            MeshRenderer tempMesh = gameObject.GetComponent<MeshRenderer>();

            tempObject.AddComponent<MeshRenderer>();
            tempObject.AddComponent<MeshFilter>();

            thisObjectCopies = new GameObject[2];

            thisObjectCopies[0] = Instantiate(tempObject, new Vector3(transform.position.x + offset, transform.position.y, transform.position.z), transform.rotation, transform);
            thisObjectCopies[1] = Instantiate(tempObject, new Vector3(transform.position.x - offset, transform.position.y, transform.position.z), transform.rotation, transform);

            copyExist = true;
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

            //if (script.gameObject.layer != "Interactable")
            //    //script.gameObject.tag = "Interactable";
        }
    }
#endif
}