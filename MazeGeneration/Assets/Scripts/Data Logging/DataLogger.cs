using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Globalization;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DataLogger : MonoBehaviour
{
    public DataHandler dataHandler;
    public AntiWallCollision wallCollision;
    public GameObject mapManagerObj;
    private GameObject mapObj;
    public int conditionAmount = 3;

    // Save location:
    [HideInInspector] public string filePath = @"Assets/Resources/CSV";
    private string fileName = "/session";
    private string fullPath;

    // Scriptable Objects:
    public FloatValue fpsData;
    public StringValue firstSicknessData, secondSicknessData, ageData, experienceData, genderData, locationData, /*proficiencyData,*/ understandingData;
    public FloatArrayValue prefWidthsData, prefHeightsData, loggedTimesData, wallHitsData;

    // Bools:
    public bool onlineLogging = true, logData = true;
    [HideInInspector] public bool logFrameRate, logAverageFrameRate, logTime, logGender, logAge, logLocation, logExperience,
        logWallHits, logPreferredWidth, logPreferredHeight, logGeographic, useSceneSwitch, logPlayAreaSize, logVRSickness, logUnderstanding;
    private bool active = false, initial = true, onFirstData = true, timerRunning;
    private bool[] dataWritten = new []{false, false};

    // Lists & Arrays:
    private List<string> dataList;
    private string[] csvLabels;
    private float[] prefWidths, prefHeights, loggedTimes, wallHits;

    // Other variables:
    private int sessionNumber = -1, fpsSum, fpsCount, wallHitsCount;
    private float updateInterval = 1.0f, avgFrames, frameRate, timeSpend;
    private string sicknessFirstRes, sicknessSecondRes, ageRes, experienceRes, genderRes, locationRes;
    private StreamWriter file;
    private WaitForSeconds delay;

    private void Start()
    {
        if (!logData)
            return;

        if (logPreferredWidth)
            if (mapManagerObj != null)
                mapObj = mapManagerObj.transform.GetChild(0).gameObject;

        Initialize();

        if (onlineLogging)
        {
            if (dataHandler == null)
                dataHandler = GetComponent<DataHandler>();

            if (dataHandler == null) // is still null?
                logData = false;
        }
        else
            InitializeCSV();
    }

    private void Initialize()
    {
        dataList = new List<string>();

        if (logTime)
            if (loggedTimesData.values.Length == 0)
                loggedTimesData.values = new float[conditionAmount];

        if (logWallHits)
            if (wallHitsData.values.Length == 0)
                wallHitsData.values = new float[conditionAmount];

        if (logPreferredWidth)
            if (prefWidthsData.values.Length == 0)
                prefWidthsData.values = new float[conditionAmount];

        if (logPreferredHeight)
            if (prefHeightsData.values.Length == 0)
                prefHeightsData.values = new float[conditionAmount];
    }

    private void UpdateDataList()
    {
        string spec = "G";
        CultureInfo ci = CultureInfo.CreateSpecificCulture("en-US");

        if (!onlineLogging)
            dataList.Add(sessionNumber.ToString());

        if (logFrameRate)
            dataList.Add(logAverageFrameRate ? avgFrames.ToString(spec, ci) : frameRate.ToString(spec, ci));

        if (logTime)
        {
            foreach (float time in loggedTimesData.values)
            {
                dataList.Add(time.ToString(spec, ci));
            }
        }

        if (logWallHits)
        {
            foreach (float hits in wallHitsData.values)
            {
                dataList.Add(hits.ToString());
            }
        }

        if (logPreferredWidth)
        {
            foreach (float width in prefWidthsData.values)
            {
                dataList.Add(width.ToString(spec, ci));
            }
        }

        if (logPreferredHeight)
        {
            foreach (float height in prefHeightsData.values)
            {
                dataList.Add(height.ToString(spec, ci));
            }
        }

        if (logPlayAreaSize)
        {
            if (!Application.isEditor)
                dataList.Add(GetPlayAreaSize().ToString());
            else
                dataList.Add("Data not available");
        }

        if (logVRSickness)
        {
            if (String.IsNullOrEmpty(firstSicknessData.value))
                dataList.Add("No data");
            else
                dataList.Add(firstSicknessData.value);

            if (String.IsNullOrEmpty(secondSicknessData.value))
                dataList.Add("No data");
            else
                dataList.Add(secondSicknessData.value);
        }

        if (onlineLogging)
            if (logGeographic)
            {
                if (logGender)
                {
                    if (String.IsNullOrEmpty(genderData.value))
                        dataList.Add("No data");
                    else
                        dataList.Add(genderData.value);
                }
                if (logAge)
                {
                    if (String.IsNullOrEmpty(ageData.value))
                        dataList.Add("No data");
                    else
                        dataList.Add(ageData.value);
                }
                if (logLocation)
                {
                    if (String.IsNullOrEmpty(locationData.value))
                        dataList.Add("No data");
                    else
                        dataList.Add(locationData.value);
                }
                if (logExperience)
                {
                    if (String.IsNullOrEmpty(experienceData.value))
                        dataList.Add("No data");
                    else
                        dataList.Add(experienceData.value);
                }
                if (logUnderstanding)
                    if (String.IsNullOrEmpty(understandingData.value))
                        dataList.Add("No data");
                    else
                        dataList.Add(understandingData.value);
            }
    }

    /// <summary>
    /// This function will send all data to Google Forms - So this should be called at the very end when all tests are done.
    /// </summary>
    public void PostDataOnline()
    {
        //Debug.Log("Posting Online");
        if (dataHandler == null)
            return;

        UpdateDataList();

        dataHandler.SendData(dataList);
        //Debug.Log("Data is sent!");
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

    public void FirstSicknessResponse(bool response)
    {
        //Debug.Log("first sickness");
        if (firstSicknessData == null)
            return;

        if (response)
            firstSicknessData.value = "Yes";
        else
            firstSicknessData.value = "No";
    }

    public void SecondSicknessResponse(bool response)
    {
        //Debug.Log("second sickness");
        if (secondSicknessData == null)
            return;

        if (response)
            secondSicknessData.value = "Yes";
        else
            secondSicknessData.value = "No";
    }

    public void PreferredWidthResponse(int testIndex)
    {
        //Debug.Log("Width");
        if (prefWidthsData != null && mapObj != null)
            if (testIndex >= 0 && testIndex < prefWidthsData.values.Length)
                prefWidthsData.values[testIndex] = mapObj.transform.localScale.x;
    }

    public void PreferredHeightResponse(int testIndex)
    {
        //Debug.Log("height");
        if (prefHeightsData != null && mapObj != null)
            if (testIndex >= 0 && testIndex < prefHeightsData.values.Length)
                prefHeightsData.values[testIndex] = mapObj.transform.localScale.y;
    }


    /// <param name="age">= = 19 or less, 1 = 20-29, 2 = 30-39, 3 = 40 or more</param>
    public void AgeResponse(int ageIndex)
    {
        //Debug.Log("age");
        if (ageData == null)
            return;

        switch (ageIndex)
        {
            case 0:
                ageData.value = "19 or less";
                break;
            case 1:
                ageData.value = "20-29";
                break;
            case 2:
                ageData.value = "30-39";
                break;
            case 3:
                ageData.value = "40 or more";
                break;
            default:
                ageData.value = "Not specified";
                break;
        }
    }

    public void ExperienceResponse(int experienceIndex)
    {
        if (experienceData == null)
            return;

        experienceData.value = experienceIndex.ToString();
    }

    /// <param name="answerIndex">0: with eyes closed, 1: not finding width comfortable, 2: as fast as possible</param>
    public void UnderstandTaskResponse(int answerIndex)
    {
        switch (answerIndex)
        {
            case 0:
                understandingData.value = "With my eyes closed";
                break;
            case 1:
                understandingData.value = "Until I do not find the width comfortable";
                break;
            case 2:
                understandingData.value = "As fast as possible";
                break;
            default:
                break;
        }
    }

    public void LogTimeStart()
    {
        ToggleTimer(true);
    }

    public void LogTimeStop(int testIndex)
    {
        ToggleTimer(false);

        if (loggedTimesData != null)
            if (testIndex >= 0 && testIndex < loggedTimesData.values.Length)
                loggedTimesData.values[testIndex] = timeSpend;
    }

    /// <summary>
    /// This should be called at the END of the test condition session.
    /// </summary>
    public void LogWallHits(int testIndex)
    {
        if (wallHitsData != null)
            if (testIndex >= 0 && testIndex < wallHitsData.values.Length)
            {
                wallHitsData.values[testIndex] = wallHitsCount;
                wallHitsCount = 0; // TODO check if I can set to 0 already?
            }
    }

    /// <param name="gender">0 = male, 1 = female, 2 = other</param>
    public void GenderResponse(int gender)
    {
        //Debug.Log("gender");
        if (genderData == null)
            return;

        if (gender == 0)
            genderData.value = "Male";
        else if (gender == 1)
            genderData.value = "Female";
        else
            genderData.value = "Other";
    }

    /// <param name="locationIndex">0 = Africa, 1 = Asia, 2 = Australia, 3 = Europe, 4 = North America, 5 = South America</param>
    public void LocationResponse(int locationIndex)
    {
        //Debug.Log("location");
        if (locationData == null)
            return;

        if (locationIndex == 0)
            locationData.value = "Africa";
        else if (locationIndex == 1)
            locationData.value = "Asia";
        else if (locationIndex == 2)
            locationData.value = "Australia";
        else if (locationIndex == 3)
            locationData.value = "Europe";
        else if (locationIndex == 4)
            locationData.value = "North America";
        else if (locationIndex == 5)
            locationData.value = "South America";
    }

    /// <param name="proficiencyIndex">0: Not at all, 1: A little, 2: Decently, 3: Fluently</param>
    public void ProficiencyResponse(int proficiencyIndex)
    {
        //if (proficiencyData == null)
        //    return;

        //switch (proficiencyIndex)
        //{
        //    case 0:
        //        proficiencyData.value = "Not at all";
        //        break;
        //    case 1:
        //        proficiencyData.value = "A little";
        //        break;
        //    case 2:
        //        proficiencyData.value = "Decently";
        //        break;
        //    case 3:
        //        proficiencyData.value = "Fluently";
        //        break;
        //    default:
        //        proficiencyData.value = "Wrong value given";
        //        break;
        //}
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

        if (Input.GetKeyDown(KeyCode.K))
        {
            PostDataOnline();
        }

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

    #region CSV
    private void InitializeCSV()
    {
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
        {
            int i = 1;
            foreach (float time in loggedTimes)
            {
                tempLabels.Add("TimeSpend Test " + i);
                i++;
            }
        }

        if (logWallHits)
        {
            int i = 1;
            foreach (float hits in wallHits)
            {
                tempLabels.Add("WallHits Test " + i);
                i++;
            }
        }

        if (logPreferredWidth)
        {
            int i = 1;
            foreach (float width in prefWidths)
            {
                tempLabels.Add("PreferredWidth Test " + i);
                i++;
            }
        }

        if (logPreferredHeight)
        {
            int i = 1;
            foreach (float height in prefHeights)
            {
                tempLabels.Add("PreferredHeight Test " + i);
                i++;
            }
        }

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
        var script = target as DataLogger;

        if (script.wallCollision == null)
            EditorGUILayout.HelpBox("Warning! - AntiCheat script reference null (can be found under CenterEyeAnchor)", MessageType.Error);

        if (script.mapManagerObj == null)
            EditorGUILayout.HelpBox("Warning! - No reference to mapmanager object (null reference)", MessageType.Error);

        DrawDefaultInspector();

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
                script.logUnderstanding = EditorGUILayout.Toggle("Understand Task", script.logUnderstanding);
                EditorGUI.indentLevel -= 1;
            }

            EditorGUI.indentLevel -= 1;
        }
    }
}
#endif
#endregion