// This script must be placed on the portal render quad in portal prefab
using UnityEngine;

public class IsVisible : MonoBehaviour
{
    public PortalRenderController pRController;
    public bool isStereoscopic = true;
    private Renderer thisRenderer;
    private Texture2D disabledTexture;
    private Material enabledTexture;
    private Texture rightText, leftText;
    private bool active, isAndroid;

    private void Start()
    {
        Invoke("DelayedStart", 1.0f);

        pRController = transform.root.GetComponent<PortalRenderController>();

        if (pRController == null)
        {
            enabledTexture = Resources.Load("Materials/Next" + (isStereoscopic ? "Stereo" : "Mono")) as Material;

            if (isStereoscopic)
            {
                leftText = enabledTexture.GetTexture("_LeftText");
                rightText = enabledTexture.GetTexture("_RightTex");
            }
        }
        else
        {
            enabledTexture = Resources.Load("Materials/Next" + (pRController.isStereoscopic ? "Stereo" : "Mono")) as Material;

            if (pRController.isStereoscopic)
            {
                leftText = enabledTexture.GetTexture("_LeftTex");
                rightText = enabledTexture.GetTexture("_RightTex");
            }
        }

        #if UNITY_ANDROID
        isAndroid = true;
        #endif
        #if UNITY_EDITOR
        isAndroid = false;
        #endif
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
        if (camera == null)
            return false;

        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }

    public void Render()
    {
        if (!VisibleFromCamera(thisRenderer, Camera.main))
        {
            if (!isAndroid)
            {
                thisRenderer.material.SetTexture(1, disabledTexture);
            }
            else
            {
                thisRenderer.material.SetTexture("_LeftTex", disabledTexture);
                thisRenderer.material.SetTexture("_RightTex", disabledTexture);
            }
            return;
        }

        if (!isAndroid)
        {
            thisRenderer.material.SetTexture(1, enabledTexture.mainTexture);
        }
        else
        {
            thisRenderer.material.SetTexture("_LeftTex", leftText);
            thisRenderer.material.SetTexture("_RightTex", rightText);
        }
    }

    private void LateUpdate()
    {
        if (active)
            Render();
    }
}
