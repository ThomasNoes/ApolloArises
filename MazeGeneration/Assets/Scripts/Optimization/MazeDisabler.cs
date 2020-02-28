// Attach this to map manager object
using UnityEngine;

public class MazeDisabler : MonoBehaviour
{
    public bool enable = true;
    public int bufferAmount = 3;
    private PortalRenderController pRController;
    private MapManager mapManager;
    private GameObject[][] portals;
    private bool active = true;
    private int currentMaze;

    private void Start()
    {
        if (!enable)
            return;

        mapManager = gameObject.GetComponent<MapManager>();
        pRController = FindObjectOfType<PortalRenderController>();

        if (mapManager == null || pRController == null)
            active = false;
        else if (Application.isEditor)
        {
#if UNITY_EDITOR
            Invoke("Initialize", 0.3f);
#endif
        }
    }

    public void Initialize()
    {
        InitializePortalArray();

        if (portals.Length <= 0)
        {
            active = false;
            return;
        }

        for (int i = 0; i < mapManager.mapSequence.Length; i++)
            mapManager.mapSequence[i].mapObject.SetActive(false);

        foreach (var t in portals)
        {
            t[0].SetActive(false);
            t[1].SetActive(false);
        }

        UpdateDisabled();
    }

    private void InitializePortalArray()
    {
        GameObject[] tempPortals = GameObject.FindGameObjectsWithTag("Portal");

        if (tempPortals == null)
            return;

        portals = new GameObject[tempPortals.Length / 2][];

        for (var i = 0; i < portals.Length; i++)
            portals[i] = new GameObject[2];

        var j = 0;

        foreach (var t in portals)
        {
            t[0] = tempPortals[j].gameObject;
            t[1] = tempPortals[j + 1].gameObject;
            j += 2;
        }
    }

    public void UpdateDisabled()
    {
        currentMaze = pRController.currentMaze;
        SetMaze(currentMaze, true);

        for (int i = 1; i < bufferAmount + 1; i++)
        {
            SetMaze(currentMaze + i, true);
            SetMaze(currentMaze - i, true);
        }

        SetMaze(currentMaze + (bufferAmount + 1), false);
        SetMaze(currentMaze - (bufferAmount + 1), false);

    }

    private void SetMaze(int index, bool enable)
    {
        if (index < 0 || index > mapManager.mapSequence.Length - 1)
            return;

        mapManager.mapSequence[index].mapObject.SetActive(enable);

        if (index != portals.Length)
        {
            portals[index][0].SetActive(enable);
            portals[index][1].SetActive(enable);
        }
    }
}
