using UnityEngine;

public class RespawnableObject : MonoBehaviour
{
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Respawn"))
        {
            gameObject.transform.position = mainCam.gameObject.transform.position;
        }
    }
}