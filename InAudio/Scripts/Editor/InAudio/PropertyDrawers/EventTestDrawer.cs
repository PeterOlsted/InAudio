//using System.Linq;
//using InAudioSystem;
//using InAudioSystem.ExtensionMethods;
//using UnityEditor;
//using UnityEngine;
//using Object = UnityEngine.Object;

//[CustomEditor(typeof(EventTester))]
//public class EventTestDrawer : Editor
//{
//    EventTester eventTester
//    {
//        get { return target as EventTester; }
//    }

//    private int LineHeight = 22;
//    private int DragHeight = 20;

//    public int GetPropertyHeight()
//    {
//        int extraHeight = 20;
//        var events = eventTester.EventList.Events;
//        if (events.Count == 0)
//            extraHeight = 0;
//        return eventTester.EventList.Events.Count * LineHeight + DragHeight + 20 + extraHeight;
//    }
//    public override void OnInspectorGUI()
//    {
//        Rect pos = EditorGUILayout.BeginVertical(GUILayout.Height(GetPropertyHeight()));

//        var labelPos = pos;
//        Color backgroundColor = GUI.color;

//        GUI.skin.label.alignment = TextAnchor.UpperLeft;
//        labelPos.height = 14;

//        var events = eventTester.EventList.Events;
        
//        if (events.Count > 0)
//        {
//            labelPos.y += 5;
//            Rect postAllRect = labelPos;
//            postAllRect.width = 50;
//            if (GUI.Button(labelPos, "Post all"))
//            {
//                for (int i = 0; i < events.Count; i++)
//                {
//                    //InAudio.PostEvents(eventTester.gameObject, events[i].Event);
//                }
//            }
//            labelPos.y += 15;
//        }
        

//        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
//        labelPos.y += 5;
//        for (int i = 0; i < events.Count; ++i)
//        {
//            labelPos.height = 20;
//            //EditorGUILayout.Separator();   

//            EventHookListData audioEvent = eventTester.EventList.Events[i];
//            if (audioEvent.Event != null)
//                GUI.Label(labelPos, audioEvent.Event.GetName);
//            else
//                GUI.Label(labelPos, "Missing event");


//            Rect buttonPos = labelPos;
//            buttonPos.x = pos.width - 200; //Align to right side
//            buttonPos.width = 50;
//            if (audioEvent == null)
//                GUI.enabled = false;

//            buttonPos.width += 40;
//            if (GUI.Button(buttonPos, "Post Event"))
//            {
//                //InAudio.PostEvent(eventTester.gameObject, audioEvent.Event);
//            }
//            buttonPos.width -= 40;
//            buttonPos.x += 100;
//            if (GUI.Button(buttonPos, "Find"))
//            {
//                EditorWindow.GetWindow<EventWindow>().Find(audioEvent.Event);
//            }
//            GUI.enabled = true;
//            buttonPos.x = pos.width - 44;
//            buttonPos.width = 35;
//            if (GUI.Button(buttonPos, "X"))
//            {
//                UndoHelper.RecordObjectFull(target, "Removed Event");
//                events.RemoveAt(i);
//                GUI.changed = true;
//            }
//            labelPos.y += LineHeight;

//        }
//        labelPos.y += 10;
//        EditorGUILayout.Separator();
//        labelPos.height = DragHeight;

//        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
//        GUI.color = backgroundColor;
//        GUI.Button(labelPos, "Drag event here to add event");
//        OnDragging.OnDraggingObject(DragAndDrop.objectReferences, labelPos, CanDrop, objects =>
//        {
//            UndoHelper.RecordObjectFull(target, "Added event");
//            objects.ForEach(obj => events.Add(new EventHookListData(obj as AudioEvent)));
//        });

//            //objects => ));

//        GUI.color = backgroundColor;

//        labelPos.height += 1;

//        EditorGUILayout.Separator();
//        EditorGUILayout.EndVertical();

//    }

//    private bool CanDrop(Object[] objects)
//    {
//        return
//            objects.All(obj => (obj as AudioEvent) != null) &&
//            objects.All(obj => (obj as AudioEvent).Type == EventNodeType.Event);
//    }
//}
