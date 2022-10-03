using UnityEditor;
using UnityEngine;

public static class EditorFunction
{
    private const string UNDO_STR_SNAP = "Snap objects";
    
    [MenuItem("Edit/Snap Objects %&s", isValidateFunction: true)]
    public static bool SnapObjectsValidate()
    {
        return Selection.gameObjects.Length > 0;
    }
    
    [MenuItem("Edit/Snap Objects %&s")]
    public static void SnapObjects()
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            Undo.RecordObject(go.transform, UNDO_STR_SNAP);
            Vector3 pos = go.transform.position.Round();
            go.transform.position = pos;
        }
        Debug.Log("Snapped objects");
    }

    // Extension methods
    public static Vector3 Round(this Vector3 v)
    {
        v.x = Mathf.Round(v.x);
        v.y = Mathf.Round(v.y);
        v.z = Mathf.Round(v.z);
        return v;
    }
}