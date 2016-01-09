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
}
}