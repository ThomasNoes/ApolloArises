using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class TestSceneManager : MonoBehaviour
{
    public BoolValue sessionChecker;
    public GameObject companion1, companion2;
    private TestCompanion companion1Script, companion2Script;
    private GameObject camObj;
    public DataLogger dataLogger;
    public int testSceneIndexFrom = 1, testSceneIndexTo = 3;

    private int currentSceneIndex = 0, sceneRange;

    private void Start()
    {
        if (sessionChecker == null)
            return;

        dataLogger?.LogTimeStart();
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        camObj = Camera.main.gameObject;
        sceneRange = (testSceneIndexTo + 1) - testSceneIndexFrom;

        if (!sessionChecker.value)
        {
            sessionChecker.value = true;

            ScenesVisitedContainer SVC = new ScenesVisitedContainer();
            SVC.sceneVisited = new bool[sceneRange];
            PlayerPrefs.SetString("ScenesVisited", JsonUtility.ToJson(SVC));
        }

        if (camObj != null && companion1 != null && companion2 != null)
        {
            companion1Script = companion1.GetComponent<TestCompanion>();
            companion2Script = companion2.GetComponent<TestCompanion>();

            if (companion1Script == null || companion2Script == null)
                return;

            if (Vector3.Distance(camObj.transform.position, companion1.transform.position) > Vector3.Distance(camObj.transform.position, companion2.transform.position))
            {
                companion1Script.EnableMesh();
                companion2Script.DisableMesh();
            }
            else
            {
                companion1Script.DisableMesh();
                companion2Script.EnableMesh();
            }
        }
    }

    public void NextSceneRandom()
    {
        ScenesVisitedContainer SVC = new ScenesVisitedContainer();

        string dataString = PlayerPrefs.GetString("ScenesVisited");
        SVC = JsonUtility.FromJson<ScenesVisitedContainer>(dataString);

        if (SVC == null)
            return;


        int index = DateTime.Now.Second % 3;

        if (SVC.sceneVisited != null)
        {
            for (int i = 0; i < SVC.sceneVisited.Length; i++)
            {
                if (index >= 0 && index < SVC.sceneVisited.Length)
                {
                    if (SVC.sceneVisited[index] != true)
                    {
                        SVC.sceneVisited[index] = true;
                        PlayerPrefs.SetString("ScenesVisited", JsonUtility.ToJson(SVC));
                        SceneManager.LoadScene(index);
                    }
                    else
                        index = (index + 1) % sceneRange;
                }
            }
            NextSceneLastIndex();
        }
    }

    public void NextSceneAtIndex(int index)
    {
        if (index >= 0 && index < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(index);
        }
    }

    public void NextSceneLastIndex()
    {
        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    }
}

public class ScenesVisitedContainer
{
    /// <summary>
    /// Index 0: is walled condition, 1: windows, 2: open
    /// </summary>
    public bool[] sceneVisited;
}
