using UnityEngine;

public class Drawer : MonoBehaviour
{
    public int uniqueId;
    public Material colourMaterial;
    private Renderer renderer;

    private void Start()
    {
        renderer = GetComponent<Renderer>();

        if (renderer != null && colourMaterial != null)
            Invoke("DelayedStart", 0.5f);
    }

    private void DelayedStart()
    {
        renderer.material = colourMaterial;
    }
}