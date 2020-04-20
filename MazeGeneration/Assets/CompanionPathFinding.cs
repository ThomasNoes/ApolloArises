using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacks;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CompanionPathFinding : MonoBehaviour
{
    Vector3 posOffset = new Vector3(0, -0.2f, 0);
    LayerMask layerMask;
    GameObject player;


    public Tile currentTile;
    public Tile TargetTile;

    List<MapGenerator> maps;
    MapManager mm;

    public float speed = 1;
    List<Tile> pathPoints;

    AStarPathFinding Astar = new AStarPathFinding();


    // Start is called before the first frame update
    void Start()
    {

        mm = GameObject.Find("MapManager").GetComponent<MapManager>();
        maps = mm.mapScripts;

        //placing the companion on a star tile.
        Transform tile = maps[0].aStarTiles[0].transform;

        //placing companion on non a star tile
        foreach (Tile t in maps[0].tileArray)
        {
            if (!t.isAStarTile)
            {
                tile = t.transform;
            }
        }

        float height = FindObjectOfType<TerrainGenerator>().wallHeight;
        transform.position = new Vector3(tile.position.x, tile.position.y + height, tile.position.z) + posOffset;


        // debug placing 
        TargetTile = maps[0].aStarTiles[maps[0].aStarTiles.Count - 1];


        layerMask = LayerMask.GetMask("Floor");
    }

    // Update is called once per frame
    void Update()
    {
        //currentTile = GetObjectTile(gameObject);
        //Debug.Log("cureent tile is " + currentTile.name);
    }

    public void GoToTarget()
    {
        currentTile = GetTileUnderObject(gameObject);
        PlanRoute(TargetTile);
        StartCoroutine("MoveToTarget");
    }

    private IEnumerator MoveToTarget()
    {
        for (int i = 0; i < pathPoints.Count; i++)
        {
            Vector3 point = pathPoints[i].transform.position;
            point.y = transform.position.y;
            while (transform.position != point)
            {
                transform.position = Vector3.MoveTowards(transform.position, point, speed * Time.deltaTime);
                yield return null;
            }
        }
    }

    private void PlanRoute(Tile Target)
    {
        Debug.Log("target name "+Target.name);
        if (Target == null)
        {
            Debug.Log("target is null");
            return;
        }
        List<Tile> tempPath = new List<Tile>();
        if (Target.partOfMaze == currentTile.partOfMaze) // if the companion have to navigate in the current maze
        {
            
            if (Target.isAStarTile && currentTile.isAStarTile) // if both are a AStar tile 
            {
                Debug.Log("dedededede");
                tempPath = GetPartofAStarPath(currentTile, Target);
            }
            else // i need to find the a custom path, find path again.
            {
                tempPath =Astar.NPCPathFinding(maps[currentTile.partOfMaze].tileArray, currentTile, Target,false,false);
            }
        }
        if (Target.partOfMaze > currentTile.partOfMaze)
        {
            //move towards next maze with nextdistance
            //first find path to the next portal -> check if the current tile is a astar tile.
            //then the Astar path through the next maze segments
            // in the tartet maze, find the path to the target tile -> check if it is astar tile
        }
        if (Target.partOfMaze < currentTile.partOfMaze)
        {
            //move towards prev maze with prevdistance
        }
        pathPoints = tempPath;
    }

    private List<Tile> GetPartofAStarPath(Tile from, Tile to)
    {
        List<Tile> returnList = new List<Tile>();
        List<Tile> aStars = maps[from.partOfMaze].aStarTiles;


        int fromIndex = aStars.IndexOf(from);
        int toIndex = aStars.IndexOf(to);

        while (fromIndex != toIndex)
        {
            returnList.Add(aStars[fromIndex]);

            if (fromIndex < toIndex)
            {
                fromIndex++;
            }
            else
            {
                fromIndex--;
            }
        }
        returnList.Add(to);

        return returnList;
    }

   
    private void ListDebug()
    {
        for (int i = 0; i < maps.Count; i++)
        {
            for (int j = 0; j < maps[i].aStarTiles.Count; j++)
            {
                Debug.Log(maps[i].aStarTiles[j].name + " is astar number " + j + " in maze " + i);
            }
        }
    }


    private Tile GetTileUnderObject(GameObject go)
    {
        RaycastHit hit;

        Debug.DrawRay(go.transform.position, Vector3.down * 7, Color.yellow, 25);

        if (Physics.Raycast(go.transform.position, Vector3.down, out hit, 7.0f, layerMask))
        {
            Debug.Log(go.name+" has hit "+hit.collider.name +" on "+hit.collider.transform.parent.name + " on "+hit.collider.transform.parent.parent.name);
            Tile tempTile = hit.collider.gameObject.GetComponentInParent<Tile>();

            if (tempTile != null)
            {
                Debug.Log("found a tile");
                return tempTile;
            }
        }
        else
        {
            Debug.Log("no floor under " + go.name);
        }
        return null;
    }



}

#if UNITY_EDITOR
[CustomEditor(typeof(CompanionPathFinding))]
public class Companion_Inspector : UnityEditor.Editor
{
    private GUIStyle headerStyle;

    public override void OnInspectorGUI()
    {
        var script = target as CompanionPathFinding;

        DrawDefaultInspector(); // for other non-HideInInspector fields

        if (GUILayout.Button("Go to Target"))
        {
            Debug.Log("pressing button");
            script.GoToTarget();
        }
    }
}
#endif
