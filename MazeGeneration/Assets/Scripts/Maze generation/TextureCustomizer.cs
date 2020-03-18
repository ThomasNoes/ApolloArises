using UnityEditor;
using UnityEngine;

public class TextureCustomizer : MonoBehaviour
{
    //public GameObject floorPrefab, wallPrefab, pillarPrefab, ceilingPrefab;
    public Material floorMaterial, wallMaterial, pillarMaterial, ceilingMaterial, towerMaterial;

    public Material[] floorMats, wallMats, pillarMats, ceilingMats, towerMats;

    public bool autoSwitchTextures = true;
    [HideInInspector] public int frequency = 10, matIndex = -1;

    //public GameObject testPrefab;
    //public Material[] testMaterials;
    //private bool testBool = false;

    public void UpdateTextures()
    {
        UpdateTextures(0);
    }

    public void UpdateTextures(int index) // TODO: figure out if you can update a prefab when APK is built?
    {
        if (floorMaterial != null)
        {
            if (floorMats != null)
            {
                if (floorMats.Length > index)
                {
                    floorMaterial.SetTexture(1, floorMats[index].mainTexture);
                }
            }
        }

        if (wallMaterial != null)
        {
            if (wallMats != null)
            {
                if (wallMats.Length > index)
                {
                    wallMaterial.SetTexture(1, wallMats[index].mainTexture);
                }
            }
        }

        if (pillarMaterial != null)
        {
            if (pillarMats != null)
            {
                if (pillarMats.Length > index)
                {
                    pillarMaterial.SetTexture(1, pillarMats[index].mainTexture);
                }
            }
        }

        if (ceilingMaterial != null)
        {
            if (ceilingMats != null)
            {
                if (ceilingMats.Length > index)
                {
                   ceilingMaterial.SetTexture(1, ceilingMats[index].mainTexture);
                }
            }
        }

        if (towerMaterial != null)
        {
            if (towerMats != null)
            {
                if (towerMats.Length > index)
                {
                    towerMaterial.SetTexture(1, towerMats[index].mainTexture);
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

        script.matIndex = EditorGUILayout.IntField("Current material index", script.matIndex);

        if (GUILayout.Button("Change Textures"))
        {
            script.UpdateTextures(script.matIndex);
            Debug.Log("TextureCustomizer: called texture switcher");
        }
    }
}
#endif