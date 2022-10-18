using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WBuilder
{
    /*
     * Spawns Edge
     */
    public static WEdge InstantiateEdge(WVertex Source, WVertex Destination)
    {
        // Instantiating GameObject
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.hideFlags = HideFlags.HideInInspector;
        go.transform.parent = WGraph.Instance.Root.transform;
        go.transform.position = Source.Position;

        // Creating and initializing Edge
        WEdge edge = go.AddComponent<WEdge>();
        edge.Init(Source, Destination);
        
        return edge;
    }

    /*
     * Spawns Vertex on position
     */
    public static WVertex InstantiateVertex(Vector3 position, List<WVertex> connections = null)
    {
        // Instantiating GameObject
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.hideFlags = HideFlags.HideInInspector;
        go.transform.parent = WGraph.Instance.Root.transform;
        go.transform.position = position;
        go.transform.localScale = new Vector3(0.5f, 1f, 0.5f);

        // Creating and initializing Vertex
        WVertex vertex = go.AddComponent<WVertex>(); 
        vertex.Init(position, connections);
        
        return vertex;
    }

    /*
     * Spawns vertex on Vector.zero
     */
    public static WVertex InstantiateVertex(WGraph owner)
    {
        return InstantiateVertex(Vector3.zero);
    }
}
