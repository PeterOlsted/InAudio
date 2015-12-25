using System.Linq;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{ 
public static  class DrawerHelper  {
    public static void BoldLabel(Rect labelPos, string toDisplay, GUIStyle labelStyle)
    {
        labelStyle.fontStyle = FontStyle.Bold;
        GUI.Label(labelPos, toDisplay, labelStyle);
        labelStyle.fontStyle = FontStyle.Normal;
    }

    public static void HandleEventDrag(SerializedProperty prop)
    {
        if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
        {
            bool canDropObject = true;
            int clipCount = DragAndDrop.objectReferences.Count(obj =>
            {
                var audioEvent = obj as InAudioEventNode;
                if (audioEvent == null)
                    return false;
                return audioEvent._type == EventNodeType.Event;
            });

            if (clipCount != DragAndDrop.objectReferences.Length || clipCount == 0)
                canDropObject = false;

            if (canDropObject)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                if (Event.current.type == EventType.DragPerform)
                { 
                    //int arraySize = prop.arraySize;
                    prop.arraySize++;
                    //prop.InsertArrayElementAtIndex(prop.arraySize - 1);
                    var arrIndex = prop.GetArrayElementAtIndex(prop.arraySize - 1);
                    var arrEvent = arrIndex.FindPropertyRelative("Event");

                    arrEvent.objectReferenceValue = DragAndDrop.objectReferences[0];
                    arrEvent.serializedObject.ApplyModifiedProperties();
                    GUI.changed = true;
                    Event.current.UseEvent();
                }
            }
        }
    }

    //public static bool HandleEventDrag(InAudioEvent audioEvent)
    //{
    //    if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
    //    {
    //        bool canDropObject = true;
    //        int clipCount = DragAndDrop.objectReferences.Count(obj =>
    //        {
    //            var inEvent = obj as InAudioEventNode;
    //            if (inEvent == null)
    //                return false;
    //            return inEvent.Type == EventNodeType.Event;
    //        });

    //        if (clipCount != DragAndDrop.objectReferences.Length || clipCount == 0)
    //            canDropObject = false;

    //        if (canDropObject)
    //        {
    //            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
    //            if (Event.current.type != EventType.DragPerform)
    //                EditorEventUtil.UseEvent();

    //            if (Event.current.type == EventType.DragPerform)
    //            {
    //                audioEvent.Add(DragAndDrop.objectReferences[0] as InAudioEventNode, EventHookListData.PostEventAt.AttachedTo);
    //                GUI.changed = true;
    //                EditorEventUtil.UseEvent();
    //                return true;
    //            }
    //        }
    //        else
    //        {
    //            DragAndDrop.visualMode = DragAndDropVisualMode.None;
    //        }
    //    }
    //    return false;
    //}

    public static void HandleBankDrag(SerializedProperty prop)
    {
        if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
        {

            bool canDropObject = true;
            int clipCount = DragAndDrop.objectReferences.Count(obj =>
            {
                var audioBank = obj as InAudioBankLink;
                if (audioBank == null)
                    return false;
                return audioBank._type == AudioBankTypes.Bank;
            });

            if (clipCount != DragAndDrop.objectReferences.Length || clipCount == 0)
                canDropObject = false;

            if (canDropObject)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                if (Event.current.type == EventType.DragPerform)
                {
                    int arraySize = prop.arraySize;
                    prop.arraySize++;
                    var arrEvent = prop.GetArrayElementAtIndex(arraySize - 1).FindPropertyRelative("Bank");
                    arrEvent.objectReferenceValue = DragAndDrop.objectReferences[0];
                }
            }
        }
    }
}
}