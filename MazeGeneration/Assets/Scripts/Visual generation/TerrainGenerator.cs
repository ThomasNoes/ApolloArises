namespace EventCallbacks {
    using UnityEngine;

    public class TerrainGenerator : MonoBehaviour
    {
        // Variables:
        public bool useNewPlacementMethod = false, useTextureSwitcherInEditor = false, useTextureSwitcherOnAndroid = true;
        public float wallOffset = 0f, wallHeight = 1f, pillarScale = 1.5f;
        private float heightScale, wallWidth = 0.05f, tileScale;

        // Objects:
        public GameObject ceilingObj, wallObj, pillarObj, towerObj;
        private GameObject tempCeiling, tempWall, tempPillar;

        // Refs:
        private TextureCustomizer textureCustomizer;


        void Awake ()
        {
            GenerateTerrainEvent.RegisterListener(OnGenerateTerrain);
            GenerateTowersEvent.RegisterListener(TowerGenerator);
            TextureSwitchEvent.RegisterListener(TextureSwitch);
            textureCustomizer = GetComponent<TextureCustomizer>();

            if (useTextureSwitcherInEditor)
                textureCustomizer?.ResetTextures();
        }

        private void OnGenerateTerrain (GenerateTerrainEvent generateTerrain) {

            Tile tempTile = generateTerrain.tileArray[generateTerrain.tileRowPos, generateTerrain.tileColPos];

            Transform tileTransform = generateTerrain.go.transform;
            tileTransform.localPosition = new Vector3(tileTransform.localPosition.x, 0, tileTransform.localPosition.z);
            tileScale = tileTransform.localScale.y;
            heightScale = wallHeight / tileScale;

            if (tempTile.isPortalTile)
                TextureSwitchPortalOverride(true, tempTile);

        
            if (!tempTile.isOpenRoof)
            {
                // Place ceiling on the tile
                // Set the object as a child of the current tile
                tempCeiling = Instantiate(ceilingObj, new Vector3(tileTransform.position.x, tileTransform.position.y + wallHeight, tileTransform.position.z), Quaternion.AngleAxis(90, Vector3.left));
                tempCeiling.transform.parent = tileTransform;
                tempCeiling.transform.localScale = new Vector3(1, 1, 1);
            }

            if (tempTile.isPortalTile)
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

                        tempWall = Instantiate(wallObj, wallTransform, Quaternion.AngleAxis(i * 90, Vector3.up));

                        Wall tempWallScript = tempWall.GetComponent<Wall>(); // NOTE: Might not be optimized(?)
                        if (tempWallScript != null)
                        {
                            if (tempWallScript.meshes != null && tempTile.outerWalls != null)
                            {
                                tempWallScript.SetMesh(tempTile.outerWalls[i]);
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
                        tempWall = Instantiate(wallObj, tileTransform.position, Quaternion.AngleAxis(i * 90, Vector3.up), tileTransform); // instantiate at the position of the tile
                        tempWall.transform.localPosition = localPos;
                        tempWall.transform.localScale = localScale;
                        tempWall.name = "wall index " + i;
                        switch (i)
                        {
                            case 0:
                                tempWall.transform.Translate((tempWall.transform.forward * tileScale / 2) - (tempWall.transform.forward * wallWidth / 2 * tileScale) * 1.01f);
                                //tempWall.transform.localPosition = new Vector3(0, tempWall.transform.localPosition.y, 0.469f);
                                break;
                            case 1:
                                tempWall.transform.Translate((-tempWall.transform.right * tileScale / 2) + (tempWall.transform.right * wallWidth / 2 * tileScale) * 1.01f);
                                //tempWall.transform.localPosition = new Vector3(0.469f, tempWall.transform.localPosition.y, 0);
                                break;
                            case 2:
                                tempWall.transform.Translate((-tempWall.transform.forward * tileScale / 2) + (tempWall.transform.forward * wallWidth / 2 * tileScale) * 1.01f);
                                //tempWall.transform.localPosition = new Vector3(0, tempWall.transform.localPosition.y, -0.469f);
                                break;
                            case 3:
                                tempWall.transform.Translate((tempWall.transform.right * tileScale / 2) - (tempWall.transform.right * wallWidth / 2 * tileScale) * 1.01f);
                                //tempWall.transform.localPosition = new Vector3(-0.469f, tempWall.transform.localPosition.y, 0);
                                break;
                            default:
                                break;
                        }

                        Wall tempWallScript = tempWall.GetComponent<Wall>(); // NOTE: Might not be optimized(?)
                        if (tempWallScript != null)
                        {
                            if (tempWallScript.meshes != null && tempTile.outerWalls != null)
                            {
                                tempWallScript.SetMesh(tempTile.outerWalls[i]);
                            }
                        }
                    }
                }
            }

            if (tempTile.isOuterTile && !tempTile.isPortalTile)
                PlaceOuterPillars(tileTransform, generateTerrain, tempTile);
            else if (tempTile.isRoomTile)
                PlaceRoomPillars(tileTransform, generateTerrain);
            else
                PlaceAllPillars(tileTransform, generateTerrain);

            if (tempTile.isPortalTile)
                TextureSwitchPortalOverride(false, tempTile);
        }

        private void PlaceOuterPillars(Transform tileTransform, GenerateTerrainEvent generateTerrain, Tile tile)
        {
            for (int i = 0; i < tile.outerWalls.Length; i++)
            {
                if (tile.outerWalls[i] != 2 && generateTerrain.wallArray[i] != 1)
                {
                    PlacePillar(tileTransform, generateTerrain, i);
                    PlacePillar(tileTransform, generateTerrain, (i + 1) % 4);
                }
            }
        }

        private void PlaceRoomPillars(Transform tileTransform, GenerateTerrainEvent generateTerrain)
        {
            for (int i = 0; i < generateTerrain.wallArray.Length; i++)
            {
                if (generateTerrain.wallArray[i] == 0)
                {
                    PlacePillar(tileTransform, generateTerrain, i);
                    PlacePillar(tileTransform, generateTerrain, (i + 1) % 4);
                }
            }
        }

        /// <summary>
        /// Places a pillar at specified position.
        /// </summary>
        /// <param name="position">Positions = 0: upper left corner, 1: upper right corner, 2: lower right corner, 3: lower left corner</param>
        private void PlacePillar(Transform tileTransform, GenerateTerrainEvent generateTerrain, int position)
        {
            if (!PillarOrganizer(generateTerrain, position)) // Checks if pillar exist and otherwise adds to array of existing pillars
                return;
            
            // Instantiate a corner piller and place it
            // Set the scaling of the pillar so the height matches the walls
            // Place the object as a child of the tile on which it is placed
            tempPillar = Instantiate(pillarObj, 
                new Vector3(tileTransform.transform.position.x, 
                    tileTransform.position.y + (0.5f * wallHeight),
                    tileTransform.transform.position.z),
                Quaternion.identity);

            //tempPillar.transform.localScale *= tileScale;
            tempPillar.transform.localScale = new Vector3((tempPillar.transform.localScale.x * tileScale) * pillarScale,
                tempPillar.transform.localScale.y * tileScale, (tempPillar.transform.localScale.z * tileScale) * pillarScale);

            TileTransform(tileTransform, tempPillar);

            switch (position)
            {
                case 0:
                    tempPillar.name = "upper left";
                    tempPillar.transform.localPosition = new Vector3(-0.5f, tempPillar.transform.localPosition.y, 0.5f);
                    break;
                case 1:
                    tempPillar.name = "upper right";
                    tempPillar.transform.localPosition = new Vector3(0.5f, tempPillar.transform.localPosition.y, 0.5f);
                    break;
                case 2:
                    tempPillar.name = "lower right";
                    tempPillar.transform.localPosition = new Vector3(0.5f, tempPillar.transform.localPosition.y, -0.5f);
                    break;
                case 3:
                    tempPillar.name = "lower left";
                    tempPillar.transform.localPosition = new Vector3(-0.5f, tempPillar.transform.localPosition.y, -0.5f); // old values = 0.45
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

        /// <param name="pos">0: upper left corner, 1: upper right corner, 2: lower right corner, 3: lower left corner</param>
        /// <returns></returns>
        private static bool PillarOrganizer(GenerateTerrainEvent generateTerrain, int pos) // This function updates pillar bool array and checks if pillars can be placed
        {
            switch (pos)
            {
                case 0:
                    if (!generateTerrain.mapGeneratorRef.pillarBoolArray[generateTerrain.tileRowPos, generateTerrain.tileColPos])
                    {
                        generateTerrain.mapGeneratorRef.pillarBoolArray[generateTerrain.tileRowPos, generateTerrain.tileColPos] = true;
                        return true;
                    }
                    else
                        return false;
                case 1:
                    if (!generateTerrain.mapGeneratorRef.pillarBoolArray[generateTerrain.tileRowPos, generateTerrain.tileColPos + 1])
                    {
                        generateTerrain.mapGeneratorRef.pillarBoolArray[generateTerrain.tileRowPos, generateTerrain.tileColPos + 1] = true;
                        return true;
                    } else
                        return false;
                case 2:
                    if (!generateTerrain.mapGeneratorRef.pillarBoolArray[generateTerrain.tileRowPos + 1, generateTerrain.tileColPos + 1])
                    {
                        generateTerrain.mapGeneratorRef.pillarBoolArray[generateTerrain.tileRowPos + 1, generateTerrain.tileColPos + 1] = true;
                        return true;
                    } else
                        return false;
                case 3:
                    if (!generateTerrain.mapGeneratorRef.pillarBoolArray[generateTerrain.tileRowPos + 1, generateTerrain.tileColPos])
                    {
                        generateTerrain.mapGeneratorRef.pillarBoolArray[generateTerrain.tileRowPos + 1, generateTerrain.tileColPos] = true;
                        return true;
                    }
                    else
                        return false;
                default:
                    return false;
            }
        }

        private void TileTransform(Transform thisTransform, GameObject thisObj)
        {
            thisObj.transform.localScale = new Vector3(thisObj.transform.localScale.x, wallHeight, thisObj.transform.localScale.z);
            thisObj.transform.parent = thisTransform;
        }

        private void TowerGenerator(GenerateTowersEvent generateTowers)
        {
            if (towerObj == null)
                return;

            Transform towerTransform = generateTowers.go.transform;

            GameObject tempTower = Instantiate(towerObj, towerTransform.position, Quaternion.identity, towerTransform);
            tempTower.transform.localScale = new Vector3(generateTowers.widthX, tempTower.transform.localScale.y, generateTowers.widthY);

            tempTower.transform.Translate((generateTowers.widthX / 2f) - (generateTowers.tileWidth / 2f), 0, (-generateTowers.widthY / 2f) + (generateTowers.tileWidth / 2f));
        }

        private void TextureSwitch(TextureSwitchEvent textureSwitch)
        {
            if (!AllowTextureSwitch(true))
                return;

            if ((textureSwitch.partOfMaze % textureCustomizer.frequency) == 0)
            {
                textureCustomizer.UpdateTextures(textureCustomizer.matIndex + 1);
                textureCustomizer.matIndex++;
            }
        }

        private void TextureSwitchPortalOverride(bool initial, Tile tile)
        {
            if (!AllowTextureSwitch(true))
                return;

            if (initial)
            {
                if (((tile.partOfMaze - 1) % textureCustomizer.frequency) == 0)
                    textureCustomizer.UpdateTextures(textureCustomizer.matIndex - 1);
            }
            else
                if ((tile.partOfMaze % textureCustomizer.frequency) == 0)
                textureCustomizer.UpdateTextures(textureCustomizer.matIndex);
        }

        private bool AllowTextureSwitch(bool placeholder)
        {
            if (Application.isEditor && !useTextureSwitcherInEditor)
                return false;

        #if UNITY_ANDROID
            if (!Application.isEditor && !useTextureSwitcherOnAndroid)
                return false;
        #endif

            if (textureCustomizer == null)
                return false;

            if (!textureCustomizer.autoSwitchTextures)
                return false;

                return true;
        }
    }
}