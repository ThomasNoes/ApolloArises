using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TextureCustomizer : MonoBehaviour
{
    public GameObject floorPrefab, wallPrefab, pillarPrefab, ceilingPrefab;

    [HideInInspector] public List<GameObject> newFloorPrefabs, newWallPrefabs, newPillarPrefabs, newCeilingPrefabs;

    public Material[] pillarMats, wallMats, floorMats, ceilingMats;

    public bool autoSwitchTextures;
    [HideInInspector] public int frequency = 10, matIndex = 0;

    public void UpdateTextures()
    {
        UpdateTextures(0);
    }

    public void UpdateTextures(int index) // TODO: figure out if you can update a prefab when APK is built?
    {
        if (floorPrefab != null)
        {
            if (floorMats != null)
            {
                if (floorMats.Length > index)
                {
                    floorPrefab.GetComponent<Renderer>().material = floorMats[index];
                }
            }
        }

        if (wallPrefab != null)
        {
            if (wallMats != null)
            {
                if (wallMats.Length > index)
                {
                    wallPrefab.gameObject.transform.GetChild(0).GetComponent<Renderer>().material = wallMats[index];
                }
            }
        }

        if (pillarPrefab != null)
        {
            if (pillarMats != null)
            {
                if (pillarMats.Length > index)
                {
                    pillarPrefab.GetComponent<Renderer>().material = pillarMats[index];
                }
            }
        }

        if (ceilingPrefab != null)
        {
            if (ceilingMats != null)
            {
                if (ceilingMats.Length > index)
                {
                    ceilingPrefab.GetComponent<Renderer>().material = ceilingMats[index];
                }
            }
        }
    }

    public void GenerateNewPrefabs() // TODO: Testing - not done yet - Not working as intended as prefabs cannot be generated to memory
    {
        if (wallMats == null || wallPrefab == null)
            return;

        for (int i = 0; i < wallMats.Length; i++)
        {
            GameObject tempObj = wallPrefab.gameObject;
            tempObj.GetComponent<Renderer>().material = wallMats[i];
            newWallPrefabs.Add(tempObj);

            if (floorMats.Length > i)
            {
                if (floorPrefab != null)
                {
                    tempObj = floorPrefab.gameObject;
                    tempObj.GetComponent<Renderer>().material = floorMats[i];
                    newFloorPrefabs.Add(tempObj);
                }
            }

            if (pillarMats.Length > i)
            {
                if (pillarPrefab != null)
                {
                    tempObj = pillarPrefab.gameObject;
                    tempObj.GetComponent<Renderer>().material = pillarMats[i];
                    newPillarPrefabs.Add(tempObj);
                }
            }

            if (ceilingMats.Length > i)
            {
                if (ceilingPrefab != null)
                {
                    tempObj = ceilingPrefab.gameObject;
                    tempObj.GetComponent<Renderer>().material = ceilingMats[i];
                    newCeilingPrefabs.Add(tempObj);
                }
            }
        }
    }

    /// <summary>
    /// Gives you the current material array length
    /// </summary>
    /// <param name="type">Type 0: pillar, type 1: wall, type 2: floor, type 3: ceiling</param>
    public int getMaterialArrayLength(int type)
    {
        switch (type)
        {
            case 0:
                if (pillarMats != null)
                    return pillarMats.Length;
                else
                    return 0;
            case 1:
                if (wallMats != null)
                    return wallMats.Length;
                else
                    return 0;
            case 2:
                if (floorMats != null)
                    return floorMats.Length;
                else
                    return 0;
            case 3:
                if (ceilingMats != null)
                    return ceilingMats.Length;
                else
                    return 0;
            default:
                return -1;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TextureCustomizer))]
public class TextureCustomizer_Editor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var script = target as TextureCustomizer;

        if (script.autoSwitchTextures)
        {
            script.frequency = EditorGUILayout.IntField("Frequency:", script.frequency);
        }

        EditorGUILayout.Space(10.0f);

        if (GUILayout.Button("[Not Working] Generate Prefabs"))
        {
            script.GenerateNewPrefabs();
            Debug.Log("TextureCustomizer: generated new prefabs");
        }

        EditorGUILayout.Space(10.0f);

        script.matIndex = EditorGUILayout.IntField("Material Index", script.matIndex);

        if (GUILayout.Button("Change Textures"))
        {
            script.UpdateTextures(script.matIndex);
            Debug.Log("TextureCustomizer: called texture switcher");
        }
    }
}
#endif
