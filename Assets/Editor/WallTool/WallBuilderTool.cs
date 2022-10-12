using UnityEditor;
using UnityEngine;

public class WallBuilderTool : EditorWindow
{
    public enum SelectedType
    {
        Vertex, Edge, Graph, Multiple, None
    }
    
    public SerializedObject so;
    
    [MenuItem("Tools/WallBuilderTool")]
    private static void ShowWindow()
    {
        var window = GetWindow<WallBuilderTool>();
        window.titleContent = new GUIContent("WallBuilderTool");
        window.Show();
    }

    private void OnEnable()
    {
        so = new SerializedObject(this);

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
        
    }

    private void OnGUI()
    {
        so.Update();
        if (Selection.gameObjects.Length > 0)
        {
            SelectedType selectedType = GetSelectedType();
            if (selectedType != SelectedType.None)
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    switch (selectedType)
                    {
                        case SelectedType.Vertex:
                            OnGUIVertex();
                            break;
                        case SelectedType.Edge:
                            OnGUIEdge();
                            break;
                        case SelectedType.Graph:
                            OnGUIGraph();
                            break;
                        case SelectedType.Multiple:
                            OnGUIMultiple();
                            break;
                    }
                }
            }
        }
        so.ApplyModifiedProperties();
    }

    private void OnGUIVertex()
    {
        GUILayout.Label("Selection : Vertex", EditorStyles.boldLabel);

    }


    private void OnGUIEdge()
    {
        GUILayout.Label("Selection : Edge", EditorStyles.boldLabel);
    }

    private void OnGUIGraph()
    {
        GUILayout.Label("Selection : Graph", EditorStyles.boldLabel);
    }

    private void OnGUIMultiple()
    {
        GUILayout.Label("Selection : Multiple", EditorStyles.boldLabel);
    }

    private SelectedType GetSelectedType()
    {
        bool flagVertex = false;
        bool flagEdge = false;
        bool flagGraph = false;
        
        foreach (GameObject go in Selection.gameObjects)
        {
            flagVertex = go.GetComponent<WallVertex>();
            flagEdge = go.GetComponent<WallEdge>();
            flagGraph = go.GetComponent<WallManager>();
        }

        if (flagVertex && !flagEdge && !flagGraph)
        {
            return SelectedType.Vertex;
        }
        if (flagEdge && !flagVertex && !flagGraph)
        {
            return SelectedType.Edge;
        }
        if (flagGraph && !flagVertex && !flagEdge)
        {
            return SelectedType.Graph;
        }
        if (flagGraph && flagVertex && flagEdge)
        {
            return SelectedType.Multiple;
        }
        return SelectedType.None;
    }
}
