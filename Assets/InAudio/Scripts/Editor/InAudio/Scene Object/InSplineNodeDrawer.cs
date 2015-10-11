using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using InAudioSystem;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.Internal;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{


[CanEditMultipleObjects]
[CustomEditor(typeof(InSplineNode))]
public class InSplineNodeDrawer : Editor
{
    InSplineNode SplineNode
    {
        get { return target as InSplineNode; }
    }

    private static float range = 0.0f;
    private static float maxRange = 100.0f;

    private bool expandedConnections = true;

    void OnDisable()
    {   
        if (InAudioInstanceFinder.IsValid)
        {
            InAudioInstanceFinder.InAudioGuiUserPrefs.SelectedSplineController = null;
        }   
    }
    
    void OnEnable()
    {
        if (InAudioInstanceFinder.IsValid)
        {
            InAudioInstanceFinder.InAudioGuiUserPrefs.SelectedSplineController = SplineNode.SplineController;
        }
    }

    public override void OnInspectorGUI()
    {
        if (!InAudioInstanceFinder.IsValid)
        {
            EditorGUILayout.HelpBox("Please add the InAudio Manager to the scene", MessageType.Info);
            if (GUILayout.Button("Add manager to scene"))
            {
                ErrorDrawer.AddManagerToScene();
            }
        }


        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        if (serializedObject.FindProperty("SplineController").hasMultipleDifferentValues)
        {
            EditorGUILayout.HelpBox("Different spline controllers", MessageType.Warning);
            return;
        }
        if(SplineNode.SplineController == null)
            EditorGUILayout.HelpBox("Missing spline controller, please assign one", MessageType.Warning);

        if (InAudioInstanceFinder.IsValid)
        {
            InAudioInstanceFinder.InAudioGuiUserPrefs.SelectedSplineController = SplineNode.SplineController;
        }
               
        bool add = GUILayout.Button("Add Node");
        bool selectNew = false;
        if (GUILayout.Button("Add and Select"))
        {
            add = true;
            selectNew = true;
        }
        EditorGUILayout.Separator();


        var objectField = EditorGUILayout.ObjectField("Controlling Spline", serializedObject.FindProperty("SplineController").objectReferenceValue, typeof(InSpline), true);
        if (serializedObject.FindProperty("SplineController").objectReferenceValue == null)
        {
            serializedObject.FindProperty("SplineController").objectReferenceValue = objectField;
        }
        
        if (Selection.objects.Length == 1)
        {
            GUILayout.Button("Drag node here to connect");
            OnDragging.DraggingObject<Object>(GUILayoutUtility.GetLastRect(), o =>
            {
                GameObject go = o as GameObject;
                if (go != null)
                {
                    var node = go.GetComponent<InSplineNode>();
                    if (node != SplineNode && !SplineNode.SplineController.ContainsConnection(SplineNode, node))
                        return true;
                }
                return false;
            }, o =>
            {
                InUndoHelper.RecordObject(SplineNode.SplineController, "Connect nodes");
                (o as GameObject).GetComponent<InSplineNode>().ConnectTo(SplineNode);
            });

            //var a = new SerializedObject(SplineNode.SplineController)
            if (SplineNode.SplineController != null)
            {
                expandedConnections = EditorGUILayout.Foldout(expandedConnections, "Connected to");
                
                for (int i = 0; i < SplineNode.SplineController.Connections.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUI.enabled = false;
                    var conc = SplineNode.SplineController.Connections[i];
                    if (conc.NodeA == SplineNode)
                    {
                        EditorGUILayout.ObjectField(conc.NodeB, typeof (InSplineNode), true);
                        GUI.enabled = true;
                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            InUndoHelper.RecordObject(SplineNode.SplineController, "Remove spline connection");
                            SplineNode.SplineController.RemoveConnections(conc);
                        }
                        EditorUtility.SetDirty(SplineNode.SplineController);
                    }
                    else if (conc.NodeB == SplineNode)
                    {
                        EditorGUILayout.ObjectField(conc.NodeA, typeof (InSplineNode), true);
                        GUI.enabled = true;
                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            InUndoHelper.RecordObject(SplineNode.SplineController, "Remove spline connection");
                            SplineNode.SplineController.RemoveConnections(conc);
                        }
                        EditorUtility.SetDirty(SplineNode.SplineController);
                    }

                    
                    EditorGUILayout.EndHorizontal();
                }
                
            }
        }



        EditorGUILayout.Separator();
        bool delete = true;
        if (GUILayout.Button("Delete"))
        {
            InUndoHelper.DoInGroup(() =>
            {
                
            #if UNITY_4_1 || UNITY_4_2
                Undo.RegisterSceneUndo("Combine nodes");
            #else   
                UndoAll("Delete node");
            #endif
                foreach (var gameObject in Selection.gameObjects)
                {
                    InUndoHelper.Destroy(gameObject);
                }
                
                delete = true;
            });
        }


        if (add)
        {
            InUndoHelper.DoInGroup(() =>
            {
                #if UNITY_4_1 || UNITY_4_2
                Undo.RegisterSceneUndo("Delete element in spline");
                #else                                
                UndoAll("Add new spline node");
                #endif

                GameObject go = InUndoHelper.CreateGO(SplineNode.SplineController.gameObject.name + " Node");
                go.transform.parent = SplineNode.SplineController.transform;
                go.transform.position = SplineNode.transform.position + SplineNode.transform.forward;
                go.transform.position = SplineNode.transform.position;

                var newNode = go.AddComponent<InSplineNode>();
                newNode.SplineController = SplineNode.SplineController;
                newNode.ConnectTo(SplineNode);
                SplineNode.SplineController.Nodes.Add(newNode);

                if (selectNew)
                    Selection.activeGameObject = go;
            });
        }

        if (EditorGUI.EndChangeCheck() && delete == false)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    void OnSceneGUI()
    {
        int inSplineNodeSelectionCount = Selection.gameObjects.CountIf(go => go.GetComponent<InSplineNode>() != null);

        
        if (inSplineNodeSelectionCount == 1)
        {
            if (Selection.objects.Length == 1)
            {
                Handles.BeginGUI();
                
                range = EditorGUI.Slider(new Rect(15, 10, 300, 18), "Range", range, 0.0f, maxRange);

                if (SplineNode.SplineController == null)
                    GUI.enabled = false;
                if (GUI.Button(new Rect(15, 30, 200, 20), "Combine nodes in Range"))
                {
                    CombineInRange(SplineNode, range);
                    Repaint();
                }

                if (GUI.Button(new Rect(15, 50, 200, 20), "Connect all nodes in Range"))
                {
                    string message = "Connect nodes in range";
                    UndoAll(message);             

                    ConnectInRange(SplineNode, range);
                    Repaint();
                }

                GUI.enabled = true;
                Handles.EndGUI();
                if (SplineNode.SplineController != null && Selection.activeGameObject != null)
                {
                    Handles.color = Color.red;
                    Handles.CubeCap(0, Selection.activeGameObject.transform.position, Quaternion.identity, 1.2f * 0.3f);
                    for (int i = 0; i < SplineNode.SplineController.Nodes.Count; i++)
                    {
                        var node = SplineNode.SplineController.Nodes[i];

                        if (node == SplineNode || node == null)
                            continue;

                        if (SplineNode == null)
                            break;

                        if (Vector3.Distance(node.transform.position, SplineNode.transform.position) < range)
                        {
                            Handles.CubeCap(0, node.gameObject.transform.position, Quaternion.identity, 1.2f * 0.3f);
                        }
                    }
                    Handles.color = Color.white;
                }

                var color = Color.gray;
                Handles.color = color;

                if (SplineNode != null)
                {
                    //range = Handles.RadiusHandle(Quaternion.identity, SplineNode.transform.position, range);
                }
            }
        }
        else if(inSplineNodeSelectionCount == Selection.objects.Length)
        {
            if (Selection.activeGameObject != SplineNode.gameObject)
            {
                Handles.color = Color.red;
                if(Selection.activeGameObject != null)
                    Handles.CubeCap(0, Selection.activeGameObject.transform.position, Quaternion.identity, 0.2f);
                Selection.gameObjects.ForEach(go =>
                {
                    var node = go.GetComponent<InSplineNode>();
                    if(node != null)
                        Handles.CubeCap(0, node.gameObject.transform.position, Quaternion.identity, 0.2f);

                });
                Handles.color = Color.white;

                GUI.enabled = true;
                Handles.BeginGUI();
                if (GUI.Button(new Rect(15, 10, 200, 20), "Combine selected nodes"))
                {
                    CombineSelectedNodes();
                }
                Handles.EndGUI();
            }
        }

        
        Handles.color = Color.white;
        if (SplineNode != null)
        {
            /*if (Selection.activeGameObject != SplineNode.gameObject)
            {

                return;
            }*/

            //if (!Selection.Contains(SplineNode.gameObject))
              //  return;

            InSplineDrawer.OnSceneDraw(SplineNode.SplineController);
        }

      /*  if (Event.current.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(controlID);
        }*/

        if (GUI.changed && target != null)    
          EditorUtility.SetDirty(target);
    }

    private void UndoAll(string message)
    {
        InUndoHelper.RecordObject(
            SplineNode.SplineController.Nodes.Cast<Object>().ToArray().Add(SplineNode.SplineController), message);
    }

    private void CombineSelectedNodes()
    {
        InSplineNode splineNode = Selection.activeGameObject.GetComponent<InSplineNode>();
        GameObject[] selectedGO = Selection.gameObjects.FindAllNoNulls(go =>
        {
            var node = go.GetComponent<InSplineNode>();
            if (node == null || splineNode == node)
                return false;
            return true;
        });
        InSplineNode[] selected = selectedGO.Convert(go => go.GetComponent<InSplineNode>());

        CombineNodes(splineNode, selected);
    }

    private void CombineNodes(InSplineNode splineNode, InSplineNode[] conditioned)
    {
        InUndoHelper.DoInGroup(() => { 
            #if UNITY_4_1 || UNITY_4_2
                    Undo.RegisterSceneUndo("Combine nodes");
            #else
                    UndoAll("Combine nodes");
            #endif
            HashSet<InSplineNode> periphery = new HashSet<InSplineNode>();
            foreach (var node in conditioned)
            {
                var connected = node.SplineController.FindConnectedNodes(node);
                for (int i = 0; i < connected.Length; i++)
                {
                    var connectedTo = connected[i];
                    periphery.Add(connectedTo);
                }

                node.UnconnectAll();
            }

            foreach (var node in conditioned)
            {
                if(node != null)
                    InUndoHelper.Destroy(node.gameObject);
            }

            foreach (var node in periphery)
            {
                splineNode.ConnectTo(node);
            }

            var connections = splineNode.SplineController.Connections;
            for (int i = 0; i < connections.Count; i++)
            {
                if (!connections[i].IsValid())
                {
                    connections.SafeRemoveAt(ref i);
                }
            }
        });
        //Selection.activeGameObject = null;
        Repaint();
    }

    private void CombineInRange(InSplineNode splineNode, float range)
    {
        InSplineNode[] inRange = splineNode.SplineController.Nodes.FindAllNoNulls(
                n => Vector3.Distance(n.transform.position, splineNode.transform.position) < range && n != splineNode);

        CombineNodes(splineNode, inRange);
    }

    private void ConnectInRange(InSplineNode splineNode, float range)
    {
        InSplineNode[] inRange = splineNode.SplineController.Nodes.FindAllNoNulls(
                n => Vector3.Distance(n.transform.position, splineNode.transform.position) < range);
        for (int i = 0; i < inRange.Length; i++)
        {
            for (int j = i; j < inRange.Length; j++)
            {
                inRange[i].ConnectTo(inRange[j]);
            }
        }
    }
}
}