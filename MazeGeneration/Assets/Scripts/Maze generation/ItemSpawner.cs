using System.Collections.Generic;
using EventCallbacks;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public bool enable = false;

    public GameObject keyPrefab, doorPrefab, puzzleRobotPrefab, leverPrefab, cabinetPrefab, puzzleItemPrefab;

    public bool spawnDoors = true, spawnKeysInDeadEnds = false, spawnPuzzleRobots = true, spawnLevers = true, 
        spawnPuzzleObjects = true, spawnDrawersInDeadEnds = true;
    [Tooltip("Example: 2 means for every second room")] public int spawnFrequency = 3;

    public Material[] colourMaterials;

    private MapManager mapManager;
    private TerrainGenerator terrainGenerator;
    private bool[] itemSpawnedChecker;
    private int mapAmount = 0;
    private float wallHeight, tileWidth;

    private void Start()
    {
        if (!enable)
            return;

        mapManager = GameObject.FindGameObjectWithTag("MapManager")?.GetComponent<MapManager>();
        if (mapManager == null)
            mapManager = FindObjectOfType<MapManager>();

        Invoke("DelayedStart", 1.0f);
    }

    private void DelayedStart()
    {
        if (mapManager != null)
        {
            terrainGenerator = mapManager.terrainGenerator;

            mapAmount = mapManager.mapSequence.Length;
            tileWidth = mapManager.tileWidth;

            transform.rotation = mapManager.gameObject.transform.rotation;
            transform.position = mapManager.gameObject.transform.position;

            if (terrainGenerator == null)
                terrainGenerator = FindObjectOfType<TerrainGenerator>();
            wallHeight = terrainGenerator != null ? terrainGenerator.wallHeight : 2.5f;

            if (spawnDoors)
                PuzzleSpawner();

            if (spawnLevers)
                LeverSpawner();
        }
    }

    private bool SpawnKey(int mazeIndex, int uniqueId)
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

    private bool SpawnCabinet(int mazeIndex, int uniqueId)
    {
        if (cabinetPrefab == null)
            return false;

        if (mazeIndex - 1 >= 0)
        {
            for (int i = mazeIndex - 1; i >= 0; i--)
            {
                if (mapManager.deadEndList[i].Count != 0)
                {
                    int index = mapManager.deadEndList[i].Count - 1;
                    GameObject tempCabinet = SpawnWallObjectInDeadEndAtIndex(cabinetPrefab, index, true, 1.0f);

                    tempCabinet.transform.localScale = new Vector3(tempCabinet.transform.localScale.x * tileWidth, 
                        tempCabinet.transform.localScale.y * tileWidth, tempCabinet.transform.localScale.z * tileWidth);

                    if (tempCabinet.GetComponent<Cabinet>() != null)
                    {
                        Cabinet tempCabinetScript = tempCabinet.GetComponent<Cabinet>();
                        tempCabinetScript.objToSpawnInCabinet = puzzleItemPrefab;
                        tempCabinetScript.SpawnItemInCabinet(tempCabinet.transform);
                    }

                    return true;
                }
            }
            return false;
        }
        return false;
    }

    private void PuzzleSpawner()
    {
        if (doorPrefab == null)
            return;

        int counter = 0;
        int uniqueId = 0;
        foreach (var room in mapManager.roomList)
        {
            if (counter == 0)
            {
                if (room.mazeID == mapAmount - 1)
                    break;

                if (spawnKeysInDeadEnds)
                {
                    if (SpawnKey(room.mazeID, uniqueId + 1))
                        SpawnDoor(room, uniqueId);
                }
                else if (spawnDrawersInDeadEnds)
                    if (SpawnCabinet(room.mazeID, uniqueId + 1))
                        SpawnDoor(room, uniqueId);
            }

            counter = (counter + 1) % (spawnFrequency + 1);
        }
    }

    private void SpawnDoor(Room room, int uniqueId)
    {
        Vector3 tilePosition = room.exitTile.gameObject.transform.position;

        int dir = DirectionCheckerNext(room.exitTile, room.mazeID);

        GameObject tempDoor = Instantiate(doorPrefab, tilePosition,
            transform.rotation, transform);

        room.exitTile.blocked = true;

        tempDoor.transform.Translate(GetEdgePositionWall(room.exitTile, dir, 0));
        tempDoor.transform.Rotate(GetEulerRotation(dir));

        uniqueId++;
        if (tempDoor.GetComponent<Door>() != null)
        {
            Door tempDoorScript = tempDoor.GetComponent<Door>();
            tempDoorScript.uniqueId = uniqueId;
            tempDoorScript.doorMainObj.transform.localScale = new Vector3(tempDoor.transform.localScale.x * room.exitTile.tileWidth,
                wallHeight, tempDoor.transform.localScale.z * room.exitTile.tileWidth);
            tempDoorScript.colourMaterial = GetMaterialFromId(uniqueId);
            tempDoorScript.height = wallHeight;
        }
        else
            tempDoor.transform.localScale = new Vector3(tempDoor.transform.localScale.x * room.exitTile.tileWidth, wallHeight, tempDoor.transform.localScale.z * room.exitTile.tileWidth);

        if (spawnPuzzleRobots)
            PuzzleRobotSpawner(room, uniqueId);
    }

    public void SpawnGroundObjectInDeadEndAtIndex(GameObject objToSpawn, int inMazeIndex)
    {
        if (mapManager == null)
            return;

        if (inMazeIndex >= 0 && inMazeIndex < mapAmount)
        {
            if (mapManager.deadEndList[inMazeIndex].Count != 0)
            {
                Tile tempTile = mapManager.deadEndList[inMazeIndex][0];

                Vector3 tilePos = tempTile.transform.position;
                Vector3 spawnPos = new Vector3(tilePos.x, tilePos.y + 0.5f, tilePos.z);

                Instantiate(objToSpawn, spawnPos, Quaternion.identity, transform);

                mapManager.deadEndList[inMazeIndex].RemoveAt(0);
                tempTile.occupied = true;
            }
        }
    }

    public GameObject SpawnWallObjectInDeadEndAtIndex(GameObject objToSpawn, int inMazeIndex, bool returnObj, float modifier)
    {
        if (mapManager == null)
            return new GameObject();

        if (inMazeIndex >= 0 && inMazeIndex < mapAmount)
        {
            if (mapManager.deadEndList[inMazeIndex].Count != 0)
            {
                Tile tempTile = mapManager.deadEndList[inMazeIndex][0];
                int dir = SuitableWallReturner(tempTile, true);

                GameObject tempObj = Instantiate(objToSpawn, tempTile.transform.position, transform.rotation, transform);
                //tempObj.transform.localScale = new Vector3(mapManager.tileWidth, mapManager.tileWidth, mapManager.tileWidth);

                tempObj.transform.Translate(GetEdgePositionWall(tempTile, dir, modifier));
                tempObj.transform.Rotate(GetEulerRotation(dir));

                mapManager.deadEndList[inMazeIndex].RemoveAt(0);
                tempTile.occupied = true;
                return tempObj;
            }
        }

        Debug.LogError("Item Spawner Error: spawning empty object");
        return new GameObject();
    }

    public void SpawnWallObjectInDeadEndAtIndex(GameObject objToSpawn, int inMazeIndex, float modifier)
    {
        SpawnWallObjectInDeadEndAtIndex(objToSpawn, inMazeIndex, false, modifier);
    }

    public void SpawnWallObjectOnSpecificTileAndWall(GameObject objToSpawn, Tile tile, int wall)
    {
        GameObject tempObj = Instantiate(objToSpawn, tile.transform.position, transform.rotation, transform);

        tempObj.transform.Translate(GetEdgePositionWall(tile, wall, 0.2f));
        tempObj.transform.Rotate(GetEulerRotation(wall));

        tile.occupied = true;
    }

    private void LeverSpawner() // TODO: optimize this code
    {
        if (mapManager == null && leverPrefab != null)
            return;

        itemSpawnedChecker = new bool[mapAmount];

        for (int i = 0; i < mapAmount; i++)
        {
            if (mapManager.deadEndList[i].Count != 0)
            {
                SpawnWallObjectInDeadEndAtIndex(leverPrefab, i, 0.2f);
                itemSpawnedChecker[i] = true;
            }
            else
            {
                for (int j = 0; j < mapManager.mapScripts[i].mazeRows; j++)
                {
                    for (int k = 0; k < mapManager.mapScripts[i].mazeColumns; k++)
                    {
                        if (itemSpawnedChecker[i])
                            break;

                        Tile tempTile = mapManager.mapScripts[i].tileArray[j, k];

                        if (!tempTile.isRoomTile && !tempTile.isPortalTile && !tempTile.isDeadEnd && !tempTile.occupied)
                        {
                            if (SuitableWallReturner(tempTile, true) == -1)
                                continue;

                            SpawnWallObjectOnSpecificTileAndWall(leverPrefab, tempTile, SuitableWallReturner(tempTile, true));
                            itemSpawnedChecker[i] = true;
                        }
                    }
                }
            }

            if (!itemSpawnedChecker[i]) // If item still is not spawned, try again with rooms
            {
                for (int j = 0; j < mapManager.mapScripts[i].mazeRows; j++)
                {
                    for (int k = 0; k < mapManager.mapScripts[i].mazeColumns; k++)
                    {
                        if (itemSpawnedChecker[i])
                            break;

                        Tile tempTile = mapManager.mapScripts[i].tileArray[j, k];

                        if (tempTile.isRoomTile && !tempTile.occupied)
                        {
                            if (SuitableOverrideReturner(tempTile) == -1)
                                continue;

                            SpawnWallObjectOnSpecificTileAndWall(leverPrefab, tempTile,
                                SuitableOverrideReturner(tempTile));
                            itemSpawnedChecker[i] = true;
                        }
                    }
                }
            }
        }

        for (int i = 0; i < itemSpawnedChecker.Length; i++) // For debugging TODO - disable later
        {
            if (itemSpawnedChecker[i] == false)
                Debug.LogError("Lever in maze " + i + " not spawned!");
        }
    }

    private void PuzzleRobotSpawner(Room room, int uniqueId)
    {
        if (puzzleRobotPrefab == null)
            return;

        List<Tile> tempTiles = new List<Tile>();

        foreach (var tile in room.tiles)
        {
            if (tile.isRoomTile && !tile.blocked)
            {
                tempTiles.Add(tile);
            }
        }

        if (tempTiles.Count == 0)
            return;

        int tileIndex = 0;
        int dir = SuitableWallReturner(tempTiles[tileIndex], false);

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

        GameObject tempPuzzleRobot = Instantiate(puzzleRobotPrefab, tempTiles[tileIndex].transform.position,
            transform.rotation, transform);
        PuzzleRobot puzzleRobot = tempPuzzleRobot.GetComponent<PuzzleRobot>();

        tempPuzzleRobot.transform.Translate(GetEdgePositionGround(tempTiles[tileIndex], dir));
        tempPuzzleRobot.transform.Rotate(GetEulerRotation(dir));

        tempTiles[tileIndex].occupied = true;
        tempTiles[tileIndex].blocked = true;

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

    public Material GetMaterialFromId(int id)
    {
        if (colourMaterials == null)
            return new Material(Shader.Find("Standard"));

        return colourMaterials.Length != 0 ? colourMaterials[(id - 1) % colourMaterials.Length] : new Material(Shader.Find("Standard"));
    }

    private int SuitableWallReturner(Tile tile, bool excludeWindowsAndBalconies)
    {
        int dir = -1;
        for (int i = 0; i < tile.wallArray.Length; i++)
        {
            if (tile.isShutOffTile)
                break;

            if (!excludeWindowsAndBalconies && tile.wallArray[i] == 0)
            {
                dir = i;
            }
            else if (excludeWindowsAndBalconies)
            {
                if (tile.wallArray[i] == 0 && !tile.isOuterTile)
                {
                    dir = i;
                }
                else if (tile.wallArray[i] == 0 && tile.isOuterTile)
                {
                    if (tile.outerWalls[i] == 0 || tile.outerWalls[i] == -1)
                        dir = i;
                }
            }
        }
        return dir;
    }

    private int SuitableOverrideReturner(Tile tile)
    {
        int dir = -1;
        for (int i = 0; i < tile.wallArray.Length; i++)
        {
            if (tile.isShutOffTile)
                break;

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

    private Vector3 GetEulerRotation(int dir)
    {
        switch (dir)
        {
            case 0:
                return new Vector3(0, 180, 0);
            case 1:
                return new Vector3(0, 270, 0);
            case 2:
                return new Vector3(0, 0, 0);
            case 3:
                return new Vector3(0, 90, 0);
            default:
                return new Vector3(0,0,0);
        }
    }

    private int GetOpenDirection(Tile tile)
    {
        int dir = -1;

        for (int i = 0; i < tile.wallArray.Length; i++)
        {
            if (tile.wallArray[i] == 1)
                dir = i;
        }
        return dir;
    }

    /// <param name="dir">0: up, 1: right, 2: down, 3: left</param>
    private Vector3 GetEdgePositionWall(Tile tile, int dir, float modifier)
    {
        float tileWidth = tile.tileWidth;

        switch (dir)
        {
            case 0:
                return new Vector3(0, terrainGenerator.wallHeight / 2.0f, tileWidth / (2.0f + modifier));
            case 1:
                return new Vector3(tileWidth / (2.0f + modifier), terrainGenerator.wallHeight / 2.0f, 0);
            case 2:
                return new Vector3(0, terrainGenerator.wallHeight / 2.0f, -tileWidth / (2.0f + modifier));
            case 3:
                return new Vector3(-tileWidth / (2.0f + modifier), terrainGenerator.wallHeight / 2.0f, 0);
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
                return new Vector3(0, 0, tileWidth / 2.8f);
            case 1:
                return new Vector3(tileWidth / 2.8f, 0, 0);
            case 2:
                return new Vector3(0, 0, -tileWidth / 2.8f);
            case 3:
                return new Vector3(-tileWidth / 2.8f, 0, 0);
            default:
                return new Vector3(0, 0, 0);
        }
    }
}