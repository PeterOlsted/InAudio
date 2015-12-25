using System;
using System.Linq;
using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem.InAudioEditor
{

    public static class OnDragging
    {
        public static InAudioBankLink BusDragging(Rect area)
        {
            if (area.Contains(Event.current.mousePosition) && Event.current.type == EventType.DragUpdated ||
                Event.current.type == EventType.DragPerform)
            {
                if (DragAndDrop.objectReferences.Length != 0)
                {
                    var bankLink = DragAndDrop.objectReferences[0] as InAudioBankLink;
                    if (bankLink != null && bankLink._type == AudioBankTypes.Bank)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                        if (Event.current.type == EventType.DragPerform)
                        {
                            return DragAndDrop.objectReferences[0] as InAudioBankLink;
                        }
                    }
                }
            }
            return null;
        }

        public static T DraggingObject<T>(Rect area) where T : Object
        {
            if (area.Contains(Event.current.mousePosition) && Event.current.IsDragging() &&
                DragAndDrop.objectReferences.Length > 0)
            {
                T casted = DragAndDrop.objectReferences[0] as T;

                if (casted != null)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        return DragAndDrop.objectReferences[0] as T;
                    }
                }
            }
            return null;
        }

        public static T DraggingObject<T>(Rect area, Func<T, bool> predicate) where T : Object
        {
            if (area.Contains(Event.current.mousePosition) && Event.current.IsDragging() &&
                DragAndDrop.objectReferences.Length > 0)
            {
                T casted = DragAndDrop.objectReferences[0] as T;

                if (casted != null && predicate(casted))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        return DragAndDrop.objectReferences[0] as T;
                    }
                }
            }
            return null;
        }

        public static T DraggingObject<T>(Rect area, Func<T, bool> predicate, Action<T> afterDrag) where T : Object
        {
            if (area.Contains(Event.current.mousePosition) && Event.current.IsDragging() &&
                DragAndDrop.objectReferences.Length > 0)
            {
                T casted = DragAndDrop.objectReferences[0] as T;

                if (casted != null && predicate(casted))
                {

                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        afterDrag(DragAndDrop.objectReferences[0] as T);
                        SceneView.RepaintAll();
                        var returnV = DragAndDrop.objectReferences[0] as T;

                        Event.current.UseEvent();
                        return returnV;
                    }
                    else
                    {
                        Event.current.UseEvent();
                    }
                }
                else
                {
                    //DragAndDrop.visualMode = DragAndDropVisualMode.None;
                }
            }
            return null;
        }

        public static bool OnDraggingObject<T>(Object[] objects, Rect area, Func<T[], bool> predicate,
            Action<T[]> OnDrop)
        {

            if (area.Contains(Event.current.mousePosition) && Event.current.IsDragging() &&
                DragAndDrop.objectReferences.Length > 0)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if (!(objects[i] is T))
                    {
                        return false;
                    }
                }
                T[] castedObjects = objects.Cast<T>().ToArray();
                if (predicate(castedObjects))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        OnDrop(castedObjects);
                        GUI.changed = true;
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool OnDraggingObject(Object[] objects, Rect area, Func<Object[], bool> predicate, Action<Object[]> OnDrop)
        {
            if (area.Contains(Event.current.mousePosition) && Event.current.IsDragging() &&
                DragAndDrop.objectReferences.Length > 0)
            {

                if (predicate(objects))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        OnDrop(objects);
                        GUI.changed = true;
                        return true;
                    }
                }
            }
            return false;
        }
    }

}