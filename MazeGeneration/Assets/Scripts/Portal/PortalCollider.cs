using UnityEngine;

public class PortalCollider : MonoBehaviour
{
    [HideInInspector] public bool onPortalTile;

    public NewTeleporter teleporter;
    private Collider playerCollider;
    private Renderer planeRenderer;
    private bool active;

    private void Start()
    {
        if (teleporter == null)
            teleporter = transform.parent.GetComponent<NewTeleporter>();

        playerCollider = GameObject.FindGameObjectWithTag("PlayerCollider")?.GetComponent<Collider>();

        planeRenderer = GetComponent<Renderer>();

        Invoke("DelayedActive", 1.0f);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag != "PlayerCollider" || !active) return;

        onPortalTile = false;
        teleporter.Teleport(col);
    }

    public void Teleport()
    {
        if (playerCollider != null)
            teleporter.Teleport(playerCollider);
    }

    private static bool VisibleFromCamera(Renderer renderer, Camera camera)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }

    private void DelayedActive()
    {
        active = true;
    }
}