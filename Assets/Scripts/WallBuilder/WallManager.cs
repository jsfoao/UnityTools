using UnityEngine;

[ExecuteInEditMode]
public class WallManager : MonoBehaviour
{
    public WGraph graph;

    public void Init()
    {
        graph = new WGraph();
        WVertex vertex = new WVertex(Vector3.zero, graph);
        WVertex vertex1 = new WVertex(Vector3.zero, graph);
        // WVertex vertex2 = new WVertex(new Vector3(10,0,0), graph);
        // WVertex vertex3 = new WVertex(new Vector3(20, 0, 0), graph);

        vertex.AddConnection(vertex1);
        // vertex.AddConnection(vertex2);
        // vertex1.AddConnection(vertex3);
    }

    private void OnEnable()
    {
        Init();
    }

    private void OnDisable()
    {
        graph.Vertices.Clear();
        graph.Edges.Clear();
    }

    private void OnDrawGizmos()
    {
        graph.Render();
        Debug.Log("VertexCount: " + graph.Vertices.Count);
        Debug.Log("EdgeCount: " + graph.Edges.Count);
        Debug.Log("Pos: " + graph.Vertices[0].Position);
    }
}