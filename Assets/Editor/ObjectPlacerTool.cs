using System;
using Codice.Client.BaseCommands.Filters;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.XR;
using Random = UnityEngine.Random;

namespace Editor
{
    public class ObjectPlacerTool : EditorWindow
    {
        [MenuItem("Tools/ObjectPlacer")]
        private static void ShowWindow()
        {
            var window = GetWindow<ObjectPlacerTool>();
            window.titleContent = new GUIContent("ObjectPlacer");
            window.Show();
        }

        public float radius = 2f;
        public int spawnCount = 8;

        public bool active = false;
        
        private Vector2[] randPoints;

        private SerializedProperty propRadius;
        private SerializedProperty propSpawnCount;
        private SerializedObject so;

        private void OnEnable()
        {
            so = new SerializedObject(this);
            propRadius = so.FindProperty("radius");
            propSpawnCount = so.FindProperty("spawnCount");
            
            GenerateRandomPoints(spawnCount, radius);
            
            SceneView.duringSceneGui += DuringSceneGUI;
            Selection.selectionChanged += Repaint;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
            Selection.selectionChanged -= Repaint;
        }

        // Editor Update
        private void DuringSceneGUI(SceneView sceneView)
        {
            Handles.zTest = CompareFunction.Disabled;
            Transform camTf = sceneView.camera.transform;

            // Make sure scene repaints on mouse move
            if (Event.current.type == EventType.MouseMove)
            {
                sceneView.Repaint();
            }
            
            // Event.current.modifiers is a bit field
            bool holdingCtrl = (Event.current.modifiers & EventModifiers.Control) != 0;

            // Change radius with scroll wheel
            if (Event.current.type == EventType.ScrollWheel && holdingCtrl)
            {
                float scrollDir = Mathf.Sign(Event.current.delta.y);
                
                // Applying this property change on SerializedObject so it can be used for Undo
                so.Update();
                // Percentual increase instead of adding
                propRadius.floatValue *= 1f + scrollDir * 0.05f;
                so.ApplyModifiedProperties();
                
                GenerateRandomPoints(spawnCount, radius);
                // Updates editor window
                Repaint();
                
                // Consume event, sets any other events after it to none
                Event.current.Use();
            }
            
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Tangent space coordinate system
                Vector3 tangent = Vector3.Cross(hit.normal, camTf.forward).normalized;
                Vector3 bitangent = Vector3.Cross(hit.normal, tangent).normalized;

                foreach (Vector2 point in randPoints)
                {
                    Vector3 spacePoint = (tangent * point.x) + (bitangent * point.y);
                    Vector3 finalPoint = hit.point + spacePoint + (hit.normal * 2f);
                    Ray pointRay = new Ray(finalPoint,  hit.normal * -4f);
                    
                    // Handles.color = Color.yellow;
                    // Handles.SphereHandleCap(-1, finalPoint, Quaternion.identity, 0.1f, EventType.Repaint);
                    // Handles.color = Color.red;
                    // Handles.DrawAAPolyLine(finalPoint, finalPoint + hit.normal * -4f);
                    
                    if (Physics.Raycast(pointRay, out RaycastHit pointHit))
                    {
                        Handles.color = Color.green;
                        Handles.SphereHandleCap(-1, pointHit.point, Quaternion.identity, 0.2f, EventType.Repaint);
                    }
                }
                
                Handles.color = Color.blue;
                Handles.DrawAAPolyLine(hit.point, hit.point + hit.normal);
                Handles.color = Color.green;
                Handles.DrawAAPolyLine(hit.point, hit.point + bitangent);
                Handles.color = Color.red;
                Handles.DrawAAPolyLine(hit.point, hit.point + tangent);


                const int circDetail = 64;
                Vector3[] circlePoints = new Vector3[circDetail + 1];
                
                Vector3 prevPos = hit.point + (tangent * radius);
                for (int i = 0; i < circDetail + 1; i++)
                {
                    float t = i / (float)circDetail;
                    const float TAU = 6.28318530718f;
                    float angRad = t * TAU;
                    Vector2 dir = new Vector2(Mathf.Cos(angRad), Mathf.Sin(angRad));

                    Vector3 pos = hit.point + (tangent * dir.x * radius) + (bitangent * dir.y * radius);
                    
                    Vector3 finalPos = pos;
                    Ray circRay = new Ray(pos + hit.normal * 2f, -hit.normal);
                    if (Physics.Raycast(circRay, out RaycastHit circHit))
                    {
                        finalPos = circHit.point;
                    }

                    Handles.color = Color.yellow;
                    Handles.DrawLine(prevPos, finalPos, 3f);
                    // Handles.DrawAAPolyLine(prevPos, finalPos);
                    prevPos = finalPos;
                }
            }
        }

        // Editor Window
        private void OnGUI()
        {
            so.Update();

            bool button = GUILayout.Button("Hello");

            GUILayout.Toggle(false, "Activate");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propRadius);
            EditorGUILayout.PropertyField(propSpawnCount);
            if (EditorGUI.EndChangeCheck())
            {
                GenerateRandomPoints(propSpawnCount.intValue, radius);
            }

            // If clicked left mouse button in editor window
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                // Removes focus from UI
                GUI.FocusControl(null);
                Repaint();
            }
            
            // Forces scene to repaint if there's modified properties (eliminates laggy feels when changing properties)
            if (so.ApplyModifiedProperties())
            {
                SceneView.RepaintAll();
            }
        }

        void SpawnObjects()
        {
            
            Debug.Log("SpawnObjects");
        }

        void GenerateRandomPoints(int count, float radius)
        {
            randPoints = new Vector2[count];
            for (int i = 0; i < count; i++)
            {
                randPoints[i] = Random.insideUnitCircle * radius;
            }
        }
    }
}