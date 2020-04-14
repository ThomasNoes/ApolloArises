using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class DynamicPortalResolution : MonoBehaviour
{
    public float maxResolutionWidthScale = 1.0f;
    public float maxResolutionHeightScale = 1.0f;
    public float minResolutionWidthScale = 0.5f;
    public float minResolutionHeightScale = 0.5f;
    public float scaleWidthIncrement = 0.1f;
    public float scaleHeightIncrement = 0.1f;

    float m_widthScale = 1.0f;
    float m_heightScale = 1.0f;

    // Variables for dynamic resolution algorithm that persist across frames
    uint m_frameCount = 0;

    void Start()
    {

    }

    void Update()
    {
        float oldWidthScale = m_widthScale;
        float oldHeightScale = m_heightScale;

        // lowers the resolution
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Down!");
            m_heightScale = Mathf.Max(minResolutionHeightScale, m_heightScale - scaleHeightIncrement);
            m_widthScale = Mathf.Max(minResolutionWidthScale, m_widthScale - scaleWidthIncrement);
        }

        // raises the resolution
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("Up!");
            m_heightScale = Mathf.Min(maxResolutionHeightScale, m_heightScale + scaleHeightIncrement);
            m_widthScale = Mathf.Min(maxResolutionWidthScale, m_widthScale + scaleWidthIncrement);
        }

        if (m_widthScale != oldWidthScale || m_heightScale != oldHeightScale)
        {
            ScalableBufferManager.ResizeBuffers(m_widthScale, m_heightScale);
            Debug.Log("Scale: " + m_widthScale + " | " + m_heightScale);
        }
        // DetermineResolution();
    }
}