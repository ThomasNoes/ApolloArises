using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(BoxCollider))]
public class Door : MonoBehaviour
{
    public bool colourWholeDoor = true;
    public GameObject doorMainObj, keyHoleObj;
    public int uniqueId;
    public bool useDelay;
    public float delay = 1.0f, moveSpeed = 1.0f, height;
    [HideInInspector] public Material colourMaterial;

    private bool doorControlBool;
    private Vector3 startPos, endPos;
    private float fraction;

    private void Start()
    {
        if (keyHoleObj != null)
        {
            BoxCollider thisCollider = GetComponent<BoxCollider>();
            thisCollider.size = keyHoleObj.transform.localScale;
            thisCollider.center = keyHoleObj.transform.localPosition;
        }
        else if (doorMainObj != null)
        {
            BoxCollider thisCollider = GetComponent<BoxCollider>();
            thisCollider.size = doorMainObj.transform.localScale;
            thisCollider.center = doorMainObj.transform.localPosition;
        }

        if (doorMainObj != null)
            Invoke("DelayedStart", 1.0f);

        startPos = transform.position;
        endPos = new Vector3(transform.position.x, transform.position.y - (height - 0.02f), transform.position.z);
    }

    private void DelayedStart()
    {
        if (colourWholeDoor)
            doorMainObj.GetComponent<Renderer>().material = colourMaterial;
        else
            keyHoleObj.GetComponent<Renderer>().material = colourMaterial;
    }

    public void OpenDoor()
    {
        doorControlBool = true;
        //gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Key"))
            if (col.GetComponent<Key>() != null)
            {
                int tempId = col.GetComponent<Key>().uniqueId;

                if (tempId == uniqueId)
                    if (!useDelay)
                    {
                        Destroy(col.gameObject);
                        OpenDoor();
                    }
                    else
                    {
                        Destroy(col.gameObject);
                        Invoke("OpenDoor", delay);
                    }
            }
    }

    private void LateUpdate()
    {
        if (doorControlBool)
        {
            if (fraction < 1)
            {
                fraction += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(startPos, endPos, fraction);
            }
        }
    }
}

#region CustomInspector
#if UNITY_EDITOR
[CustomEditor(typeof(Door))]
public class Door_Editor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        var script = target as Door;

        DrawDefaultInspector();

        if (GUILayout.Button("Open Door"))
        {
            script.OpenDoor();
        }
    }
}
#endif
#endregion