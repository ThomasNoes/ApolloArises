using EventCallbacks;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public bool enable = false;

    public GameObject keyPrefab, doorPrefab, puzzleRobotPrefab;
    public bool spawnDoorsAndKeys = true, spawnPuzzleRobots = true;
    [Tooltip("Example: 2 means for every second room")] public int spawnFrequency = 3;
    private MapManager mapManager;
    private TerrainGenerator terrainGenerator;

    private void Start()
    {
        if (!enable)
            return;

        mapManager = GameObject.FindGameObjectWithTag("MapManager")?.GetComponent<MapManager>();
        if (mapManager == null)
            mapManager = FindObjectOfType<MapManager>();

        if (mapManager != null)
        {
            terrainGenerator = mapManager.terrainGenerator;

            if (terrainGenerator == null)
                terrainGenerator = FindObjectOfType<TerrainGenerator>();
            if (terrainGenerator == null)
                Debug.LogError("ERROR: Terrain generator is null on item spawner - could not be found");

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

                    GameObject tempDoor = Instantiate(doorPrefab, tilePosition,
                        GetRotation(dir), transform);

                    tempDoor.transform.position = GetEdgePosition(room.exitTile, dir);
                    tempDoor.transform.localScale = new Vector3(tempDoor.transform.localScale.x * room.exitTile.tileWidth, terrainGenerator.wallHeight, tempDoor.transform.localScale.z * room.exitTile.tileWidth);
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
        switch (dir)
        {
            case 0:
                return Quaternion.Euler(0, 0, 0);
            case 1:
                return Quaternion.Euler(0, 90, 0);
            case 2:
                return Quaternion.Euler(0, 180, 0);
            case 3:
                return Quaternion.Euler(0, 270, 0);
            default:
                return Quaternion.identity;
        }
    }

    /// <param name="dir">0: up, 1: right, 2: down, 3: left</param>
    private Vector3 GetEdgePosition(Tile tile, int dir)
    {
        float tileWidth = tile.tileWidth;

        switch (dir)
        {
            case 0:
                return new Vector3(tile.transform.position.x, tile.transform.position.y + terrainGenerator.wallHeight / 2.0f, tile.transform.position.z - (tileWidth / 2.0f));
            case 1:
                return new Vector3(tile.transform.position.x + (tileWidth / 2.0f), tile.transform.position.y + terrainGenerator.wallHeight / 2.0f, tile.transform.position.z);
            case 2:
                return new Vector3(tile.transform.position.x, tile.transform.position.y + terrainGenerator.wallHeight / 2.0f, tile.transform.position.z + - (tileWidth / 2.0f));
            case 3:
                return new Vector3(tile.transform.position.x - (tileWidth / 2.0f), tile.transform.position.y + terrainGenerator.wallHeight / 2.0f, tile.transform.position.z);
            default:
                return new Vector3(0,0,0);
        }
    }
}
