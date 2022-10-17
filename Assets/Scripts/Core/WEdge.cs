using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class WEdge : MonoBehaviour
{
    [SerializeField]
    public WGraph Owner;
    
    public WVertex Source;
    public WVertex Destination;

    public void Init(WVertex source, WVertex destination)
    {
        Source = source;
        Destination = destination;
    }

    private void OnEnable()
    {
        Owner.Edges.Add(this);
    }

    private void OnDisable()
    {
        Source.Edges.Remove(this);
        Destination.Edges.Remove(this);
    }
    
    public void ClearAndDestroy()
    {
        Source.Edges.Remove(this);
        Destination.Edges.Remove(this);

        DestroyImmediate(gameObject);
    }

    public void Render()
    {
        float dist = Mathf.Abs((Source.Position - Destination.Position).magnitude);
        transform.rotation = Quaternion.LookRotation(Source.Position - Destination.Position);
        transform.position = (Source.Position + Destination.Position) / 2;
        
        // TODO: Use procedural mesh generation instead
        transform.localScale = new Vector3(0.2f, 2f, dist);
        
        Handles.DrawAAPolyLine(Source.Position, Destination.Position);
    }
}