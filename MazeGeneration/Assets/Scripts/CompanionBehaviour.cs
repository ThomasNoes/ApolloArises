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


    // Start is called before the first frame update
    void Start()
    {
        dr = GetComponent<DialogReader>();
        cpf = GetComponent<CompanionPathFinding>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //start 


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
