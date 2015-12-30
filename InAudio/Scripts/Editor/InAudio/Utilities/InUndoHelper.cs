using System;
using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem.InAudioEditor
{
    public static class InUndoHelper
    {
        public static Object[] Array(params Object[] obj)
        {
            return obj;
        }

        public static void DoInGroup(Action action)
        {
            Undo.IncrementCurrentGroup();
            action();

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            Undo.IncrementCurrentGroup();
        }



        public static void RecordObject(Object obj, string undoDescription)
        {
            RecordObjectFull(obj, undoDescription);
        }

        public static void RecordObject(Object[] obj, string undoDescription)
        {
            Object[] nonNulls = obj.TakeNonNulls();
            for (int i = 0; i < nonNulls.Length; i++)
            {
                EditorUtility.SetDirty(nonNulls[i]);
            }
            RecordObjectFull(nonNulls, undoDescription);
        }

        public static void RecordObjects(string undoDescription, params Object[] obj)
        {
            Object[] nonNulls = obj.TakeNonNulls();
            for (int i = 0; i < nonNulls.Length; i++)
            {
                EditorUtility.SetDirty(nonNulls[i]);
            }
            RecordObjectFull(nonNulls, undoDescription);
        }

        public static void RecordObjectFull(Object obj, string undoDescription)
        {
            EditorUtility.SetDirty(obj);
            Undo.RegisterCompleteObjectUndo(obj, undoDescription);
        }


        public static void RecordObjectFull(Object[] obj, string undoDescription)
        {
            Object[] nonNulls = obj.TakeNonNulls();
            for (int i = 0; i < nonNulls.Length; i++)
            {
                EditorUtility.SetDirty(nonNulls[i]);
            }
            Undo.RegisterCompleteObjectUndo(nonNulls, undoDescription);
        }

        public static Object[] NodeUndo(InAudioNode node)
        {
            var bank = node.GetBank();
            return new Object[]
            {
                node,
                node._nodeData,
                bank
            };
        }

        public static T AddComponent<T>(GameObject go) where T : Component
        {
            return AddComponentUndo(go, typeof(T)) as T;
        }

        public static Object AddComponentUndo(this GameObject go, Type type)
        {
            EditorUtility.SetDirty(go);
            return Undo.AddComponent(go, type);
        }

        public static T AddComponentUndo<T>(this GameObject go) where T : Component
        {
            return AddComponent<T>(go);
        }

        public static void CompleteObjectUndo(Object obj, string description)
        {
            EditorUtility.SetDirty(obj);
            Undo.RegisterCompleteObjectUndo(obj, description);
        }

        public static void RegisterUndo(Object obj, string description)
        {
            EditorUtility.SetDirty(obj);
            Undo.RegisterCompleteObjectUndo(obj, description);

        }

        public static void RegisterFullObjectHierarchyUndo(Object obj, string name)
        {
            EditorUtility.SetDirty(obj);
            Undo.RegisterFullObjectHierarchyUndo(obj, name);
        }

        public static void Destroy(Object obj)
        {
            /*SceneView.RepaintAll();
            HandleUtility.Repaint();*/
            if (obj != null)
            {
                EditorUtility.SetDirty(obj);
                var component = obj as MonoBehaviour;
                if (component != null)
                    EditorUtility.SetDirty(component.gameObject);
            }
            if(obj != null)
                Undo.DestroyObjectImmediate(obj);

        }

        public static void PureDestroy(Object obj)
        {
            /*SceneView.RepaintAll();
            HandleUtility.Repaint();*/
            if (obj != null)
            {
                EditorUtility.SetDirty(obj);
            }

            if (obj != null)
                Object.DestroyImmediate(obj, true);
        }

        public static void DragNDropUndo(Object obj, string description)
        {
            EditorUtility.SetDirty(obj);
            if(obj != null)
                Undo.RegisterFullObjectHierarchyUndo(obj, description);

        }

        public static void GUIUndo<T>(Object obj, string description, Func<T> displayFunction, Action<T> assignAction) 
        {
            EditorGUI.BeginChangeCheck();
            T newValue = displayFunction();
            if (EditorGUI.EndChangeCheck())
            {
                RecordObjectFull(obj, description);
                assignAction(newValue);
                EditorUtility.SetDirty(obj);
            }
        }

        public static void GUIUndo<T>(Object obj, string description, ref T value, Func<T> displayFunction)
        {
            EditorGUI.BeginChangeCheck();
            T newValue = displayFunction();
            if (EditorGUI.EndChangeCheck())
            {
                RecordObjectFull(obj, description);
                value = newValue;
                EditorUtility.SetDirty(obj);
            }
        }

        public static void GUIUndoFull<T>(Object obj, string description, ref T value, Func<T> displayFunction)
        {
            EditorGUI.BeginChangeCheck();
            T newValue = displayFunction();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterFullObjectHierarchyUndo(obj, description);
                value = newValue;
                EditorUtility.SetDirty(obj);
            }
        }


        public delegate void RefOut<T>(out T v1, out T v2);
        public delegate void RefAssign<in T>(T v1, T v2);
        public static void GUIUndo<T>(Object obj, string description, RefOut<T> displayFunction, RefAssign<T> assignAction)
        {
            EditorGUI.BeginChangeCheck();
            T v1;
            T v2;
            displayFunction(out v1, out v2);
            if (EditorGUI.EndChangeCheck())
            {
                RecordObjectFull(obj, description);
                assignAction(v1, v2);
                EditorUtility.SetDirty(obj);
            }
        }

        public static void GUIUndo<T>(Object obj, string description, ref T value1, ref T value2, RefOut<T> displayFunction)
        {
            EditorGUI.BeginChangeCheck();
            T v1;
            T v2;
            displayFunction(out v1, out v2);
            if (EditorGUI.EndChangeCheck())
            {
                RecordObjectFull(obj, description);
                value1 = v1;
                value2 = v2;
                EditorUtility.SetDirty(obj);
            }
        }

        public static bool DeleteDialogue(Action action)
        {
            bool delete = EditorUtility.DisplayDialog("Delete Item?",
                "This operation will delete an item. This cannot be undo. Delete anyway?",
                "Delete", "Do Nothing");
            if (delete)
            {
                action();
            }
            return delete;
        }

        public static GameObject CreateGO(string name)
        {        
            GameObject go = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(go, name);
            return go;

        }

        public static void CheckIfChanged(Object obj)
        {
            if(GUI.changed)
                EditorUtility.SetDirty(obj);
        }
    }
}