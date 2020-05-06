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


    [HideInInspector]
    public DialogReader dr;
    [HideInInspector]
    public CompanionPathFinding cpf;

    public GameObject controlPanel;

    MapManager mm;
    List<MapGenerator> maps;
    GameObject player;
    BeaconManager bm;
    DayNightController dnc;
    ItemSpawner itemSpawner;

    public bool isFollowPlayer;

    Tile startTile;
    Tile endtile;

    public DialogData openDoor; //
    public DialogData openFirstDoor; //
    public DialogData pickUpCogwheel; //
    public DialogData pickUpKey; //
    public DialogData wrongLever; //
    public DialogData defaultLever; //
    public DialogData secondLastLever; //
    public DialogData lastLever; //
    public DialogData throughDoor; //
    public DialogData throughWall; //
    public DialogData walkToNewTower; // correct

    bool firstDoor = true, keySpawned;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {

        dnc = FindObjectOfType<DayNightController>();
        dr = GetComponent<DialogReader>();
        cpf = GetComponent<CompanionPathFinding>();
        mm = GameObject.Find("MapManager").GetComponent<MapManager>();
        maps = mm.mapScripts;
        player = Camera.main.gameObject;
        bm = GameObject.Find("BeaconManager").GetComponent<BeaconManager>();
        itemSpawner = FindObjectOfType<ItemSpawner>();

        startTile = FindStartEndTile(maps[0]);
        endtile = FindStartEndTile(maps[maps.Count - 1]);




        Invoke("LateStart", 0); //wait a frame
    }

    void LateStart()
    {
        cpf.PlaceCompanionOnTile(startTile);
        InvokeRepeating("MyUpdate", 2.0f, 1f);
        dr.DisplayAllBranchedDialog();

        Debug.Log("0th maze pos " + maps[0].transform.position);
        Debug.Log("0th maze rot " + maps[0].transform.forward);
        Debug.Log("player pos " + player.transform.position);

        ControlPanelSetup();
    }


    // Update is called once per frame
    void MyUpdate()
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
        if (ReferenceEquals(playerTile, null))
        {
            return false;
        }
        if (bm.beacons.Count != 0)
        {
            if (!bm.beacons[playerTile.partOfMaze].isActive)
            {
                return false;
            }
            if (bm.beacons[bm.beacons.Count - 1].isActive)
            {
                return false;
            }
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
            if (!t.isPortalTile && t.isRoomTile)
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

    public void OnLeverPulledAtIndex(int mazeIndex)
    {
        if (mazeIndex == maps.Count-1) // last maze segment
        {
            cpf.GoToSpecificTile(endtile);
            dr.InjectDialog(lastLever);
        }
        else if (mazeIndex == maps.Count - 2) // second last
        {
            dr.InjectDialog(secondLastLever);
        }
        else if (mazeIndex == 0)
        {
            dr.DisplayAllMainDialog();
        }
        else if(mazeIndex >0)
        {
            dr.InjectDialog(defaultLever);
        }
    }
    public void OnWrongLeverPulled()
    {
        dr.InjectDialog(wrongLever);
    }
    public void OnAntiCheatDoor()
    {
        dr.InjectDialog(throughDoor);
    }
    public void OnAntiCheatWall()
    {
        dr.InjectDialog(throughWall);
    }
    public void OnPickUpKey()
    {
        dr.InjectDialog(pickUpKey);
    }
    public void OnPickUpCogWheel()
    {
        dr.InjectDialog(pickUpCogwheel);
    }
    public void OnTeleportToTower(int IndexForDestinationTower)
    {
        if (bm.beacons.Count != 0)
        {
            if (!bm.beacons[IndexForDestinationTower].isActive)
            {
                dr.InjectDialog(walkToNewTower);
            }
        }
    }
    public void OnOpenDoor()
    {
        if (firstDoor)
        {
            dr.InjectDialog(openFirstDoor);
                //public DialogData openFirstDoor;
            firstDoor = false;
        }
        else
        {
            Debug.Log("");
            dr.InjectDialog(openDoor);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "ControlPanel")
        {
            //play sound of booting control panel
            Invoke("SkyEvent", 3);

        }
    }

    private void SkyEvent()
    {
        dnc.StartSkyboxAndTintChange(true);
    }

    private void ControlPanelSetup()
    {
        controlPanel.transform.rotation = mm.transform.rotation;
        controlPanel.transform.position = new Vector3(endtile.transform.position.x, 26.26f, endtile.transform.position.z);
    }

    public void SpawnKeyForFirstDoor()
    {
        if (keySpawned || itemSpawner == null)
            return;

        keySpawned = true;
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);

        GameObject tempKey = Instantiate(itemSpawner.keyPrefab, spawnPos, Quaternion.identity);

        Key key = tempKey.GetComponent<Key>();

        if (key != null)
        {
            key.colourMaterial = itemSpawner.GetMaterialFromId(0);
            key.uniqueId = 1;
        }

        Debug.Log("First key spawned!");
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
