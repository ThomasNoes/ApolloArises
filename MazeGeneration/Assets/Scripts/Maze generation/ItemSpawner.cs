using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject keyPrefab, doorPrefab, puzzleRobotPrefab;
    public bool spawnDoorsAndKeys = true, spawnPuzzleRobots = true;
    [Tooltip("Example: 2 means for every second room")] public int spawnFrequency = 3;
    private MapManager mapManager;

    private void Start()
    {
        mapManager = GameObject.FindGameObjectWithTag("MapManager")?.GetComponent<MapManager>();
        if (mapManager == null)
            mapManager = FindObjectOfType<MapManager>();

        if (mapManager != null)
        {
            if (spawnDoorsAndKeys)
                DoorAndKeySpawner();
            if (spawnPuzzleRobots)
                PuzzleRobotSpawner();
        }
    }

    private bool SpawnKey(int mazeIndex)
    {
        if (keyPrefab == null)
            return false;

        if (mazeIndex - 1 >= 0)
        {
            for (int i = mazeIndex - 1; i >= 0; i--)
            {
                if (mapManager.deadEndList[i].Count != 0)
                {
                    int index = mapManager.deadEndList[i].Count - 1;
                    Tile tempTile = mapManager.deadEndList[i][index];
                    Vector3 tilePos = tempTile.transform.position;
                    Vector3 spawnPos = new Vector3(tilePos.x, tilePos.y + 0.5f, tilePos.z);
                    GameObject tempKey = Instantiate(keyPrefab, spawnPos, Quaternion.identity, transform);
                    mapManager.deadEndList[i].RemoveAt(index);

                    return true;
                }
            }
            return false;
        }
        return false;
    }

    private void DoorAndKeySpawner()
    {
        if (doorPrefab == null)
            return;

        int counter = 0;
        foreach (var room in mapManager.roomList)
        {
            if (counter == 0)
            {
                if (SpawnKey(room.mazeID))
                {
                    Vector3 tilePosition = room.exitTile.gameObject.transform.position;

                    int dir = DirectionCheckerNext(room.exitTile, room.mazeID);



                    Vector3 doorPosition = new Vector3(tilePosition.x, tilePosition.y, tilePosition.z);
                    Quaternion doorRotation = Quaternion.identity;

                    GameObject tempDoor = Instantiate(doorPrefab, doorPosition,
                        doorRotation, room.exitTile.gameObject.transform);
                }
            }

            counter = (counter + 1) % (spawnFrequency + 1);
        }

    }

    private void PuzzleRobotSpawner()
    {
        // TODO - YYY
    }

    /// <summary>
    /// Return either int 0: up, 1: right, 2: down, 3: left
    /// </summary>
    private int DirectionCheckerNext(Tile aStarTile, int currentMaze)
    {
        int currentRow = aStarTile.GetRow();
        int currentCol = aStarTile.GetCol();
        int nextRow = mapManager.mapScripts[currentMaze].aStarTiles[aStarTile.aStarId + 1].GetRow();
        int nextCol = mapManager.mapScripts[currentMaze].aStarTiles[aStarTile.aStarId + 1].GetCol();

        if (currentRow - 1 == nextRow && currentCol == nextCol)
            return 0;
        if (currentRow == nextRow && currentCol + 1 == nextCol)
            return 1;
        if (currentRow + 1 == nextRow && currentCol == nextCol)
            return 2;
        if (currentRow == nextRow && currentCol - 1 == nextCol)
            return 3;

        return -1;
    }

    private Quaternion GetRotation(int dir)
    {
        return Quaternion.identity;
    }

    private Vector3 GetPosition(int dir)
    {
        switch (dir)
        {
            case 0:

        }
    }
}
