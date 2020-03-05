namespace EventCallbacks {

    using UnityEngine;

    public class TerrainGenerator : MonoBehaviour {

        GameObject tempCeiling;
        GameObject tempWall;
        GameObject tempPillar;
        public float wallOffset = 0f;
        public float wallHeight = 1f;

        public GameObject ceiling;
        public GameObject wallSegment;
        public GameObject woodPillar;

        private TextureCustomizer textureCustomizer;


        void Awake ()
        {
            GenerateTerrainEvent.RegisterListener (OnGenerateTerrain);
            textureCustomizer = GetComponent<TextureCustomizer>();
        }

        void OnGenerateTerrain (GenerateTerrainEvent generateTerrain) {

            Transform tileTransform = generateTerrain.go.transform;
            tileTransform.localPosition = new Vector3(tileTransform.localPosition.x, 0, tileTransform.localPosition.z);

            if (!generateTerrain.noCeiling)
            {
                // Place ceiling on the tile
                // Set the object as a child of the current tile
                tempCeiling = Instantiate(ceiling, new Vector3(tileTransform.position.x, tileTransform.position.y + wallHeight, tileTransform.position.z), Quaternion.AngleAxis(90, Vector3.left));
                tempCeiling.transform.parent = tileTransform;
                tempCeiling.transform.localScale = new Vector3(1, 1, 1);
            }

            if (generateTerrain.isPortalTile)
            {
                for (int i = 0; i < generateTerrain.wallArray.Length; i++)
                {
                    if (generateTerrain.wallArray[i] == 1)
                    {
                        generateTerrain.wallArray[(i + 2) % 4] = 1;
                        break;
                    }
                }
            }

            // Read through wallArray to see how many walls should be placed and where
            // Each case corresponds to a side on the current tile
            for (int i = 0; i < generateTerrain.wallArray.Length; i++) {
                if (generateTerrain.wallArray[i] == 0) {
                    switch (i) {
                        case 0:
                            // Instatiate a wall segment and place it
                            // Set the scaling to be the preset height chosen in the inspector
                            // Set the object as a child of the current tile
                            tempWall = Instantiate (wallSegment, new Vector3 (tileTransform.position.x - (generateTerrain.tileWidth / 2f), 
                                tileTransform.position.y, tileTransform.position.z + (generateTerrain.tileWidth / 2f) - wallOffset), Quaternion.AngleAxis (i * 90, Vector3.up));
                            TileTransform(tileTransform, tempWall);
                            break;
                        case 1:
                            tempWall = Instantiate (wallSegment, new Vector3 (tileTransform.position.x + (generateTerrain.tileWidth / 2f) 
                                - wallOffset, tileTransform.position.y, tileTransform.position.z + (generateTerrain.tileWidth / 2f)), Quaternion.AngleAxis (i * 90, Vector3.up));
                            TileTransform(tileTransform, tempWall);
                            break;
                        case 2:
                            tempWall = Instantiate (wallSegment, new Vector3 (tileTransform.position.x + (generateTerrain.tileWidth / 2f), 
                                tileTransform.position.y, tileTransform.position.z - (generateTerrain.tileWidth / 2f) + wallOffset), Quaternion.AngleAxis (i * 90, Vector3.up));
                            TileTransform(tileTransform, tempWall);
                            break;
                        case 3:
                            tempWall = Instantiate (wallSegment, new Vector3 (tileTransform.position.x - (generateTerrain.tileWidth / 2f) + wallOffset, 
                                tileTransform.position.y, tileTransform.position.z - (generateTerrain.tileWidth / 2f)), Quaternion.AngleAxis (i * 90, Vector3.up));
                            TileTransform(tileTransform, tempWall);
                            break;
                        default:
                            break;
                    }
                }
            }

            if (generateTerrain.isRoomPart || generateTerrain.isOuterOpenTile) // Bool checks if tile is part of room, then it change pillar placement accordingly // TODO make work with outer open walls
            {
                if (generateTerrain.wallArray[0] == 0 && generateTerrain.wallArray[1] == 0)
                    PlacePillar(tileTransform, generateTerrain, 1);
                if (generateTerrain.wallArray[1] == 0 && generateTerrain.wallArray[2] == 0)
                    PlacePillar(tileTransform, generateTerrain,3);
                if (generateTerrain.wallArray[2] == 0 && generateTerrain.wallArray[3] == 0)
                    PlacePillar(tileTransform, generateTerrain, 0);
                if (generateTerrain.wallArray[3] == 0 && generateTerrain.wallArray[0] == 0)
                    PlacePillar(tileTransform, generateTerrain, 2);
            }
            else
            {
                PlaceAllPillars(tileTransform, generateTerrain);
            }
        }

        /// <summary>
        /// Places a pillar at specified position
        /// </summary>
        /// <param name="tileTransform"></param>
        /// <param name="generateTerrain"></param>
        /// <param name="position">Positions = 0: lower left corner, 1: upper right corner, 2: upper left corner, 3: lower right corner</param>
        private void PlacePillar(Transform tileTransform, GenerateTerrainEvent generateTerrain, int position)
        {
            // Instantiate a corner piller and place it
            // Set the scaling of the pillar so the height matches the walls
            // Place the object as a child of the tile on which it is placed
            switch (position)
            {
                case 0:
                    tempPillar = Instantiate(woodPillar, new Vector3(tileTransform.position.x - (generateTerrain.tileWidth / 2f) + (woodPillar.transform.localScale.x / 2f), tileTransform.position.y + (0.5f * wallHeight), tileTransform.position.z - (generateTerrain.tileWidth / 2f) + (woodPillar.transform.localScale.z / 2f)), Quaternion.identity);
                    TileTransform(tileTransform, tempPillar);
                    return;
                case 1:
                    tempPillar = Instantiate(woodPillar, new Vector3(tileTransform.position.x + (generateTerrain.tileWidth / 2f) - (woodPillar.transform.localScale.x / 2f), tileTransform.position.y + (0.5f * wallHeight), tileTransform.position.z + (generateTerrain.tileWidth / 2f) - (woodPillar.transform.localScale.z / 2f)), Quaternion.identity);
                    TileTransform(tileTransform, tempPillar);
                    return;

                case 2:
                    tempPillar = Instantiate(woodPillar, new Vector3(tileTransform.position.x + (generateTerrain.tileWidth / 2f) - (woodPillar.transform.localScale.x / 2f), tileTransform.position.y + (0.5f * wallHeight), tileTransform.position.z - (generateTerrain.tileWidth / 2f) + (woodPillar.transform.localScale.z / 2f)), Quaternion.identity);
                    TileTransform(tileTransform, tempPillar);
                    return;

                case 3:
                    tempPillar = Instantiate(woodPillar, new Vector3(tileTransform.position.x - (generateTerrain.tileWidth / 2f) + (woodPillar.transform.localScale.x / 2f), tileTransform.position.y + (0.5f * wallHeight), tileTransform.position.z + (generateTerrain.tileWidth / 2f) - (woodPillar.transform.localScale.z / 2f)), Quaternion.identity);
                    TileTransform(tileTransform, tempPillar);
                    return;
                default:
                    Debug.LogError("Terrain Generator Pillar Error: This is not a valid position");
                    break;
            }
        }

        private void PlaceAllPillars(Transform tileTransform, GenerateTerrainEvent generateTerrain)
        {
            for (int i = 0; i < generateTerrain.wallArray.Length; i++)
            {
                PlacePillar(tileTransform, generateTerrain, i);
            }
        }

        private void TileTransform(Transform thisTransform, GameObject thisObj)
        {
            thisObj.transform.localScale = new Vector3(thisObj.transform.localScale.x, wallHeight, thisObj.transform.localScale.z);
            thisObj.transform.parent = thisTransform;
        }
    }
}