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
public class DataLogger2 : MonoBehaviour
{
    public DataHandler dataHandler;
    public AntiWallCollision wallCollision;
    [HideInInspector] public GameObject mapManagerObj;
    private GameObject mapObj;
    public int conditionAmount = 2;

    // Save location:
    [HideInInspector] public string filePath = @"Assets/Resources/CSV";
    private string fileName = "/session";
    private string fullPath;

    // Scriptable Objects:
    public FloatValue fpsData;
    public StringValue preSicknessData, noGameSicknessData, gameSicknessData, VRData, genderData, understandingData, preferenceData;
    public StringValue comfortOverallData, comfortEnvironmentData, comfortCorridorData, comfortSoundData, comfortVisualData, comfortFramerateData, comfortTechnicalData;
    public StringValue comfortOverallGameData, comfortEnvironmentGameData, comfortCorridorGameData, comfortSoundGameData, comfortVisualGameData, comfortFramerateGameData, comfortTechnicalGameData, comfortRobotGameData, comfortInteractionGameData;
    public FloatArrayValue wallHitsData;

    // Bools:
    public bool onlineLogging = true, logData = true;
    [HideInInspector]
    public bool logFrameRate, logAverageFrameRate, logMinimumFrameRate, logGender, logVR, logPreference,
        logWallHits, logDemographic, useSceneSwitch, logPlayAreaSize, logVRSickness, logUnderstanding, logComfort;
    private bool active = false, initial = true, onFirstData = true, timerRunning;
    private bool[] dataWritten = new[] { false, false };

    // Lists & Arrays:
    private List<string> dataList;
    private string[] csvLabels;
    private float[] loggedTimes, wallHits;

    // Other variables:
    private int sessionNumber = -1, fpsSum, fpsCount, wallHitsCount;
    private float updateInterval = 1.0f, avgFrames, lowestFrame = float.MaxValue, frameRate, timeSpend;
    private string preSicknesRes, noGameSicknessRes, gameSicknessRes, VRRes, genderRes;
    private StreamWriter file;
    private WaitForSeconds delay;

    private void Start()
    {
        if (!logData)
            return;

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

        if (logWallHits)
            if (wallHitsData.values.Length == 0)
                wallHitsData.values = new float[conditionAmount];
    }

    private void UpdateDataList()
    {
        string spec = "G";
        CultureInfo ci = CultureInfo.CreateSpecificCulture("en-US");

        if (!onlineLogging)
            dataList.Add(sessionNumber.ToString());

        if (logFrameRate)
            dataList.Add(logAverageFrameRate ? avgFrames.ToString(spec, ci) : frameRate.ToString(spec, ci));

        if (logFrameRate) //added minimum -> test
            dataList.Add(logMinimumFrameRate ? lowestFrame.ToString(spec, ci) : frameRate.ToString(spec, ci));

        if (logWallHits)
        {
            foreach (float hits in wallHitsData.values)
            {
                dataList.Add(hits.ToString());
            }
        }

        if (logPlayAreaSize)
        {
            if (!Application.isEditor)
            {
                Vector3 sizeResult = GetPlayAreaSize();
                dataList.Add(sizeResult.x.ToString(spec, ci) + " x " + sizeResult.z.ToString(spec, ci));
            }
            else
                dataList.Add("Data not available");
        }

        if (logVRSickness)
        {
            if (String.IsNullOrEmpty(preSicknessData.value))
                dataList.Add("No data");
            else
                dataList.Add(preSicknessData.value);

            if (String.IsNullOrEmpty(gameSicknessData.value))
                dataList.Add("No data");
            else
                dataList.Add(gameSicknessData.value);

            if (String.IsNullOrEmpty(noGameSicknessData.value))
                dataList.Add("No data");
            else
                dataList.Add(noGameSicknessData.value);
        }

        if (onlineLogging)
        {
            if (logDemographic)
            {
                if (logGender)
                {
                    if (String.IsNullOrEmpty(genderData.value))
                        dataList.Add("No data");
                    else
                        dataList.Add(genderData.value);
                }

                if (logVR)
                {
                    if (String.IsNullOrEmpty(VRData.value))
                        dataList.Add("No data");
                    else
                        dataList.Add(VRData.value);
                }
                if (logUnderstanding)
                    if (String.IsNullOrEmpty(understandingData.value))
                        dataList.Add("No data");
                    else
                        dataList.Add(understandingData.value);
                if (logPreference)
                {
                    if (String.IsNullOrEmpty(preferenceData.value))
                        dataList.Add("No data");
                    else
                        dataList.Add(preferenceData.value);
                }
            }
        }
        if (logComfort)
        {
            AddToDataList(comfortOverallData);
            AddToDataList(comfortOverallGameData);
            AddToDataList(comfortEnvironmentData);
            AddToDataList(comfortEnvironmentGameData);
            AddToDataList(comfortCorridorData);
            AddToDataList(comfortCorridorGameData);
            AddToDataList(comfortSoundData);
            AddToDataList(comfortSoundGameData);
            AddToDataList(comfortVisualData);
            AddToDataList(comfortVisualGameData);
            AddToDataList(comfortFramerateData);
            AddToDataList(comfortFramerateGameData);
            AddToDataList(comfortTechnicalData);
            AddToDataList(comfortTechnicalGameData);
            AddToDataList(comfortRobotGameData);
            AddToDataList(comfortInteractionGameData);
        }
    }


    private void AddToDataList(StringValue data)
    {
        if (String.IsNullOrEmpty(data.value))
            dataList.Add("No data");
        else
            dataList.Add(data.value);
    }
    /// <summary>
    /// <summary>
    /// This function will send all data to Google Forms - So this should be called at the very end when all tests are done.
    /// </summary>
    public void PostDataOnline()
    {
        if (GetComponent<GuardainCalibration>().RoomScaling3x4Check(0.57f)) //only getting data if their play area is large enough
        {
            Debug.Log("Posting Online");
            if (dataHandler == null)
            return;

            UpdateDataList();

            dataHandler.SendData(dataList);
            //Debug.Log("Data is sent!");
        }
    }

    private Vector3 GetPlayAreaSize()
    {
        Vector3 size = new Vector3();
        Vector3 chaperone = OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea);

        if (chaperone != null)
        {
            size = new Vector3(chaperone.x, 0, chaperone.z);
        }
        return size;
    }

    public void PreSicknessResponse(bool response)
    {
        //Debug.Log("first sickness");
        if (preSicknessData == null)
            return;

        if (response)
            preSicknessData.value = "Yes";
        else
            preSicknessData.value = "No";
    }

    public void GameSicknessResponse(bool response)
    {
        //Debug.Log("second sickness");
        if (gameSicknessData == null)
            return;

        if (response)
            gameSicknessData.value = "Yes";
        else
            gameSicknessData.value = "No";
    }

    public void NoGameSicknessResponse(bool response)
    {
        //Debug.Log("second sickness");
        if (noGameSicknessData == null)
            return;

        if (response)
            noGameSicknessData.value = "Yes";
        else
            noGameSicknessData.value = "No";
    }

    public void VRResponse(int experienceIndex)
    {
        if (VRData == null)
            return;

        VRData.value = experienceIndex.ToString();
    }

    /// <param name="answerIndex">0: with eyes closed, 1: not finding width comfortable, 2: as fast as possible</param>
    public void UnderstandTaskResponse(int answerIndex)
    {
        switch (answerIndex)
        {
            case 0:
                understandingData.value = "Find out how you switched to another maze";
                break;
            case 1:
                understandingData.value = "Reach the end of the maze";
                break;
            case 2:
                understandingData.value = "Count the amount of towers";
                break;
            default:
                break;
        }
    }

    public void LogTimeStart()
    {
        ToggleTimer(true);
    }

    /// <summary>
    /// This should be called at the END of the test condition session.
    /// </summary>
    public void LogWallHits(int testIndex)
    {
        if (wallHitsData != null && wallCollision != null)
            if (testIndex >= 0 && testIndex < wallHitsData.values.Length)
                wallHitsData.values[testIndex] = wallCollision.wallHits;
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
    public void PreferenceResponse(bool prefGame)
    {
        //Debug.Log("gender");
        if (preferenceData == null)
            return;

        if (prefGame)
            preferenceData.value = "with game";
        else
            preferenceData.value = "without game";
    }
    public void ComfortOverallResponse(int level)
    {
        //Debug.Log("gender");
        if (comfortOverallData == null)
            return;
        string answer;
        switch (level)
        {
            case 0:
                answer = "Comfortable";
                break;
            case 1:
                answer = "Slightly uncomfortable";
                break;
            case 2:
                answer = "Moderately uncomfortable";
                break;
            default:
                answer = "Severely uncomfortable";
                break;
        }
        comfortOverallData.value = answer;
    }
    public void ComfortOverallGameResponse(int level)
    {
        //Debug.Log("gender");
        if (comfortOverallGameData == null)
            return;
        string answer;
        switch (level)
        {
            case 0:
                answer = "Comfortable";
                break;
            case 1:
                answer = "Slightly uncomfortable";
                break;
            case 2:
                answer = "Moderately uncomfortable";
                break;
            default:
                answer = "Severely uncomfortable";
                break;
        }
        comfortOverallGameData.value = answer;
    }
    public void ComfortEnvironmentResponse(int level)
    {
        //Debug.Log("gender");
        if (comfortEnvironmentData == null)
            return;
        string answer;
        switch (level)
        {
            case 0:
                answer = "Comfortable";
                break;
            case 1:
                answer = "Slightly uncomfortable";
                break;
            case 2:
                answer = "Moderately uncomfortable";
                break;
            default:
                answer = "Severely uncomfortable";
                break;
        }
        comfortEnvironmentData.value = answer;
    }
    public void ComfortEnvironmentGameResponse(int level)
    {
        //Debug.Log("gender");
        if (comfortEnvironmentGameData == null)
            return;
        string answer;
        switch (level)
        {
            case 0:
                answer = "Comfortable";
                break;
            case 1:
                answer = "Slightly uncomfortable";
                break;
            case 2:
                answer = "Moderately uncomfortable";
                break;
            default:
                answer = "Severely uncomfortable";
                break;
        }
        comfortEnvironmentGameData.value = answer;
    }
    public void ComfortCorridorResponse(int level)
    {
        //Debug.Log("gender");
        if (comfortCorridorData == null)
            return;
        string answer;
        switch (level)
        {
            case 0:
                answer = "Comfortable";
                break;
            case 1:
                answer = "Slightly uncomfortable";
                break;
            case 2:
                answer = "Moderately uncomfortable";
                break;
            default:
                answer = "Severely uncomfortable";
                break;
        }
        comfortCorridorData.value = answer;
    }
    public void ComfortCorridorGameResponse(int level)
    {
        //Debug.Log("gender");
        if (comfortCorridorGameData == null)
            return;
        string answer;
        switch (level)
        {
            case 0:
                answer = "Comfortable";
                break;
            case 1:
                answer = "Slightly uncomfortable";
                break;
            case 2:
                answer = "Moderately uncomfortable";
                break;
            default:
                answer = "Severely uncomfortable";
                break;
        }
        comfortCorridorGameData.value = answer;
    }
    public void ComfortSoundResponse(int level)
    {
        //Debug.Log("gender");
        if (comfortSoundData == null)
            return;
        string answer;
        switch (level)
        {
            case 0:
                answer = "Comfortable";
                break;
            case 1:
                answer = "Slightly uncomfortable";
                break;
            case 2:
                answer = "Moderately uncomfortable";
                break;
            default:
                answer = "Severely uncomfortable";
                break;
        }
        comfortSoundData.value = answer;
    }
    public void ComfortSoundGameResponse(int level)
    {
        //Debug.Log("gender");
        if (comfortSoundGameData == null)
            return;
        string answer;
        switch (level)
        {
            case 0:
                answer = "Comfortable";
                break;
            case 1:
                answer = "Slightly uncomfortable";
                break;
            case 2:
                answer = "Moderately uncomfortable";
                break;
            default:
                answer = "Severely uncomfortable";
                break;
        }
        comfortSoundGameData.value = answer;
    }
    public void ComfortVisualResponse(int level)
    {
        //Debug.Log("gender");
        if (comfortVisualData == null)
            return;
        string answer;
        switch (level)
        {
            case 0:
                answer = "Comfortable";
                break;
            case 1:
                answer = "Slightly uncomfortable";
                break;
            case 2:
                answer = "Moderately uncomfortable";
                break;
            default:
                answer = "Severely uncomfortable";
                break;
        }
        comfortVisualData.value = answer;
    }
    public void ComfortVisualGameResponse(int level)
    {
        //Debug.Log("gender");
        if (comfortVisualGameData == null)
            return;
        string answer;
        switch (level)
        {
            case 0:
                answer = "Comfortable";
                break;
            case 1:
                answer = "Slightly uncomfortable";
                break;
            case 2:
                answer = "Moderately uncomfortable";
                break;
            default:
                answer = "Severely uncomfortable";
                break;
        }
        comfortVisualGameData.value = answer;
    }
    public void ComfortFramerateResponse(int level)
    {
        //Debug.Log("gender");
        if (comfortFramerateData == null)
            return;
        string answer;
        switch (level)
        {
            case 0:
                answer = "Comfortable";
                break;
            case 1:
                answer = "Slightly uncomfortable";
                break;
            case 2:
                answer = "Moderately uncomfortable";
                break;
            default:
                answer = "Severely uncomfortable";
                break;
        }
        comfortFramerateData.value = answer;
    }
    public void ComfortFramerateGameResponse(int level)
    {
        //Debug.Log("gender");
        if (comfortFramerateGameData == null)
            return;
        string answer;
        switch (level)
        {
            case 0:
                answer = "Comfortable";
                break;
            case 1:
                answer = "Slightly uncomfortable";
                break;
            case 2:
                answer = "Moderately uncomfortable";
                break;
            default:
                answer = "Severely uncomfortable";
                break;
        }
        comfortFramerateGameData.value = answer;
    }
    public void ComfortTechnicalResponse(int level)
    {
        //Debug.Log("gender");
        if (comfortTechnicalData == null)
            return;
        string answer;
        switch (level)
        {
            case 0:
                answer = "Comfortable";
                break;
            case 1:
                answer = "Slightly uncomfortable";
                break;
            case 2:
                answer = "Moderately uncomfortable";
                break;
            default:
                answer = "Severely uncomfortable";
                break;
        }
        comfortTechnicalData.value = answer;
    }
    public void ComfortTechnicalGameResponse(int level)
    {
        //Debug.Log("gender");
        if (comfortTechnicalGameData == null)
            return;
        string answer;
        switch (level)
        {
            case 0:
                answer = "Comfortable";
                break;
            case 1:
                answer = "Slightly uncomfortable";
                break;
            case 2:
                answer = "Moderately uncomfortable";
                break;
            default:
                answer = "Severely uncomfortable";
                break;
        }
        comfortTechnicalGameData.value = answer;
    }
    public void ComfortRobotGameResponse(int level)
    {
        //Debug.Log("gender");
        if (comfortRobotGameData == null)
            return;
        string answer;
        switch (level)
        {
            case 0:
                answer = "Comfortable";
                break;
            case 1:
                answer = "Slightly uncomfortable";
                break;
            case 2:
                answer = "Moderately uncomfortable";
                break;
            default:
                answer = "Severely uncomfortable";
                break;
        }
        comfortRobotGameData.value = answer;
    }
    public void ComfortInteractionGameResponse(int level)
    {
        //Debug.Log("gender");
        if (comfortInteractionGameData == null)
            return;
        string answer;
        switch (level)
        {
            case 0:
                answer = "Comfortable";
                break;
            case 1:
                answer = "Slightly uncomfortable";
                break;
            case 2:
                answer = "Moderately uncomfortable";
                break;
            default:
                answer = "Severely uncomfortable";
                break;
        }
        comfortInteractionGameData.value = answer;
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
                avgFrames = fpsSum / fpsCount;
            }
            if (logMinimumFrameRate)
            {
                if (frameRate <lowestFrame)
                {
                    lowestFrame = frameRate;
                }
            }
        }

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

        if (logWallHits)
        {
            int i = 1;
            foreach (float hits in wallHits)
            {
                tempLabels.Add("WallHits Test " + i);
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
[CustomEditor(typeof(DataLogger2))]
public class DataLogger_Editor2 : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        var script = target as DataLogger2;

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
                script.logMinimumFrameRate = EditorGUILayout.Toggle("Log minimum", script.logMinimumFrameRate);
                EditorGUI.indentLevel -= 1;
            }
            script.logWallHits = EditorGUILayout.Toggle("Wall Hits", script.logWallHits);
            script.logPlayAreaSize = EditorGUILayout.Toggle("Play Area Size", script.logPlayAreaSize);
            script.logVRSickness = EditorGUILayout.Toggle("VR Sickness", script.logVRSickness);
            script.logComfort = EditorGUILayout.Toggle("Comfort", script.logComfort);
            script.logDemographic = EditorGUILayout.Toggle("Demographics", script.logDemographic);

            if (script.logDemographic)
            {
                EditorGUI.indentLevel += 1;
                script.logGender = EditorGUILayout.Toggle("Gender", script.logGender);
                script.logVR = EditorGUILayout.Toggle("VR Experience", script.logVR);
                script.logUnderstanding = EditorGUILayout.Toggle("Understand Task", script.logUnderstanding);
                EditorGUI.indentLevel -= 1;
            }

            EditorGUI.indentLevel -= 1;
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
        }
    }
}
#endif
#endregion
