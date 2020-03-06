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

    //public GameObject testPrefab;
    //public Material[] testMaterials;
    //private bool testBool = false;

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

        //if (testPrefab != null)
        //{
        //    if (testMaterials != null)
        //    {
        //        if (testMaterials.Length > index)
        //        {
        //            testPrefab.GetComponent<Renderer>().material = testMaterials[index];
        //        }
        //    }
        //}
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

    //private void Update()
    //{
    //    if (OVRInput.GetDown(OVRInput.Button.Two))
    //    {
    //        UpdateTextures(testBool ? 0 : 1);
    //        testBool = !testBool;

    //        Instantiate(testPrefab, Camera.main.transform.position, Quaternion.identity);
    //    }
    //}
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

        script.matIndex = EditorGUILayout.IntField("Material Index", script.matIndex);

        if (GUILayout.Button("Change Textures"))
        {
            script.UpdateTextures(script.matIndex);
            Debug.Log("TextureCustomizer: called texture switcher");
        }
    }
}
#endif