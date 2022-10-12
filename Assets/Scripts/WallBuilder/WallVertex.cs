using System.Collections.Generic;
using UnityEngine;

public class WallVertex : MonoBehaviour
{
    public WallManager Owner;
    public HashSet<WallEdge> Edges;

    /*
     * Initialize and bind to owner graph
     */
    public void Init(WallManager owner)
    {
        Owner = owner;
        transform.position = Vector3.zero;
        Edges = new HashSet<WallEdge>();
    }

    /*
    * Initialize with position and bind to owner graph
    */
    public void Init(WallManager owner, Vector3 position)
    {
        Owner = owner;
        transform.position = position;
        Edges = new HashSet<WallEdge>();
    }

    /*
    * Initialize with position and connected edges
    */
    public void Init(WallManager owner, Vector3 position, HashSet<WallEdge> connectedEdges)
    {
        Owner = owner;
        transform.position = position;
        Edges = new HashSet<WallEdge>();
        
        foreach (var edge in connectedEdges)
        {
            Edges.Add(edge);
        }
    }
}
