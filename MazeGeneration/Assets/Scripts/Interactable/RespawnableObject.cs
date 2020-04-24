using UnityEngine;

public class RespawnableObject : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Respawn"))
        {
            gameObject.transform.position = Camera.main.gameObject.transform.position;
        }
    }
}