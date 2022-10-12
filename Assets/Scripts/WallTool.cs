using System;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[Serializable]
public class WallPoint
{
    public Transform transform;
    public Vector3 normal;
}

[ExecuteInEditMode][RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WallTool : MonoBehaviour
{
    [SerializeField] 
    public WallPoint[] wallPoints;

    [SerializeField]
    public float wallHeight = 1f;

    [SerializeField] 
    public float wallThickness = 0.5f;

    public int VertexCount => wallPoints.Length * 4;
    
    public Mesh mesh;
    private MeshFilter meshFilter;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangleIndices = new List<int>();
        
        

        for (int i = 0; i < wallPoints.Length; i++)
        {
            Vector3 position = wallPoints[i].transform.position;
            Vector3 heightPosition = position + Vector3.up * wallHeight;

            Vector3 normal = Vector3.zero;
            Vector3 averagedNormal;
            
            if (i == 0)
            {
                Vector3 dir = position - wallPoints[i + 1].transform.position;
                Vector3 heightDir = position - heightPosition;
                normal = Vector3.Cross(dir, heightDir).normalized;
                
                wallPoints[i].normal = normal;
                continue;
            }
            
            if (i < wallPoints.Length - 1)
            {
                Vector3 dir = position - wallPoints[i + 1].transform.position;
                Vector3 heightDir = position - heightPosition;
                normal = Vector3.Cross(dir, heightDir).normalized;
                
                averagedNormal = ((normal + wallPoints[i - 1].normal) / 2).normalized;
                wallPoints[i].normal = averagedNormal;
            }

            if (i == wallPoints.Length - 1)
            {
                Vector3 dir = wallPoints[i - 1].transform.position - position;
                Vector3 heightDir = position - heightPosition;
                normal = Vector3.Cross(dir, heightDir).normalized;
                
                wallPoints[i].normal = normal;
                continue;
            }
        }
        
        // FRONT FACE VERTICES
        for (int i = 0; i < wallPoints.Length; i++)
        {
            
            Vector3 position = wallPoints[i].transform.position;
            Vector3 heightPosition = position + Vector3.up * wallHeight;
            
            vertices.Add(position);
            vertices.Add(heightPosition);

            uvs.Add(new Vector2(i, 0));
            uvs.Add(new Vector2(i, 1));
            
            normals.Add(new Vector3(0,0,1));
            normals.Add(new Vector3(0,0,1));
        }
        
        // BACK FACE VERTICES
        for (int i = 0; i < wallPoints.Length; i++)
        {
            
            Vector3 position = wallPoints[i].transform.position - wallPoints[i].normal * wallThickness;
            Vector3 heightPosition = position + Vector3.up * wallHeight;
            
            vertices.Add(position);
            vertices.Add(heightPosition);

            uvs.Add(new Vector2(i, 0));
            uvs.Add(new Vector2(i, 1));
            
            normals.Add(new Vector3(0,0,1));
            normals.Add(new Vector3(0,0, 1));
        }

        // FRONT FACE TRIANGLES
        for (int i = 0; i < wallPoints.Length - 1; i++)
        {
            int indexRoot = i * 2;
            int indexUl = indexRoot + 1;
            int indexDr = indexRoot + 2;
            int indexUr = indexRoot + 3;

            triangleIndices.Add(indexUl);
            triangleIndices.Add(indexRoot);
            triangleIndices.Add(indexDr);
            
            triangleIndices.Add(indexUr);
            triangleIndices.Add(indexUl);
            triangleIndices.Add(indexDr);
        }
        
        // BACK FACE TRIANGLES
        for (int i = wallPoints.Length - 1; i < (wallPoints.Length * 2) - 1; i++)
        {
            int indexRoot = i * 2;
            int indexUl = indexRoot + 1;
            int indexDr = indexRoot + 2;
            int indexUr = indexRoot + 3;

            triangleIndices.Add(indexRoot);
            triangleIndices.Add(indexDr);
            triangleIndices.Add(indexUl);
            
            triangleIndices.Add(indexDr);
            triangleIndices.Add(indexUr);
            triangleIndices.Add(indexUl);
        }

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangleIndices, 0);
        mesh.SetUVs(0, uvs);
        mesh.SetNormals(normals);
        
        meshFilter.sharedMesh = mesh;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < wallPoints.Length; i++)
        {
            Vector3 position = wallPoints[i].transform.position;
            Vector3 normal = wallPoints[i].normal;
            Vector3 heightPosition = position + Vector3.up * wallHeight;
            
            Vector3 offsetPosition = position - normal * wallThickness;
            Vector3 offsetHeightPosition = offsetPosition + Vector3.up * wallHeight;
            
            Gizmos.color = Color.white;
            if (i < wallPoints.Length - 1)
            {
                Vector3 nextPosition = wallPoints[i + 1].transform.position;
                Vector3 nextOffsetPosition = wallPoints[i + 1].transform.position - wallPoints[i + 1].normal * wallThickness;
                
                Gizmos.DrawLine(position, nextPosition);
                Gizmos.DrawLine(heightPosition, nextPosition + Vector3.up * wallHeight);
                Gizmos.DrawLine(offsetPosition, nextOffsetPosition);
                Gizmos.DrawLine(offsetPosition + Vector3.up * wallHeight, nextOffsetPosition + Vector3.up * wallHeight);
            }
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position, position + wallPoints[i].normal * 1f);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(position, heightPosition);
            Gizmos.DrawLine(position, offsetPosition);
            Gizmos.DrawLine(offsetPosition, offsetHeightPosition);
            Gizmos.DrawLine(offsetHeightPosition, heightPosition);
            
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(position, 0.1f);
            Gizmos.DrawSphere(heightPosition, 0.1f);
            
            Gizmos.DrawSphere(offsetPosition, 0.1f);
            Gizmos.DrawSphere(offsetHeightPosition, 0.1f);
        }
    }

    public void Init()
    {
        Debug.Log("Init WallTool");
        mesh = new Mesh();
        mesh.name = "GeneratedWall";
        meshFilter = GetComponent<MeshFilter>();
    }
}