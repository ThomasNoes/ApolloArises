using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class Key : MonoBehaviour
{
    public int uniqueId;
    public Material[] colourMaterials;
    private Renderer renderer;

    private void Start()
    {
        GetComponent<SphereCollider>().isTrigger = true;
        renderer = GetComponent<Renderer>();

        if (renderer != null && colourMaterials != null)
            Invoke("DelayedStart", 0.5f);
    }

    private void DelayedStart()
    {
        if (colourMaterials.Length == 0)
            return;

        //int colourIndex = uniqueId % colourMaterials.Length;
        //renderer.material = 
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Respawn"))
        {
            gameObject.transform.position = Camera.main.gameObject.transform.position;
        }
    }
}