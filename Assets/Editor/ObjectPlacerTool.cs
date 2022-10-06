using System.Collections.Generic;
using System.Drawing.Drawing2D;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
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
        public Vector3 prefabEulerRotation = Vector3.zero;
        public float groundOffset = 0f;
        public GameObject spawnPrefab = null;
        public Transform parentTransform = null;
        public Material previewMaterial;
        
        [Tooltip("Height from cursor normal to shoot ray from")]
        public float cursorRayHeight = 20f;
        [Tooltip("Max distance to still be accepted on selection")]
        public float cursorDistanceThreshold = 100f;

        private Vector2[] randPoints;

        private SerializedObject so;
        private SerializedProperty propRadius;
        private SerializedProperty propSpawnCount;
        private SerializedProperty propSpawnPrefab;
        private SerializedProperty propParentTransform;
        private SerializedProperty propPrefabEulerRotation;
        private SerializedProperty propGroundOffset;
        private SerializedProperty propPreviewMaterial;

        private void Init()
        {
            cursorRayHeight = 20f;
            cursorDistanceThreshold = 100f;
        }
        
        private void OnEnable()
        {
            Init();
            
            so = new SerializedObject(this);
            propRadius = so.FindProperty("radius");
            propSpawnCount = so.FindProperty("spawnCount");
            propSpawnPrefab = so.FindProperty("spawnPrefab");
            propParentTransform = so.FindProperty("parentTransform");
            propPrefabEulerRotation = so.FindProperty("prefabEulerRotation");
            propGroundOffset = so.FindProperty("groundOffset");
            propPreviewMaterial = so.FindProperty("previewMaterial");
            
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
            Init();

            GUIStyle guiStyle = new GUIStyle("Button");


            // UI STUFF
            Handles.BeginGUI();
            GUI.Button(new Rect(8, 8, 100, 20), "PlaceTool", guiStyle);
            
            
            
            Handles.EndGUI();
            // // //
            
            
            Handles.zTest = CompareFunction.LessEqual;
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


                List<RaycastHit> spawnHits = new List<RaycastHit>();
                foreach (Vector2 point in randPoints)
                {
                    Vector3 spacePoint = (tangent * point.x) + (bitangent * point.y);
                    Vector3 finalPoint = hit.point + spacePoint + (hit.normal * cursorRayHeight);
                    
                    // Handles.color = Color.yellow;
                    // Handles.SphereHandleCap(-1, finalPoint, Quaternion.identity, 0.1f, EventType.Repaint);
                    // Handles.color = Color.red;
                    // Handles.DrawAAPolyLine(finalPoint, finalPoint + hit.normal * -4f);
                    
                    if (Physics.Linecast(finalPoint, finalPoint - (hit.normal * cursorDistanceThreshold), out RaycastHit pointHit))
                    {
                        Handles.color = Color.green;
                        Handles.SphereHandleCap(-1, pointHit.point, Quaternion.identity, 0.5f, EventType.Repaint);
                        
                        Quaternion rotation = Quaternion.LookRotation(pointHit.normal) * Quaternion.Euler(prefabEulerRotation.x, prefabEulerRotation.y, prefabEulerRotation.z);
                        
                        // Mesh
                        Matrix4x4 poseMtx = Matrix4x4.TRS(pointHit.point + (pointHit.normal * groundOffset), rotation, Vector3.one);
                        MeshFilter[] meshFilter = spawnPrefab.GetComponentsInChildren<MeshFilter>();
                        foreach (var filter in meshFilter)
                        {
                            Matrix4x4 localMtx = filter.transform.localToWorldMatrix;
                            Matrix4x4 finalTransform = poseMtx * localMtx;
                            
                            Mesh mesh = filter.sharedMesh;
                            Material mat = filter.GetComponent<MeshRenderer>().sharedMaterial;
                            mat.SetPass(0);
                            Graphics.DrawMeshNow(mesh, finalTransform);
                        }
                        spawnHits.Add(pointHit);
                    }
                }
                
                if (Event.current.keyCode == KeyCode.Space && Event.current.type == EventType.KeyDown)
                {
                    TrySpawnObjects(spawnHits);
                    Event.current.Use();
                }

                Handles.color = Color.blue;
                Handles.DrawAAPolyLine(hit.point, hit.point + hit.normal);
                Handles.color = Color.green;
                Handles.DrawAAPolyLine(hit.point, hit.point + bitangent);
                Handles.color = Color.red;
                Handles.DrawAAPolyLine(hit.point, hit.point + tangent);
                

                // Drawing selection cursor
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
                    if (Physics.Linecast(pos + (hit.normal * cursorRayHeight), pos - (hit.normal * cursorDistanceThreshold), out RaycastHit circHit))
                    {
                        finalPos = circHit.point;
                    }

                    Handles.color = Color.white;
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

            EditorGUILayout.PropertyField(propGroundOffset);
            EditorGUILayout.PropertyField(propPrefabEulerRotation);
            EditorGUILayout.PropertyField(propSpawnPrefab);
            EditorGUILayout.PropertyField(propParentTransform);
            EditorGUILayout.PropertyField(propPreviewMaterial);

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

        void TrySpawnObjects(List<RaycastHit> hits)
        {
            if (spawnPrefab == null)
            {
                Debug.LogWarning("Spawn Prefab is not valid");
                return;
            }
            
            
            foreach (RaycastHit hit in hits)
            {
                Quaternion rotation = Quaternion.LookRotation(hit.normal) * Quaternion.Euler(prefabEulerRotation.x, prefabEulerRotation.y, prefabEulerRotation.z);
                Vector3 position = hit.point + (hit.normal * groundOffset);
                GameObject go = Instantiate(spawnPrefab, position, rotation, parentTransform);
            } 
        }

        void GenerateRandomPoints(int  count, float radius)
        {
            randPoints = new Vector2[count];
            for (int i = 0; i < count; i++)
            {
                randPoints[i] = Random.insideUnitCircle * radius;
            }
        } 
    }
}