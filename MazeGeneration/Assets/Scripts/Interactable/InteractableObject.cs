using UnityEditor;

namespace Assets.Scripts.Interactable
{
    using UnityEngine;

    [RequireComponent(typeof(OVRGrabbable), typeof(Rigidbody))]
    public class InteractableObject : MonoBehaviour
    {

        void Start()
        {

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