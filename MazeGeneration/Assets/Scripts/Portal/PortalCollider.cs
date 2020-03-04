using UnityEngine;

public class PortalCollider : MonoBehaviour
{
    [HideInInspector] public bool onPortalTile;

    public NewTeleporter teleporter;
    private Collider playerCollider;
    private Renderer planeRenderer;

    private void Start()
    {
        if (teleporter == null)
            teleporter = transform.parent.GetComponent<NewTeleporter>();

        playerCollider = GameObject.FindGameObjectWithTag("PlayerCollider")?.GetComponent<Collider>();

        planeRenderer = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag != "PlayerCollider") return;

        onPortalTile = false;
        Debug.Log("Player collision with render plane!");
        teleporter.Teleport(col);
    }

    //private void LateUpdate()
    //{
    //    if (!onPortalTile) return;
    //    if (!VisibleFromCamera(planeRenderer, Camera.main)) return;

    //    onPortalTile = false;
    //    if (playerCollider != null)
    //        teleporter.Teleport(playerCollider);
    //}

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
}