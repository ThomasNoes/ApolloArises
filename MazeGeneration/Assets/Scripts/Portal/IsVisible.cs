// This script must be placed on the portal render quad in portal prefab

using Assets.Scripts.Camera;
using UnityEngine;

public class IsVisible : MonoBehaviour
{
    public PortalRenderController pRController;
    public NewTeleporter thisTeleporter;
    public bool isStereoscopic = true, checkDistance, checkCurrentMaze, checkViewFrustum;
    public float minDistance = 25.0f;
    private Renderer thisRenderer;
    private Texture2D disabledTexture;
    private Material enabledTexture;
    private Texture rightText, leftText;
    private bool active, isAndroid, inNearbyMaze;
    private float currentDistance;
    private GameObject mainCamObj;
    private Camera mainCam;

    private void Start()
    {
        Invoke("DelayedStart", 1.0f);

        pRController = transform.root.GetComponent<PortalRenderController>();

        if (thisTeleporter == null)
            thisTeleporter = GetComponentInParent<NewTeleporter>();

        mainCamObj = Camera.main.gameObject;
        mainCam = Camera.main;

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

        if (checkDistance || checkCurrentMaze)
            InvokeRepeating("CustomLoop", 1.0f, 1.0f);

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

    private void LateUpdate()
    {
        if (active && checkViewFrustum)
        {
            if (!VisibleFromCamera(thisRenderer, mainCam) || !inNearbyMaze)
            {
                RenderOff();
            }
            else
            {
                if (!checkDistance)
                    renderOn();
                else if (currentDistance <= minDistance)
                    renderOn();
            }
        }
    }

    private void RenderOff()
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
    }

    private void renderOn()
    {
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

    private void CustomLoop()
    {
        if (checkDistance)
            currentDistance = Vector3.Distance(this.gameObject.transform.position, mainCamObj.transform.position);

        if (checkCurrentMaze)
        {
            if (pRController.currentMaze - 1 == thisTeleporter.mazeID || 
                pRController.currentMaze == thisTeleporter.mazeID ||
                pRController.currentMaze + 1 == thisTeleporter.mazeID)
                inNearbyMaze = true;
            else
                inNearbyMaze = false;
        }

        if (!checkViewFrustum)
        {
            if (!inNearbyMaze)
            {
                RenderOff();
            }
            else
            {
                if (!checkDistance)
                    renderOn();
                else if (currentDistance <= minDistance)
                    renderOn();
            }
        }
    }
}
