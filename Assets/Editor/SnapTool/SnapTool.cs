using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class SnapTool : EditorWindow
{
    const string UNDO_STR_SNAP = "Snap objects";
    
    [Serializable]
    public enum GridType
    {
        Cartesian,
        Polar
    }

    public bool snapRealTime = false;
    public GridType gridType = GridType.Cartesian;

    // Cartesian properties
    [Range(0f, 1f)] public float gridSpacing = 1f;
    public bool drawGrid = true;

    // Polar properties
    [Tooltip("Angle spacing in degrees")] [Range(5f, 45f)]
    public float angleSpacing = 15f;

    [Range(0.1f, 1f)] public float radiusSpacing = 1f;

    public float gridSize = 10f;

    private SerializedObject so;
    private SerializedProperty propSnapRealTime;
    private SerializedProperty propGridType;
    private SerializedProperty propDrawGrid;
    private SerializedProperty propGridSpacing;
    private SerializedProperty propGridSize;
    private SerializedProperty propAngleSpacing;
    private SerializedProperty propRadiusSpacing;

    [MenuItem("Tools/SnapTool")]
    private static void ShowWindow()
    {
        var window = GetWindow<SnapTool>();
        window.titleContent = new GUIContent("SnapTool");
        window.Show();
    }

    private void OnEnable()
    {
        so = new SerializedObject(this);
        propSnapRealTime = so.FindProperty("snapRealTime");
        propGridSpacing = so.FindProperty("gridSpacing");
        propGridSize = so.FindProperty("gridSize");
        propDrawGrid = so.FindProperty("drawGrid");
        propGridType = so.FindProperty("gridType");
        propAngleSpacing = so.FindProperty("angleSpacing");
        propRadiusSpacing = so.FindProperty("radiusSpacing");

        // Load saved configuration
        gridSpacing = EditorPrefs.GetFloat("SNAPPER_TOOL_gridSpacing", 1f);

        Selection.selectionChanged += Repaint;
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable()
    {
        EditorPrefs.SetFloat("SNAPPER_TOOL_gridSpacing", gridSpacing);

        Selection.selectionChanged -= Repaint;
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void DuringSceneGUI(SceneView sceneView)
    {
        Handles.zTest = CompareFunction.LessEqual;

        Vector3 averagedPos = Vector3.zero;
        foreach (GameObject go in Selection.gameObjects)
        {
            Undo.RecordObject(go.transform, UNDO_STR_SNAP);
            Vector3 pos = go.transform.position;
            averagedPos += pos;

            if (snapRealTime)
            {
                go.transform.position = GetSnappedPosition(pos);
            }
        }
        averagedPos /= Selection.gameObjects.Length;
            
        if (gridType == GridType.Cartesian)
        {
            if (gridSpacing < 0.1f)
                return;

            DrawCartesianGrid(averagedPos.Round(1 / gridSpacing), new Vector2(gridSize, gridSize), gridSpacing);
        }

        if (gridType == GridType.Polar)
        {
            DrawPolarGrid(averagedPos, gridSize, angleSpacing, radiusSpacing);
        }
    }

    private void OnGUI()
    {
        so.Update();
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Grid Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(propSnapRealTime);
            EditorGUILayout.PropertyField(propGridType);

            if (gridType == GridType.Cartesian)
            {
                EditorGUILayout.PropertyField(propGridSpacing);
            }
            if (gridType == GridType.Polar)
            {
                EditorGUILayout.PropertyField(propAngleSpacing);
                EditorGUILayout.PropertyField(propRadiusSpacing);
            }

            if (!snapRealTime)
            {
                if (GUILayout.Button("Snap Position"))
                {
                    foreach (GameObject go in Selection.gameObjects)
                    {
                        Undo.RecordObject(go.transform, UNDO_STR_SNAP);
                        Vector3 pos = go.transform.position;

                        go.transform.position = GetSnappedPosition(pos);
                    }
                }
            }
        }

        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Grid Gizmos", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(propDrawGrid);
            EditorGUILayout.PropertyField(propGridSize);
        }

        so.ApplyModifiedProperties();
    }

    private void OnValidate()
    {
        gridSize = Mathf.Clamp(gridSize, 1f, 100f);
    }

    private void DrawCartesianGrid(Vector3 origin, Vector2 size, float spacing)
    {
        if (!drawGrid)
            return;

        for (float x = -size.x / 2; x <= size.x / 2; x += spacing)
        {
            Vector3 offsetOrigin = origin + Vector3.forward * x;
            Handles.DrawAAPolyLine(offsetOrigin - Vector3.right * (size.x / 2),
                offsetOrigin + Vector3.right * (size.x / 2));
        }

        for (float y = -size.y / 2; y <= size.y / 2; y += spacing)
        {
            Vector3 offsetOrigin = origin + Vector3.right * y;
            Handles.DrawAAPolyLine(offsetOrigin - Vector3.forward * (size.y / 2),
                offsetOrigin + Vector3.forward * (size.y / 2));
        }
    }

    private void DrawPolarGrid(Vector3 origin, float radius, float angleSpacing, float radiusSpacing)
    {
        if (!drawGrid)
            return;

        float maxRadius = radius;

        for (float s = 0; s <= radius; s += radiusSpacing)
        {
            if (s + radiusSpacing > radius)
            {
                maxRadius = s;
            }

            Handles.DrawWireDisc(origin, Vector3.up, s);
        }

        for (float a = 0; a < 360; a += angleSpacing)
        {
            float angleRadians = a * Mathf.Deg2Rad;
            Vector3 goal = new Vector3(Mathf.Cos(angleRadians), 0f, Mathf.Sin(angleRadians)) * maxRadius;

            Handles.DrawAAPolyLine(origin, origin + goal);
        }
    }

    private Vector3 GetSnappedPosition(Vector3 position)
    {
        if (gridType == GridType.Cartesian)
        {
            return SnapLibrary.GetSnapCartesianPosition(position, gridSpacing);
        }

        if (gridType == GridType.Polar)
        {
            return SnapLibrary.GetSnapPolarPosition(position, radiusSpacing, angleSpacing);
        }
        
        return default;
    }
}

public static class SnapLibrary
{
    public static Vector3 GetSnapCartesianPosition(Vector3 position, float gridSize)
    {
        return position.Round(1 / gridSize);
    }


    public static Vector3 GetSnapPolarPosition(Vector3 position, float radiusSpacing, float angleSpacing)
    {
        Vector2 vec = new Vector2(position.x, position.z);
        float distance = vec.magnitude;
        float distanceSnapped = distance.Round(radiusSpacing);

        float angleRadians = Mathf.Atan2(vec.y, vec.x); // 0 to TAU
        float angleTurns = angleRadians / (2 * Mathf.PI); // 0 to 1
        float angleTurnsSnapped = angleTurns.Round(1f / angleSpacing);
        float angleRadSnapped = angleTurnsSnapped * 2 * Mathf.PI;

        Vector2 directionSnapped = new Vector2(Mathf.Cos(angleRadSnapped), Mathf.Sin(angleRadSnapped));
        Vector2 snappedVec = directionSnapped * distanceSnapped;

        return new Vector3(snappedVec.x, position.y, snappedVec.y);
    }
}