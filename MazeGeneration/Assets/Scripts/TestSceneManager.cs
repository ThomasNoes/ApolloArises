using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SessionHandler))]
public class TestSceneManager : MonoBehaviour
{
    public BoolValue sessionChecker, conditionChecker;
    public GameObject companion1, companion2;
    private TestCompanion companion1Script, companion2Script;
    private GameObject camObj;
    public int testSceneIndexFrom = 1, testSceneIndexTo = 3;

    private int currentSceneIndex = 0, sceneRange, arrIndex;

    private void Start()
    {
        if (sessionChecker == null)
            return;

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

            if (conditionChecker != null)
            {
                if (conditionChecker.value)
                {
                    Invoke("DelayDistanceCheck", 0.1f);
                }
                else
                {
                    conditionChecker.value = true;
                }
            }
            else
                Invoke("DelayDistanceCheck", 0.1f);
        }
    }

    private void DelayDistanceCheck()
    {
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

    public void NextSceneRandom()
    {
        ScenesVisitedContainer SVC = new ScenesVisitedContainer();

        string dataString = PlayerPrefs.GetString("ScenesVisited");
        SVC = JsonUtility.FromJson<ScenesVisitedContainer>(dataString);

        if (SVC == null)
            return;

        arrIndex = RandomNumber(sceneRange);

        if (SVC.sceneVisited != null)
        {
            for (int i = 0; i < SVC.sceneVisited.Length; i++)
            {
                if (arrIndex >= 0 && arrIndex < SVC.sceneVisited.Length)
                {
                    if (SVC.sceneVisited[arrIndex] == false)
                    {
                        SVC.sceneVisited[arrIndex] = true;
                        PlayerPrefs.SetString("ScenesVisited", JsonUtility.ToJson(SVC));
                        Invoke("DelayedSwitchScene", 0.5f);
                        return;
                    }
                    else
                        arrIndex = (arrIndex + 1) % sceneRange;
                }
            }
            NextSceneLastIndex();
        }
    }

    public void NextSceneAtIndex(int index)
    {
        if (index >= 0 && index < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(index, LoadSceneMode.Single);
        }
    }

    public void NextSceneLastIndex()
    {
        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    }

    public void NextScene()
    {
        if (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
        }
    }

    public int RandomNumber(int to)
    {
        return (System.DateTime.Now.Second % to);
    }

    public void DelayedSwitchScene()
    {
        SceneManager.LoadScene(testSceneIndexFrom + arrIndex, LoadSceneMode.Single);
    }
}

public class ScenesVisitedContainer
{
    /// <summary>
    /// Index 0: is walled condition, 1: windows, 2: open
    /// </summary>
    public bool[] sceneVisited;
}