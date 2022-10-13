using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class WVertex : MonoBehaviour
{
    public WGraph Owner;
    public Vector3 Position;
    public HashSet<WEdge> Edges;
    public Quaternion Rotation;

    public void Init(WGraph owner = null)
    {
        Position = Vector3.zero;
        Rotation = Quaternion.identity;
        Edges = new HashSet<WEdge>();

        owner?.Bind(this);
    }

    private void Update()
    {
        Position = transform.position;
    }

    public void AddConnection(WVertex vertex)
    {
        WEdge edge = WBuilder.CreateEdge(Owner, this, vertex);

        Owner.Edges.Add(edge);
    }

    public void Render()
    {
        //Handles.SphereHandleCap(0, Position, Quaternion.identity, 1f, EventType.Repaint);
    }
}
