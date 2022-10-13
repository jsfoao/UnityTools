using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class WEdge : MonoBehaviour
{
    public WVertex Source;
    public WVertex Destination;

    public void Init(WVertex source, WVertex destination)
    {
        Source = source;
        Destination = destination;
    }

    private void Update()
    {
        Debug.Log("Updating");
        float dist = Mathf.Abs((Source.Position - Destination.Position).magnitude);
        transform.rotation = Quaternion.LookRotation(Source.Position - Destination.Position);
        transform.position = (Source.Position + Destination.Position) / 2;
        transform.localScale = new Vector3(0.2f, 2f, dist);
    }

    public void Render()
    {
        Handles.DrawAAPolyLine(Source.Position, Destination.Position);
    }
}