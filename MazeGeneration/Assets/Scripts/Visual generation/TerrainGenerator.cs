namespace EventCallbacks {

    using UnityEngine;

    public class TerrainGenerator : MonoBehaviour
    {
        public bool useNewPlacementMethod = false;
        GameObject tempCeiling;
        GameObject tempWall;
        GameObject tempPillar;
        public float wallOffset = 0f;
        public float wallHeight = 1f;


        public GameObject ceiling;
        public GameObject wallSegment;
        public GameObject woodPillar;

        float heightScale;
        float wallWidth = 0.05f;
        float tileScale;


        private TextureCustomizer textureCustomizer;


        void Awake ()
        {
            GenerateTerrainEvent.RegisterListener (OnGenerateTerrain);
            textureCustomizer = GetComponent<TextureCustomizer>();
        }

        void OnGenerateTerrain (GenerateTerrainEvent generateTerrain) {

            Transform tileTransform = generateTerrain.go.transform;
            tileTransform.localPosition = new Vector3(tileTransform.localPosition.x, 0, tileTransform.localPosition.z);
            tileScale = tileTransform.localScale.y;
            heightScale = wallHeight / tileScale;

            if (textureCustomizer != null)
                if (textureCustomizer.autoSwitchTextures)
                    textureCustomizer.UpdateTextures(generateTerrain.materialIndex);


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

            if (!useNewPlacementMethod)
            {
                // Read through wallArray to see how many walls should be placed and where
                // Each case corresponds to a side on the current tile
                Vector3 wallTransform = new Vector3();

                for (int i = 0; i < generateTerrain.wallArray.Length; i++)
                {
                    if (generateTerrain.wallArray[i] == 0)
                    {
                        switch (i)
                        {
                            case 0:
                                // Set the scaling to be the preset height chosen in the inspector
                                // Set the object as a child of the current tile
                                wallTransform = new Vector3(tileTransform.position.x - (generateTerrain.tileWidth / 2f), tileTransform.position.y,
                                    tileTransform.position.z + (generateTerrain.tileWidth / 2f) - wallOffset);
                                break;
                            case 1:
                                wallTransform = new Vector3(tileTransform.position.x + (generateTerrain.tileWidth / 2f) - wallOffset, tileTransform.position.y,
                                    tileTransform.position.z + (generateTerrain.tileWidth / 2f));
                                break;
                            case 2:
                                wallTransform = new Vector3(tileTransform.position.x + (generateTerrain.tileWidth / 2f), tileTransform.position.y,
                                    tileTransform.position.z - (generateTerrain.tileWidth / 2f) + wallOffset);
                                break;
                            case 3:
                                wallTransform = new Vector3(tileTransform.position.x - (generateTerrain.tileWidth / 2f) + wallOffset, tileTransform.position.y,
                                    tileTransform.position.z - (generateTerrain.tileWidth / 2f));
                                break;
                        }

                        tempWall = Instantiate(wallSegment, wallTransform, Quaternion.AngleAxis(i * 90, Vector3.up));

                        Wall tempWallScript = tempWall.GetComponent<Wall>(); // NOTE: Might not be optimized(?)
                        if (tempWallScript != null)
                        {
                            if (tempWallScript.meshes != null && generateTerrain.outerWalls != null)
                            {
                                tempWallScript.SetMesh(generateTerrain.outerWalls[i]);
                            }
                        }
                        TileTransform(tileTransform, tempWall);
                    }
                }
            }
            else
            {
                // Read through wallArray to see how many walls should be placed and where
                // Each case corresponds to a side on the current tile
                for (int i = 0; i < generateTerrain.wallArray.Length; i++)
                {
                    if (generateTerrain.wallArray[i] == 0)
                    {

                        Vector3 localPos = new Vector3(0, heightScale / 2, 0);
                        Vector3 localScale = new Vector3(0.99f, heightScale, wallWidth);
                        tempWall = Instantiate(wallSegment, tileTransform.position, Quaternion.AngleAxis(i * 90, Vector3.up), tileTransform); // instantiate at the position of the tile
                        tempWall.transform.localPosition = localPos;
                        tempWall.transform.localScale = localScale;
                        tempWall.name = "wall index " + i;
                        switch (i)
                        {
                            case 0:
                                tempWall.transform.Translate((tempWall.transform.forward * tileScale / 2) - (tempWall.transform.forward * wallWidth / 2 * tileScale) * 1.05f);
                                //tempWall.transform.localPosition = new Vector3(0, tempWall.transform.localPosition.y, 0.469f);
                                break;
                            case 1:
                                tempWall.transform.Translate((-tempWall.transform.right * tileScale / 2) + (tempWall.transform.right * wallWidth / 2 * tileScale) * 1.05f);
                                //tempWall.transform.localPosition = new Vector3(0.469f, tempWall.transform.localPosition.y, 0);
                                break;
                            case 2:
                                tempWall.transform.Translate((-tempWall.transform.forward * tileScale / 2) + (tempWall.transform.forward * wallWidth / 2 * tileScale) * 1.05f);
                                //tempWall.transform.localPosition = new Vector3(0, tempWall.transform.localPosition.y, -0.469f);
                                break;
                            case 3:
                                tempWall.transform.Translate((tempWall.transform.right * tileScale / 2) - (tempWall.transform.right * wallWidth / 2 * tileScale) * 1.05f);
                                //tempWall.transform.localPosition = new Vector3(-0.469f, tempWall.transform.localPosition.y, 0);
                                break;
                            default:
                                break;


                        }

                        Wall tempWallScript = tempWall.GetComponent<Wall>(); // NOTE: Might not be optimized(?)
                        if (tempWallScript != null)
                        {
                            if (tempWallScript.meshes != null && generateTerrain.outerWalls != null)
                            {
                                tempWallScript.SetMesh(generateTerrain.outerWalls[i]);
                            }
                        }
                    }
                }
            }


            if (generateTerrain.isRoomPart || generateTerrain.isOuterOpenTile) // Bool checks if tile is part of room, then it change pillar placement accordingly
                PlaceRoomPillars(tileTransform, generateTerrain);
            else
                PlaceAllPillars(tileTransform, generateTerrain);
        }

        private void PlaceRoomPillars(Transform tileTransform, GenerateTerrainEvent generateTerrain)
        {
            if (generateTerrain.wallArray[0] == 0)
            {
                PlacePillar(tileTransform, generateTerrain, 1);
                PlacePillar(tileTransform, generateTerrain, 3);
            }
            if (generateTerrain.wallArray[1] == 0)
            {
                PlacePillar(tileTransform, generateTerrain, 1);
                PlacePillar(tileTransform, generateTerrain, 2);
            }
            if (generateTerrain.wallArray[2] == 0)
            {
                PlacePillar(tileTransform, generateTerrain, 0);
                PlacePillar(tileTransform, generateTerrain, 2);
            }
            if (generateTerrain.wallArray[3] == 0)
            {
                PlacePillar(tileTransform, generateTerrain, 0);
                PlacePillar(tileTransform, generateTerrain, 3);
            }
        }

        /// <summary>
        /// Places a pillar at specified position
        /// </summary>
        /// <param name="tileTransform"></param>
        /// <param name="generateTerrain"></param>
        /// <param name="position">Positions = 0: lower left corner, 1: upper right corner, 2: lower right corner, 3: upper left corner</param>
        private void PlacePillar(Transform tileTransform, GenerateTerrainEvent generateTerrain, int position)
        {
            // Instantiate a corner piller and place it
            // Set the scaling of the pillar so the height matches the walls
            // Place the object as a child of the tile on which it is placed
            tempPillar = Instantiate(woodPillar, 
                new Vector3(tileTransform.transform.position.x, 
                    tileTransform.position.y + (0.5f * wallHeight),
                    tileTransform.transform.position.z),
                Quaternion.identity);
            tempPillar.transform.localScale *= tileScale;
            //tempPillar.transform.localScale =new Vector3(tempPillar.transform.localScale.x * tileScale * 1.01f, tileScale,tempPillar.transform.localScale.z * tileScale * 1.01f);
            TileTransform(tileTransform, tempPillar);

            switch (position)
            {
                case 0:
                    tempPillar.name = "lower left";
                    tempPillar.transform.localPosition = new Vector3(-0.45f, tempPillar.transform.localPosition.y, -0.45f);
                    break;
                case 1:
                    tempPillar.name = "upper right";
                    tempPillar.transform.localPosition = new Vector3(0.45f, tempPillar.transform.localPosition.y, 0.45f);
                    break;
                case 2:
                    tempPillar.name = "lower right";
                    tempPillar.transform.localPosition = new Vector3(0.45f, tempPillar.transform.localPosition.y, -0.45f);
                    break;
                case 3:
                    tempPillar.name = "upper left";
                    tempPillar.transform.localPosition = new Vector3(-0.45f, tempPillar.transform.localPosition.y, 0.45f);
                    break;
                default:
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