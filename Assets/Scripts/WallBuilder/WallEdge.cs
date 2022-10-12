using UnityEngine;

public class WallEdge : MonoBehaviour
{
    public WallVertex Source;
    public WallVertex Destination;

    public void Init(WallVertex source, WallVertex destination)
    {
        Source = source;
        Destination = destination;
    }
}
