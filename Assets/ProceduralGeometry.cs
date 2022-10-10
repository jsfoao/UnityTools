using System;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeometry : MonoBehaviour
{
    [Range(0.01f, 1f)]
    public float radiusInner = 0.5f;
    [Range(0.01f, 2f)]
    public float thickness = 0.5f;
    [Range(3,32)]
    public int angularSegments = 3;

    private float RadiusOuter => radiusInner + thickness;

    private void Awake()
    {
        Mesh mesh = new Mesh();
    }

    private void GenerateQuad()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Procedural Quad";
 
        // Points (vertices) defining quad
        List<Vector3> points = new List<Vector3>()
        {
            new Vector3(-1,1),
            new Vector3(1,1),
            new Vector3(-1,-1),
            new Vector3(1,-1)
        };

        // Triangle indices defining triangles that compose quad
        int[] triIndices = new int[]
        {
            1,0,2,
            3,1,2
        };

        List<Vector2> uvs = new List<Vector2>()
        {
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(0,0),
            new Vector2(1,0)
        };

        List<Vector3> normals = new List<Vector3>()
        {
            new Vector3(0,0,1),
            new Vector3(0,0,1),
            new Vector3(0,0,1),
            new Vector3(0,0,1)
        };

        // Creating mesh defined by previous mesh data
        mesh.SetVertices(points);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.triangles = triIndices;

        // Auto generate normals: Good for most cases
        // mesh.RecalculateNormals();

        // Assigning created mesh to mesh filter
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private void OnDrawGizmos()
    {
        JGizmos.DrawWireCircle(transform.position, transform.rotation, radiusInner, angularSegments);
        JGizmos.DrawWireCircle(transform.position, transform.rotation, RadiusOuter, angularSegments);
    }
}