using UnityEngine;

public class WBuilder
{
    /*
     * Spawns Edge
     */
    public static WEdge InstantiateEdge(WGraph owner, WVertex Source, WVertex Destination)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.hideFlags = HideFlags.HideInInspector;
        go.transform.parent = owner.Root.transform;
        go.transform.position = Source.Position;

        WEdge edge = go.AddComponent<WEdge>();
        edge.Init(Source, Destination);
        
        Source.Edges.Add(edge);
        Destination.Edges.Add(edge);

        return edge;
    }

    /*
     * Spawns Vertex on position
     */
    public static WVertex InstantiateVertex(WGraph owner, Vector3 position)
    {
        // Instantiating and initializing Vertex GameObject
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.hideFlags = HideFlags.HideInInspector;
        go.transform.parent = owner.Root.transform;
        go.transform.position = Vector3.zero;
        go.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
        WVertex vertex = go.AddComponent<WVertex>();

        vertex.Position = position;
        owner.Bind(vertex);
        
        return vertex;
    }

    /*
     * Spawns vertex on Vector.zero
     */
    public static WVertex InstantiateVertex(WGraph owner)
    {
        return InstantiateVertex(owner, Vector3.zero);
    }
}
