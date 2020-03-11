using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DataLogger : MonoBehaviour
{
    // References:
    public AntiWallCollision antiWallCollision;
    public GameObject mapManagerObj;
    private GameObject mapObj;

    // Save location:
    public string filePath = @"Assets/Resources/CSV";
    private string fileName = "/session";
    private string fullPath;

    [Tooltip("In seconds")] public float updateInterval = 1.0f;

    // Bools:
    public bool continuousLog = false, logData = true;
    [HideInInspector] public bool logFrameRate, logAverageFrameRate, logCurrentMaze, 
        logWallHits, logPreferredWidth, logPreferredHeight, useSceneSwitch;
    private bool active = false, initial = true;

    // Lists & Arrays:
    private List<float> dataList;
    private string[] csvLabels;

    // Other variables:
    private int sessionNumber = -1, fpsSum, fpsCount;
    private float lastTime, timeSpan, avgFrames, frameRate;
    private PortalRenderController pRController;
    private StreamWriter file;
    private WaitForSeconds delay;

    private void Start()
    {
        if (logCurrentMaze)
            pRController = FindObjectOfType<PortalRenderController>();

        if (logWallHits && antiWallCollision == null)
            logWallHits = false;

        if (logPreferredWidth)
        {
            if (mapManagerObj == null)
                mapManagerObj = gameObject;

            mapObj = mapManagerObj.transform.GetChild(0).gameObject;

            if (mapObj == null)
            {
                logPreferredWidth = false;
                logPreferredHeight = false;
                Debug.LogError("Datalogger Error: Maze 1 not found");
            }
        }

#if UNITY_ANDROID
        if (!Application.isEditor)
            filePath = Application.persistentDataPath;
#endif

        fullPath = filePath + "/Dummy.csv";

        delay = updateInterval > 0 ? new WaitForSeconds(updateInterval) : new WaitForSeconds(1.0f);

        InitializeFile();
        dataList = new List<float>();

        if (PlayerPrefs.GetInt("initialStart") == 0)
        {
            PlayerPrefs.SetInt("initialStart", 1);
            PlayerPrefs.SetInt("participantNumber", 0);
        }
    }

    private IEnumerator DataLog()
    {
        if (initial)
        {
            Scene scene = SceneManager.GetActiveScene();

            fileName = "/" + scene.name + "_par" + sessionNumber + "_" + System.DateTime.Now.Day + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Year + "_" +
                       System.DateTime.Now.Hour + "-" + System.DateTime.Now.Minute + ".csv";

            fullPath = filePath + fileName;
            Debug.Log(fullPath);
            initial = false;

            using (file = File.CreateText(fullPath))
            {
                file.WriteLine(string.Join(";", csvLabels));

                while (active)
                {
                    fileStream(file);
                    yield return delay;
                }

                file.Close();
            }
        }
        else
        {
            using (Stream st = File.Open(fullPath, FileMode.Append, FileAccess.Write))
            {
                using (file = new StreamWriter(st))
                {
                    fileStream(file);

                    file.Close();
                }
            }

            initial = true;
        }
    }

    private void CancelLastLine()
    {
        using (Stream st = File.Open(fullPath, FileMode.Open, FileAccess.ReadWrite))
        {
            st.SetLength(st.Length - 1);
            st.Close();
            Debug.Log("StreamWriter: Cancelled previous line");
        }
    }

    private void fileStream(StreamWriter file)
    {
        dataList.Add(sessionNumber);

        if (logFrameRate)
            dataList.Add(logAverageFrameRate ? avgFrames : frameRate);

        if (logCurrentMaze)
            dataList.Add(pRController.currentMaze);

        if (logPreferredWidth)
            dataList.Add(mapObj.transform.localScale.x);

        if (logPreferredHeight)
            dataList.Add(mapObj.transform.localScale.y);

        if (logWallHits)
            dataList.Add(antiWallCollision.wallHits);

        file.WriteLine(string.Join(";", dataList));
        dataList.Clear();

        if (!continuousLog)
        {
            StartLogging();
        }
    }

    private void InitializeFile()
    {
        List<string> tempLabels = new List<string>();

        tempLabels.Add("Session");

        if (logFrameRate)
            tempLabels.Add("FPS");

        if (logCurrentMaze)
            tempLabels.Add("CurrentMaze");

        if (logPreferredWidth)
            tempLabels.Add("PreferredWidth");

        if (logPreferredHeight)
            tempLabels.Add("PreferredHeight");

        if (logWallHits)
            tempLabels.Add("WallHits");


        csvLabels = new string[tempLabels.Count];
        for (int i = 0; i < tempLabels.Count; i++)
        {
            csvLabels[i] = tempLabels[i];
        }
    }

    private void Update()
    {
        if (!logData)
            return;

        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            StartLogging();
            OVRInput.SetControllerVibration(0.7f, 0.1f, OVRInput.Controller.LTouch);
        }
        else if (Input.GetKeyDown("h"))
        {
            StartLogging();
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            CancelLastLine();
        }

        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (currentSceneIndex + 1 < SceneManager.sceneCount)
                SceneManager.LoadScene(currentSceneIndex + 1);
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickUp))
        {
            int newParticipantNumber = PlayerPrefs.GetInt("participantNumber") + 1;
            PlayerPrefs.SetInt("participantNumber", newParticipantNumber);
            StartCoroutine(VibrateController(newParticipantNumber, 0.7f, false));
            initial = true;
        }
        else if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickDown))
        {
            int newParticipantNumber = PlayerPrefs.GetInt("participantNumber") - 1;

            if (newParticipantNumber < 0)
                return;

            PlayerPrefs.SetInt("participantNumber", newParticipantNumber);
            StartCoroutine(VibrateController(newParticipantNumber, 0.7f, false));
            initial = true;
        }
        
        if (logFrameRate)
        {
            frameRate = (int)(1 / Time.deltaTime);
            if (logAverageFrameRate)
            {
                fpsSum += (int)frameRate;
                fpsCount++;
                avgFrames = fpsSum/fpsCount;
            }
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

            sessionNumber = PlayerPrefs.GetInt("participantNumber");
            active = true;

            StartCoroutine(DataLog());
        }
    }

    private IEnumerator VibrateController(int amount, float frequency, bool rightController)
    {
        WaitForSeconds delay1 = new WaitForSeconds(frequency / 2);
        WaitForSeconds delay2 = new WaitForSeconds(frequency);

        for (int i = 0; i < amount; i++)
        {
            if (rightController)
            {
                OVRInput.SetControllerVibration(1, 0.8f, OVRInput.Controller.RTouch);
                yield return delay1;
                OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
                yield return delay2;
            }
            else
            {
                OVRInput.SetControllerVibration(1, 0.8f, OVRInput.Controller.LTouch);
                yield return delay1;
                OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
                yield return delay2;
            }
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

        GUILayout.Space(15);

        if (script.logData)
        {
            EditorGUI.indentLevel += 1;
            script.logFrameRate = EditorGUILayout.Toggle("Log FPS", script.logFrameRate);
            if (script.logFrameRate)
            {
                EditorGUI.indentLevel += 1;
                script.logAverageFrameRate = EditorGUILayout.Toggle("Log Average", script.logAverageFrameRate);
                EditorGUI.indentLevel -= 1;
            }
            script.logCurrentMaze = EditorGUILayout.Toggle("Log Current Maze", script.logCurrentMaze);
            script.logWallHits = EditorGUILayout.Toggle("Log Wall Hits", script.logWallHits);
            script.logPreferredWidth = EditorGUILayout.Toggle("Log Pref. Width", script.logPreferredWidth);
            script.logPreferredHeight = EditorGUILayout.Toggle("Log Pref. Height", script.logPreferredHeight);

            EditorGUI.indentLevel -= 1;
        }
    }
}
#endif