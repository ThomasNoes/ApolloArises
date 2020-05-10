using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class FinalTestSceneManager : MonoBehaviour
{

    public static FinalTestSceneManager instance;

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
            ResetData();
        }
    }

    private void ResetData()
    {
        fpsCountData.value = 0;
        fpsSumData.value = 0;
        minimumFrameFloat.value = float.MaxValue;
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
        if (OVRManager.boundary.GetVisible())
        {
            OVRManager.boundary.SetVisible(false);
        }


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
        SceneManager.LoadScene(order[index]);
    }


    public string GetAverageFramerate()
    {
        return ((int)average).ToString();
    }
    public string GetMinimumFramerate()
    {
        return ((int)minimum).ToString() + " " + minimumScene;
    }
}
