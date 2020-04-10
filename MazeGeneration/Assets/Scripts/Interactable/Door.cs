using UnityEngine;

public class Door : MonoBehaviour
{
    public int uniqueId;
    public bool useDelay;
    public float delay = 1.0f;

    private void OpenDoor()
    {
        // Debug.Log("Door " + uniqueId + " opened!");
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
}