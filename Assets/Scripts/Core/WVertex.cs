using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class WVertex : MonoBehaviour
{
    [SerializeField]
    public Vector3 Position;
    public HashSet<WEdge> Edges;
    public Quaternion Rotation;

    public void Init(Vector3 position, List<WVertex> connections = null)
    {
        Position = position;
        Edges = new HashSet<WEdge>();
        WGraph.Instance.Vertices.Add(this);

        if (connections == null)
            return;

        foreach (WVertex vertexConnection in connections)
        {
            AddEdge(vertexConnection);
        }
    }

    private void OnEnable()
    {
        Init(transform.position);
    }

    private void OnDisable()
    {
        RemoveAllEdges();
        WGraph.Instance.Vertices.Remove(this);
    }

    private void Update()
    {
        Position = transform.position;
    }

    public void AddEdge(WVertex vertex)
    {
        WEdge edge = WBuilder.InstantiateEdge(this, vertex);

        WGraph.Instance.Edges.Add(edge);
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
            WGraph.Instance.Edges.Remove(edge);
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
    }
}
