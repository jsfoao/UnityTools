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

    private void OnGUI_Vertex()
    {
        WVertex vertex = Selection.activeGameObject.GetComponent<WVertex>();
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Vertex", EditorStyles.boldLabel);
            GUILayout.Label("Position: " + vertex.Position);
            if (GUILayout.Button("Add Connected Vertex"))
            {
                WVertex newVertex = WBuilder.CreateVertex(graph);
                vertex.AddConnection(newVertex);
                Debug.Log("Added connected vertex");
            }
            EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length != 2);
            if (GUILayout.Button("Join vertices"))
            {
                GUI.enabled = true;
                WVertex v1 = Selection.gameObjects[0].GetComponent<WVertex>();
                WVertex v2 = Selection.gameObjects[1].GetComponent<WVertex>();
                v1.AddConnection(v2);
                Debug.Log("Joined Vertices");
            }
            EditorGUI.EndDisabledGroup();
        }
    }

    private void OnGUI_Edge()
    {
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Edge", EditorStyles.boldLabel);
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
        else if (flagVertex && !flagEdge)
        {
            return SelectionType.Vertex;
        }
        else if (flagEdge && !flagVertex)
        {
            return SelectionType.Edge;
        }
        return SelectionType.None;
    }

    // Wallbuilder extensions

    public void InitGraph()
    {
        graph = new WGraph();

        spawnedObjects = new List<GameObject>();

        GameObject root = new GameObject();
        root.hideFlags = HideFlags.HideInHierarchy;
        graph.Root = root;

        WVertex vertex = WBuilder.CreateVertex(graph);

        spawnedObjects.Add(root);
        spawnedObjects.Add(vertex.gameObject);
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
        foreach (GameObject go in UnityEngine.Object.FindObjectsOfType<GameObject>())
        {
            DestroyImmediate(go);
        }
    }
}