using UnityEditor;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Mesh[] meshes;
    public Material[] materials;
    [HideInInspector] public int currentMeshIndex, currentMaterialIndex;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private BoxCollider collider;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        collider = GetComponent<BoxCollider>();
    }

    public void SetMesh(int index)
    {
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();

        if (index < meshes.Length && index >= 0)
            meshFilter.sharedMesh = meshes[index];
        else
            return;

        if (index == 2)
        {
            if (collider == null)
                collider = GetComponent<BoxCollider>();

            // collider.enabled = false;

            collider.center = meshes[index].bounds.center;
            collider.size = meshes[index].bounds.size;
            
        }
    }

    public void SetMaterial(int index)
    {
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();

        if (index < materials.Length && index >= 0)
            meshRenderer.material = materials[index];
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Wall))]
public class Wall_Editor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var script = target as Wall;

        if (script.meshes != null)
            script.currentMeshIndex = EditorGUILayout.IntSlider("Mesh:", script.currentMeshIndex, 0, script.meshes.Length - 1);

        if (GUILayout.Button("Change Mesh"))
        {
            script.SetMesh(script.currentMeshIndex);
        }

        if (script.materials != null)
            script.currentMaterialIndex = EditorGUILayout.IntSlider("Material:", script.currentMaterialIndex, 0, script.materials.Length - 1);

        if (GUILayout.Button("Change Material"))
        {
            script.SetMaterial(script.currentMaterialIndex);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
        }
    }
}
#endif
