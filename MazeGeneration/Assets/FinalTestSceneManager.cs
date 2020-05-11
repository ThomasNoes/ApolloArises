using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using EventCallbacks;

public class FinalTestSceneManager : MonoBehaviour
{

    public static FinalTestSceneManager instance;
    private TerrainGenerator terrainGenerator;

    public FloatValue fpsCountData, fpsSumData, minimumFrameFloat;
    public StringValue startCondition;

    int[] evenOrder = new int[6] {0, 1, 2, 3, 4, 5};
    int[] oddOrder = new int[6] { 0, 3, 4, 1, 2, 5 };

    int[] order;

    int index = 0;

    //logging framerate
    float framerate;
    float sum;
    float count;
    float average;
    float minimum = float.MaxValue;
    string minimumScene;

    bool activate = true;
     
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            ResetData();
        }
    }

    private void ResetData()
    {
        fpsCountData.value = 0;
        fpsSumData.value = 0;
        minimumFrameFloat.value = float.MaxValue;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        terrainGenerator = null;
        terrainGenerator = FindObjectOfType<TerrainGenerator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        int sec = (int) DateTime.Now.Second;

        if (sec % 2 == 0)
        {
            order = evenOrder;
            startCondition.value = "Game First";
        }
        else
        {
            order = oddOrder;
            startCondition.value = "No Game First";
        }
}


    private void Update()
    {
        framerate = 1 / Time.deltaTime;
        sum += framerate;
        count++;
        average = sum/count;

        if (Time.timeSinceLevelLoad > 10)
        {
            if (framerate < minimum)
            {
                minimum = framerate;
                minimumScene = SceneManager.GetActiveScene().name;
            }
        }

    }

    public void GoToNextScene()
    {
        index++;
        if (index < order.Length && activate)
        {
            Camera.main.GetComponent<OVRScreenFade>().ExitSceneFade();
            Invoke("NewScene", 2);
            activate = false;
            Invoke("SetToActivate", 5);
        }
    }

    void SetToActivate()
    {
        activate = true;
    }

    private void NewScene()
    {
        if (terrainGenerator != null)
            terrainGenerator.UnregisterListeners();

        SceneManager.LoadScene(order[index], LoadSceneMode.Single);
    }


    public string GetAverageFramerate()
    {
        return ((int)average).ToString();
    }
    public string GetMinimumFramerate()
    {
        return ((int)minimum).ToString() + " in " + minimumScene + " at time: " + Time.timeSinceLevelLoad;
    }
}
