﻿using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class DebugWindow : MonoBehaviour
{
    private string _dLog;
    private Queue _dLogQueue = new Queue();
    private bool active;
    public Text debugText;
    public GameObject debugPanel;
    private ScrollRect scrollRect;
    private LayerMask layerMask;

    void Awake()
    {
        //if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
        active = true;

        if (!active || debugText == null)
            return;

        //Application.logMessageReceived += Log;
        scrollRect = debugText.gameObject.GetComponent<ScrollRect>();
        InvokeRepeating("CustomUpdate", 1.0f, 4.0f);

        layerMask = LayerMask.GetMask("Head");
        layerMask |= LayerMask.GetMask("Ignore Raycast");
        layerMask = ~layerMask;
    }

    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    void Log(string logString, string stackTrace, LogType type)
    {
        _dLog = logString;
        string newString = "\n [" + type + "] : " + _dLog;
        _dLogQueue.Enqueue(newString);
        if (type == LogType.Exception)
        {
            newString = "\n" + stackTrace;
            _dLogQueue.Enqueue(newString);
        }
        _dLog = string.Empty;
        foreach (string dLog in _dLogQueue)
        {
            _dLog += dLog;
        }
    }

    private void CustomUpdate()
    {
        debugText.text = _dLog;
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            if (debugPanel != null)
                debugPanel.SetActive(!debugPanel.activeSelf);
        }

        //if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickUp))
        //    ScrollToTop();
        //else if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickDown))
        //    ScrollToBottom()

        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.transform.position, Vector3.down, out hit, 10.0f, layerMask))
            {
                Tile tempTile = hit.collider.gameObject.GetComponentInParent<Tile>();

                if (tempTile != null)
                {
                    Debug.Log(tempTile.nextDistance + " | " + tempTile.prevdistance);
                }
            }
        }

    }

    public void ScrollToTop()
    {
        if (scrollRect.normalizedPosition.y >= 0 && scrollRect.normalizedPosition.y <= 1)
            scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, scrollRect.normalizedPosition.y + 0.05f * Time.deltaTime);
    }
    public void ScrollToBottom()
    {
        if (scrollRect.normalizedPosition.y >= 0 && scrollRect.normalizedPosition.y <= 1)
            scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, scrollRect.normalizedPosition.y - 0.05f * Time.deltaTime);
    }
}