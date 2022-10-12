using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(WallManager))]
public class WallBuilderEditor : Editor
{
    private SerializedObject so;
    private WallManager targetType;

    private void OnEnable()
    {
        so = serializedObject;
        targetType = serializedObject.targetObject.GetComponent<WallManager>();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        GUILayout.Space(20);
        GUILayout.Label("Debugging", EditorStyles.boldLabel);
        if (GUILayout.Button("Init"))
        {
            targetType.Init();
        }
    }

    private void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();
        Vector3 targetPos = Handles.PositionHandle(targetType.graph.Vertices[0].Position, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            targetType.graph.Vertices[0].Position = targetPos;
        }
    }
}