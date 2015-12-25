using System;
using InAudioSystem;
using InAudioSystem.InAudioEditor;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{

    [CustomPropertyDrawer(typeof (AudioEventCollisionList))]
    public class EventHookCollisionDrawer : PropertyDrawer
    {
        private const float LineHeight = 22;
        private const float DragHeight = 20;

        private static string[] layerNames;

        private static GUIStyle boldStyle;

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            SerializedProperty arrayEnter = prop.FindPropertyRelative("EventsEnter").FindPropertyRelative("Events");
            SerializedProperty arrayExit = prop.FindPropertyRelative("EventsExit").FindPropertyRelative("Events");
            if (arrayEnter == null || arrayExit == null)
                return 100;
            float extraHeightEnter = arrayEnter.arraySize*LineHeight;
            float extraHeightExit = arrayExit.arraySize*LineHeight;

            float baseHeight = 150;
            if (!prop.isExpanded)
            {
                baseHeight = 0;
                extraHeightEnter = 0;
                extraHeightExit = 0;
            }
            if (arrayEnter.hasMultipleDifferentValues)
                extraHeightEnter = 0;
            if (arrayExit.hasMultipleDifferentValues)
                extraHeightExit = 0;

            return base.GetPropertyHeight(prop, label) + extraHeightEnter + extraHeightExit + baseHeight;
        }

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(pos, label, prop);
            var labelPos = pos;
            labelPos.y += 2;
            Color backgroundColor = GUI.color;

            var alignment = GUI.skin.label.alignment;

            if (layerNames == null)
                layerNames = RebuildLayerNames();

            SerializedProperty arrayEnter = prop.FindPropertyRelative("EventsEnter").FindPropertyRelative("Events");
            SerializedProperty arrayExit = prop.FindPropertyRelative("EventsExit").FindPropertyRelative("Events");
            EditorStyles.foldout.fontStyle = FontStyle.Bold;
            if (boldStyle == null)
            {
                boldStyle = new GUIStyle(GUI.skin.FindStyle("label"));
                boldStyle.fontStyle = FontStyle.Bold;
                boldStyle.alignment = TextAnchor.UpperLeft;
            }

            //Label
            labelPos.height = 18;
            prop.isExpanded = EditorGUI.Foldout(labelPos, prop.isExpanded, "Collision/Trigger");
            EditorStyles.foldout.fontStyle = FontStyle.Normal;

            if (prop.isExpanded)
            {
                labelPos.y += LineHeight;

                //Layers
                var layerMask3D = prop.FindPropertyRelative("Layers");
                //EditorGUI.PropertyField(labelPos, layerMask3D, new GUIContent("Layers"));
                layerMask3D.intValue = EditorGUI.MaskField(labelPos, "Layers", layerMask3D.intValue, layerNames);

                labelPos.y += DragHeight;

                //React to
                var collisionProp = prop.FindPropertyRelative("CollisionReaction");
                var triggerProp = prop.FindPropertyRelative("TriggerReaction");

                string collisionLabel = "Collision";

                EditorGUI.PropertyField(labelPos, collisionProp, new GUIContent(collisionLabel));
                labelPos.y += DragHeight;
                //EditorGUI.PropertyField(labelPos, collisionProp, new GUIContent(collisionLabel));
                EditorGUI.PropertyField(labelPos, triggerProp, new GUIContent("Trigger"));

                //Enter events

                labelPos = DrawEvents(pos, labelPos, arrayEnter, "Enter");

                //Exit events
                DrawEvents(pos, labelPos, arrayExit, "Exit");

            }
            GUI.color = backgroundColor;
            GUI.skin.label.alignment = alignment;
            EditorGUI.EndProperty();

        }

        private Rect DrawEvents(Rect pos, Rect labelPos, SerializedProperty arrayEnter, string name)
        {
            labelPos.y += DragHeight;
            DrawerHelper.BoldLabel(labelPos, name + " Events", boldStyle);

            if (arrayEnter.hasMultipleDifferentValues)
            {
                labelPos.y += DragHeight;
                EditorGUI.HelpBox(labelPos, "Cannot edit with multiple different values.", MessageType.Info);
            }
            else
            {
                labelPos.y += DragHeight;
                DrawEventArray(pos, arrayEnter, ref labelPos, boldStyle);
                GUI.Button(labelPos, "Drag event here to add to " + name + " event");
                if (labelPos.Contains(Event.current.mousePosition))
                {
                    DrawerHelper.HandleEventDrag(arrayEnter);
                }
            }
            return labelPos;
        }


        private void DrawEventArray(Rect pos, SerializedProperty array, ref Rect labelPos, GUIStyle labelStyle)
        {
            for (int i = 0; i < array.arraySize; ++i)
            {
                labelPos.height = 20;
                var audioEvent =
                    array.GetArrayElementAtIndex(i).FindPropertyRelative("Event").objectReferenceValue as
                        InAudioEventNode;

                if (audioEvent != null)
                    GUI.Label(labelPos, audioEvent.GetName, labelStyle);
                else
                    GUI.Label(labelPos, "Missing event", labelStyle);

                Rect buttonPos = labelPos;
                buttonPos.x = pos.width - 200; //Align to right side
                buttonPos.width = 100;
                if (audioEvent == null)
                    GUI.enabled = false;

                SerializedProperty postAtProperty = array.GetArrayElementAtIndex(i).FindPropertyRelative("PostAt");
                buttonPos.y += 1.5f;
                var currentValue =
                    Convert.ToInt32(EditorGUI.EnumPopup(buttonPos,
                        (EventHookListData.PostEventAt) postAtProperty.enumValueIndex));
                buttonPos.y -= 1.5f;
                if (currentValue != postAtProperty.enumValueIndex)
                    GUI.changed = true;
                postAtProperty.enumValueIndex = currentValue;

                buttonPos.x += 100;
                buttonPos.width = 50;
                if (GUI.Button(buttonPos, "Find"))
                {
                    EditorWindow.GetWindow<EventWindow>().Find(audioEvent);
                }
                GUI.enabled = true;
                buttonPos.x = pos.width - 44;
                buttonPos.width = 35;
                if (GUI.Button(buttonPos, "X"))
                {
                    array.DeleteArrayElementAtIndex(i);
                    GUI.changed = true;
                }
                labelPos.y += LineHeight;
            }
        }

        public static string[] RebuildLayerNames()
        {
            int lastLayer = 0;
            for (int i = 0; i < 32; i++)
            {
                if (LayerMask.LayerToName(i) != "")
                {
                    lastLayer = i;
                }
            }
            string[] names = new string[lastLayer + 2];
            for (int i = 0; i < lastLayer + 2; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (layerName != "")
                {
                    names[i] = i + ": " + layerName;
                }
            }
            return names;
        }
    }
}