using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotePadScript : MonoBehaviour {
    // Gameobject of notepad canvas:
    public GameObject notepadCanvasObject;

    // Strings and Sprites:
    public string[] roomNameStrings;
    public string[] clueHintStrings;
    public string[] clueTextStrings;
    public Sprite[] clueNumberSprites;
    // public Sprite textCrossOutSprite;

    // Found notes:
    public GameObject foundNote;

    // UI Objects:
    Text roomNameText;
    Text clueHintText;
    Text clueText;
    Text pageNumber;
    Image clueNumberImage;
    Image crossOutTextImage;

    // Other variables:
    int currentPage = 0;
    int roomsEntered = 1;
    int pageAmount = 0; // not counting 0
    bool[] clueFound;

    void Start () {
        // Find all the necesarry components: 
        roomNameText = notepadCanvasObject.transform.Find ("RoomNameText").GetComponent<Text> ();
        clueHintText = notepadCanvasObject.transform.Find ("ClueHintText").GetComponent<Text> ();
        clueText = notepadCanvasObject.transform.Find ("ClueText").GetComponent<Text> ();
        pageNumber = notepadCanvasObject.transform.Find ("PageNumberText").GetComponent<Text> ();
        clueNumberImage = notepadCanvasObject.transform.Find ("ClueNumberImage").GetComponent<Image> ();
        crossOutTextImage = notepadCanvasObject.transform.Find ("CrossOutImage").GetComponent<Image> ();

        pageAmount = roomNameStrings.Length; // The amount of rooms is determined by the amount of room name strings in the array
        clueFound = new bool[pageAmount]; // Boolean array used to check if the clue was found in the specific room
        foundNote.SetActive (true); // Introduction note disabled/enable from the beginning
        clueNumberImage.gameObject.SetActive (false);

        for (int i = 0; i < pageAmount; i++) {
            clueFound[i] = false;
        }
    }

    public void NotepadEnterNewRoom () {
        if (roomsEntered < pageAmount) {
            currentPage = (currentPage + 1) % pageAmount;
            roomsEntered++;
            FindObjectOfType<AudioManager> ().Play ("NoteUpdateSound");
            SetNotepadToBlank ();

        } else if (roomsEntered != 0) {
            foundNote.SetActive (false);
            currentPage = (currentPage + 1) % roomsEntered;
            Debug.Log ("Current page:" + currentPage);
            SwitchPage (currentPage);
        }

    }

    public void NotepadSwitchPage () {
        Debug.Log ((currentPage + 1) + " " + roomsEntered);
        if ((currentPage + 1) == roomsEntered && foundNote.activeSelf == false) {
            foundNote.SetActive (true);
            SetNotepadToBlank ();

        } else if (roomsEntered != 0) {
            foundNote.SetActive (false);
            currentPage = (currentPage + 1) % roomsEntered;
            Debug.Log ("Current page:" + currentPage);
            SwitchPage (currentPage);
        }

    }

    public void NotepadTriggerUpdatePage () {
        if (clueFound[currentPage] == false) {
            FindObjectOfType<AudioManager> ().Play ("NoteUpdateSound");
            ClueUpdate ();
        } else {
            Debug.Log ("Clue " + currentPage + " already found!");
        }

    }

    private void ClueUpdate () // Used when the clue is found
    {
        clueText.text = clueTextStrings[currentPage];
        clueNumberImage.sprite = clueNumberSprites[currentPage];
        clueNumberImage.gameObject.SetActive (true);
        crossOutTextImage.gameObject.SetActive (true);
        clueFound[currentPage] = true;
    }

    private void SwitchPage (int page) // Method used to switch the pages
    {

        if (clueFound[page] == true) {
            clueText.text = clueTextStrings[page];
            clueNumberImage.sprite = clueNumberSprites[page];
            clueNumberImage.gameObject.SetActive (true);
            crossOutTextImage.gameObject.SetActive (true);
            roomNameText.text = roomNameStrings[page];
            clueHintText.text = clueHintStrings[page];
            pageNumber.text = ((currentPage + 1) + " of " + roomsEntered);
        } else {
            EnterNewRoom (page);
        }

    }

    private void EnterNewRoom (int page) // Method used when the player enters a new room
    {
        if (page < pageAmount) {
            crossOutTextImage.gameObject.SetActive (false);
            clueNumberImage.gameObject.SetActive (false);
            clueText.text = null;
            roomNameText.text = roomNameStrings[page];
            clueHintText.text = clueHintStrings[page];
            pageNumber.text = ((currentPage + 1) + " of " + roomsEntered);
        } else {
            Debug.Log ("Notepad: Page does not exist!");
        }

    }

    private void SetNotepadToBlank () {
        crossOutTextImage.gameObject.SetActive (false);
        clueNumberImage.gameObject.SetActive (false);
        clueText.text = null;
        roomNameText.text = null;
        clueHintText.text = null;
        pageNumber.text = null;
    }

}