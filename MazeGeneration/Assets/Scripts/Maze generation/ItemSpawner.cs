using System.Collections.Generic;
using EventCallbacks;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public bool enable = false;

    public GameObject keyPrefab, doorPrefab, puzzleRobotPrefab;
    public bool spawnDoors = true, spawnKeysInDeadEnds = true, spawnPuzzleRobots = true;
    [Tooltip("Example: 2 means for every second room")] public int spawnFrequency = 3;

    public Material[] colourMaterials;

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

            if (spawnDoors)
                DoorAndKeySpawner();
        }
    }

    private bool SpawnKey(int mazeIndex, int uniqueId)
    {
        if (!spawnKeysInDeadEnds)
            return true;

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

                    if (tempKey.GetComponent<Key>() != null)
                    {
                        Key tempKeyScript = tempKey.GetComponent<Key>();
                        tempKeyScript.uniqueId = uniqueId;
                        tempKeyScript.colourMaterial = GetMaterialFromId(uniqueId);
                    }


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
        int uniqueId = 0;
        foreach (var room in mapManager.roomList)
        {
            if (counter == 0)
            {
                if (room.mazeID == mapManager.mapSequence.Length - 1)
                    break;

                if (SpawnKey(room.mazeID, uniqueId + 1))
                {
                    Vector3 tilePosition = room.exitTile.gameObject.transform.position;

                    int dir = DirectionCheckerNext(room.exitTile, room.mazeID);

                    GameObject tempDoor = Instantiate(doorPrefab, tilePosition,
                        GetRotation(dir), transform);

                    tempDoor.transform.position = GetEdgePositionWall(room.exitTile, dir);

                    uniqueId++;
                    if (tempDoor.GetComponent<Door>() != null)
                    {
                        Door tempDoorScript = tempDoor.GetComponent<Door>();
                        tempDoorScript.uniqueId = uniqueId;
                        tempDoorScript.doorMainObj.transform.localScale = new Vector3(tempDoor.transform.localScale.x * room.exitTile.tileWidth,
                            terrainGenerator.wallHeight, tempDoor.transform.localScale.z * room.exitTile.tileWidth);
                        tempDoorScript.colourMaterial = GetMaterialFromId(uniqueId);
                    }
                    else
                        tempDoor.transform.localScale = new Vector3(tempDoor.transform.localScale.x * room.exitTile.tileWidth, terrainGenerator.wallHeight, tempDoor.transform.localScale.z * room.exitTile.tileWidth);

                    if (spawnPuzzleRobots)
                        PuzzleRobotSpawner(room, uniqueId);
                }
            }

            counter = (counter + 1) % (spawnFrequency + 1);
        }

    }

    public void SpawnGroundObjectInDeadEndAtIndex(GameObject objToSpawn, int inMazeIndex)
    {
        if (mapManager == null)
            return;

        if (inMazeIndex >= 0 && inMazeIndex < mapManager.mapSequence.Length)
        {
            if (mapManager.deadEndList[inMazeIndex].Count != 0)
            {
                Tile tempTile = mapManager.deadEndList[inMazeIndex][0];

                Vector3 tilePos = tempTile.transform.position;
                Vector3 spawnPos = new Vector3(tilePos.x, tilePos.y + 0.5f, tilePos.z);

                Instantiate(objToSpawn, spawnPos, Quaternion.identity, transform);
                mapManager.deadEndList[inMazeIndex].RemoveAt(0);
            }
        }
    }

    public void SpawnWallObjectInDeadEndAtIndex(GameObject objToSpawn, int inMazeIndex)
    {
        if (mapManager == null)
            return;

        if (inMazeIndex >= 0 && inMazeIndex < mapManager.mapSequence.Length)
        {
            if (mapManager.deadEndList[inMazeIndex].Count != 0)
            {
                Tile tempTile = mapManager.deadEndList[inMazeIndex][0];
                int dir = SuitableWallReturner(tempTile);

                GameObject tempObj = Instantiate(objToSpawn, GetEdgePositionWall(tempTile, dir), GetRotation((dir + 2) % 4), transform);
                tempObj.transform.localScale = new Vector3(mapManager.tileWidth, mapManager.tileWidth, mapManager.tileWidth);

                mapManager.deadEndList[inMazeIndex].RemoveAt(0);
            }
        }
    }

    private void PuzzleRobotSpawner(Room room, int uniqueId)
    {
        if (puzzleRobotPrefab == null)
            return;

        List<Tile> tempTiles = new List<Tile>();

        foreach (var tile in room.tiles)
        {
            if (!tile.isAStarTile && tile.isRoomTile)
            {
                tempTiles.Add(tile);
            }
        }

        if (tempTiles.Count == 0)
            return;

        int tileIndex = 0;
        int dir = SuitableWallReturner(tempTiles[tileIndex]);

        foreach (var tile in tempTiles)
        {
            if (tempTiles[tileIndex].wallArray[(dir + 1) % 4] != 0 &&
                tempTiles[tileIndex].wallArray[(dir - 1) % 4] != 0)
            {
                if (tileIndex + 1 < tempTiles.Count)
                    tileIndex++;
            }
            else
                break;
        }

        GameObject tempPuzzleRobot = Instantiate(puzzleRobotPrefab, GetEdgePositionGround(tempTiles[tileIndex], dir),
            GetRotation(dir), transform);
        PuzzleRobot puzzleRobot = tempPuzzleRobot.GetComponent<PuzzleRobot>();


        if (puzzleRobot == null)
            Debug.LogError("Error: item spawner could not find puzzle robot script.");
        else
        {
            Vector3 scaleVector = new Vector3(mapManager.tileWidth, mapManager.tileWidth, puzzleRobot.mainScreenObj.transform.localScale.z);
            puzzleRobot.mainScreenObj.transform.localScale = scaleVector;
            puzzleRobot.headObj.transform.localScale = scaleVector;
            puzzleRobot.uniqueId = uniqueId;
            puzzleRobot.SetVisualGeneratorObject(gameObject);
        }
    }

    private Material GetMaterialFromId(int id)
    {
        if (colourMaterials == null)
            return new Material(Shader.Find("Standard"));

        return colourMaterials.Length != 0 ? colourMaterials[(id - 1) % colourMaterials.Length] : new Material(Shader.Find("Standard"));
    }

    private int SuitableWallReturner(Tile tile)
    {
        int dir = 0;
        for (int i = 0; i < tile.wallArray.Length; i++)
        {
            if (tile.wallArray[i] == 0)
            {
                dir = i;
            }
        }

        return dir;
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
                return Quaternion.Euler(0, 180, 0);
            case 1:
                return Quaternion.Euler(0, 270, 0);
            case 2:
                return Quaternion.Euler(0, 0, 0);
            case 3:
                return Quaternion.Euler(0, 90, 0);
            default:
                return Quaternion.identity;
        }
    }

    /// <param name="dir">0: up, 1: right, 2: down, 3: left</param>
    private Vector3 GetEdgePositionWall(Tile tile, int dir)
    {
        float tileWidth = tile.tileWidth;

        switch (dir)
        {
            case 0:
                return new Vector3(tile.transform.position.x, tile.transform.position.y + terrainGenerator.wallHeight / 2.0f, tile.transform.position.z + (tileWidth / 2.0f));
            case 1:
                return new Vector3(tile.transform.position.x + (tileWidth / 2.0f), tile.transform.position.y + terrainGenerator.wallHeight / 2.0f, tile.transform.position.z);
            case 2:
                return new Vector3(tile.transform.position.x, tile.transform.position.y + terrainGenerator.wallHeight / 2.0f, tile.transform.position.z - (tileWidth / 2.0f));
            case 3:
                return new Vector3(tile.transform.position.x - (tileWidth / 2.0f), tile.transform.position.y + terrainGenerator.wallHeight / 2.0f, tile.transform.position.z);
            default:
                return new Vector3(0,0,0);
        }
    }

    /// <param name="dir">0: up, 1: right, 2: down, 3: left</param>
    private Vector3 GetEdgePositionGround(Tile tile, int dir)
    {
        float tileWidth = tile.tileWidth;

        switch (dir)
        {
            case 0:
                return new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z - (tileWidth / 2.8f));
            case 1:
                return new Vector3(tile.transform.position.x + (tileWidth / 2.8f), tile.transform.position.y, tile.transform.position.z);
            case 2:
                return new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z + -(tileWidth / 2.8f));
            case 3:
                return new Vector3(tile.transform.position.x - (tileWidth / 2.8f), tile.transform.position.y, tile.transform.position.z);
            default:
                return new Vector3(0, 0, 0);
        }
    }
}
