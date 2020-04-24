using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class Key : MonoBehaviour
{
    public int uniqueId;
    public Material colourMaterial;
    public ItemSpawner itemSpawner;
    private Renderer renderer;

    private void Start()
    {
        GetComponent<SphereCollider>().isTrigger = true;
        renderer = GetComponent<Renderer>();

        if (renderer != null)
            Invoke("DelayedStart", 1.0f);
    }

    private void DelayedStart()
    {
        if (itemSpawner == null)
            return;

        colourMaterial = itemSpawner.GetMaterialFromId(uniqueId);
        renderer.material = colourMaterial; // TODO, make work with new key texture (when it is done)
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Respawn"))
        {
            gameObject.transform.position = Camera.main.gameObject.transform.position;
        }
    }
}