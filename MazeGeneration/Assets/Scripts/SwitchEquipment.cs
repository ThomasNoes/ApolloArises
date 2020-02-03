using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SwitchEquipment : MonoBehaviour {
    public NotePadScript notePadScript;
    //public string inputKey;
    GameObject equipmentList;
    public List<GameObject> equipments;
    Transform clipBoardPos;
    Transform timeDevicePos;
    public SteamVR_Action_Boolean triggerClick;
    public SteamVR_Action_Boolean touchPadClick;
    public SteamVR_Input_Sources inputSource;

    public SwitchEquipment otherSwitcher;

    int currentEquipmentIndex;

    public string currentEquipmentName;

    // Start is called before the first frame update
    void Start () {
        //hand = GetComponent<Hand>();

        //get equipment from equipment list
        equipmentList = GameObject.Find ("Equipment List");

        if (gameObject.name == "LeftHand") {
            AddEquipment ("EmptyLeft");
            SwitchToSpecificEquipment ("EmptyLeft");
        } else if (gameObject.name == "RightHand") {
            AddEquipment ("EmptyRight");
            SwitchToSpecificEquipment ("EmptyRight");
        }
        AddEquipment ("TimeDevice");
        AddEquipment ("ClipBoard");

        //GetAllEquipment();
        GetEquipmentPositions (); // check if it is needed

        SwitchToSpecificEquipment ("TimeDevice");
    }
    void Update () {

        if (Input.GetKeyDown ("m") && gameObject.name == "LeftHand") {
            AddEquipment ("TimeDevice");
            SwitchToSpecificEquipment ("TimeDevice");
        }

        //if (hand.controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && currentEquipmentName == "NoteBook")
        if (triggerClick.GetStateDown (inputSource) && currentEquipmentName == "ClipBoard") {
            Debug.Log ("erere");
            FindObjectOfType<AudioManager> ().Play ("NoteSwitchSound");
            notePadScript.NotepadSwitchPage ();
        }
        if (touchPadClick.GetStateDown (inputSource)) {
            Debug.Log ("switch equipment");
            SwitchToNextEquipment ();
        }
        positionEquipment (equipments[currentEquipmentIndex]);
    }

    void GetAllEquipment () {
        equipmentList = GameObject.Find ("Equipment List");
        for (int i = 0; i < equipmentList.transform.childCount; i++) {
            equipments.Add (equipmentList.transform.GetChild (i).gameObject);
        }
    }

    void AddEquipment (string equipmentName) {
        for (int i = 0; i < equipmentList.transform.childCount; i++) {
            if (equipmentList.transform.GetChild (i).name == equipmentName) {
                equipments.Add (equipmentList.transform.GetChild (i).gameObject);
            }
        }
        SwitchToSpecificEquipment (equipmentName);

    }

    void GetEquipmentPositions () {
        for (int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild (i);
            if (child.name == "ClipBoardPos") {
                clipBoardPos = child;
            } else if (child.name == "TimeDevicePos") {
                timeDevicePos = child;
            }
        }
    }

    void SwitchToNextEquipment () {
        //set the currentequipment to be the next in the order by incrementing the index
        currentEquipmentIndex++;

        //set currentEquipment to index 0
        if (currentEquipmentIndex > equipments.Count - 1) {
            currentEquipmentIndex = 0;
        }

        if (otherSwitcher.currentEquipmentName == equipments[currentEquipmentIndex].name // check if the other hand is holding the item you want to switch to, if so skip to the next item again.
            &&
            otherSwitcher.currentEquipmentName != "EmptyHand") //allow the player to hand to empty hands
        {
            currentEquipmentIndex++;
        }

        //set currentEquipment to index 0
        if (currentEquipmentIndex > equipments.Count - 1) {
            currentEquipmentIndex = 0;
        }

        ShowOnlyCurrentEquipment (currentEquipmentIndex);

    }

    void SwitchToSpecificEquipment (string name) {
        for (int i = 0; i < equipments.Count; i++) {

            if (equipments[i].name == name) {
                currentEquipmentIndex = i;
                break;
            }
        }
        ShowOnlyCurrentEquipment (currentEquipmentIndex);
    }

    void ShowOnlyCurrentEquipment (int currentEquipment) {
        for (int i = 0; i < equipments.Count; i++) {
            if (i == currentEquipment) //turn on the equipment that is player has in hand
            {
                equipments[i].SetActive (true);
                currentEquipmentName = equipments[i].name;
                //positionEquipment(equipments[i]);
            } else if (equipments[i].name != otherSwitcher.currentEquipmentName) //you should not set the equipment on the other hand off
            {
                //Debug.Log(currentEquipmentName);
                //Debug.Log(otherSwitcher.currentEquipmentName);
                equipments[i].SetActive (false);
            }
        }
    }

    void positionEquipment (GameObject equipment) {
        if (equipment.name == "ClipBoard") {
            equipment.transform.SetPositionAndRotation (clipBoardPos.position, clipBoardPos.rotation);
        } else if (equipment.name == "TimeDevice") {
            equipment.transform.SetPositionAndRotation (timeDevicePos.position, timeDevicePos.rotation);
        }
    }

}