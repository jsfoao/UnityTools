using UnityEditor;
using UnityEngine;
public class SnapperTool : EditorWindow
{
    [MenuItem("Tools/Snapper")]
    public static void Open()
    {
        GetWindow<SnapperTool>("Snapper");
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
        Handles.DrawLine(new Vector3(0f, 0f, 0f), new Vector3(10f, 0f, 0f) );
    }

    private void OnGUI()
    {
        using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
        {
            if (GUILayout.Button( "Something"))
            {
                EditorFunction.SnapObjects();
            }
        }
    }
}
