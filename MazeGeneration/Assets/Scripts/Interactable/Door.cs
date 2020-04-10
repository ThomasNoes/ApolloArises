using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Door : MonoBehaviour
{
    public bool colourWholeDoor = true;
    public GameObject doorMainObj, keyHoleObj;
    public int uniqueId;
    public bool useDelay;
    public float delay = 1.0f;
    [HideInInspector] public Material colourMaterial;
    private bool doorControlBool;

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
    }

    private void DelayedStart()
    {
        if (colourWholeDoor)
            doorMainObj.GetComponent<Renderer>().material = colourMaterial;
        else
        {
            // TODO redo this at some point - not working currently
            keyHoleObj.GetComponent<Renderer>().materials[2] = colourMaterial;
        }
    }

    private void OpenDoor()
    {
        // TODO: make door slide downwards
        doorControlBool = true;
        gameObject.SetActive(false);
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

        }
    }
}