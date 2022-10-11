using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(WallTool))]
public class WallToolTypeEditor : Editor
{
    private SerializedObject so;
    private WallTool targetType;

    private void OnEnable()
    {
        so = serializedObject;
        targetType = serializedObject.targetObject.GetComponent<WallTool>();
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
        if (GUILayout.Button("Rebuild"))
        {
        }
    }
}