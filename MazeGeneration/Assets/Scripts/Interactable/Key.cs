using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class Key : MonoBehaviour
{
    public int uniqueId;

    private void Start()
    {
        GetComponent<SphereCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Respawn"))
        {
            gameObject.transform.position = Camera.main.gameObject.transform.position;
        }
    }
}