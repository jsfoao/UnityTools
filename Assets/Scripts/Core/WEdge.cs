using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class WEdge : MonoBehaviour
{
    public WVertex Source;
    public WVertex Destination;

    private delegate void Initialize();

    private Initialize initialize;

    public bool isInitialized;

    public void Init(WVertex source, WVertex destination, WGraph owner = null)
    {
        Source = source;
        Destination = destination;

        Source.Edges.Add(this);
        Destination.Edges.Add(this);

        initialize += OnInitialize;
        initialize.Invoke();
    }

    private void OnInitialize()
    {
        isInitialized = true;
        Debug.Log("Initialized Edge");
    }

    private void OnEnable()
    {
        isInitialized = false;
        Debug.Log("Enabled Edge");
    }

    private void OnDisable()
    {
        WGraph.Instance.Edges.Remove(this);
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