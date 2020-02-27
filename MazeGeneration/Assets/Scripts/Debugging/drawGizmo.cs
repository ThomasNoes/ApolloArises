using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawGizmo : MonoBehaviour
{
    public bool displayGizmo = true;


    [Header("Costumize Your Gizmo's Appearance")]
    public GizmoType gizmo;
    public Color color = Color.black;
    public Vector3 size = new Vector3(1, 1, 1);

    public enum GizmoType
    {
        WireCube,
        WireSphere,
        Cube,
        Sphere
    }

    void OnDrawGizmosSelected()
    {
        DrawGizmo();
    }
    void OnDrawGizmos()
    {
        Gizmos.color = color;
        DrawGizmo();
    }

    void DrawGizmo()
    {
        if (displayGizmo)
        {
            switch (gizmo)
            {
                case GizmoType.WireCube:
                    Gizmos.DrawWireCube(transform.position, size);
                    break;
                case GizmoType.WireSphere:
                    Gizmos.DrawWireSphere(transform.position, size.magnitude / 2);
                    break;
                case GizmoType.Cube:
                    Gizmos.DrawCube(transform.position, size);
                    break;
                case GizmoType.Sphere:
                    Gizmos.DrawSphere(transform.position, size.magnitude / 2);
                    break;
            }
        }
    }

    public void SetColor(Color color)
    {
        this.color = color;
    }

    public void SetSize(float size)
    {
        this.size = new Vector3(size, size, size);
    }
}
