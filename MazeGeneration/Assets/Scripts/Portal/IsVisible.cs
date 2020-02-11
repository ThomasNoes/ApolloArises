// This script must be placed on the portal render quad in portal prefab
// TODO: Currently not working, look at it later
using UnityEngine;

public class IsVisible : MonoBehaviour
{
    private Renderer thisRenderer;
    private Texture2D disabledTexture;
    private RenderTexture enabledTexture;
    private bool active = false;

    private void Start()
    {
        Invoke("DelayedStart", 1.0f);
    }

    private void DelayedStart()
    {
        thisRenderer = gameObject.GetComponent<Renderer>();
        enabledTexture = RenderTexture.active;
        disabledTexture = new Texture2D(1, 1);
        disabledTexture.SetPixel(0, 0, Color.black);
        disabledTexture.Apply();

        if (thisRenderer != null && enabledTexture != null)
            active = true;

        Debug.Log("IS WORKING: " + active);
    }

    static bool VisibleFromCamera(Renderer renderer, Camera camera)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }

    public void Render()
    {
        if (!VisibleFromCamera(thisRenderer, Camera.main))
        {
            thisRenderer.material.SetTexture("_MainTex", disabledTexture);
            return;
        }
        thisRenderer.material.SetTexture("_MainTex", enabledTexture);
    }

    private void LateUpdate()
    {
        if (active)
            Render();
    }
}
