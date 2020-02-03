using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventCallbacks {
    public class TerrainGenerator : MonoBehaviour {
        GameObject newCeilingLamp;
        GameObject newCeiling;
        GameObject newWall;
        GameObject newPillar;
        public float wallOffset = 0f;
        public float wallHeight = 1f;

        public GameObject ceilingLamp;
        public GameObject ceiling;
        public GameObject wallSegment;
        public GameObject woodPillar;
        // Start is called before the first frame update
        void Awake () {
            GenerateTerrainEvent.RegisterListener (OnGenerateTerrain);
        }

        void OnGenerateTerrain (GenerateTerrainEvent generateTerrain) {
            Transform tileTransform = generateTerrain.go.transform;

            // Place ceiling on the tile
            // Set the object as a child of the current tile
            newCeiling = Instantiate (ceiling, new Vector3 (tileTransform.position.x, wallHeight, tileTransform.position.z), Quaternion.AngleAxis (90, Vector3.left));
            newCeiling.transform.parent = tileTransform;

            // Place ceiling lamp in the center of the tile
            // Set the object as a child of the current tile
            //newCeilingLamp = Instantiate (ceilingLamp, new Vector3 (tileTransform.position.x, ceilingLamp.transform.position.y + wallHeight, tileTransform.position.z), Quaternion.identity);
            //newCeilingLamp.transform.parent = tileTransform;

            // Read through wallArray to see how many walls should be placed and where
            // Each case corresponds to a side on the current tile
            for (int i = 0; i < generateTerrain.wallArray.Length; i++) {
                if (generateTerrain.wallArray[i] == 0) {
                    switch (i) {
                        case 0:
                            // Instatiate a wall segment and place it on the top side
                            // Set the scaling to be the preset height chosen in the inspector
                            // Set the object as a child of the current tile
                            newWall = Instantiate (wallSegment, new Vector3 (tileTransform.position.x - (generateTerrain.tileWidth / 2f), tileTransform.position.y, tileTransform.position.z + (generateTerrain.tileWidth / 2f) - wallOffset), Quaternion.AngleAxis (i * 90, Vector3.up));
                            newWall.transform.localScale = new Vector3 (newWall.transform.localScale.x, wallHeight, newWall.transform.localScale.z);
                            newWall.transform.parent = tileTransform;
                            break;
                        case 1:
                            // Instatiate a wall segment and place it on the right side
                            // Set the scaling to be the preset height chosen in the inspector
                            // Set the object as a child of the current tile
                            newWall = Instantiate (wallSegment, new Vector3 (tileTransform.position.x + (generateTerrain.tileWidth / 2f) - wallOffset, tileTransform.position.y, tileTransform.position.z + (generateTerrain.tileWidth / 2f)), Quaternion.AngleAxis (i * 90, Vector3.up));
                            newWall.transform.localScale = new Vector3 (newWall.transform.localScale.x, wallHeight, newWall.transform.localScale.z);
                            newWall.transform.parent = tileTransform;
                            break;
                        case 2:
                            // Instatiate a wall segment and place it on the bottom side
                            // Set the scaling to be the preset height chosen in the inspector
                            // Set the object as a child of the current tile
                            newWall = Instantiate (wallSegment, new Vector3 (tileTransform.position.x + (generateTerrain.tileWidth / 2f), tileTransform.position.y, tileTransform.position.z - (generateTerrain.tileWidth / 2f) + wallOffset), Quaternion.AngleAxis (i * 90, Vector3.up));
                            newWall.transform.localScale = new Vector3 (newWall.transform.localScale.x, wallHeight, newWall.transform.localScale.z);
                            newWall.transform.parent = tileTransform;
                            break;
                        case 3:
                            // Instatiate a wall segment and place it on the left side
                            // Set the scaling to be the preset height chosen in the inspector
                            // Set the object as a child of the current tile
                            newWall = Instantiate (wallSegment, new Vector3 (tileTransform.position.x - (generateTerrain.tileWidth / 2f) + wallOffset, tileTransform.position.y, tileTransform.position.z - (generateTerrain.tileWidth / 2f)), Quaternion.AngleAxis (i * 90, Vector3.up));
                            newWall.transform.localScale = new Vector3 (newWall.transform.localScale.x, wallHeight, newWall.transform.localScale.z);
                            newWall.transform.parent = tileTransform;
                            break;
                        default:
                            break;
                    }
                }
            }

            // Instantiate a corner piller and place it in the lower left corner
            // Set the scaling of the pillar so the height matches the walls
            // Place the object as a child of the tile on which it is placed
            newPillar = Instantiate (woodPillar, new Vector3 (tileTransform.position.x - (generateTerrain.tileWidth / 2f) + (woodPillar.transform.localScale.x / 2f), tileTransform.position.y + (0.5f * wallHeight), tileTransform.position.z - (generateTerrain.tileWidth / 2f) + (woodPillar.transform.localScale.z / 2f)), Quaternion.identity);
            newPillar.transform.localScale = new Vector3 (newPillar.transform.localScale.x, wallHeight, newPillar.transform.localScale.z);
            newPillar.transform.parent = tileTransform;

            // Instantiate a corner piller and place it in the upper right corner
            // Set the scaling of the pillar so the height matches the walls
            // Place the object as a child of the tile on which it is placed
            newPillar = Instantiate (woodPillar, new Vector3 (tileTransform.position.x + (generateTerrain.tileWidth / 2f) - (woodPillar.transform.localScale.x / 2f), tileTransform.position.y + (0.5f * wallHeight), tileTransform.position.z + (generateTerrain.tileWidth / 2f) - (woodPillar.transform.localScale.z / 2f)), Quaternion.identity);
            newPillar.transform.localScale = new Vector3 (newPillar.transform.localScale.x, wallHeight, newPillar.transform.localScale.z);
            newPillar.transform.parent = tileTransform;

            // Instantiate a corner piller and place it in the upper left corner
            // Set the scaling of the pillar so the height matches the walls
            // Place the object as a child of the tile on which it is placed
            newPillar = Instantiate (woodPillar, new Vector3 (tileTransform.position.x + (generateTerrain.tileWidth / 2f) - (woodPillar.transform.localScale.x / 2f), tileTransform.position.y + (0.5f * wallHeight), tileTransform.position.z - (generateTerrain.tileWidth / 2f) + (woodPillar.transform.localScale.z / 2f)), Quaternion.identity);
            newPillar.transform.localScale = new Vector3 (newPillar.transform.localScale.x, wallHeight, newPillar.transform.localScale.z);
            newPillar.transform.parent = tileTransform;

            // Instantiate a corner piller and place it in the lower right corner
            // Set the scaling of the pillar so the height matches the walls
            // Place the object as a child of the tile on which it is placed
            newPillar = Instantiate (woodPillar, new Vector3 (tileTransform.position.x - (generateTerrain.tileWidth / 2f) + (woodPillar.transform.localScale.x / 2f), tileTransform.position.y + (0.5f * wallHeight), tileTransform.position.z + (generateTerrain.tileWidth / 2f) - (woodPillar.transform.localScale.z / 2f)), Quaternion.identity);
            newPillar.transform.localScale = new Vector3 (newPillar.transform.localScale.x, wallHeight, newPillar.transform.localScale.z);
            newPillar.transform.parent = tileTransform;
        }
    }
}