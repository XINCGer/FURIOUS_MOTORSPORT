using UnityEngine;
using System.Collections;

public class GizmoObject : MonoBehaviour
{

    public Color GizmoColor = Color.white;
    public float GizmoSize = 1.0f;

    void OnDrawGizmos()
    {
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 2.0f;

        Gizmos.color = GizmoColor;
        Gizmos.DrawRay(transform.position, direction);

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one * GizmoSize);
        Gizmos.matrix = rotationMatrix;

        Gizmos.DrawCube(Vector3.zero, Vector3.one * GizmoSize);
    }
}
