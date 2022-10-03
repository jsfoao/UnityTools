using System;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(Entity))]
public class EntityTypeEditor : Editor
{
    public enum Operation
    {
        Add, Subtract
    }

    public Operation currentOp;
    private float value;

    // Serialization
    private SerializedObject so;
    private SerializedProperty propValue;
    
    private void OnEnable()
    {
        Debug.Log("OnEnable");
        
        // Serialized version of object we're inspecting
        // Alternative
        // so = new SerializedObject(target);
        so = serializedObject;
        propValue = so.FindProperty("value");
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
    }

    public override void OnInspectorGUI()
    {
        // Explicit positioning using rect
        // Use these if you need very specific positioning
        // GUI
        // EditorGUI
        
        // Implicit positioning, auto-layout
        // GUILayout
        // EditorGUILayout
        
        Entity entity = (Entity)target;

        GUILayout.Button("Some button");

        so.Update();
        EditorGUILayout.PropertyField(propValue);
        if (so.ApplyModifiedProperties())
        {
            // if something changed
            
        }
        
        using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Operation: ");
            currentOp = (Operation)EditorGUILayout.EnumPopup(currentOp);
            
            float newValue = EditorGUILayout.FloatField(value);
            if (value != entity.value)
            {
                Undo.RecordObject(entity, "Change entity value");
                entity.value = newValue;
            }
        }

        GUILayout.Space(40);
        GUILayout.Label("Hello", EditorStyles.boldLabel);
        EditorGUILayout.ObjectField("Assign: ",null, typeof(Transform), true);
    }
}