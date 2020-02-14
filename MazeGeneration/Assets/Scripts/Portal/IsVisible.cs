// This script must be placed on the portal render quad in portal prefab
using UnityEngine;

public class IsVisible : MonoBehaviour
{
    public PortalRenderController pRController;
    public bool isStereoscopic = true;
    private Renderer thisRenderer;
    private Texture2D disabledTexture;
    private Material enabledTexture;
    private bool active = false;

    private void Start()
    {
        Invoke("DelayedStart", 1.0f);

        if (pRController == null)
            enabledTexture = Resources.Load("Materials/Next" + (isStereoscopic ? "Stereo" : "Mono")) as Material;
        else
            enabledTexture = Resources.Load("Materials/Next" + (pRController.isStereoscopic ? "Stereo" : "Mono")) as Material;
    }

    private void DelayedStart()
    {
        thisRenderer = gameObject.GetComponent<Renderer>();
        disabledTexture = new Texture2D(1, 1);
        disabledTexture.SetPixel(0, 0, Color.black);
        disabledTexture.Apply();

        if (thisRenderer != null && enabledTexture != null)
            active = true;

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
        thisRenderer.material.SetTexture("_MainTex", enabledTexture.mainTexture);
    }

    private void LateUpdate()
    {
        if (active)
            Render();
    }
}
