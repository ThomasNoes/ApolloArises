using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MapGenerator {

    public override void Generate () {
        Generate (0, 0, 1);
    }
    public override void Generate (MapInfo info) {
        Generate (info.startSeed, info.roomName);
    }
    public override void Generate (TileInfo startSeed, string roomName = "RoomTemplate") {
        Generate (startSeed.row, startSeed.column, startSeed.direction, roomName);
    }
    public override void Generate (int startRow, int startCol, int startDir, int endRow, int endCol, int endDir, string roomName = "RoomTemplate") {
        Generate (startRow, startCol, startDir, roomName);
    }
    public override void Generate (int startRow, int startCol, int startDirection, string roomName = "RoomTemplate") {
        int sR = 0;
        int sC = 0;
        int eR = mazeRows;
        int eC = mazeColumns;
        int doorDirection = 0;

        switch (startDirection) {
            case 0:

                Tile.ConnectTiles (tileArray[1, startCol], tileArray[0, startCol], startDirection);
                Tile.ConnectTiles (tileArray[mazeRows - 2, startCol], tileArray[mazeRows - 1, startCol], (startDirection + 2) % 4);

                if (startCol == 0) {
                    doorDirection = 1;
                    sC++;
                } else {
                    doorDirection = 3;
                    eC--;
                }
                tileArray[0, startCol].OpenWall (doorDirection);
                tileArray[mazeRows - 1, startCol].OpenWall (doorDirection);
                for (int i = startRow + 1; i < mazeRows - 1 - startRow; i++)
                    tileArray[i, startCol].SetTileID (0);
                break;
            case 1:
                Tile.ConnectTiles (tileArray[startRow, mazeColumns - 2], tileArray[startRow, mazeColumns - 1], startDirection);
                Tile.ConnectTiles (tileArray[startRow, 1], tileArray[startRow, 0], (startDirection + 2) % 4);
                if (startRow == 0) {
                    doorDirection = 2;
                    sR++;
                } else {
                    doorDirection = 0;
                    eR--;
                }
                tileArray[startRow, 0].OpenWall (doorDirection);
                tileArray[startRow, mazeColumns - 1].OpenWall (doorDirection);
                for (int i = startCol - 1; i > 1; i--)
                    tileArray[startRow, i].SetTileID (0);
                break;
            case 2:

                Tile.ConnectTiles (tileArray[mazeRows - 2, startCol], tileArray[mazeRows - 1, startCol], startDirection);
                Tile.ConnectTiles (tileArray[1, startCol], tileArray[0, startCol], (startDirection + 2) % 4);

                if (startCol == 0) {
                    doorDirection = 1;
                    sC++;
                } else {
                    doorDirection = 3;
                    eC--;
                }
                tileArray[mazeRows - 1, startCol].OpenWall (doorDirection);
                tileArray[0, startCol].OpenWall (doorDirection);
                for (int i = startRow - 1; i > 1; i--)
                    tileArray[i, startCol].SetTileID (0);
                break;
            case 3:

                Tile.ConnectTiles (tileArray[startRow, 1], tileArray[startRow, 0], startDirection);
                Tile.ConnectTiles (tileArray[startRow, mazeColumns - 2], tileArray[startRow, mazeColumns - 1], (startDirection + 2) % 4);

                if (startRow == 0) {
                    doorDirection = 2;
                    sR++;
                } else {
                    doorDirection = 0;
                    eR--;
                }
                tileArray[startRow, 0].OpenWall (doorDirection);
                tileArray[startRow, mazeColumns - 1].OpenWall (doorDirection);
                for (int i = startCol + 1; i < mazeColumns - startCol - 1; i++)
                    tileArray[startRow, i].SetTileID (0);
                break;
            default:
                break;
        }
        if (roomName == "")
            GenerateEmptyRoom (sR, sC, eR, eC);
        else {
            CreatePremadeRoom (roomName, doorDirection);
            DeleteUnusedTiles (sR, sC, eR, eC);
        }
        GenerateIntArray ();
    }

    void CreatePremadeRoom (string roomName, int alignment) {
        GameObject premadeRoom = Instantiate ((GameObject) Resources.Load ("Room Prefabs/" + roomName), transform.position, Quaternion.identity);
        premadeRoom.transform.parent = transform;
        premadeRoom.transform.Translate (mazeColumns * tileWidth / 2f - tileWidth / 2f, 0f, -mazeRows * tileWidth / 2f + tileWidth / 2f, Space.World);
        premadeRoom.transform.Rotate (0, 90f * (alignment + 2), 0, Space.Self);
        premadeRoom.transform.Translate (0f, 0f, -tileWidth / 2f + .1f, Space.Self);
    }

    void DeleteUnusedTiles (int startRow, int startCol, int endRow, int endCol) {
        for (int i = startRow; i < endRow; i++) {
            for (int j = startCol; j < endCol; j++) {
                Destroy (tileArray[i, j].gameObject);
            }
        }
    }

    void GenerateEmptyRoom (int startRow, int startCol, int endRow, int endCol) {
        for (int i = startRow; i < endRow; i++) {
            for (int j = startCol; j < endCol; j++) {
                tileArray[i, j].SetTileID (15);
                if (i == 0)
                    tileArray[i, j].CloseWall (0);
                if (i == mazeRows - 1)
                    tileArray[i, j].CloseWall (2);
                if (j == 0)
                    tileArray[i, j].CloseWall (3);
                if (j == mazeColumns - 1)
                    tileArray[i, j].CloseWall (1);
            }
        }
    }
}