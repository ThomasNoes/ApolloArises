// Attach this to map manager object
using UnityEngine;

public class MazeDisabler : MonoBehaviour
{
    private PortalRenderController pRController;
    private MapManager mapManager;
    private GameObject[][] portals;
    private bool active = true;
    private int currentMaze;

    private void Start()
    {
        mapManager = gameObject.GetComponent<MapManager>();
        pRController = FindObjectOfType<PortalRenderController>();

        if (mapManager == null || pRController == null)
            active = false;
        else
            Invoke("Initialize", 0.5f);
    }

    private void Initialize()
    {
        InitializePortalArray();

        if (portals.Length <= 0)
        {
            active = false;
            return;
        }

        for (int i = 0; i < mapManager.mapSequence.Length; i++)
            mapManager.mapSequence[i].mapObject.SetActive(false);

        for (int i = 0; i < portals.Length; i++)
        {
            portals[i][0].SetActive(false);
            portals[i][1].SetActive(false);
        }

        UpdateDisabled();
    }

    private void InitializePortalArray()
    {
        GameObject[] tempPortals = GameObject.FindGameObjectsWithTag("Portal");

        if (tempPortals == null)
            return;

        portals = new GameObject[tempPortals.Length / 2][];

        for (int i = 0; i < portals.Length; i++)
            portals[i] = new GameObject[2];
        

        Debug.Log(tempPortals);

        int j = 0;
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i][0] = tempPortals[j].gameObject;
            portals[i][1] = tempPortals[j + 1].gameObject;
            j += 2;
        }
    }

    public void UpdateDisabled()
    {
        currentMaze = pRController.currentMaze;
        mapManager.mapSequence[currentMaze].mapObject.SetActive(true);
        portals[currentMaze][0].SetActive(true);
        portals[currentMaze][1].SetActive(true);

        for (int i = 1; i < 3; i++)
        {
            if (currentMaze + i != mapManager.mapSequence.Length - 1)
            {
                mapManager.mapSequence[currentMaze + i].mapObject.SetActive(true);
                portals[currentMaze + 1][0].SetActive(true);
                portals[currentMaze + 1][1].SetActive(true);
            }

            if (currentMaze - i >= 0)
            {
                mapManager.mapSequence[currentMaze - i].mapObject.SetActive(true);
                portals[currentMaze - 1][0].SetActive(true);
                portals[currentMaze - 1][0].SetActive(true);
            }
        }
    }
}
