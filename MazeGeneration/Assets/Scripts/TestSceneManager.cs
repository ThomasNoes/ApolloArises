using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestSceneManager : MonoBehaviour
{
    public GameObject companion1, companion2;
    private TestCompanion companion1Script, companion2Script;
    private GameObject camObj;
    public DataLogger dataLogger;
    public int testSceneIndexFrom = 1, testSceneIndexTo = 3;

    private int currentSceneIndex = 0;

    private void Start()
    {
        return; // TODO: temp disable
        dataLogger?.LogTimeStart();
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        camObj = Camera.main.gameObject;

        if (PlayerPrefs.GetInt("TestRunning") != 1)
        {
            PlayerPrefs.SetInt("TestRunning", 1);

            ScenesVisitedContainer SVC = new ScenesVisitedContainer();
            SVC.sceneVisited = new bool[3];
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
        return; // TODO: temp disable

        ScenesVisitedContainer SVC = new ScenesVisitedContainer();

        string dataString = PlayerPrefs.GetString("ScenesVisited");
        SVC = JsonUtility.FromJson<ScenesVisitedContainer>(dataString);

        if (SVC == null)
            return;

        int index = Random.Range(testSceneIndexFrom, testSceneIndexTo);

        if (SVC.sceneVisited != null)
        {
            for (int i = 0; i < SVC.sceneVisited.Length; i++)
            {
                if (index >= 0 && index < SVC.sceneVisited.Length)
                {

                }
            }
        }
    }

    public void NextSceneAtIndex(int index)
    {

    }

    public void NextSceneLastIndex()
    {

    }
}

public class ScenesVisitedContainer
{
    /// <summary>
    /// Index 0: is walled condition, 1: windows, 2: open
    /// </summary>
    public bool[] sceneVisited;
}
