using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class WallBuilderTool : EditorWindow
{
    private SerializedObject so;
    private WallManager targetType;

    public WGraph graph;
    public List<GameObject> spawnedObjects;

    public enum SelectionType
    {
        Vertex, Edge, Multiple, None
    }

    [MenuItem("Tools/WallBuilderTool")]
    public static void Open()
    {
        GetWindow<WallBuilderTool>("WalllBuilderTool");
    }

    private void OnEnable()
    {
        Selection.selectionChanged += Repaint;
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= Repaint;
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void DuringSceneGUI(SceneView sceneView)
    {
        Repaint();
        if (graph == null)
            return;

        bool holdingCtrl = (Event.current.modifiers & EventModifiers.Control) != 0;
        if (Event.current.keyCode == KeyCode.E 
            && Event.current.type == EventType.KeyDown 
            && EvaluateSelection() == SelectionType.Vertex)
        {
            WVertex vertex = Selection.activeGameObject.GetComponent<WVertex>();
            WVertex newVertex = WBuilder.InstantiateVertex(vertex.Position);
            vertex.AddEdge(newVertex);
            Selection.activeGameObject = newVertex.gameObject;
            Debug.Log("Added connected vertex");
            Event.current.Use();
        }

        if (Event.current.keyCode == KeyCode.J 
            && Event.current.type == EventType.KeyDown &&
            holdingCtrl)
        {
            WVertex v1 = Selection.gameObjects[0].GetComponent<WVertex>();
            WVertex v2 = Selection.gameObjects[1].GetComponent<WVertex>();
            v1.AddEdge(v2);
            Debug.Log("Joined Vertices");
        }
        graph.Render();
    }
    
    private void OnGUI()
    {
        SelectionType selectionType = EvaluateSelection();

        switch (selectionType)
        {
            case SelectionType.Vertex:
                OnGUI_Vertex();
                break;
            case SelectionType.Edge:
                OnGUI_Edge();
                break;
            case SelectionType.Multiple:
                OnGUI_Multiple();
                break;
            case SelectionType.None:
                OnGUI_None();
                break;
            default:
                break;
        }

        GUILayout.Space(10);
        if (GUILayout.Button("New Graph"))
        {
            InitGraph();
            Debug.Log("Created new graph");
        }
        if (GUILayout.Button("Clear Builder"))
        {
            ClearAll();
            Debug.Log("Cleared spawned objects");
        }
        if (GUILayout.Button("Clear Scene"))
        {
            ClearScene();
            Debug.Log("Cleared all objects in scene");
        }
    }

    private void OnGUI_None()
    {
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Selection", EditorStyles.boldLabel);
        }
    }
    
    private void OnGUI_Multiple()
    {
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Selection", EditorStyles.boldLabel);
        }
    }

    private void OnGUI_Vertex()
    {
        WVertex vertex = Selection.activeGameObject.GetComponent<WVertex>();
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Vertex", EditorStyles.boldLabel);
            GUILayout.Label("Position: " + vertex.Position);
            GUILayout.Label("Connections: " + vertex.Edges.Count);
            if (GUILayout.Button("Add Connected Vertex (E)"))
            {
                WVertex newVertex = WBuilder.InstantiateVertex(vertex.Position);
                vertex.AddEdge(newVertex);
                Selection.activeGameObject = newVertex.gameObject;
                Debug.Log("Added connected vertex");
            }
            EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length != 2);
            if (GUILayout.Button("Join vertices"))
            {
                GUI.enabled = true;
                WVertex v1 = Selection.gameObjects[0].GetComponent<WVertex>();
                WVertex v2 = Selection.gameObjects[1].GetComponent<WVertex>();
                v1.AddEdge(v2);
                Debug.Log("Joined Vertices");
            }
            EditorGUI.EndDisabledGroup();
        }
    }

    private void OnGUI_Edge()
    {
        WEdge edge = Selection.activeGameObject.GetComponent<WEdge>();
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Edge", EditorStyles.boldLabel);
            GUILayout.Label("Source: " + edge.Source.name);
            GUILayout.Label("Destination: " + edge.Destination.name);
        }
    }
    

    public SelectionType EvaluateSelection()
    {
        if (Selection.gameObjects.Length <= 0)
            return SelectionType.None;

        bool flagVertex = false;
        bool flagEdge = false;

        foreach (GameObject go in Selection.gameObjects)
        {
            if (go.GetComponent<WVertex>() != null)
            {
                flagVertex = true;
            }
            else if (go.GetComponent<WEdge>() != null)
            {
                flagEdge = true;
            }
        }

        if (flagVertex && flagEdge)
        {
            return SelectionType.Multiple;
        }
        if (flagVertex && !flagEdge)
        {
            return SelectionType.Vertex;
        }
        if (flagEdge && !flagVertex)
        {
            return SelectionType.Edge;
        }
        return SelectionType.None;
    }

    public void InitGraph()
    {
        graph = new WGraph();
        
        GameObject root = new GameObject
        {
            hideFlags = HideFlags.HideInHierarchy
        };
        
        graph.Root = root;

        WVertex vertex = WBuilder.InstantiateVertex(graph);

        graph.AddObject(root);
        graph.AddObject(vertex.gameObject);
    }

    public void ClearAll()
    {
        graph = null;
        foreach (GameObject go in spawnedObjects)
        {
            DestroyImmediate(go);
        }
    }

    public void ClearScene()
    {
        graph = null;
        foreach (GameObject go in FindObjectsOfType<GameObject>())
        {
            DestroyImmediate(go);
        }
    }
}