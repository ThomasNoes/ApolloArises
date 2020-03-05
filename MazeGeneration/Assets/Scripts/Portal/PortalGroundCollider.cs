using UnityEngine;

public class PortalGroundCollider : MonoBehaviour
{
    public BoxCollider renderPlaneCollider;
    public float activationAngle = 65.0f;
    private PortalCollider renderColScript;
    private bool isInCollider, cooldownActive;
    private Renderer portalRenderer;

    private void Start()
    {
        if (renderPlaneCollider == null)
            return;

        renderColScript = renderPlaneCollider.GetComponent<PortalCollider>();
        portalRenderer = renderColScript.GetComponent<Renderer>();

        //InvokeRepeating("TestLoop", 4.0f, 2.0f);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "PlayerCollider")
        {
            //Debug.Log("Player on tile");
            isInCollider = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "PlayerCollider")
        {
            //Debug.Log("Player off tile");
            isInCollider = false;
        }
    }

    private void LateUpdate()
    {
        if (!isInCollider) return;
        //if (!VisibleFromCamera()) return;
        if (!IsLookingTowardsPortal()) return;
        if (cooldownActive) return;

        isInCollider = false;
        //StartCooldown(1.0f);
        //Debug.Log("Should teleport! " + portalRenderer.name + " | " + transform.parent.name);
        renderColScript.Teleport();
    }

    private bool VisibleFromCamera()
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (GeometryUtility.TestPlanesAABB(frustumPlanes, portalRenderer.bounds))
        {
            if (Vector3.Distance(portalRenderer.gameObject.transform.position, Camera.main.transform.position) < 2.0f)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsLookingTowardsPortal()
    {
        Vector2 camForward2D = new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z);
        Vector2 dir2d = new Vector2(renderPlaneCollider.gameObject.transform.position.x, renderPlaneCollider.gameObject.transform.position.z) -
                        new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.z);

        if (Vector2.Angle(camForward2D, dir2d) < activationAngle)
            return true;
        else
            return false;
    }

    private void StartCooldown(float time)
    {
        cooldownActive = true;
        Invoke("Cooldown", time);
    }
    private void Cooldown() { cooldownActive = false; }

    private void TestLoop()
    {
        if (isInCollider)
        {
            Vector2 camForward2D = new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z);
            Vector2 dir2d = new Vector2(renderPlaneCollider.gameObject.transform.position.x, renderPlaneCollider.gameObject.transform.position.z) -
                           new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.z);
            Debug.Log("Player 2d angle to portal: " + Vector2.Angle(camForward2D, dir2d));
        }
    }
}