using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class WVertex : MonoBehaviour
{
    [SerializeField]
    public WGraph Owner;
    
    [SerializeField]
    public Vector3 Position;
    public HashSet<WEdge> Edges;
    public Quaternion Rotation;

    public void Init(WGraph owner = null)
    {
        Edges = new HashSet<WEdge>();
        owner?.Bind(this);
    }

    private void OnEnable()
    {
        Init();
    }

    private void OnDisable()
    {
        
        RemoveAllEdges();
        Owner.Vertices.Remove(this);
    }

    private void Update()
    {
        Position = transform.position;
    }

    public void AddEdge(WVertex vertex)
    {
        WEdge edge = WBuilder.InstantiateEdge(Owner, this, vertex);

        Owner.Edges.Add(edge);
        vertex.Edges.Add(edge);
    }

    public void RemoveEdge(WVertex vertex)
    {
        WEdge edge = GetEdge(vertex);
        if (edge != null)
        {
            edge.ClearAndDestroy();
        }
    }

    public void RemoveAllEdges()
    {
        List<WEdge> toDestroy = new List<WEdge>();
        foreach (var edge in Edges)
        {
            toDestroy.Add(edge);
        }

        foreach (var edge in toDestroy)
        {
            edge.ClearAndDestroy();
            Owner.Edges.Remove(edge);
        }
    }

    public WEdge GetEdge(WVertex vertex)
    {
        foreach (var edge in Edges)
        {
            if (edge.Source == this && edge.Destination == vertex ||
                edge.Source == vertex && edge.Destination == this)
            {
                return edge;
            }
        }
        return null;
    }

    public void Render()
    {
        Handles.SphereHandleCap(0, Position, Quaternion.identity, 1f, EventType.Repaint);
    }
}
