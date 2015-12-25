using System;
using System.Linq;
using InAudioSystem;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.InAudioEditor;
using InAudioSystem.Internal;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

//namespace InAudioSystem.InAudioEditor

namespace InAudioSystem.InAudioEditor
{

    [CustomEditor(typeof (InSpline))]
    public class InSplineDrawer : Editor
    {
        private InSpline SplineController
        {
            get { return target as InSpline; }
        }

        private static Vector3[] points = new Vector3[2];

        private void OnDisable()
        {
            if (InAudioInstanceFinder.IsValid)
            {
                InAudioInstanceFinder.InAudioGuiUserPrefs.SelectedSplineController = null;

            }
        }

        private void OnEnable()
        {
            if (SplineController.gameObject == Selection.activeGameObject)
            {
                if (InAudioInstanceFinder.IsValid)
                {
                    InAudioInstanceFinder.InAudioGuiUserPrefs.SelectedSplineController = SplineController;
                }
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SplineAudioEvent"));
            EditorGUILayout.Separator();

            bool add = GUILayout.Button("Add Node");
            bool selectNew = false;
            if (GUILayout.Button("Add and Select"))
            {
                add = true;
                selectNew = true;
            }

            var nodeArray = serializedObject.FindProperty("Nodes");
            var nodeArraySize = nodeArray.FindPropertyRelative("Array.size");

            var connectionArray = serializedObject.FindProperty("Connections");
            var connectionArraySize = connectionArray.FindPropertyRelative("Array.size");

            nodeArray.isExpanded = EditorGUILayout.Foldout(nodeArray.isExpanded, "Nodes");
            DrawList(nodeArray, nodeArraySize, i =>
            {
                var node = nodeArray.GetArrayElementAtIndex(i).objectReferenceValue as InSplineNode;

                nodeArray.DeleteArrayElementAtIndex(i);
                nodeArray.DeleteArrayElementAtIndex(i);

                for (int j = 0; j < connectionArray.arraySize; j++)
                {
                    var prop = connectionArray.GetArrayElementAtIndex(j);
                    if (prop.FindPropertyRelative("NodeA").objectReferenceValue == node ||
                        prop.FindPropertyRelative("NodeB").objectReferenceValue == node)
                    {
                        connectionArray.DeleteArrayElementAtIndex(j);
                        --j;
                    }
                }

                if (node != null)
                {
                    InUndoHelper.Destroy(node.gameObject);
                }
            });

            connectionArray.isExpanded = EditorGUILayout.Foldout(connectionArray.isExpanded, "Connections");
            DrawList(connectionArray, connectionArraySize, connectionArray.DeleteArrayElementAtIndex);

            if (add)
            {
                InUndoHelper.DoInGroup(() =>
                {

                    UndoAll("Add new spline node");

                    GameObject newNodeGO = InUndoHelper.CreateGO(SplineController.gameObject.name + " Node");
                    var newNode = newNodeGO.AddComponent<InSplineNode>();
                    newNodeGO.transform.position = SplineController.transform.position;
                    newNode.SplineController = SplineController;

                    newNodeGO.transform.parent = SplineController.transform;

                    newNodeGO.transform.position = SplineController.transform.position +
                                                   SplineController.transform.forward;

                    int count = SplineController.Nodes.Count;
                    if (count > 0)
                        SplineController.Nodes[0].ConnectTo(newNode);

                    SplineController.Nodes.Add(newNode);

                    if (selectNew)
                        Selection.activeGameObject = newNodeGO;
                });
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

        }

        private void DrawList(SerializedProperty arrayProp, SerializedProperty arrayPropSize,
            Action<int> deleteAction = null)
        {
            if (arrayProp.isExpanded)
            {
                if (!arrayPropSize.hasMultipleDifferentValues)
                {
                    for (int i = 0; i < arrayProp.arraySize; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUI.enabled = false;
                        EditorGUILayout.PropertyField(arrayProp.GetArrayElementAtIndex(i));
                        GUI.enabled = true;

                        if (deleteAction != null)
                        {
                            if (GUILayout.Button("X", GUILayout.Width(20)))
                            {
                                int index = i;
                                InUndoHelper.DoInGroup(() =>
                                {
#if UNITY_4_1 || UNITY_4_2
                                        Undo.RegisterSceneUndo("Delete element in spline");
                                    #else
                                    UndoAll("Delete element in spline");
#endif
                                    deleteAction(index);

                                });

                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Cannot show lists of different lengths.", MessageType.None);
                }
            }
        }

        public static void OnSceneDraw(InSpline spline)
        {

            if (spline == null)
                return;

            if (spline.Nodes.Count < 2)
                return;



            for (int i = 0; i < spline.Connections.Count; i++)
            {
                InSplineNode nodeA = spline.Connections[i].NodeA;
                InSplineNode nodeB = spline.Connections[i].NodeB;

                if (nodeA == null || nodeB == null)
                    continue;

                float weight = 2.0f;
                if (Selection.Contains(nodeA.gameObject) || Selection.Contains(nodeB.gameObject))
                {
                    weight = 6.0f;
                }

                Handles.DrawAAPolyLine(weight, SetPoints(nodeA.transform.position, nodeB.transform.position));
            }
        }

        private void OnSceneGUI()
        {
            if (!Selection.Contains(SplineController.gameObject))
                return;

            for (int i = 0; i < SplineController.Nodes.Count; i++)
            {
                if (SplineController.Nodes[i] != null)
                    SplineController.Nodes[i].SplineController = SplineController;
            }

            OnSceneDraw(SplineController);
        }

        private static Vector3[] SetPoints(Vector3 v1, Vector3 v2)
        {
            points[0] = v1;
            points[1] = v2;
            return points;
        }


        private void UndoAll(string message)
        {
            InUndoHelper.RecordObject(
                SplineController.Nodes.Cast<Object>().ToArray().Add(SplineController), message);
        }
    }

}