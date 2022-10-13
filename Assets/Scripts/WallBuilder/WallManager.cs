using UnityEngine;
using System;
using System.Collections.Generic;

[ExecuteInEditMode]
public class WallManager : MonoBehaviour
{
    public WGraph graph;
    public List<GameObject> spawnGameobjects;

    public void Init()
    {
        gameObject.hideFlags = HideFlags.HideInHierarchy;

        graph = new WGraph();
        spawnGameobjects = new List<GameObject>();

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        hideFlags = HideFlags.HideInInspector;
        go.name = "Vertex";
        go.AddComponent<WVertex>();
        go.transform.parent = transform;
        spawnGameobjects.Add(go);







        // WVertex vertex2 = new WVertex(new Vector3(10,0,0), graph);
        // WVertex vertex3 = new WVertex(new Vector3(20, 0, 0), graph);

        //vertex.AddConnection(vertex1);
        // vertex.AddConnection(vertex2);
        // vertex1.AddConnection(vertex3);
    }

    public void Clear()
    {
        foreach (GameObject go in spawnGameobjects)
        {
            DestroyImmediate(go);
        }
    }

    private void OnEnable()
    {
        Init();
    }

    private void OnDisable()
    {
        graph.Vertices.Clear();
        graph.Edges.Clear();
        Clear();
    }

    private void OnDrawGizmos()
    {
        graph.Render();
        Debug.Log("VertexCount: " + graph.Vertices.Count);
        Debug.Log("EdgeCount: " + graph.Edges.Count);
    }
}