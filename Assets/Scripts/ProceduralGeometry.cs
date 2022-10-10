using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ProceduralGeometry : MonoBehaviour
{
    [Range(0.01f, 1f)]
    public float radiusInner = 0.5f;
    [Range(0.01f, 2f)]
    public float thickness = 0.5f;
    [Range(3,32)]
    public int angularSegments = 3;

    private float RadiusOuter => radiusInner + thickness;
    private int VertexCount => angularSegments * 2;

    private Mesh mesh;

    private void Awake()
    {
        mesh = new Mesh();
        mesh.name = "QuadRing";
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private void Update()
    {
        GenerateQuadRing();
    }

    private void GenerateQuadRing()
    {
        mesh.Clear();

        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        // Vertices
        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < angularSegments; i++)
        {
            float t = i / (float) angularSegments;
            float angRad = t * JMath.TAU;
            Vector2 dir = JMath.GetUnitVectorByAngle(angRad);
            vertices.Add(dir * RadiusOuter);
            vertices.Add(dir * radiusInner);
            
            normals.Add(Vector3.forward);
            normals.Add(Vector3.forward);
            
            uvs.Add(new Vector2(t, 1));
            uvs.Add(new Vector2(t, 0));
        }
        
        // Triangles
        List<int> triangleIndices = new List<int>();
        for (int i = 0; i < angularSegments; i++)
        {
            int rootIndex = i * 2;
            int indexInnerRoot = rootIndex + 1;
            int indexOuterNext = (rootIndex + 2) % VertexCount;
            int indexInnerNext = (rootIndex + 3) % VertexCount;
            
            triangleIndices.Add(rootIndex);
            triangleIndices.Add(indexOuterNext);
            triangleIndices.Add(indexInnerNext);
            
            triangleIndices.Add(rootIndex);
            triangleIndices.Add(indexInnerNext);
            triangleIndices.Add(indexInnerRoot);
        }
        
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangleIndices, 0);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);

        GetComponent<MeshFilter>().sharedMesh = mesh;
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