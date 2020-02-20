using UnityEngine;

namespace EventCallbacks {
    public class TerrainGenerator : MonoBehaviour {
        GameObject newCeiling;
        GameObject newWall;
        GameObject newPillar;
        public float wallOffset = 0f;
        public float wallHeight = 1f;

        public Material pillarMat;
        public Material wallMat;
        public Material floorMat;
        public Material ceilingMat;

        public GameObject ceiling;
        public GameObject wallSegment;
        public GameObject woodPillar;
        // Start is called before the first frame update
        void Awake () {
            GenerateTerrainEvent.RegisterListener (OnGenerateTerrain);
        }

        void OnGenerateTerrain (GenerateTerrainEvent generateTerrain) {
            Transform tileTransform = generateTerrain.go.transform;


            // Get floor
            if (floorMat != null)
                if (tileTransform.GetChild(0) != null)
                    tileTransform.GetChild(0).GetComponent<Renderer>().material = floorMat;

            // Place ceiling on the tile
            // Set the object as a child of the current tile
            newCeiling = Instantiate (ceiling, new Vector3 (tileTransform.position.x, wallHeight, tileTransform.position.z), Quaternion.AngleAxis (90, Vector3.left));
            newCeiling.transform.parent = tileTransform;
            if (ceilingMat != null)
                newCeiling.GetComponent<Renderer>().material = ceilingMat;

            // Read through wallArray to see how many walls should be placed and where
            // Each case corresponds to a side on the current tile
            for (int i = 0; i < generateTerrain.wallArray.Length; i++) {
                if (generateTerrain.wallArray[i] == 0) {
                    switch (i) {
                        case 0:
                            // Instatiate a wall segment and place it on the top side
                            // Set the scaling to be the preset height chosen in the inspector
                            // Set the object as a child of the current tile
                            newWall = Instantiate (wallSegment, new Vector3 (tileTransform.position.x - (generateTerrain.tileWidth / 2f), 
                                tileTransform.position.y, tileTransform.position.z + (generateTerrain.tileWidth / 2f) - wallOffset), Quaternion.AngleAxis (i * 90, Vector3.up));
                            TileTransform(tileTransform, newWall);
                            break;
                        case 1:
                            newWall = Instantiate (wallSegment, new Vector3 (tileTransform.position.x + (generateTerrain.tileWidth / 2f) 
                                - wallOffset, tileTransform.position.y, tileTransform.position.z + (generateTerrain.tileWidth / 2f)), Quaternion.AngleAxis (i * 90, Vector3.up));
                            TileTransform(tileTransform, newWall);
                            break;
                        case 2:
                            newWall = Instantiate (wallSegment, new Vector3 (tileTransform.position.x + (generateTerrain.tileWidth / 2f), 
                                tileTransform.position.y, tileTransform.position.z - (generateTerrain.tileWidth / 2f) + wallOffset), Quaternion.AngleAxis (i * 90, Vector3.up));
                            TileTransform(tileTransform, newWall);
                            break;
                        case 3:
                            newWall = Instantiate (wallSegment, new Vector3 (tileTransform.position.x - (generateTerrain.tileWidth / 2f) + wallOffset, 
                                tileTransform.position.y, tileTransform.position.z - (generateTerrain.tileWidth / 2f)), Quaternion.AngleAxis (i * 90, Vector3.up));
                            TileTransform(tileTransform, newWall);
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
            TileTransform(tileTransform, newPillar);

            newPillar = Instantiate (woodPillar, new Vector3 (tileTransform.position.x + (generateTerrain.tileWidth / 2f) - (woodPillar.transform.localScale.x / 2f), tileTransform.position.y + (0.5f * wallHeight), tileTransform.position.z + (generateTerrain.tileWidth / 2f) - (woodPillar.transform.localScale.z / 2f)), Quaternion.identity);
            TileTransform(tileTransform, newPillar);

            newPillar = Instantiate (woodPillar, new Vector3 (tileTransform.position.x + (generateTerrain.tileWidth / 2f) - (woodPillar.transform.localScale.x / 2f), tileTransform.position.y + (0.5f * wallHeight), tileTransform.position.z - (generateTerrain.tileWidth / 2f) + (woodPillar.transform.localScale.z / 2f)), Quaternion.identity);
            TileTransform(tileTransform, newPillar);

            newPillar = Instantiate (woodPillar, new Vector3 (tileTransform.position.x - (generateTerrain.tileWidth / 2f) + (woodPillar.transform.localScale.x / 2f), tileTransform.position.y + (0.5f * wallHeight), tileTransform.position.z + (generateTerrain.tileWidth / 2f) - (woodPillar.transform.localScale.z / 2f)), Quaternion.identity);
            TileTransform(tileTransform, newPillar);
        }

        private void TileTransform(Transform thisTransform, GameObject thisObj)
        {
            thisObj.transform.localScale = new Vector3(thisObj.transform.localScale.x, wallHeight, thisObj.transform.localScale.z);
            thisObj.transform.parent = thisTransform;

            if (pillarMat != null)
                thisObj.GetComponent<Renderer>().material = pillarMat;

            if (wallMat != null)
                thisObj.GetComponent<Renderer>().material = wallMat;
        }
    }
}