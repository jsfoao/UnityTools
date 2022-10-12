using System.Collections.Generic;
using UnityEngine;

public class WVertex
{
    public WGraph Owner;
    public Vector3 Position;
    public HashSet<WEdge> Edges;
    
    public WVertex(WGraph owner = null)
    {
        Position = Vector3.zero;
        Edges = new HashSet<WEdge>();

        owner?.Bind(this);
    }
    
    public WVertex(Vector3 position, WGraph owner = null)
    {
        Position = position;
        Edges = new HashSet<WEdge>();

        owner?.Bind(this);
    }

    public void AddConnection(WVertex vertex)
    {
        WEdge edge = new WEdge(this, vertex);
        Owner.Edges.Add(edge);
    }

    public void Render()
    {
        
    }
}
