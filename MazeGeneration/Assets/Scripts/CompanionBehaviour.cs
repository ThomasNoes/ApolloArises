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

    static public CompanionBehaviour instance;

    public DialogReader dr;
    public CompanionPathFinding cpf;
    MapManager mm;
    List<MapGenerator> maps;
    GameObject player;
    BeaconManager bm;


    public bool isFollowPlayer;

    Tile startTile;
    Tile endtile;


    public DialogData goToGenDialog;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {


        dr = GetComponent<DialogReader>();
        cpf = GetComponent<CompanionPathFinding>();
        mm = GameObject.Find("MapManager").GetComponent<MapManager>();
        maps = mm.mapScripts;
        player = Camera.main.gameObject;
        bm = GameObject.Find("BeaconManager").GetComponent<BeaconManager>();

        startTile = FindStartEndTile(maps[0]);
        endtile = FindStartEndTile(maps[maps.Count - 1]);

        Invoke("LateStart",0); //wait a frame
    }

    void LateStart()
    {
        cpf.PlaceCompanionOnTile(startTile);
    }


    // Update is called once per frame
    void Update()
    {
        //movement companion
        isFollowPlayer = UpdateIsFollowPlayer();
        if (isFollowPlayer)
        {
            cpf.FollowPlayer();
        }
    }


    public bool UpdateIsFollowPlayer()
    {
        Tile playerTile = cpf.GetTileUnderObject(player);
        if (bm.beacons.Count != 0)
        {
            if (!bm.beacons[playerTile.partOfMaze].isActive)
            {
                return false;
            }
            if (bm.beacons[bm.beacons.Count-1].isActive)
            {
                return false;
            }
        }
        if (ReferenceEquals(playerTile, null))
        {
            return false;
        }
        return true;
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

    public void OnLeverPulled(int mazeIndex)
    {

    }
    public void OnWrongLeverPulled()
    {

    }
    public void OnAntiCheatDoor()
    {

    }
    public void OnAntiCheatWall()
    {

    }
    public void OnPickUpKey()
    {

    }
    public void OnPickUpCogWheel()
    {

    }
    public void OnTeleportToTower(int IndexForDestinationTower)
    {

    }
    public void OnOpenDoor()
    {

    }

    //end event
    public void GoToGenerator()
    {
        Debug.Log("gotogenerator");
        cpf.GoToSpecificTile(endtile);
        dr.InjectDialog(goToGenDialog);
    }
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
