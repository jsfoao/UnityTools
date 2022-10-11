using System.Collections.Generic;
using System.Numerics;

public class JVertex
{
    public Vector3 Position;
    public HashSet<JEdge> Edges;

    public JVertex(Vector3 position)
    {
        Position = position;
        Edges = new HashSet<JEdge>();
    }

    public void ClearEdges()
    {
        
    }
}
