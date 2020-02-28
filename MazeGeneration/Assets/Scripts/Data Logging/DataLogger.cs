using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DataLogger : MonoBehaviour
{
    // Save location:
    public string filePath = @"Assets/Resources/CSV";
    public string fileName = "/session";
    private string fullPath;

    // Bools:
    public bool logData = true;
    [HideInInspector] public bool logFrameRate, logCurrentMaze;
    private bool active = false, initial = true;
    [Tooltip("In seconds")] public float updateInterval = 1.0f;

    // Lists:
    private List<float> dataList;

    // Other variables:
    private int sessionNumber = -1;
    private float lastTime, timeSpan, avgFrames, frameRate;
    private PortalRenderController pRController;
    private StreamWriter file;
    private WaitForSeconds delay;

    private static string[] csvLabels =
    {
        "Session number" /*[0]*/, "FPS" /*[1]*/, "Current Maze" /*[2]*/
    };

    private void Start()
    {
        if (logCurrentMaze)
            pRController = FindObjectOfType<PortalRenderController>();

#if UNITY_ANDROID
        if (!Application.isEditor)
            filePath = Application.persistentDataPath;
#endif

        fullPath = filePath + "/Dummy.csv";

        delay = updateInterval > 0 ? new WaitForSeconds(updateInterval) : new WaitForSeconds(1.0f);

        //InitializeFile();
        dataList = new List<float>();
    }

    private IEnumerator DataLog()
    {
        fileName = "/ses" + sessionNumber + "_" + System.DateTime.Now.Day + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Year + "_" +
                   System.DateTime.Now.Hour + "-" + System.DateTime.Now.Minute + ".csv"; 

        fullPath = filePath + fileName;
        Debug.Log(fullPath);

        using (file = File.CreateText(fullPath))
        {
            file.WriteLine(string.Join(";", csvLabels));

            while (active)
            {
                dataList.Add(sessionNumber);

                if (logFrameRate)
                {
                    dataList.Add(frameRate);
                }

                if (logCurrentMaze)
                    dataList.Add(pRController.currentMaze);

                file.WriteLine(string.Join(";", dataList));
                dataList.Clear();

                yield return delay;
            }

            file.Close();
        }
    }

    private void InitializeFile()
    {

#if UNITY_EDITOR
        if (!AssetDatabase.IsValidFolder(filePath))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            AssetDatabase.CreateFolder("Assets/Resources", "CSV");
        }
#endif

        // System language generalization
        string spec = "G";
        CultureInfo ci = CultureInfo.CreateSpecificCulture("en-US");
    }

    private void Update()
    {
        if (!logData)
            return;

        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
        {
            StartLogging();
        }
        else if (Input.GetKeyDown("h"))
        {
            StartLogging();
        }

        if (logFrameRate)
        {
            //avgFrames += ((Time.deltaTime / Time.timeScale) - avgFrames) * 0.03f;
            frameRate = (int)(1 / Time.deltaTime); 
        }
    }

    public void StartLogging()
    {
        if (active)
        {
            Debug.Log(sessionNumber + " session log saved!");
            active = false;
        }
        else
        {
            StopAllCoroutines();

            sessionNumber += 1;
            active = true;

            StartCoroutine(DataLog());

            Debug.Log("Started logging session: " + sessionNumber);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DataLogger))]
public class DataLogger_Editor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var script = target as DataLogger;

        if (script.logData)
        {
            script.logFrameRate = EditorGUILayout.Toggle("Log FPS", script.logFrameRate);
            script.logCurrentMaze = EditorGUILayout.Toggle("Log Current Maze", script.logCurrentMaze);
        }
    }
}
#endif