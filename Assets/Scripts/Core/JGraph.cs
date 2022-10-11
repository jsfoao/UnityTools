using System.Collections.Generic;

public class JGraph
{
    public List<JVertex> Vertices;
    public HashSet<JEdge> Edges;

    public JGraph()
    {
        Vertices = new List<JVertex>();
    }

    public void BindVertex(JVertex vertex)
    {
        Vertices.Add(vertex);
    }

    public void UnbindVertex(JVertex vertex)
    {
        vertex.ClearEdges();
        Vertices.Remove(vertex);
    }
}
