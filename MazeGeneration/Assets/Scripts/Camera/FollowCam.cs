using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class FollowCam : MonoBehaviour
{
    Vector3 offset;
    public bool isStereoscopic;
    public bool isRightEye;
    private Camera mainCam;

    Vector3 nextOffset;
    Vector3 prevOffset;
    bool placeForward;

    void Awake()
    {
        if (!isStereoscopic && !isRightEye)
        {
            gameObject.SetActive(false);
        }
    }

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        SetCamera();
    }

    private void SetCamera()
    {
        Vector3 mainCameraPosition;
        if (!isStereoscopic)
        {
            transform.position = new Vector3(mainCam.transform.position.x + offset.x, mainCam.transform.position.y + offset.y, mainCam.transform.position.z + +offset.z);
        }
        else
        {
            if (isRightEye)
            {
                Matrix4x4 viewMatrix = mainCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
                mainCameraPosition = viewMatrix.inverse.GetColumn(3);
            }
            else
            {
                Matrix4x4 viewMatrix = mainCam.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
                mainCameraPosition = viewMatrix.inverse.GetColumn(3);
            }
            transform.position = new Vector3(mainCameraPosition.x + offset.x, mainCameraPosition.y + offset.y, mainCameraPosition.z + offset.z);
        }
        transform.rotation = mainCam.transform.rotation;
    }

    public void SwitchOffset()
    {
        if (offset == nextOffset)
        {
            offset = prevOffset;
        }
        else
        {
            offset = nextOffset;
        }
    }
    public void SetOffsets(Vector3 next, Vector3 prev)
    {
        SetNextOffset(next);
        SetPrevOffset(prev);
    }
    public void SetToNext()
    {
        offset = nextOffset;
        SetCamera();
    }
    public void SetToPrev()
    {
        offset = prevOffset;
        SetCamera();
    }
    public void SetNextOffset(Vector3 offset)
    {
        nextOffset = offset;
    }
    public void SetPrevOffset(Vector3 offset)
    {
        prevOffset = offset;
    }
}
