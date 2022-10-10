using System;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class RoadSegment : MonoBehaviour
{
    [SerializeField]
    public Mesh2D shape2D;
    
    [SerializeField]
    private Transform[] controlPoints = new Transform[4];

    [Range(0, 1)][SerializeField] 
    private float t = 0f;
    Vector3 GetPos(int i) => controlPoints[i].position;

    private Vector3 GetBezierPoint(float t)
    {
        Vector3 p0 = GetPos(0);
        Vector3 p1 = GetPos(1);
        Vector3 p2 = GetPos(2);
        Vector3 p3 = GetPos(3);

        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        Vector3 c = Vector3.Lerp(p2, p3, t);
        
        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);
        
        return Vector3.Lerp(d, e, t);
    }

    private OrientedPoint GetBezierOrientedPoint(float t)
    {
        Vector3 p0 = GetPos(0);
        Vector3 p1 = GetPos(1);
        Vector3 p2 = GetPos(2);
        Vector3 p3 = GetPos(3);

        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        Vector3 c = Vector3.Lerp(p2, p3, t);
        
        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);

        Vector3 position = Vector3.Lerp(d, e, t);
        Vector3 tangent = (e - d).normalized;
        
        return new OrientedPoint(position, tangent);
    }


    private Vector3 GetBezierTangent(float t)
    {
        Vector3 p0 = GetPos(0);
        Vector3 p1 = GetPos(1);
        Vector3 p2 = GetPos(2);
        Vector3 p3 = GetPos(3);

        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        Vector3 c = Vector3.Lerp(p2, p3, t);
        
        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);

        return (e - d).normalized;
    }
    
    private Quaternion GetBezierRotation(float t)
    {
        Vector3 tangent = GetBezierTangent(t);
        return Quaternion.LookRotation(tangent);
    }
    public void OnDrawGizmos()
    {
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawSphere(GetPos(i), 0.1f);
        }

        Handles.DrawBezier
        (
            GetPos(0), 
            GetPos(3), 
            GetPos(1), 
            GetPos(2),
            Color.white,
            EditorGUIUtility.whiteTexture,
            1f
        );

        OrientedPoint testPoint = GetBezierOrientedPoint(t);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(testPoint.position, 0.1f);
        Handles.PositionHandle(testPoint.position, testPoint.rotation);

        for (int i = 0; i < shape2D.vertices.Length; i++)
        {
            Gizmos.DrawSphere(testPoint.LocalToWorld(shape2D.vertices[i].point), 0.1f);
        }
    }
}