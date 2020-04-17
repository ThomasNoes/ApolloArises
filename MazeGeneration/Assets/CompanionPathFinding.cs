using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacks;

public class CompanionPathFinding : MonoBehaviour
{
    Vector3 posOffset = new Vector3(0, -0.2f, 0);
    LayerMask layerMask;
    GameObject player;


    Tile currentTile;
    Tile TargetTile;

    List<MapGenerator> maps;
    MapManager mm;

    public float speed = 1;
    Transform[] pathPoints;

    // Start is called before the first frame update
    void Start()
    {

        mm = GameObject.Find("MapManager").GetComponent<MapManager>();
        maps = mm.mapScripts;

        //placing the companion on specific tile for now.
        Transform tile = maps[0].aStarTiles[0].transform;
        float height = FindObjectOfType<TerrainGenerator>().wallHeight;
        transform.position = new Vector3(tile.position.x, tile.position.y + height, tile.position.z) + posOffset;



        layerMask = LayerMask.GetMask("Head");
        layerMask |= LayerMask.GetMask("Ignore Raycast");
        layerMask = ~layerMask;

        currentTile = GetObjectTile(gameObject);
        Debug.Log(currentTile.name);

    }

    // Update is called once per frame
    void Update()
    {

    }

      

    private void MoveTo(Tile Target)
    {
        if (Target.partOfMaze == currentTile.partOfMaze)
        {
            //calculate path a star
        }
        if (Target.partOfMaze > currentTile.partOfMaze)
        {
            //move towards next maze with nextdistance
        }
        if (Target.partOfMaze < currentTile.partOfMaze)
        {
            //move towards prev maze with prevdistance
        }
    }


    private Tile GetObjectTile(GameObject go)
    {
        RaycastHit hit;

        if (Physics.Raycast(go.transform.position, Vector3.down, out hit, 7.0f, layerMask))
        {
            Tile tempTile = hit.collider.gameObject.GetComponentInParent<Tile>();

            if (tempTile != null)
            {
                return tempTile;
            }
        }
        return null;
    }
}
