using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Globalization;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DataLogger : MonoBehaviour
{
    // References:
    public AntiWallCollision antiWallCollision;
    public DataHandler dataHandler;
    private GameObject mapManagerObj; // can be made public if manual ref required
    private GameObject mapObj;

    // Save location:
    [HideInInspector] public string filePath = @"Assets/Resources/CSV";
    private string fileName = "/session";
    private string fullPath;

    // Bools:
    public bool onlineLogging = true, logData = true;
    [HideInInspector] public bool logFrameRate, logAverageFrameRate, logTime, logGender, logAge, logLocation, logExperience,
        logWallHits, logPreferredWidth, logPreferredHeight, logGeographic, useSceneSwitch, logPlayAreaSize, logVRSickness;
    private bool active = false, initial = true, onFirstData = true, timerRunning;
    private bool[] dataWritten = new []{false, false};

    // Lists & Arrays:
    private List<string> dataList;
    private string[] csvLabels;

    // Other variables:
    private int sessionNumber = -1, fpsSum, fpsCount;
    private float updateInterval = 1.0f, avgFrames, frameRate, timeSpend;
    private string sicknessFirstRes, sicknessSecondRes, ageRes, experienceRes, genderRes, locationRes;
    private StreamWriter file;
    private WaitForSeconds delay;

    private void Start()
    {
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

        dataList = new List<string>();

        if (onlineLogging)
        {
            dataHandler = GetComponent<DataHandler>();
            return;
        }

#if UNITY_ANDROID
        if (!Application.isEditor)
            filePath = Application.persistentDataPath;
#endif
        delay = updateInterval > 0 ? new WaitForSeconds(updateInterval) : new WaitForSeconds(1.0f);

        fullPath = filePath + "/Dummy.csv";
        InitializeCSVFile();

        if (PlayerPrefs.GetInt("initialStart") == 0)
        {
            PlayerPrefs.SetInt("initialStart", 1);
            PlayerPrefs.SetInt("participantNumber", 0);
        }
    }

    #region CSV
    private IEnumerator CSVDataLogRoutine()
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

                dataWritten[0] = true;
                file.Close();
            }
        }
        else
        {
            if (onFirstData)
            {
                CSVOverWriteLine();
            }
            else
            {
                if (!dataWritten[1])
                {
                    CSVAppendLine();
                    dataWritten[1] = true;
                }
                else
                    CSVOverWriteLine();
            }
        }
    }

    private void CSVAppendLine()
    {
        using (Stream st = File.Open(fullPath, FileMode.Append, FileAccess.Write))
        {
            using (file = new StreamWriter(st))
            {
                fileStream(file);
                file.Close();
            }
        }
    }

    private void CSVOverWriteLine()
    {
        using (Stream st = File.Open(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            StreamReader reader = new StreamReader(st);
            List<string> tempList = new List<string>();

            while (true)
            {
                string dataString = reader.ReadLine();
                if (dataString == null)
                    break;

                tempList.Add(dataString);
            }

            UpdateDataList();

            if (onFirstData && dataWritten[0])
                tempList[1] = string.Join(";", dataList);
            else if (dataWritten[1])
                tempList[2] = string.Join(";", dataList);

            st.Close();

            using (file = File.CreateText(fullPath))
            {
                foreach (var t in tempList)
                {
                    file.WriteLine(t);
                }

                file.Close();
            }

            ToggleCSVLogging();
            dataList.Clear();
        }
    }

    private void fileStream(StreamWriter file)
    {
        UpdateDataList();
        file.WriteLine(string.Join(";", dataList));
        dataList.Clear();
    }

    private void InitializeCSVFile()
    {
        List<string> tempLabels = new List<string>();

        tempLabels.Add("Session");

        if (logFrameRate)
            tempLabels.Add("FPS");

        if (logTime)
            tempLabels.Add("TimeSpend");

        if (logWallHits)
            tempLabels.Add("WallHits");

        if (logPreferredWidth)
            tempLabels.Add("PreferredWidth");

        if (logPreferredHeight)
            tempLabels.Add("PreferredHeight");

        if (logPlayAreaSize && !Application.isEditor)
            tempLabels.Add("PlayArea Size");

        if (logVRSickness)
        {
            tempLabels.Add("VR Sickness First");
            tempLabels.Add("VR Sickness Second");
        }

        csvLabels = new string[tempLabels.Count];
        for (int i = 0; i < tempLabels.Count; i++)
        {
            csvLabels[i] = tempLabels[i];
        }
    }

    public void ToggleCSVLogging()
    {
        if (active)
        {
            active = false;
        }
        else
        {
            StopAllCoroutines();

            sessionNumber = PlayerPrefs.GetInt("participantNumber");
            active = true;

            StartCoroutine(CSVDataLogRoutine());
        }
    }
    #endregion

    private void UpdateDataList()
    {
        string spec = "G";
        CultureInfo ci = CultureInfo.CreateSpecificCulture("en-US");

        dataList.Add(sessionNumber.ToString());

        if (logFrameRate)
            dataList.Add(logAverageFrameRate ? avgFrames.ToString(spec, ci) : frameRate.ToString(spec, ci));

        if (logTime)
            dataList.Add(timeSpend.ToString(spec, ci));

        if (logWallHits)
            dataList.Add(antiWallCollision.wallHits.ToString());

        if (logPreferredWidth)
            dataList.Add(mapObj.transform.localScale.x.ToString(spec, ci));

        if (logPreferredHeight)
            dataList.Add(mapObj.transform.localScale.y.ToString(spec, ci));

        if (logPlayAreaSize && !Application.isEditor)
            dataList.Add(GetPlayAreaSize().ToString());

        if (logVRSickness)
        {
            dataList.Add(sicknessFirstRes);
            dataList.Add(sicknessSecondRes);
        }

        if (onlineLogging)
            if (logGeographic)
            {
                if (logGender)
                    dataList.Add(genderRes);
                if (logAge)
                    dataList.Add(ageRes);
                if (logLocation)
                    dataList.Add(locationRes);
                if (logExperience)
                    dataList.Add(experienceRes);
            }
    }

    public void PostDataOnline()
    {
        if (dataHandler == null)
            return;

        UpdateDataList();

        dataHandler.SendData(dataList);
    }

    private Vector3 GetPlayAreaSize()
    {
        Vector3 size = new Vector3();
        Vector3 chaperone = OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea);

        if (chaperone != null)
        {
            size = new Vector3(Mathf.Round(chaperone.x), 0, Mathf.Round(chaperone.z));
        }
        return size;
    }

    public void SicknessResponse(bool firstResponse, bool response)
    {
        if (firstResponse)
        {
            if (response)
                sicknessFirstRes = "Yes";
            else
                sicknessFirstRes = "No";
        }
        else
        {
            if (response)
                sicknessSecondRes = "Yes";
            else
                sicknessSecondRes = "No";
        }
    }

    public void AgeResponse(int age)
    {
        ageRes = age.ToString();
    }

    public void ExperienceResponse(int experienceIndex)
    {
        experienceRes = experienceIndex.ToString();
    }

    /// <param name="gender">0 = male, 1 = female, 2 = other</param>
    public void GenderResponse(int gender)
    {
        if (gender == 0)
            genderRes = "Male";
        else if (gender == 1)
            genderRes = "Female";
        else
            genderRes = "Other";
    }

    /// <param name="locationIndex">0 = Africa, 1 = Asia, 2 = Australia, 3 = Europe, 4 = North America, 5 = South America</param>
    public void LocationResponse(int locationIndex)
    {
        if (locationIndex == 0)
            locationRes = "Africa";
        else if (locationIndex == 1)
            locationRes = "Asia";
        else if (locationIndex == 2)
            locationRes = "Australia";
        else if (locationIndex == 3)
            locationRes = "Europe";
        else if (locationIndex == 4)
            locationRes = "North America";
        else if (locationIndex == 5)
            locationRes = "South America";
    }

    private void Update()
    {
        if (!logData)
            return;

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

        if (logTime && timerRunning)
            timeSpend += Time.deltaTime;

        if (onlineLogging)
            return;

        #region OVR-Input
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || Input.GetKeyDown("h"))
        {
            ToggleCSVLogging();
            if (!Application.isEditor)
                OVRInput.SetControllerVibration(0.7f, 0.1f, OVRInput.Controller.LTouch);
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKeyDown("l"))
        {
            onFirstData = !onFirstData;
            if (!Application.isEditor)
                VibrateController(onFirstData ? 1 : 2, 0.5f, false);
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
        #endregion
    }

    public void ToggleTimer(bool start)
    {
        if (start)
        {
            timerRunning = true;
            timeSpend = 0;
        }
        else
        {
            timerRunning = false;
        }
    }

    #region OVR-ControllerFeedback
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
    #endregion
}

#region CustomInspector
#if UNITY_EDITOR
[CustomEditor(typeof(DataLogger))]
public class DataLogger_Editor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var script = target as DataLogger;

        if (!script.onlineLogging)
        {
            GUILayout.Space(15);
            script.filePath = EditorGUILayout.TextField("Local path:", script.filePath);
        }

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
            script.logTime = EditorGUILayout.Toggle("Time Spend", script.logTime);
            script.logWallHits = EditorGUILayout.Toggle("Wall Hits", script.logWallHits);
            script.logPreferredWidth = EditorGUILayout.Toggle("Pref. Width", script.logPreferredWidth);
            script.logPreferredHeight = EditorGUILayout.Toggle("Pref. Height", script.logPreferredHeight);
            script.logPlayAreaSize = EditorGUILayout.Toggle("Play Area Size", script.logPlayAreaSize);
            script.logVRSickness = EditorGUILayout.Toggle("VR Sickness", script.logVRSickness);
            script.logGeographic = EditorGUILayout.Toggle("Geographics", script.logGeographic);

            if (script.logGeographic)
            {
                EditorGUI.indentLevel += 1;
                script.logGender = EditorGUILayout.Toggle("Gender", script.logGender);
                script.logAge = EditorGUILayout.Toggle("Age", script.logAge);
                script.logLocation = EditorGUILayout.Toggle("Location", script.logLocation);
                script.logExperience = EditorGUILayout.Toggle("VR Experience", script.logExperience);
                EditorGUI.indentLevel -= 1;
            }

            EditorGUI.indentLevel -= 1;
        }
    }
}
#endif
#endregion