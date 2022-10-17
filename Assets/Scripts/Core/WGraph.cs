using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WGraph
{
    public GameObject Root;
    public List<WVertex> Vertices;
    public HashSet<WEdge> Edges;

    public WGraph()
    {
        Vertices = new List<WVertex>();
        Edges = new HashSet<WEdge>();
    }

    public void Bind(WVertex vertex)
    {
        vertex.Owner = this;
        Vertices.Add(vertex);

        foreach (WEdge edge in vertex.Edges)
        {
            Edges.Add(edge);
        }
    }

    public void Unbind(WVertex vertex)
    {
        vertex.Owner = null;
        Vertices.Remove(vertex);

        foreach (WEdge edge in vertex.Edges)
        {
            if (Edges.Contains(edge))
            {
                Edges.Remove(edge);
            }
        }
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
