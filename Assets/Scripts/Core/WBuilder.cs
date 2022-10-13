using UnityEngine;

public class WBuilder
{
    public static WEdge CreateEdge(WGraph owner, WVertex Source, WVertex Destination)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.hideFlags = HideFlags.HideInInspector;
        go.transform.parent = owner.Root.transform;
        go.transform.position = Source.Position;

        WEdge edge = go.AddComponent<WEdge>();
        edge.Init(Source, Destination);

        return edge;
    }

    public static WVertex CreateVertex(WGraph owner, Vector3 position)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.hideFlags = HideFlags.HideInInspector;
        go.transform.parent = owner.Root.transform;
        go.transform.position = Vector3.zero;
        go.transform.localScale = new Vector3(0.5f, 1f, 0.5f);

        WVertex vertex = go.AddComponent<WVertex>();
        vertex.Init(owner);

        return vertex;
    }


    public static WVertex CreateVertex(WGraph owner)
    {
        return CreateVertex(owner, Vector3.zero);
    }
}
