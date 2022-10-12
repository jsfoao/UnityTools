using System.Collections.Generic;
using UnityEngine;

public class WGraph
{
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

    public void Add(WVertex vertex)
    {
        Bind(vertex);
    }

    public void Render()
    {
        foreach (WVertex vertex in Vertices)
        {
            Gizmos.DrawSphere(vertex.Position, 0.3f);
        }

        foreach (WEdge edge in Edges)
        {
            Gizmos.DrawLine(edge.Source.Position, edge.Destination.Position);
        }
    }
}
