#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioManager))]
public class AudioManager_Inspector : UnityEditor.Editor
{
    private GUIStyle headerStyle;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Audio Manager by Jakob Husted", MessageType.None);

        var script = target as AudioManager;

        DrawDefaultInspector(); // for other non-HideInInspector fields

        if (script.soundEvents != null)
            for (int i = 0; i < script.soundEvents.Length; i++)
            {
                DrawUILine(false);

                SerializedProperty triggerEventProp = serializedObject.FindProperty("soundEvents.Array.data[" + i + "].triggerEvent");
                EditorGUILayout.PropertyField(triggerEventProp);

                SerializedProperty audioClipProp = serializedObject.FindProperty("soundEvents.Array.data[" + i + "].audioClip");
                EditorGUILayout.PropertyField(audioClipProp);

                script.soundEvents[i].runOnce = EditorGUILayout.Toggle("Run Once?", script.soundEvents[i].runOnce);
                script.soundEvents[i].runOnStart = EditorGUILayout.Toggle("Run On Start?", script.soundEvents[i].runOnStart);
                script.soundEvents[i].loopSound = EditorGUILayout.Toggle("Loop Sound?", script.soundEvents[i].loopSound);

                script.soundEvents[i].triggerDelay =
                    EditorGUILayout.FloatField("Trigger Delay", script.soundEvents[i].triggerDelay);

                DrawUILine(false);

                serializedObject.ApplyModifiedProperties();

                if (GUI.changed)
                {
                    EditorUtility.SetDirty(script);
                }
            }
    }

    #region DrawUILine function
    public static void DrawUILine(bool start)
    {
        Color color = new Color(1, 1, 1, 0.3f);
        int thickness = 1;
        if (start)
            thickness = 7;
        int padding = 8;

        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }
    #endregion
}
#endif