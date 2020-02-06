using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class FollowCam : MonoBehaviour
{
    public float offset;
    public bool isStereoscopic;
    public bool isRightEye;

    void Awake()
    {
        if (!isStereoscopic && !isRightEye)
        {
            gameObject.SetActive(false);
        }
    }

    void LateUpdate()
    {
        Vector3 mainCameraPosition;
        if (!isStereoscopic)
        {
            transform.position = new Vector3(Camera.main.transform.position.x + offset, Camera.main.transform.position.y, Camera.main.transform.position.z);
        }
        else
        {
            if (isRightEye)
            {
                Matrix4x4 viewMatrix = Camera.main.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
                mainCameraPosition = viewMatrix.inverse.GetColumn(3);
            }
            else
            {
                Matrix4x4 viewMatrix = Camera.main.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
                mainCameraPosition = viewMatrix.inverse.GetColumn(3);
            }
            transform.position = new Vector3(mainCameraPosition.x + offset, mainCameraPosition.y, mainCameraPosition.z);
        }
        transform.rotation = Camera.main.transform.rotation;
    }

    public void SetOffset(float o)
    {
        offset = o;
    }
}
