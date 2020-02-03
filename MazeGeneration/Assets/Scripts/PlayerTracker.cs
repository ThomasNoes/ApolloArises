using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CSVWrite))]
public class PlayerTracker : MonoBehaviour
{
    InputPromptTest promptTrigger;
    private int mazeCount;
    private float mazeOffset;
    Vector3 startPos;
    float tileWidth;

    Vector3 currentPos;
    bool isLoggerRunning;
    public bool logPosition;
    public bool promptPlayer;
    public float timeBetweenPings;
    public int currentMaze;
    public int currentRow;
    public int currentColumn;
    private CSVWrite dataLogger;
    // Start is called before the first frame update
    void Start()
    {
        dataLogger = GetComponent<CSVWrite>();
        MapManager mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        tileWidth = mapManager.tileWidth;
        startPos = new Vector3(mapManager.transform.position.x - tileWidth / 2f, 0, mapManager.transform.position.z + tileWidth / 2f);
        mazeCount = mapManager.mapSequence.Length;
        mazeOffset = mapManager.mazeCols * tileWidth + 1f;

        promptTrigger = GetComponent<InputPromptTest>();

        if(!isLoggerRunning && logPosition)
            StartCoroutine("PositionToConsole");
    }
    // Update is called once per frame
    void Update()
    {
        currentPos = new Vector3(transform.position.x - startPos.x, 0, transform.position.z - startPos.z);
        currentRow = (int)(-currentPos.z / tileWidth);
        currentMaze = (int)(currentPos.x / mazeOffset);
        currentColumn = (int)((currentPos.x - currentMaze * mazeOffset) / tileWidth);
    }

    private IEnumerator PositionToConsole()
    {
        isLoggerRunning = true;
        while (true)
        {
            Debug.Log("Time: " + Time.time + ", Maze: " + currentMaze + ", Row: " + currentRow + ", Column: " + currentColumn + ".");
            dataLogger.Save(Time.time, currentMaze, currentRow, currentColumn);
            if (promptPlayer)
            {
                promptTrigger.promptPlayer();
            }

            yield return new WaitForSeconds(timeBetweenPings);
        }
    }
}
