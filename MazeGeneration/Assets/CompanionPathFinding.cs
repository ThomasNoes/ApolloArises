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

    public bool isFollowPlayer;
    public GameObject player;
    public Tile currentTile;
    public Tile targetTile;

    //other scripts
    List<MapGenerator> maps;
    MapManager mm;
    TeleportableObject tele;

    public float speed = 1;
    List<Tile> pathPoints;
    bool isTravelling = false;

    AStarPathFinding Astar = new AStarPathFinding();


    // Start is called before the first frame update
    void Start()
    {

        mm = GameObject.Find("MapManager").GetComponent<MapManager>();
        maps = mm.mapScripts;
        tele = GetComponent<TeleportableObject>();


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
        //targetTile = maps[6].aStarTiles[3];// maps[1].aStarTiles.Count - 1];
        foreach (Tile t in maps[6].tileArray)
        {
            if (!t.isAStarTile)
            {
                targetTile = t;
            }
        }

        layerMask = LayerMask.GetMask("Floor");
    }

    // Update is called once per frame
    void Update()
    {
        if (isFollowPlayer && player != null)
        {
            targetTile = GetTileUnderObject(player);
        }

        if (!isTravelling)
        {
            if (currentTile == null)
            {
                currentTile = GetTileUnderObject(gameObject);
            }

            if (currentTile != targetTile && targetTile != null)
            {
                GoToTarget();
            }
        }
    }

    public void GoToTarget()
    {
        isTravelling = true;
        PlanRoute(targetTile);
        StartCoroutine("MoveToTarget");
    }

    private IEnumerator MoveToTarget()
    {
        Tile pastPoint = currentTile;
        for (int i = 0; i < pathPoints.Count; i++)
        {
            //teleport
            if (pathPoints[i].isPortalTile && pastPoint.isPortalTile) // if the tile companion is going a portal and is currently on a portal tile 
            {

                if (pathPoints[i].partOfMaze > pastPoint.partOfMaze) //teleport Next maze forward
                {
                    tele.Teleport(true);
                }
                else if (pathPoints[i].partOfMaze < pastPoint.partOfMaze) //teleport prev maze backward
                {
                    tele.Teleport(false);
                }
            }
            //traverse
            Vector3 point = pathPoints[i].transform.position;
            point.y = transform.position.y;
            while (transform.position != point)
            {
                transform.position = Vector3.MoveTowards(transform.position, point, speed * Time.deltaTime);
                yield return null;
            }
            pastPoint = pathPoints[i];
        }
        isTravelling = false;
    }

    private void PlanRoute(Tile target)
    {
        currentTile = GetTileUnderObject(gameObject);

        if (target == null)
        {
            Debug.Log("target is null");
            return;
        }
        List<Tile> tempPath = new List<Tile>();
        tempPath.Add(currentTile);

        int targetMaze = target.partOfMaze;
        int currentMaze = currentTile.partOfMaze;
        Tile tempTile = currentTile;
        Tile tempTarget;

        while (tempTile != target)
        {
            //currentMaze = tempTile.partOfMaze;
            //for travelling to prev segment
            if (target.partOfMaze < tempTile.partOfMaze && // if the companion needs to travel to prev segment
                tempTile == maps[currentMaze].aStarTiles[0]) // it stands on the portal tile to the prev segment 
            {
                currentMaze--;
                tempTile = maps[currentMaze].tileArray[tempTile.GetRow(), tempTile.GetCol()];
            }
            else if (target.partOfMaze > tempTile.partOfMaze && // if the companion needs to travel to next segment
                tempTile == maps[currentMaze].aStarTiles[maps[currentMaze].aStarTiles.Count-1]) // it stands on the portal tile to the next segment 
            {
                currentMaze++;
                tempTile = maps[currentMaze].tileArray[tempTile.GetRow(), tempTile.GetCol()];
            }

            if (target.partOfMaze < tempTile.partOfMaze)
            {
                tempTarget = maps[currentMaze].aStarTiles[0];
            }
            else if (target.partOfMaze > tempTile.partOfMaze)
            {
                tempTarget = maps[currentMaze].aStarTiles[maps[currentMaze].aStarTiles.Count - 1];
            }
            else
            {
                tempTarget = target;
            }

            tempPath.AddRange(Astar.NPCPathFinding(maps[tempTile.partOfMaze].tileArray, tempTile, tempTarget, false, false));
            tempTile = tempTarget;
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
