using System;
using InAudioSystem;
using InAudioSystem.InAudioEditor;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{

    [CustomPropertyDrawer(typeof (BankHookActions))]
    public class BankHookDrawer : PropertyDrawer
    {

        private float LineHeight = 22;
        private float DragHeight = 20;

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            var array = prop.FindPropertyRelative("Actions");
            return base.GetPropertyHeight(prop, label) + array.arraySize*LineHeight + DragHeight + 25;
        }

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(pos, label, prop);
            var actions = prop.FindPropertyRelative("Actions");

            var labelPos = pos;

            var labelStyle = GUI.skin.GetStyle("label");
            var alignment = labelStyle.alignment;
            labelStyle.alignment = TextAnchor.MiddleLeft;
            ;

            labelPos.height = 14;
            DrawerHelper.BoldLabel(labelPos, prop.FindPropertyRelative("Title").stringValue, labelStyle);

            for (int i = 0; i < actions.arraySize; ++i)
            {
                var currentElement = actions.GetArrayElementAtIndex(i);
                labelPos.y += LineHeight;
                labelPos.height = 20;
                InAudioBankLink bankLink =
                    currentElement.FindPropertyRelative("Bank").objectReferenceValue as InAudioBankLink;
                if (bankLink != null)
                    GUI.Label(labelPos, bankLink.GetName, labelStyle);
                else
                    GUI.Label(labelPos, "Missing event", labelStyle);

                Rect buttonPos = labelPos;
                buttonPos.x = pos.width - 172; //Align to right side
                buttonPos.width = 70;
                var actionProp = currentElement.FindPropertyRelative("BankAction");
                actionProp.enumValueIndex =
                    Convert.ToInt32(EditorGUI.EnumPopup(buttonPos, (BankHookActionType) actionProp.enumValueIndex));

                buttonPos.width = 50;

                buttonPos.x = pos.width - 100; //Align to right side
                if (bankLink == null)
                    GUI.enabled = false;

                if (GUI.Button(buttonPos, "Find"))
                {
                    EditorWindow.GetWindow<AuxWindow>().FindBank(bankLink);
                }
                GUI.enabled = true;
                buttonPos.x = pos.width - 44;
                buttonPos.width = 35;
                if (GUI.Button(buttonPos, "X"))
                {
                    actions.DeleteArrayElementAtIndex(i);
                }
            }
            labelPos.y += DragHeight + 4;
            labelPos.height = DragHeight;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;

            GUI.Button(labelPos, "Drag bank here for " + prop.FindPropertyRelative("Title").stringValue);
            if (labelPos.Contains(Event.current.mousePosition))
            {
                DrawerHelper.HandleBankDrag(actions);
            }

            labelStyle.alignment = alignment;
            EditorGUI.EndProperty();
        }
    }
}