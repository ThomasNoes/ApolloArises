using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CenterObjectToPlayArea : MonoBehaviour
{
    private Vector3 playAreaCenter;

    void Start()
    {
        if (Application.isEditor)
            return;

        #if UNITY_ANDROID
        Debug.Log("Play area size: " + OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea));

        Debug.Log("PlayAreaTransform().position  " + VRTK_DeviceFinder.PlayAreaTransform().position);
        Debug.Log("PlayAreaTransform().rotation  " + VRTK_DeviceFinder.PlayAreaTransform().rotation);
#endif
    }

}
