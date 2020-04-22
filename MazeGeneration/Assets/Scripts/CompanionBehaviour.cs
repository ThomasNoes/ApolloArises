using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CompanionBehaviour : MonoBehaviour
{
    //this script manages the companions behaviour
    //like what tile to move to and when
    //what to say and when to say
    //and generally listen to events that happens in the game, and what player actions happens

    public DialogReader dr;
    public CompanionPathFinding cpf;
    MapManager mm;
    List<MapGenerator> maps;


    public bool isFollowPlayer;

    Tile startTile;
    Tile endtile;



    // Start is called before the first frame update
    void Start()
    {
        dr = GetComponent<DialogReader>();
        cpf = GetComponent<CompanionPathFinding>();
        mm = GameObject.Find("MapManager").GetComponent<MapManager>();
        maps = mm.mapScripts;

        startTile = FindStartEndTile(maps[0]);
        endtile = FindStartEndTile(maps[maps.Count - 1]);

        Invoke("LateStart",0);
    }

    void LateStart()
    {
        cpf.PlaceCompanionOnTile(startTile);
    }

    // Update is called once per frame
    void Update()
    {
        if (true)
        {

        }

        if(isFollowPlayer)
        {
            cpf.FollowPlayer();
        }
    }


    private Tile FindStartEndTile(MapGenerator map)
    {
        foreach (Tile t in map.tileArray) // finding the ideal tile
        {
            if (!t.isOuterTile && !t.isPortalTile && t.isRoomTile)
            {
                Debug.Log("ideal spot");
                return t;
            }
        }
        foreach (Tile t in map.tileArray) // else allow for outertiles
        {
            if ( !t.isPortalTile && t.isRoomTile)
            {
                Debug.Log("it is in a room");
                return t;
            }
        }
        foreach (Tile t in map.tileArray) // else just place it on a tile that is not a outer tile
        {
            if (!t.isOuterTile)
            {
                Debug.Log("just not a outer tile");
                return t;
            }
        }
        return null;
    }

    //start end event






}










#if UNITY_EDITOR
[CustomEditor(typeof(CompanionBehaviour))]
public class Companion_Inspector : UnityEditor.Editor
{
    private GUIStyle headerStyle;

    public override void OnInspectorGUI()
    {
        var script = target as CompanionBehaviour;

        DrawDefaultInspector(); // for other non-HideInInspector fields

        if (GUILayout.Button("Go to Target"))
        {
            Debug.Log("Go To Target Pressed");
            script.cpf.GoToTarget();
        }
        if (GUILayout.Button("Display Dialog"))
        {
            Debug.Log("Display Dialog Pressed");
            script.dr.DisplayDialog();
        }

    }
}
#endif
