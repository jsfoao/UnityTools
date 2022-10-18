using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WGraph
{
    public static WGraph Instance;
    
    public GameObject Root;
    public List<WVertex> Vertices;
    public HashSet<WEdge> Edges;

    public List<GameObject> SpawnedObjects;
    
    public WGraph()
    {
        Instance = this;

        Vertices = new List<WVertex>();
        Edges = new HashSet<WEdge>();
        SpawnedObjects = new List<GameObject>();
    }

    public void AddObject(GameObject go)
    {
        SpawnedObjects.Add(go);
    }

    public void RemoveObject(GameObject go)
    {
        SpawnedObjects.Remove(go);
    }

    public void Connect(WVertex v1, WVertex v2)
    {
        v1.AddEdge(v2);
    }

    public void Render()
    {
        Gizmos.color = Color.white;
        foreach (WEdge edge in Edges)
        {
            edge.Render();
        }
        
        Gizmos.color = Color.red;
        foreach (WVertex vertex in Vertices)
        {
            vertex.Render();
        }
    }
}
