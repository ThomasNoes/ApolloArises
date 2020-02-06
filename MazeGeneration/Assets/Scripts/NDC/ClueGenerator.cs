using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueGenerator : MonoBehaviour
{

    public MapManager mapManager;
    public MapGenerator[] mapGenerators;
    public GameObject sittingMiner;
    public List<GameObject> clues;
    private GameObject instantiateMiner;
    private GameObject instantiateClue;
    private List<TileInfo> deadEnd;

    // Start is called before the first frame update
    void Start()
    {
        List<TileInfo> deadEnd = new List<TileInfo>();
        mapGenerators = new MapGenerator[mapManager.mapSequence.Length];

        for (int i = 0; i < mapGenerators.Length; i++)
        {
            mapGenerators[i] = mapManager.transform.GetChild(i).GetComponent<MapGenerator>();
            deadEnd = mapGenerators[i].GetDeadEndListTileInfo();
            Debug.Log(deadEnd);
            if (deadEnd.Remove(mapManager.mapSequence[i].startSeed))
            {
                /* Debug.Log("removed entrance portal: ");
                mapManager.mapSequence[i].startSeed.PrintTile(); */
            }
            if (deadEnd.Remove(mapManager.mapSequence[i].endSeed))
            {
                /* Debug.Log("removed exit portal: ");
                mapManager.mapSequence[i].endSeed.PrintTile(); */
            }

            if (deadEnd.Count > 0 && clues.Count > 0)
            {
                TileInfo deadEndTile = deadEnd[Random.Range(0,deadEnd.Count)];
                Debug.Log("PRINT TILE DIRECTION: " + deadEndTile.direction);
                
                GameObject go = mapGenerators[i].tileArray[deadEndTile.row,deadEndTile.column].gameObject;

                int randomPosition = Random.Range(0,2);
                Vector3 localTransformation = new Vector3(0,0,0);
                Vector3 localRotation = new Vector3(0,0,0);

                switch (randomPosition)
                {
                    case 0:
                        localTransformation = new Vector3(.02f, .034f, -.2f);
                        localRotation = new Vector3(4.5f, 90f * deadEndTile.direction - 35f, 0f);
                        break;
                    
                    case 1:
                        localTransformation = new Vector3(-.05f, .034f, -.2f);
                        localRotation = new Vector3(4.5f, 90f * deadEndTile.direction + 35f, 0);
                        break;

                }
                
                instantiateMiner = Instantiate (sittingMiner, new Vector3(go.transform.position.x, go.transform.position.y ,go.transform.position.z), Quaternion.identity);
                instantiateMiner.transform.parent = go.transform;
                instantiateMiner.transform.Rotate(localRotation, Space.Self);
                instantiateMiner.transform.Translate(localTransformation, Space.Self);

                int clueIndex = Random.Range(0,clues.Count);

                instantiateClue = Instantiate (clues[clueIndex], new Vector3(go.transform.position.x, 0f, go.transform.position.z), Quaternion.Euler(90f, Random.Range(0,100), 0f));
                instantiateClue.transform.parent = instantiateMiner.transform;

                clues.Remove(clues[clueIndex]);
            }

            

            /* foreach (var item in deadEnd)
            {
                Debug.Log("PRINT TILE -------------");
                item.PrintTile();
            } */
        }
    }
}
